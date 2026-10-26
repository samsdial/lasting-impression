using UnityEngine;

namespace GuardiansOfTheWetland.Core
{
    /// <summary>
    /// Central place for shared tuning values and colors.
    /// Keeping every color here makes the accessibility contrast review (WCAG 2.2)
    /// easier, because all of them can be checked from a single file.
    /// </summary>
    public static class GameConfig
    {
        // ---------- World ----------

        // One tile of the level map equals one Unity world unit.
        public const float TileSize = 1f;

        // Half of the vertical size of the screen, in world units.
        public const float CameraOrthographicSize = 6f;

        // ---------- Gameplay ----------

        // Number of water drops (lives) the player starts with.
        public const int MaxHealth = 3;

        // Seconds the player cannot be hurt again after taking damage.
        public const float InvulnerabilitySeconds = 1.2f;

        // Duration of the gray-to-green restoration transition.
        public const float RestorationSeconds = 2.5f;

        // ---------- Palette: polluted (gray) state of the wetland ----------

        public static readonly Color SkyPolluted = HexToColor("8E979E");
        public static readonly Color GroundTopPolluted = HexToColor("6E6A64");
        public static readonly Color GroundFillPolluted = HexToColor("4F4B47");

        // ---------- Palette: restored (green) state of the wetland ----------

        public static readonly Color SkyRestored = HexToColor("7EC8E3");
        public static readonly Color GroundTopRestored = HexToColor("4CAF50");
        public static readonly Color GroundFillRestored = HexToColor("6D4C2F");

        // Tint applied to the water sprite once the wetland is clean.
        public static readonly Color WaterCleanTint = new Color(0.75f, 0.95f, 1f, 1f);

        // Restored tiles start darker and brighten up, so the change is easy to notice.
        public static readonly Color RestorationStartTint = new Color(0.55f, 0.55f, 0.55f, 1f);

        // ---------- Placeholder colors (used only while an art file is missing) ----------

        // The player uses a warm, saturated color so it stands out
        // against the desaturated background (visual hierarchy).
        public static readonly Color PlayerColor = HexToColor("F2B705");
        public static readonly Color WaterPlaceholder = HexToColor("4A5A4F");
        public static readonly Color GorsePlaceholder = HexToColor("8A6D1F");
        public static readonly Color TrashPlaceholder = HexToColor("B0B0B0");
        public static readonly Color SeedPlaceholder = HexToColor("8B5A2B");
        public static readonly Color FinishPlaceholder = HexToColor("2E8B57");

        // ---------- HUD (checked with WebAIM Contrast Checker) ----------

        // Every HUD text sits on this dark, opaque panel.
        public static readonly Color HudPanel = HexToColor("1B1F23");

        // White on #1B1F23 is about 16:1 (passes WCAG AA and AAA).
        public static readonly Color HudText = HexToColor("FFFFFF");

        // Gold on #1B1F23 is about 12:1. Gold on white would fail (about 1.4:1),
        // which is why the HUD never places gold text on light backgrounds.
        public static readonly Color HudAccent = HexToColor("FFD700");

        // Remaining and lost water drops in the health bar.
        public static readonly Color HudDropFull = HexToColor("4FC3F7");
        public static readonly Color HudDropEmpty = HexToColor("4A5560");

        /// <summary>
        /// Converts a hexadecimal string such as "FFD700" into a Unity Color.
        /// </summary>
        /// <param name="hex">Six-digit hex color without the leading '#'.</param>
        /// <returns>The parsed color, or magenta if the text is not valid.</returns>
        public static Color HexToColor(string hex)
        {
            // ColorUtility expects the HTML format, which starts with '#'.
            if (ColorUtility.TryParseHtmlString("#" + hex, out Color color))
            {
                return color;
            }

            // Magenta is the standard "missing" color, easy to spot during testing.
            Debug.LogWarning($"[GameConfig] Invalid hex color '{hex}'. Using magenta.");
            return Color.magenta;
        }
    }
}
