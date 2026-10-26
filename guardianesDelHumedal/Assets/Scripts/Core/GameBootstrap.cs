using GuardiansOfTheWetland.Audio;
using GuardiansOfTheWetland.Gameplay;
using GuardiansOfTheWetland.Level;
using GuardiansOfTheWetland.Player;
using GuardiansOfTheWetland.UI;
using UnityEngine;

namespace GuardiansOfTheWetland.Core
{
    /// <summary>
    /// Entry point of "Guardians of the Wetland".
    /// When Play is pressed, this class builds the entire game from code:
    /// systems (session, audio, subtitles), background, level, player, camera and HUD.
    /// No manual setup in the Editor is needed.
    /// </summary>
    public static class GameBootstrap
    {
        private const string CameraTag = "MainCamera";
        private const int PlayerSortingOrder = 10;

        /// <summary>
        /// Called automatically by Unity once the first scene has finished loading.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BuildGame()
        {
            Camera mainCamera = SetupCamera();

            // Systems are created first because the level and the HUD use them.
            GameObject systems = new GameObject("GameSystems");
            GameSession session = systems.AddComponent<GameSession>();
            systems.AddComponent<AudioManager>();
            SubtitleManager subtitles = systems.AddComponent<SubtitleManager>();

            BackgroundController background = BackgroundController.Create(mainCamera);

            // Empty parent object that groups every tile in the Hierarchy window.
            Transform levelRoot = new GameObject("Level").transform;
            LevelBuildResult level = LevelBuilder.Build(LevelData.Map, levelRoot);
            session.SetTotals(level.TrashCount, level.SeedCount);

            PlayerController player = CreatePlayer(level.SpawnPoint);

            CameraFollow cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
            cameraFollow.Setup(player.transform, level.Bounds);

            Canvas canvas = UiFactory.CreateCanvas("HUD");
            HudController hud = canvas.gameObject.AddComponent<HudController>();
            hud.Initialize(canvas, player.GetComponent<PlayerHealth>(), subtitles);

            RestorationController restoration = systems.AddComponent<RestorationController>();
            restoration.Initialize(level, background, mainCamera);

            // First caption tells the player the goal of the level.
            subtitles.Show("Recoge toda la basura y llega al letrero para restaurar el humedal");

            Debug.Log("[GameBootstrap] Game built from code.");
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

            // Without an AudioListener no sound would be heard.
            if (mainCamera.GetComponent<AudioListener>() == null)
            {
                mainCamera.gameObject.AddComponent<AudioListener>();
            }

            mainCamera.orthographic = true;
            mainCamera.orthographicSize = GameConfig.CameraOrthographicSize;
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = GameConfig.SkyPolluted;
            return mainCamera;
        }

        /// <summary>
        /// Creates the player GameObject with its sprite, physics, animation and health.
        /// </summary>
        private static PlayerController CreatePlayer(Vector3 spawnPoint)
        {
            GameObject playerObject = new GameObject("Player");

            // The SpriteRenderer is added first because the other components read it in Awake.
            SpriteRenderer renderer = playerObject.AddComponent<SpriteRenderer>();
            renderer.sprite = SpriteFactory.Get("player_idle", GameConfig.PlayerColor);

            // Higher sorting order draws the player in front of the level tiles.
            renderer.sortingOrder = PlayerSortingOrder;

            // RequireComponent adds the Rigidbody2D and BoxCollider2D automatically.
            PlayerController controller = playerObject.AddComponent<PlayerController>();
            controller.Initialize(spawnPoint);

            // Swaps the sprite according to the state (idle, walk, jump, hurt). Made by Esteban.
            playerObject.AddComponent<PlayerAnimator>();

            // Water drops (lives); added after the animator so it can trigger the hurt sprite.
            playerObject.AddComponent<PlayerHealth>();

            return controller;
        }
    }
}
