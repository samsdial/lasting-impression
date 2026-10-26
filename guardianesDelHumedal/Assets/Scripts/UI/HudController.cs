using GuardiansOfTheWetland.Audio;
using GuardiansOfTheWetland.Core;
using GuardiansOfTheWetland.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace GuardiansOfTheWetland.UI
{
    /// <summary>
    /// Heads-up display built entirely from code:
    /// - Top-left: water drops (health), trash and seed counters with their icons.
    /// - Top-right: music and effects volume with the keys that change them.
    /// - Bottom-left: controls reminder.
    /// - Bottom-center: subtitle panel (handled by <see cref="SubtitleManager"/>).
    /// - Top-center: banner for big moments (restoration).
    /// All texts use white or gold on a dark opaque panel to keep contrast above 4.5:1.
    /// </summary>
    public class HudController : MonoBehaviour
    {
        /// <summary>Single active HUD.</summary>
        public static HudController Instance { get; private set; }

        private const int BodyFontSize = 34;
        private const int SmallFontSize = 26;
        private const int BannerFontSize = 48;
        private const float IconSize = 44f;
        private const float RowHeight = 56f;

        // A counter grows briefly when it changes (visual feedback for collecting).
        private const float PulseSeconds = 0.35f;
        private const float PulseExtraScale = 0.3f;

        private PlayerHealth playerHealth;
        private Image[] dropImages;
        private Text healthText;
        private Text trashText;
        private Text seedText;
        private Text volumeText;
        private GameObject bannerPanel;
        private Text bannerText;
        private float bannerHideTime;

        private int lastTrash = -1;
        private int lastSeeds = -1;
        private float trashPulseEnd;
        private float seedPulseEnd;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            // Stop listening to events of objects that may outlive the HUD.
            if (GameSession.Instance != null)
            {
                GameSession.Instance.CountsChanged -= RefreshCounters;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.VolumesChanged -= RefreshVolume;
            }

            if (playerHealth != null)
            {
                playerHealth.HealthChanged -= RefreshHealth;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Builds every HUD element and subscribes to the game events.
        /// </summary>
        public void Initialize(Canvas canvas, PlayerHealth health, SubtitleManager subtitles)
        {
            playerHealth = health;
            Transform root = canvas.transform;

            BuildStatsPanel(root);
            BuildVolumePanel(root);
            BuildControlsPanel(root);
            BuildSubtitlePanel(root, subtitles);
            BuildBanner(root);

            // Events keep the HUD updated without checking every frame.
            if (GameSession.Instance != null)
            {
                GameSession.Instance.CountsChanged += RefreshCounters;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.VolumesChanged += RefreshVolume;
            }

            playerHealth.HealthChanged += RefreshHealth;

            RefreshHealth();
            RefreshCounters();
            RefreshVolume();
        }

        /// <summary>
        /// Shows a large message at the top of the screen for a few seconds.
        /// </summary>
        public void ShowBanner(string message, float seconds)
        {
            bannerText.text = message;
            bannerPanel.SetActive(true);
            bannerHideTime = Time.time + seconds;
        }

        private void Update()
        {
            // Animate the counters that changed recently.
            ApplyPulse(trashText, trashPulseEnd);
            ApplyPulse(seedText, seedPulseEnd);

            if (bannerPanel.activeSelf && Time.time >= bannerHideTime)
            {
                bannerPanel.SetActive(false);
            }
        }

        // ---------- Building ----------

        private void BuildStatsPanel(Transform root)
        {
            RectTransform panel = UiFactory.CreatePanel(root, "StatsPanel", GameConfig.HudPanel,
                new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -24f), new Vector2(460f, 200f));

            // Row 1: one drop icon per point of health, plus the number (not only color).
            dropImages = new Image[GameConfig.MaxHealth];
            for (int i = 0; i < dropImages.Length; i++)
            {
                dropImages[i] = UiFactory.CreateIcon(panel, "Drop" + i, null, GameConfig.HudDropFull,
                    new Vector2(20f + i * 34f, -24f), 24f);
            }

            healthText = CreateRowText(panel, "HealthText", 0, 20f + GameConfig.MaxHealth * 34f);

            // Row 2 and 3: the same sprites used in the level, so players match icon and object.
            UiFactory.CreateIcon(panel, "TrashIcon", SpriteFactory.Get("trash", GameConfig.TrashPlaceholder),
                Color.white, new Vector2(16f, -16f - RowHeight), IconSize);
            trashText = CreateRowText(panel, "TrashText", 1, 72f);

            UiFactory.CreateIcon(panel, "SeedIcon", SpriteFactory.Get("seed", GameConfig.SeedPlaceholder),
                Color.white, new Vector2(16f, -16f - RowHeight * 2f), IconSize);
            seedText = CreateRowText(panel, "SeedText", 2, 72f);
        }

        private void BuildVolumePanel(Transform root)
        {
            RectTransform panel = UiFactory.CreatePanel(root, "VolumePanel", GameConfig.HudPanel,
                new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-24f, -24f), new Vector2(520f, 90f));

            volumeText = UiFactory.CreateText(panel, "VolumeText", string.Empty, SmallFontSize,
                GameConfig.HudText, TextAnchor.MiddleLeft);
        }

        private void BuildControlsPanel(Transform root)
        {
            RectTransform panel = UiFactory.CreatePanel(root, "ControlsPanel", GameConfig.HudPanel,
                new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(24f, 24f), new Vector2(560f, 90f));

            // Alternative input options are listed so every player knows they exist.
            UiFactory.CreateText(panel, "ControlsText",
                "Mover: A/D, flechas o palanca\nSaltar: Espacio, W, flecha arriba o botón A",
                SmallFontSize, GameConfig.HudText, TextAnchor.MiddleLeft);
        }

        private void BuildSubtitlePanel(Transform root, SubtitleManager subtitles)
        {
            RectTransform panel = UiFactory.CreatePanel(root, "SubtitlePanel", GameConfig.HudPanel,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 130f), new Vector2(1100f, 80f));

            Text text = UiFactory.CreateText(panel, "SubtitleText", string.Empty, BodyFontSize,
                GameConfig.HudText, TextAnchor.MiddleCenter);

            subtitles.Initialize(panel.gameObject, text);
        }

        private void BuildBanner(Transform root)
        {
            RectTransform panel = UiFactory.CreatePanel(root, "BannerPanel", GameConfig.HudPanel,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -150f), new Vector2(1000f, 110f));

            bannerText = UiFactory.CreateText(panel, "BannerText", string.Empty, BannerFontSize,
                GameConfig.HudAccent, TextAnchor.MiddleCenter);

            bannerPanel = panel.gameObject;
            bannerPanel.SetActive(false);
        }

        /// <summary>
        /// Creates a text positioned on one row of the stats panel.
        /// </summary>
        private static Text CreateRowText(RectTransform panel, string textName, int row, float leftMargin)
        {
            Text text = UiFactory.CreateText(panel, textName, string.Empty, BodyFontSize,
                GameConfig.HudText, TextAnchor.MiddleLeft);

            // Re-anchor the text to its row instead of the whole panel.
            RectTransform rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.offsetMin = new Vector2(leftMargin, -12f - RowHeight * (row + 1));
            rect.offsetMax = new Vector2(-12f, -12f - RowHeight * row);
            return text;
        }

        // ---------- Refreshing ----------

        private void RefreshHealth()
        {
            // Full drops are blue, lost drops are gray; the number repeats the information.
            for (int i = 0; i < dropImages.Length; i++)
            {
                dropImages[i].color = i < playerHealth.CurrentHealth ? GameConfig.HudDropFull : GameConfig.HudDropEmpty;
            }

            healthText.text = $"Gotas {playerHealth.CurrentHealth}/{playerHealth.MaxHealth}";
        }

        private void RefreshCounters()
        {
            GameSession session = GameSession.Instance;
            if (session == null)
            {
                return;
            }

            // Start a pulse only when the value really changed (not on the first refresh).
            if (lastTrash >= 0 && session.TrashCollected != lastTrash)
            {
                trashPulseEnd = Time.time + PulseSeconds;
            }

            if (lastSeeds >= 0 && session.SeedsCollected != lastSeeds)
            {
                seedPulseEnd = Time.time + PulseSeconds;
            }

            lastTrash = session.TrashCollected;
            lastSeeds = session.SeedsCollected;

            trashText.text = $"Basura {session.TrashCollected}/{session.TrashTotal}";
            seedText.text = $"Semillas {session.SeedsCollected}/{session.SeedsTotal}";

            // Gold highlights a completed goal.
            trashText.color = session.AllTrashCollected ? GameConfig.HudAccent : GameConfig.HudText;
        }

        private void RefreshVolume()
        {
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager == null)
            {
                return;
            }

            int music = Mathf.RoundToInt(audioManager.MusicVolume * 100f);
            int effects = Mathf.RoundToInt(audioManager.SfxVolume * 100f);
            volumeText.text = $"Música {music}%  (teclas 1 / 2)\nEfectos {effects}%  (teclas 3 / 4)";
        }

        /// <summary>
        /// Scales a text up while its pulse is active, then back to normal size.
        /// </summary>
        private static void ApplyPulse(Text text, float pulseEndTime)
        {
            float remaining = pulseEndTime - Time.time;
            float scale = remaining > 0f ? 1f + PulseExtraScale * (remaining / PulseSeconds) : 1f;
            text.rectTransform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
