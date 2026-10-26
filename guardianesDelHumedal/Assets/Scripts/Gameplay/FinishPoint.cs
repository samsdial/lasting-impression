using GuardiansOfTheWetland.Core;
using GuardiansOfTheWetland.Player;
using GuardiansOfTheWetland.UI;
using UnityEngine;

namespace GuardiansOfTheWetland.Gameplay
{
    /// <summary>
    /// The sign at the end of the level. Reaching it with all the trash collected
    /// restores the wetland; otherwise it tells the player how much trash is left.
    /// </summary>
    public class FinishPoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Only the player can activate the finish point.
            if (other.GetComponent<PlayerController>() == null)
            {
                return;
            }

            GameSession session = GameSession.Instance;

            // Nothing to do if there is no session or the wetland is already restored.
            if (session == null || session.IsRestored)
            {
                return;
            }

            // The RestorationController reacts to the Restored event.
            if (session.TryRestore())
            {
                return;
            }

            int remainingTrash = session.TrashTotal - session.TrashCollected;
            if (SubtitleManager.Instance != null)
            {
                SubtitleManager.Instance.Show($"Aún quedan {remainingTrash} basuras por recoger para restaurar el humedal");
            }
        }
    }
}
