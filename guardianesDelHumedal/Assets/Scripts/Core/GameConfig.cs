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

        // ---------- Palette: polluted (gray) state of the wetland ----------

        public static readonly Color SkyPolluted = HexToColor("8E979E");
        public static readonly Color GroundTopPolluted = HexToColor("6E6A64");
        public static readonly Color GroundFillPolluted = HexToColor("4F4B47");

        // The player uses a warm, saturated color so it stands out
        // against the desaturated background (visual hierarchy).
        public static readonly Color PlayerColor = HexToColor("F2B705");

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
