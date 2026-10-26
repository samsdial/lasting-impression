using System.Collections;
using UnityEngine;

namespace GuardiansOfTheWetland.Level
{
    /// <summary>
    /// Draws the wetland landscape behind the level and crossfades it
    /// from the polluted version to the restored version.
    /// The background is a child of the camera, so it always fills the screen.
    /// </summary>
    public class BackgroundController : MonoBehaviour
    {
        // Far behind every other sprite.
        private const int PollutedSortingOrder = -100;
        private const int RestoredSortingOrder = -99;

        // Distance in front of the camera (the camera sits at z = -10).
        private const float DepthInFrontOfCamera = 10f;

        private Camera targetCamera;
        private SpriteRenderer pollutedRenderer;
        private SpriteRenderer restoredRenderer;

        /// <summary>
        /// Creates the background if the art exists; returns null otherwise
        /// (the camera's plain color is used instead).
        /// </summary>
        public static BackgroundController Create(Camera camera)
        {
            Sprite pollutedSprite = Resources.Load<Sprite>("Sprites/background_polluted");
            Sprite restoredSprite = Resources.Load<Sprite>("Sprites/background_restored");

            if (pollutedSprite == null)
            {
                return null;
            }

            GameObject root = new GameObject("Background");
            root.transform.SetParent(camera.transform, false);
            root.transform.localPosition = new Vector3(0f, 0f, DepthInFrontOfCamera);

            BackgroundController controller = root.AddComponent<BackgroundController>();
            controller.targetCamera = camera;
            controller.pollutedRenderer = controller.CreateLayer("Polluted", pollutedSprite, PollutedSortingOrder);

            // The restored layer starts fully transparent on top of the polluted one.
            if (restoredSprite != null)
            {
                controller.restoredRenderer = controller.CreateLayer("Restored", restoredSprite, RestoredSortingOrder);
                controller.SetAlpha(controller.restoredRenderer, 0f);
            }

            controller.FitToCamera();
            return controller;
        }

        // Refit every frame in case the window is resized.
        private void LateUpdate()
        {
            FitToCamera();
        }

        /// <summary>
        /// Fades in the restored landscape over the given time.
        /// </summary>
        public void CrossfadeToRestored(float duration)
        {
            if (restoredRenderer != null)
            {
                StartCoroutine(CrossfadeRoutine(duration));
            }
        }

        private IEnumerator CrossfadeRoutine(float duration)
        {
            float elapsed = 0f;

            // Increase the opacity of the restored layer a little each frame.
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                SetAlpha(restoredRenderer, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            SetAlpha(restoredRenderer, 1f);
        }

        private SpriteRenderer CreateLayer(string layerName, Sprite sprite, int sortingOrder)
        {
            GameObject layer = new GameObject(layerName);
            layer.transform.SetParent(transform, false);

            SpriteRenderer renderer = layer.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        /// <summary>
        /// Scales the background so it covers the whole visible area without gaps.
        /// </summary>
        private void FitToCamera()
        {
            if (targetCamera == null || pollutedRenderer == null)
            {
                return;
            }

            float viewHeight = targetCamera.orthographicSize * 2f;
            float viewWidth = viewHeight * targetCamera.aspect;
            Vector2 spriteSize = pollutedRenderer.sprite.bounds.size;

            // Use the larger ratio so both width and height are covered.
            float scale = Mathf.Max(viewWidth / spriteSize.x, viewHeight / spriteSize.y);
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        private void SetAlpha(SpriteRenderer renderer, float alpha)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }
}
