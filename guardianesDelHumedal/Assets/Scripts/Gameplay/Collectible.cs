using GuardiansOfTheWetland.Audio;
using GuardiansOfTheWetland.Core;
using GuardiansOfTheWetland.Player;
using GuardiansOfTheWetland.UI;
using UnityEngine;

namespace GuardiansOfTheWetland.Gameplay
{
    /// <summary>Kinds of items the player can pick up.</summary>
    public enum CollectibleType
    {
        Trash,
        Seed
    }

    /// <summary>
    /// An item (trash or native seed) that is collected when the player touches it.
    /// Collecting gives redundant feedback: a sound, a subtitle and a HUD counter pulse.
    /// </summary>
    public class Collectible : MonoBehaviour
    {
        // Vertical distance and speed of the floating animation that makes items noticeable.
        private const float BobAmplitude = 0.1f;
        private const float BobSpeed = 2.5f;

        private CollectibleType type;
        private Vector3 basePosition;
        private bool isCollected;

        /// <summary>
        /// Sets the item type and remembers its resting position.
        /// </summary>
        public void Initialize(CollectibleType collectibleType)
        {
            type = collectibleType;
            basePosition = transform.position;
        }

        private void Update()
        {
            // The X position is used as a phase offset so items do not move in sync.
            float offset = Mathf.Sin((Time.time + basePosition.x) * BobSpeed) * BobAmplitude;
            transform.position = basePosition + Vector3.up * offset;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignore anything that is not the player, and never collect twice.
            if (isCollected || other.GetComponent<PlayerController>() == null)
            {
                return;
            }

            isCollected = true;

            GameSession session = GameSession.Instance;
            if (session != null)
            {
                session.RegisterCollected(type);
            }

            // The audio manager also shows the matching subtitle.
            string soundName = type == CollectibleType.Trash ? SoundNames.Trash : SoundNames.Seed;
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(soundName);
            }

            // When the last piece of trash is collected, tell the player what to do next.
            bool lastTrash = type == CollectibleType.Trash && session != null && session.AllTrashCollected;
            if (lastTrash && SubtitleManager.Instance != null)
            {
                SubtitleManager.Instance.Show("¡Recogiste toda la basura! Llega al letrero para restaurar el humedal");
            }

            Destroy(gameObject);
        }
    }
}
