using UnityEngine;
using UnityEngine.UI;

namespace GuardiansOfTheWetland.UI
{
    /// <summary>
    /// Helper methods that create UI elements from code (canvas, panels, texts, icons),
    /// so the HUD does not need to be assembled in the Unity Editor.
    /// </summary>
    public static class UiFactory
    {
        // Reference resolution: the UI scales proportionally on any screen size.
        private static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

        private static Font cachedFont;

        /// <summary>
        /// Built-in font shipped with Unity. Unity 2022.2+ calls it "LegacyRuntime.ttf",
        /// older versions call it "Arial.ttf", so both names are tried.
        /// </summary>
        public static Font DefaultFont
        {
            get
            {
                if (cachedFont == null)
                {
                    cachedFont = TryLoadBuiltinFont("LegacyRuntime.ttf");
                }

                if (cachedFont == null)
                {
                    cachedFont = TryLoadBuiltinFont("Arial.ttf");
                }

                return cachedFont;
            }
        }

        /// <summary>
        /// Creates a screen-space canvas drawn on top of the game.
        /// </summary>
        public static Canvas CreateCanvas(string canvasName)
        {
            GameObject canvasObject = new GameObject(canvasName);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.matchWidthOrHeight = 0.5f;

            return canvas;
        }

        /// <summary>
        /// Creates a solid rectangle anchored to a point of its parent.
        /// </summary>
        /// <param name="anchor">Point of the parent the panel is attached to (0..1).</param>
        /// <param name="pivot">Point of the panel placed on the anchor (0..1).</param>
        /// <param name="position">Offset from the anchor, in reference pixels.</param>
        /// <param name="size">Width and height, in reference pixels.</param>
        public static RectTransform CreatePanel(Transform parent, string panelName, Color color,
            Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
        {
            GameObject panelObject = new GameObject(panelName, typeof(RectTransform));
            panelObject.transform.SetParent(parent, false);

            Image image = panelObject.AddComponent<Image>();
            image.color = color;

            RectTransform rect = (RectTransform)panelObject.transform;
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        /// <summary>
        /// Creates a text that fills its parent, leaving a small inner margin.
        /// </summary>
        public static Text CreateText(Transform parent, string textName, string content,
            int fontSize, Color color, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(textName, typeof(RectTransform));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.AddComponent<Text>();
            text.font = DefaultFont;
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            // Stretch to the parent with a 16 px margin on every side.
            RectTransform rect = (RectTransform)textObject.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(16f, 8f);
            rect.offsetMax = new Vector2(-16f, -8f);
            return text;
        }

        /// <summary>
        /// Creates a square image (icon) at a position relative to the parent's top-left corner.
        /// </summary>
        public static Image CreateIcon(Transform parent, string iconName, Sprite sprite, Color color,
            Vector2 topLeftPosition, float size)
        {
            GameObject iconObject = new GameObject(iconName, typeof(RectTransform));
            iconObject.transform.SetParent(parent, false);

            Image image = iconObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.preserveAspect = true;

            RectTransform rect = (RectTransform)iconObject.transform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = topLeftPosition;
            rect.sizeDelta = new Vector2(size, size);
            return image;
        }

        private static Font TryLoadBuiltinFont(string fontName)
        {
            // GetBuiltinResource throws if the name does not exist in this Unity version.
            try
            {
                return Resources.GetBuiltinResource<Font>(fontName);
            }
            catch (System.ArgumentException)
            {
                return null;
            }
        }
    }
}
