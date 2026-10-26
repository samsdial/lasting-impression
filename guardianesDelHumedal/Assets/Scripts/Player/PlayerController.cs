using UnityEngine;

namespace GuardiansOfTheWetland.Player
{
    /// <summary>
    /// Handles the player's horizontal movement, jumping, ground detection and respawn.
    /// All tuning values are public so the game designer can adjust them
    /// in the Inspector while the game is running.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Horizontal speed in tiles per second.")]
        public float moveSpeed = 6f;

        [Tooltip("Maximum jump height in tiles.")]
        public float jumpHeight = 3.2f;

        [Tooltip("Multiplier of the global gravity. Higher values make the jump feel less floaty.")]
        public float gravityScale = 3f;

        [Tooltip("Fraction of upward speed kept when the jump button is released early.")]
        [Range(0f, 1f)]
        public float jumpCutMultiplier = 0.5f;

        [Header("Assists")]
        [Tooltip("Seconds after leaving a platform during which a jump is still allowed.")]
        public float coyoteTime = 0.1f;

        [Tooltip("Seconds a jump press is remembered before landing.")]
        public float jumpBufferTime = 0.1f;

        [Header("Respawn")]
        [Tooltip("If the player falls below this height, it returns to the spawn point.")]
        public float fallLimitY = -3f;

        // Size of the player's collision box. It matches the visible character
        // in the 64x64 art (about 31x52 px), so walls are touched where they look touched.
        private static readonly Vector2 ColliderSize = new Vector2(0.5f, 0.85f);

        // Seconds without player control after being pushed by a hazard.
        private const float KnockbackLockSeconds = 0.25f;

        // A contact counts as "floor" when its normal points mostly upwards.
        private const float GroundNormalThreshold = 0.5f;

        // Value used to mark timers as "expired".
        private const float ExpiredTime = -100f;

        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private Vector3 spawnPoint;
        private float lastGroundedTime = ExpiredTime;
        private float lastJumpPressedTime = ExpiredTime;
        private bool isGrounded;
        private float controlLockedUntil = ExpiredTime;

        /// <summary>True while the player is standing on a surface.</summary>
        public bool IsGrounded => isGrounded;

        /// <summary>Current horizontal speed, used later by the animation system.</summary>
        public float HorizontalSpeed => GetVelocity().x;

        /// <summary>
        /// Stores the position where the player starts and respawns.
        /// </summary>
        public void Initialize(Vector3 spawn)
        {
            spawnPoint = spawn;
            transform.position = spawn;
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Physics setup for a responsive platformer character.
            body.gravityScale = gravityScale;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            // The body must never sleep; otherwise collision callbacks stop
            // and the ground detection would fail while standing still.
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.size = ColliderSize;

            // Zero friction prevents the player from sticking to walls in mid-air.
            boxCollider.sharedMaterial = new PhysicsMaterial2D("PlayerNoFriction")
            {
                friction = 0f,
                bounciness = 0f
            };
        }

        private void Update()
        {
            // Falling out of the level sends the player back to the start.
            if (transform.position.y < fallLimitY)
            {
                Respawn();
                return;
            }

            // During a knockback the push is not overridden by the player's input.
            if (Time.time < controlLockedUntil)
            {
                return;
            }

            float horizontal = PlayerInput.GetHorizontal();

            // Remember when jump was pressed (jump buffer).
            if (PlayerInput.JumpPressed())
            {
                lastJumpPressedTime = Time.time;
            }

            Vector2 velocity = GetVelocity();
            velocity.x = horizontal * moveSpeed;

            // A jump happens if it was pressed recently AND the player touched the ground recently.
            bool jumpRequested = Time.time - lastJumpPressedTime <= jumpBufferTime;
            bool canJump = Time.time - lastGroundedTime <= coyoteTime;

            if (jumpRequested && canJump)
            {
                velocity.y = CalculateJumpVelocity();

                // Consume both timers so one press produces exactly one jump.
                lastJumpPressedTime = ExpiredTime;
                lastGroundedTime = ExpiredTime;
            }

            // Releasing the button early produces a shorter jump (better control).
            if (PlayerInput.JumpReleased() && velocity.y > 0f)
            {
                velocity.y *= jumpCutMultiplier;
            }

            SetVelocity(velocity);

            // Face the direction of movement.
            if (horizontal != 0f && spriteRenderer != null)
            {
                spriteRenderer.flipX = horizontal < 0f;
            }
        }

        /// <summary>
        /// Moves the player back to the spawn point and stops all movement.
        /// Public so other systems (health, hazards) can call it.
        /// </summary>
        public void Respawn()
        {
            transform.position = spawnPoint;
            SetVelocity(Vector2.zero);
            controlLockedUntil = ExpiredTime;
        }

        /// <summary>
        /// Pushes the player with the given velocity and briefly ignores the controls,
        /// so the push is clearly felt (used by hazards such as the gorse).
        /// </summary>
        public void ApplyKnockback(Vector2 velocity)
        {
            SetVelocity(velocity);
            controlLockedUntil = Time.time + KnockbackLockSeconds;
        }

        /// <summary>
        /// Initial upward speed needed to reach jumpHeight: v = sqrt(2 * g * h).
        /// </summary>
        private float CalculateJumpVelocity()
        {
            float gravity = Mathf.Abs(Physics2D.gravity.y) * body.gravityScale;
            return Mathf.Sqrt(2f * gravity * jumpHeight);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            // Check every contact point; if any of them points up, we are on the floor.
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (collision.GetContact(i).normal.y > GroundNormalThreshold)
                {
                    isGrounded = true;
                    lastGroundedTime = Time.time;
                    return;
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            isGrounded = false;
        }

        // Unity 6 renamed Rigidbody2D.velocity to linearVelocity.
        // These helpers keep the script compatible with Unity 2022 LTS and Unity 6.
        private Vector2 GetVelocity()
        {
#if UNITY_6000_0_OR_NEWER
            return body.linearVelocity;
#else
            return body.velocity;
#endif
        }

        private void SetVelocity(Vector2 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            body.linearVelocity = velocity;
#else
            body.velocity = velocity;
#endif
        }
    }
}
