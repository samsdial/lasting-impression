using System;
using GuardiansOfTheWetland.Audio;
using GuardiansOfTheWetland.Core;
using GuardiansOfTheWetland.Player;
using GuardiansOfTheWetland.UI;
using UnityEngine;

namespace GuardiansOfTheWetland.Gameplay
{
    /// <summary>
    /// Water drops (lives) of the player. Each hit removes one drop and gives
    /// redundant feedback: hurt sprite, blinking, sound and subtitle.
    /// With no drops left, the player restarts at the spawn point with full drops.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerHealth : MonoBehaviour
    {
        // Time between visibility toggles while the player is invulnerable.
        private const float BlinkInterval = 0.1f;

        // Speed applied when the gorse pushes the player away.
        private const float KnockbackHorizontal = 5f;
        private const float KnockbackVertical = 7f;

        private PlayerController controller;
        private PlayerAnimator playerAnimator;
        private SpriteRenderer spriteRenderer;
        private float invulnerableUntil;

        /// <summary>Drops the player has right now.</summary>
        public int CurrentHealth { get; private set; }

        /// <summary>Drops the player starts with.</summary>
        public int MaxHealth => GameConfig.MaxHealth;

        /// <summary>True during the short safety time after a hit.</summary>
        public bool IsInvulnerable => Time.time < invulnerableUntil;

        /// <summary>Raised every time the number of drops changes (used by the HUD).</summary>
        public event Action HealthChanged;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();
            playerAnimator = GetComponent<PlayerAnimator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            CurrentHealth = MaxHealth;
        }

        private void Update()
        {
            if (spriteRenderer == null)
            {
                return;
            }

            // While invulnerable the sprite blinks; afterwards it is always visible.
            if (IsInvulnerable)
            {
                spriteRenderer.enabled = Mathf.FloorToInt(Time.time / BlinkInterval) % 2 == 0;
            }
            else if (!spriteRenderer.enabled)
            {
                spriteRenderer.enabled = true;
            }
        }

        /// <summary>
        /// Removes one drop, unless the player is still invulnerable from a previous hit.
        /// </summary>
        /// <param name="hazard">The hazard that caused the damage.</param>
        public void TakeDamage(Hazard hazard)
        {
            if (IsInvulnerable)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - 1);
            invulnerableUntil = Time.time + GameConfig.InvulnerabilitySeconds;

            if (playerAnimator != null)
            {
                playerAnimator.PlayHurt();
            }

            // Each hazard has its own sound (with subtitle) and its own reaction.
            if (hazard.Type == HazardType.PollutedWater)
            {
                PlaySound(SoundNames.Water);
                controller.Respawn();
            }
            else
            {
                PlaySound(SoundNames.Damage);

                // Push the player away from the hazard; default to the left if exactly on top.
                float direction = Mathf.Sign(transform.position.x - hazard.transform.position.x);
                if (Mathf.Approximately(direction, 0f))
                {
                    direction = -1f;
                }

                controller.ApplyKnockback(new Vector2(direction * KnockbackHorizontal, KnockbackVertical));
            }

            if (CurrentHealth == 0)
            {
                RestartWithFullHealth();
            }

            HealthChanged?.Invoke();
        }

        /// <summary>
        /// Retry without reloading the scene: back to the start with all drops.
        /// Collected items are kept so the player does not lose progress.
        /// </summary>
        private void RestartWithFullHealth()
        {
            controller.Respawn();
            CurrentHealth = MaxHealth;

            if (SubtitleManager.Instance != null)
            {
                SubtitleManager.Instance.Show("[Sin gotas: vuelves al inicio con las gotas recargadas]");
            }
        }

        private static void PlaySound(string soundName)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(soundName);
            }
        }
    }
}
