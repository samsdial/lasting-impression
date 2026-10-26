using UnityEngine;

namespace GuardiansOfTheWetland.Gameplay
{
    /// <summary>Threats of the wetland that hurt the player.</summary>
    public enum HazardType
    {
        // Falling in sends the player back to the spawn point.
        PollutedWater,

        // Touching it pushes the player back.
        Gorse
    }

    /// <summary>
    /// Trigger area that removes one water drop from the player on contact.
    /// It is disabled after the wetland is restored.
    /// </summary>
    public class Hazard : MonoBehaviour
    {
        /// <summary>What kind of threat this is (changes sound and reaction).</summary>
        public HazardType Type { get; private set; }

        /// <summary>When false, touching the hazard has no effect.</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Sets the hazard type after the object is created by the level builder.
        /// </summary>
        public void Initialize(HazardType hazardType)
        {
            Type = hazardType;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            TryDamage(other);
        }

        // Staying inside also hurts once the invulnerability time is over.
        private void OnTriggerStay2D(Collider2D other)
        {
            TryDamage(other);
        }

        /// <summary>
        /// Applies damage only if the hazard is active and the collider belongs to the player.
        /// </summary>
        private void TryDamage(Collider2D other)
        {
            if (!IsActive)
            {
                return;
            }

            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(this);
            }
        }
    }
}
