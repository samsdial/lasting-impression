using System.Collections;
using GuardiansOfTheWetland.Audio;
using GuardiansOfTheWetland.Core;
using GuardiansOfTheWetland.Level;
using GuardiansOfTheWetland.UI;
using UnityEngine;

namespace GuardiansOfTheWetland.Gameplay
{
    /// <summary>
    /// Turns the polluted (gray) wetland into a restored (green) one when the player
    /// reaches the finish with all the trash. This is the core of the game's message:
    /// the player's actions visibly and audibly heal the ecosystem.
    /// </summary>
    public class RestorationController : MonoBehaviour
    {
        // Seconds to wait after the transition before the rescued species appears.
        private const float SpeciesDelay = 0.5f;
        private const float BannerSeconds = 6f;

        // The species appears one tile left of the finish sign and one tile up.
        private static readonly Vector3 SpeciesOffset = new Vector3(-1f, 1f, 0f);

        private LevelBuildResult level;
        private BackgroundController background;
        private Camera mainCamera;

        /// <summary>
        /// Stores references to the level objects and listens for the restoration event.
        /// </summary>
        public void Initialize(LevelBuildResult builtLevel, BackgroundController levelBackground, Camera camera)
        {
            level = builtLevel;
            background = levelBackground;
            mainCamera = camera;

            if (GameSession.Instance != null)
            {
                GameSession.Instance.Restored += OnRestored;
            }
        }

        private void OnDestroy()
        {
            if (GameSession.Instance != null)
            {
                GameSession.Instance.Restored -= OnRestored;
            }
        }

        private void OnRestored()
        {
            StartCoroutine(RestoreRoutine());
        }

        private IEnumerator RestoreRoutine()
        {
            float duration = GameConfig.RestorationSeconds;

            // Sound and music change first, so the moment is heard as well as seen.
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(SoundNames.Restore);
                AudioManager.Instance.CrossfadeToCalmMusic(duration);
            }

            // A restored wetland is safe: hazards stop hurting the player.
            foreach (Hazard hazard in level.Hazards)
            {
                hazard.IsActive = false;
            }

            SwapGroundSprites();

            if (background != null)
            {
                background.CrossfadeToRestored(duration);
            }

            float elapsed = 0f;

            // Animate every frame until the transition is complete.
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                ApplyTransition(Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            ApplyTransition(1f);

            yield return new WaitForSeconds(SpeciesDelay);
            ShowRescuedSpecies();

            if (HudController.Instance != null)
            {
                HudController.Instance.ShowBanner("¡Humedal restaurado! Gracias, guardián", BannerSeconds);
            }
        }

        /// <summary>
        /// Replaces the gray ground sprites with the green ones, starting slightly dark.
        /// </summary>
        private void SwapGroundSprites()
        {
            Sprite restoredTop = SpriteFactory.Get("ground_top_restored", GameConfig.GroundTopRestored);
            Sprite restoredFill = SpriteFactory.Get("ground_fill_restored", GameConfig.GroundFillRestored);

            foreach (GroundTile tile in level.GroundTiles)
            {
                tile.Renderer.sprite = tile.IsSurface ? restoredTop : restoredFill;
                tile.Renderer.color = GameConfig.RestorationStartTint;
            }
        }

        /// <summary>
        /// Applies one step of the transition. Progress goes from 0 (start) to 1 (fully restored).
        /// </summary>
        private void ApplyTransition(float progress)
        {
            // Ground brightens from the dark start tint to its natural colors.
            Color groundTint = Color.Lerp(GameConfig.RestorationStartTint, Color.white, progress);
            foreach (GroundTile tile in level.GroundTiles)
            {
                tile.Renderer.color = groundTint;
            }

            // Water becomes clear and blue.
            Color waterTint = Color.Lerp(Color.white, GameConfig.WaterCleanTint, progress);
            foreach (SpriteRenderer water in level.WaterRenderers)
            {
                water.color = waterTint;
            }

            // The invasive gorse fades away.
            foreach (SpriteRenderer gorse in level.GorseRenderers)
            {
                Color color = gorse.color;
                color.a = 1f - progress;
                gorse.color = color;
            }

            // Sky color (visible only when there is no background image).
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = Color.Lerp(GameConfig.SkyPolluted, GameConfig.SkyRestored, progress);
            }
        }

        /// <summary>
        /// Shows the species that returns to the wetland, with its call and subtitle.
        /// </summary>
        private void ShowRescuedSpecies()
        {
            Sprite speciesSprite = Resources.Load<Sprite>("Sprites/species");

            // The species is drawn only when its art exists; the sound and subtitle play anyway.
            if (speciesSprite != null)
            {
                GameObject species = new GameObject("RescuedSpecies");
                species.transform.position = level.FinishPoint + SpeciesOffset;

                SpriteRenderer renderer = species.AddComponent<SpriteRenderer>();
                renderer.sprite = speciesSprite;
                renderer.sortingOrder = 6;
            }

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(SoundNames.Species);
            }
        }
    }
}
