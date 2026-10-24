using System.Collections.Generic;
using UnityEngine;

namespace GuardiansOfTheWetland.Core
{
    /// <summary>
    /// Supplies the sprite for every object in the game.
    /// It first looks for the final art inside "Assets/Resources/Sprites".
    /// If that file does not exist yet, it generates a solid-color placeholder,
    /// so the game is always playable while the art is still being produced.
    /// </summary>
    public static class SpriteFactory
    {
        // Size in pixels of the generated placeholder textures.
        private const int PlaceholderResolution = 16;

        // Darkening factor used for the 1-pixel border of each placeholder tile.
        private const float BorderShade = 0.75f;

        // Generated placeholders are cached so each color is only created once.
        private static readonly Dictionary<Color, Sprite> placeholderCache =
            new Dictionary<Color, Sprite>();

        /// <summary>
        /// Returns the art sprite with the given name, or a placeholder if it is missing.
        /// </summary>
        /// <param name="spriteName">File name inside Resources/Sprites, without extension.</param>
        /// <param name="placeholderColor">Color used when the art file is not available.</param>
        public static Sprite Get(string spriteName, Color placeholderColor)
        {
            // Resources.Load returns null when the file does not exist.
            Sprite artSprite = Resources.Load<Sprite>("Sprites/" + spriteName);
            if (artSprite != null)
            {
                return artSprite;
            }

            return GetPlaceholder(placeholderColor);
        }

        /// <summary>
        /// Creates (or reuses) a square sprite of one world unit filled with a color.
        /// </summary>
        public static Sprite GetPlaceholder(Color color)
        {
            // Reuse the sprite if this color was already generated.
            if (placeholderCache.TryGetValue(color, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            int size = PlaceholderResolution;
            Texture2D texture = new Texture2D(size, size);

            // Point filtering keeps hard pixel edges instead of blurring them.
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Color borderColor = color * BorderShade;
            borderColor.a = color.a;

            Color[] pixels = new Color[size * size];

            // Loop through every pixel: the outer ring gets the darker border color
            // so neighbouring tiles remain distinguishable from each other.
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isBorder = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                    pixels[y * size + x] = isBorder ? borderColor : color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            // Pixels-per-unit equal to the resolution makes the sprite exactly 1x1 unit.
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size);

            placeholderCache[color] = sprite;
            return sprite;
        }
    }
}
