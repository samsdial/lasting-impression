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
        [SerializeField, Min(0.01f)] private float walkFramesPerSecond = 8f;
        [SerializeField, Min(0f)] private float movementThreshold = 0.05f;
        [SerializeField, Min(0f)] private float hurtDuration = 0.35f;

        private PlayerController controller;
        private SpriteRenderer spriteRenderer;
        private Rigidbody2D body;
        private Sprite idleSprite;
        private Sprite walkSprite1;
        private Sprite walkSprite2;
        private Sprite jumpSprite;
        private Sprite hurtSprite;
        private float walkTime;
        private float hurtUntil = float.NegativeInfinity;

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

        private void OnEnable()
        {
            walkTime = 0f;
            hurtUntil = float.NegativeInfinity;
            spriteRenderer.sprite = idleSprite;
        }

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

            if (Mathf.Abs(controller.HorizontalSpeed) > movementThreshold)
            {
                int frame = Mathf.FloorToInt(walkTime * Mathf.Max(0.01f, walkFramesPerSecond)) % 2;
                spriteRenderer.sprite = frame == 0 ? walkSprite1 : walkSprite2;
                walkTime = Mathf.Repeat(walkTime + Time.deltaTime,
                    2f / Mathf.Max(0.01f, walkFramesPerSecond));
                return;
            }

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

        private Sprite LoadSprite(string spriteName)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + spriteName);
            if (sprite != null)
            {
                return sprite;
            }

            Debug.LogWarning("[PlayerAnimator] Missing sprite: Sprites/" + spriteName +
                ". Using the idle sprite.", this);
            return idleSprite;
        }

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
