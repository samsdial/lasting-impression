using GuardiansOfTheWetland.Level;
using GuardiansOfTheWetland.Player;
using UnityEngine;

namespace GuardiansOfTheWetland.Core
{
    /// <summary>
    /// Entry point of "Guardians of the Wetland".
    /// When Play is pressed, this class builds the entire scene from code:
    /// camera, level, player and camera follow. No manual setup in the Editor is needed.
    /// </summary>
    public static class GameBootstrap
    {
        private const string CameraTag = "MainCamera";

        /// <summary>
        /// Called automatically by Unity once the first scene has finished loading.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BuildGame()
        {
            Camera mainCamera = SetupCamera();

            // Empty parent object that groups every tile in the Hierarchy window.
            Transform levelRoot = new GameObject("Level").transform;
            LevelBuildResult level = LevelBuilder.Build(LevelData.Map, levelRoot);

            PlayerController player = CreatePlayer(level.SpawnPoint);

            CameraFollow cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
            cameraFollow.Setup(player.transform, level.Bounds);

            Debug.Log("[GameBootstrap] Scene built from code.");
        }

        /// <summary>
        /// Reuses the scene's main camera (or creates one) and configures it for 2D.
        /// </summary>
        private static Camera SetupCamera()
        {
            Camera mainCamera = Camera.main;

            // If the scene has no camera, create one from scratch.
            if (mainCamera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = CameraTag;
                mainCamera = cameraObject.AddComponent<Camera>();
            }

            mainCamera.orthographic = true;
            mainCamera.orthographicSize = GameConfig.CameraOrthographicSize;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = GameConfig.SkyPolluted;
            return mainCamera;
        }

        /// <summary>
        /// Creates the player GameObject with its sprite, physics and controller.
        /// </summary>
        private static PlayerController CreatePlayer(Vector3 spawnPoint)
        {
            GameObject playerObject = new GameObject("Player");

            // The SpriteRenderer is added first because PlayerController reads it in Awake.
            SpriteRenderer renderer = playerObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Get("player_idle", GameConfig.PlayerColor);

            // Higher sorting order draws the player in front of the level tiles.
            renderer.sortingOrder = 10;

            // RequireComponent adds the Rigidbody2D and BoxCollider2D automatically.
            PlayerController controller = playerObject.AddComponent<PlayerController>();
            controller.Initialize(spawnPoint);
            return controller;
        }
    }
}
