#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GuardiansOfTheWetland.EditorTools
{
    /// <summary>
    /// Applies the correct import settings automatically to every art and audio file
    /// added to the Resources folders. Teammates only need to drop the files in the
    /// right folder (even from the GitHub website); nobody has to configure them by hand.
    /// </summary>
    public class GameAssetImportProcessor : AssetPostprocessor
    {
        private const string SpriteFolder = "Assets/Resources/Sprites/";
        private const string AudioFolder = "Assets/Resources/Audio/";

        // One level tile is 64x64 pixels, so 64 pixels equal one world unit.
        private const int PixelsPerTile = 64;

        // Music files start with this prefix; they are streamed instead of fully loaded.
        private const string MusicPrefix = "music_";

        /// <summary>
        /// Called by Unity before importing any texture.
        /// </summary>
        private void OnPreprocessTexture()
        {
            // Only touch files inside the game's sprite folder.
            if (!assetPath.StartsWith(SpriteFolder))
            {
                return;
            }

            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerTile;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;

            // Point filter and no compression keep pixel art sharp and colors exact,
            // which also preserves the contrast values checked for accessibility.
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
        }

        /// <summary>
        /// Called by Unity before importing any audio clip.
        /// </summary>
        private void OnPreprocessAudio()
        {
            // Only touch files inside the game's audio folder.
            if (!assetPath.StartsWith(AudioFolder))
            {
                return;
            }

            AudioImporter importer = (AudioImporter)assetImporter;
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;

            bool isMusic = Path.GetFileName(assetPath).StartsWith(MusicPrefix);

            // Long music tracks are streamed to save memory;
            // short effects are decompressed on load so they play instantly.
            settings.loadType = isMusic
                ? AudioClipLoadType.Streaming
                : AudioClipLoadType.DecompressOnLoad;

            importer.defaultSampleSettings = settings;
        }
    }
}
#endif
