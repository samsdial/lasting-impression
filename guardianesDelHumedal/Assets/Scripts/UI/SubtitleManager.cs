using System.Collections.Generic;
using GuardiansOfTheWetland.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace GuardiansOfTheWetland.UI
{
    /// <summary>
    /// Shows short text captions at the bottom of the screen.
    /// Every important sound has a caption, so players who are deaf, hard of hearing
    /// or playing muted get the same information (WCAG 2.2 - 1.2 / UDL - multiple means of representation).
    /// </summary>
    public class SubtitleManager : MonoBehaviour
    {
        /// <summary>Single active subtitle manager.</summary>
        public static SubtitleManager Instance { get; private set; }

        // Seconds a caption stays visible.
        private const float DisplaySeconds = 3f;

        // Caption text for each sound effect (agreed with the sound designer).
        private static readonly Dictionary<string, string> CaptionsBySound = new Dictionary<string, string>
        {
            { SoundNames.Trash, "[Basura recogida]" },
            { SoundNames.Seed, "[Semilla recogida]" },
            { SoundNames.Damage, "[Espinas del retamo: pierdes una gota]" },
            { SoundNames.Water, "[Agua contaminada: pierdes una gota]" },
            { SoundNames.Restore, "[El humedal se restaura]" },
            { SoundNames.Species, "[Canto de la tingua bogotana]" }
        };

        private GameObject captionPanel;
        private Text captionText;
        private float hideAtTime;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Receives the UI elements created by the HUD.
        /// </summary>
        public void Initialize(GameObject panel, Text text)
        {
            captionPanel = panel;
            captionText = text;
            captionPanel.SetActive(false);
        }

        /// <summary>
        /// Shows the caption associated with a sound, if it has one.
        /// </summary>
        public void ShowForSound(string soundName)
        {
            if (CaptionsBySound.TryGetValue(soundName, out string caption))
            {
                Show(caption);
            }
        }

        /// <summary>
        /// Shows any text as a caption. A new caption replaces the current one.
        /// </summary>
        public void Show(string caption)
        {
            // Captions requested before the HUD exists are ignored.
            if (captionPanel == null)
            {
                return;
            }

            captionText.text = caption;
            captionPanel.SetActive(true);
            hideAtTime = Time.time + DisplaySeconds;
        }

        private void Update()
        {
            // Hide the caption once its time is over.
            if (captionPanel != null && captionPanel.activeSelf && Time.time >= hideAtTime)
            {
                captionPanel.SetActive(false);
            }
        }
    }
}
