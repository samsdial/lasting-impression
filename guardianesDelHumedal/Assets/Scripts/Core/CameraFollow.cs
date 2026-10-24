using UnityEngine;

namespace GuardiansOfTheWetland.Core
{
    /// <summary>
    /// Smoothly follows a target (the player) and keeps the view
    /// inside the limits of the level, so the empty area outside the map is never shown.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [Tooltip("Time in seconds the camera takes to catch up with the target.")]
        public float smoothTime = 0.15f;

        [Tooltip("Offset from the target, so the player sees a bit more ahead and above.")]
        public Vector2 offset = new Vector2(2f, 1f);

        // Cameras in 2D must stay behind the sprites on the Z axis.
        private const float CameraDepth = -10f;

        private Transform target;
        private Rect levelBounds;
        private Camera followCamera;
        private Vector3 currentVelocity;

        /// <summary>
        /// Assigns the object to follow and the rectangle the camera must stay within.
        /// </summary>
        public void Setup(Transform followTarget, Rect bounds)
        {
            target = followTarget;
            levelBounds = bounds;
            followCamera = GetComponent<Camera>();

            // Jump directly to the start position to avoid an initial slide.
            transform.position = GetDesiredPosition();
        }

        // LateUpdate runs after the player has moved in Update, which prevents jitter.
        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            transform.position = Vector3.SmoothDamp(
                transform.position,
                GetDesiredPosition(),
                ref currentVelocity,
                smoothTime);
        }

        /// <summary>
        /// Calculates where the camera should be, clamped to the level limits.
        /// </summary>
        private Vector3 GetDesiredPosition()
        {
            Vector3 desired = target.position + (Vector3)offset;

            // Half of the visible area, in world units.
            float halfHeight = followCamera.orthographicSize;
            float halfWidth = halfHeight * followCamera.aspect;

            desired.x = ClampAxis(desired.x, levelBounds.xMin + halfWidth, levelBounds.xMax - halfWidth);
            desired.y = ClampAxis(desired.y, levelBounds.yMin + halfHeight, levelBounds.yMax - halfHeight);
            desired.z = CameraDepth;
            return desired;
        }

        /// <summary>
        /// Clamps a value between two limits. If the level is smaller than the screen
        /// on that axis (min greater than max), the camera stays centered instead.
        /// </summary>
        private static float ClampAxis(float value, float min, float max)
        {
            if (min > max)
            {
                return (min + max) * 0.5f;
            }

            return Mathf.Clamp(value, min, max);
        }
    }
}
