using GuardiansOfTheWetland.Core;
using UnityEngine;

namespace GuardiansOfTheWetland.Player
{
    /// <summary>
    /// Displays idle, walk, jump and hurt sprites without changing player physics.
    /// PlayerController remains responsible for facing left or right.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        // How many walk frames are shown per second (higher = faster steps).
        [SerializeField, Min(0.01f)] private float walkFramesPerSecond = 8f;

        // Horizontal speed below which the player is considered to be standing still.
        [SerializeField, Min(0f)] private float movementThreshold = 0.05f;

        // Seconds the hurt sprite stays visible after a hit.
        [SerializeField, Min(0f)] private float hurtDuration = 0.35f;

        // Components of the player read every frame.
        private PlayerController controller;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D body;

        // One sprite per animation state, loaded once in Awake.
        private Sprite idleSprite;
        private Sprite walkSprite1;
        private Sprite walkSprite2;
        private Sprite jumpSprite;
        private Sprite hurtSprite;

        // Time accumulated while walking, used to choose the walk frame.
        private float walkTime;

        // Moment (Time.time) when the hurt sprite stops being shown.
        private float hurtUntil = float.NegativeInfinity;

        /// <summary>
        /// Gets the player components and loads every sprite from Resources/Sprites.
        /// </summary>
        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            body = GetComponent<Rigidbody2D>();

            idleSprite = SpriteFactory.Get("player_idle", GameConfig.PlayerColor);
            walkSprite1 = LoadSprite("player_walk_1");
            walkSprite2 = LoadSprite("player_walk_2");
            jumpSprite = LoadSprite("player_jump");
            hurtSprite = LoadSprite("player_hurt");
        }

        /// <summary>
        /// Resets the animation to the idle state every time the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            walkTime = 0f;
            hurtUntil = float.NegativeInfinity;
            spriteRenderer.sprite = idleSprite;
        }

        /// <summary>
        /// Chooses the sprite after the player has moved this frame.
        /// Priority order: hurt, then jump, then walk, then idle.
        /// </summary>
        private void LateUpdate()
        {
            // Hurt takes priority over jumping, walking and standing still.
            if (Time.time < hurtUntil)
            {
                walkTime = 0f;
                spriteRenderer.sprite = hurtSprite;
                return;
            }

            // Upward velocity covers the first jump frame, before ground contacts update.
            if (!controller.IsGrounded || GetVerticalSpeed() > 0.1f)
            {
                walkTime = 0f;
                spriteRenderer.sprite = jumpSprite;
                return;
            }

            // Moving horizontally on the ground: alternate between the two walk frames.
            if (Mathf.Abs(controller.HorizontalSpeed) > movementThreshold)
            {
                // The modulo 2 turns the elapsed time into frame 0 or frame 1.
                int frame = Mathf.FloorToInt(walkTime * Mathf.Max(0.01f, walkFramesPerSecond)) % 2;
                spriteRenderer.sprite = frame == 0 ? walkSprite1 : walkSprite2;
                // Repeat keeps walkTime inside one full cycle (two frames) so it never grows forever.
                walkTime = Mathf.Repeat(walkTime + Time.deltaTime,
                    2f / Mathf.Max(0.01f, walkFramesPerSecond));
                return;
            }

            // Standing still on the ground.
            walkTime = 0f;
            spriteRenderer.sprite = idleSprite;
        }

        /// <summary>
        /// Call only when the health or hazard system accepts a damage event.
        /// This method does not subtract health, apply knockback or respawn the player.
        /// </summary>
        public void PlayHurt()
        {
            hurtUntil = Time.time + Mathf.Max(0f, hurtDuration);
        }

        /// <summary>
        /// Loads a sprite by file name. If the file is missing, a warning is shown
        /// and the idle sprite is used so the game keeps working.
        /// </summary>
        private Sprite LoadSprite(string spriteName)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
            // The file exists: use it.
            if (sprite != null)
            {
                return sprite;
            }

            Debug.LogWarning("[PlayerAnimator] Missing sprite: Sprites/" + spriteName +
                ". Using the idle sprite.", this);
            return idleSprite;
        }

        /// <summary>
        /// Vertical speed of the player. Unity 6 renamed velocity to linearVelocity,
        /// so the correct property is chosen at compile time.
        /// </summary>
        private float GetVerticalSpeed()
        {
#if UNITY_6000_0_OR_NEWER
            return body.linearVelocity.y;
#else
            return body.velocity.y;
#endif
        }
    }
}
