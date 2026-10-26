using System.Collections.Generic;
using GuardiansOfTheWetland.Core;
using GuardiansOfTheWetland.Gameplay;
using UnityEngine;

namespace GuardiansOfTheWetland.Level
{
    /// <summary>
    /// A ground tile and whether it is a surface (grass edge) or fill (soil) tile.
    /// The restoration needs this to choose the correct green sprite.
    /// </summary>
    public struct GroundTile
    {
        public SpriteRenderer Renderer;
        public bool IsSurface;
    }

    /// <summary>
    /// Everything other systems need to know about the level after it is built.
    /// </summary>
    public class LevelBuildResult
    {
        // World position where the player appears.
        public Vector3 SpawnPoint;

        // Rectangle that contains the whole level, used by the camera.
        public Rect Bounds;

        // Position of the finish sign, used to place the rescued species.
        public Vector3 FinishPoint;

        public int TrashCount;
        public int SeedCount;

        // Objects that change when the wetland is restored.
        public readonly List<GroundTile> GroundTiles = new List<GroundTile>();
        public readonly List<SpriteRenderer> WaterRenderers = new List<SpriteRenderer>();
        public readonly List<SpriteRenderer> GorseRenderers = new List<SpriteRenderer>();
        public readonly List<Hazard> Hazards = new List<Hazard>();
    }

    /// <summary>
    /// Converts the text map from <see cref="LevelData"/> into GameObjects:
    /// sprites for the visuals, colliders for the physics and triggers for the gameplay.
    /// </summary>
    public static class LevelBuilder
    {
        // Map symbols (see the legend in LevelData).
        public const char GroundSymbol = '#';
        public const char SpawnSymbol = 'P';
        public const char WaterSymbol = '~';
        public const char GorseSymbol = 'R';
        public const char TrashSymbol = 'T';
        public const char SeedSymbol = 'S';
        public const char FinishSymbol = 'F';

        // Drawing order: background < tiles < items < player (10).
        private const int TileSortingOrder = 0;
        private const int ItemSortingOrder = 5;

        // Thickness of the invisible walls placed at both ends of the level.
        private const float BoundaryWallWidth = 1f;

        /// <summary>
        /// Builds every tile of the map as a child of the given parent.
        /// </summary>
        /// <param name="map">Rows of the level, first row = top.</param>
        /// <param name="parent">Transform that groups all level objects in the Hierarchy.</param>
        public static LevelBuildResult Build(string[] map, Transform parent)
        {
            LevelBuildResult result = new LevelBuildResult();
            int rowCount = map.Length;
            int maxWidth = 0;

            // Walk through every row of the text map.
            for (int row = 0; row < rowCount; row++)
            {
                string line = map[row];
                maxWidth = Mathf.Max(maxWidth, line.Length);

                // Text rows grow downwards, world Y grows upwards, so the index is inverted.
                int worldY = rowCount - 1 - row;
                int column = 0;

                // Walk through the characters of this row.
                while (column < line.Length)
                {
                    char symbol = line[column];

                    if (symbol == GroundSymbol)
                    {
                        // Group consecutive ground tiles into one "run" so the whole run
                        // shares a single collider. This avoids the player getting stuck
                        // on the seams between separate box colliders.
                        int runStart = column;
                        while (column < line.Length && line[column] == GroundSymbol)
                        {
                            column++;
                        }

                        CreateGroundRun(map, row, runStart, column - runStart, worldY, parent, result);
                        continue;
                    }

                    Vector3 position = new Vector3(column * GameConfig.TileSize, worldY * GameConfig.TileSize, 0f);
                    CreateSingleTile(symbol, position, parent, result);
                    column++;
                }
            }

            // Tiles are centered on integer coordinates, so the level starts at -0.5.
            result.Bounds = new Rect(-0.5f, -0.5f, maxWidth * GameConfig.TileSize, rowCount * GameConfig.TileSize);

            CreateBoundaryWalls(result.Bounds, parent);
            return result;
        }

        /// <summary>
        /// Creates the object for any symbol that is not ground. Empty space creates nothing.
        /// </summary>
        private static void CreateSingleTile(char symbol, Vector3 position, Transform parent, LevelBuildResult result)
        {
            switch (symbol)
            {
                case SpawnSymbol:
                    result.SpawnPoint = position;
                    break;

                case WaterSymbol:
                    CreateWater(position, parent, result);
                    break;

                case GorseSymbol:
                    CreateGorse(position, parent, result);
                    break;

                case TrashSymbol:
                    CreateCollectible(CollectibleType.Trash, position, parent);
                    result.TrashCount++;
                    break;

                case SeedSymbol:
                    CreateCollectible(CollectibleType.Seed, position, parent);
                    result.SeedCount++;
                    break;

                case FinishSymbol:
                    CreateFinish(position, parent);
                    result.FinishPoint = position;
                    break;
            }
        }

        /// <summary>
        /// Creates a horizontal run of ground tiles with one shared collider.
        /// </summary>
        private static void CreateGroundRun(string[] map, int row, int startColumn, int length, int worldY,
            Transform parent, LevelBuildResult result)
        {
            GameObject run = new GameObject($"GroundRun_{row}_{startColumn}");
            run.transform.SetParent(parent);

            // The run object sits in the middle of its tiles.
            float centerX = startColumn + (length - 1) * 0.5f;
            run.transform.position = new Vector3(centerX, worldY, 0f);

            BoxCollider2D runCollider = run.AddComponent<BoxCollider2D>();
            runCollider.size = new Vector2(length, 1f);

            // Create one visual tile per column of the run.
            for (int i = 0; i < length; i++)
            {
                int column = startColumn + i;
                bool isSurface = IsSurfaceTile(map, row, column);

                // Surface tiles (grass edge) and fill tiles (soil) use different art.
                Sprite sprite = isSurface
                    ? SpriteFactory.Get("ground_top", GameConfig.GroundTopPolluted)
                    : SpriteFactory.Get("ground_fill", GameConfig.GroundFillPolluted);

                SpriteRenderer renderer = CreateSpriteObject($"Tile_{column}", sprite,
                    new Vector3(column, worldY, 0f), run.transform, TileSortingOrder);

                result.GroundTiles.Add(new GroundTile { Renderer = renderer, IsSurface = isSurface });
            }
        }

        /// <summary>
        /// Polluted water: not solid, so the player falls in and loses a drop.
        /// </summary>
        private static void CreateWater(Vector3 position, Transform parent, LevelBuildResult result)
        {
            SpriteRenderer renderer = CreateSpriteObject("Water",
                SpriteFactory.Get("water_polluted", GameConfig.WaterPlaceholder), position, parent, TileSortingOrder);

            // Slightly lower trigger so the player visibly touches the water before reacting.
            BoxCollider2D trigger = AddTrigger(renderer.gameObject, new Vector2(1f, 0.8f));
            trigger.offset = new Vector2(0f, -0.1f);

            Hazard hazard = renderer.gameObject.AddComponent<Hazard>();
            hazard.Initialize(HazardType.PollutedWater);

            result.WaterRenderers.Add(renderer);
            result.Hazards.Add(hazard);
        }

        /// <summary>
        /// Gorse ("retamo"): invasive thorny bush that pushes the player back.
        /// </summary>
        private static void CreateGorse(Vector3 position, Transform parent, LevelBuildResult result)
        {
            SpriteRenderer renderer = CreateSpriteObject("Gorse",
                SpriteFactory.Get("gorse", GameConfig.GorsePlaceholder), position, parent, ItemSortingOrder);

            AddTrigger(renderer.gameObject, new Vector2(0.7f, 0.8f));

            Hazard hazard = renderer.gameObject.AddComponent<Hazard>();
            hazard.Initialize(HazardType.Gorse);

            result.GorseRenderers.Add(renderer);
            result.Hazards.Add(hazard);
        }

        /// <summary>
        /// Trash or seed that floats in place until the player picks it up.
        /// </summary>
        private static void CreateCollectible(CollectibleType type, Vector3 position, Transform parent)
        {
            bool isTrash = type == CollectibleType.Trash;
            Sprite sprite = isTrash
                ? SpriteFactory.Get("trash", GameConfig.TrashPlaceholder)
                : SpriteFactory.Get("seed", GameConfig.SeedPlaceholder);

            SpriteRenderer renderer = CreateSpriteObject(isTrash ? "Trash" : "Seed", sprite, position, parent, ItemSortingOrder);

            CircleCollider2D trigger = renderer.gameObject.AddComponent<CircleCollider2D>();
            trigger.isTrigger = true;
            trigger.radius = 0.35f;

            Collectible collectible = renderer.gameObject.AddComponent<Collectible>();
            collectible.Initialize(type);
        }

        /// <summary>
        /// Finish sign. Its trigger is taller than one tile so it is easy to reach.
        /// </summary>
        private static void CreateFinish(Vector3 position, Transform parent)
        {
            SpriteRenderer renderer = CreateSpriteObject("Finish",
                SpriteFactory.Get("finish", GameConfig.FinishPlaceholder), position, parent, ItemSortingOrder);

            BoxCollider2D trigger = AddTrigger(renderer.gameObject, new Vector2(1f, 2f));
            trigger.offset = new Vector2(0f, 0.5f);

            renderer.gameObject.AddComponent<FinishPoint>();
        }

        /// <summary>
        /// Creates a GameObject with a SpriteRenderer at the given position.
        /// </summary>
        private static SpriteRenderer CreateSpriteObject(string objectName, Sprite sprite, Vector3 position,
            Transform parent, int sortingOrder)
        {
            GameObject spriteObject = new GameObject(objectName);
            spriteObject.transform.SetParent(parent);
            spriteObject.transform.position = position;

            SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = sortingOrder;
            return renderer;
        }

        private static BoxCollider2D AddTrigger(GameObject target, Vector2 size)
        {
            BoxCollider2D trigger = target.AddComponent<BoxCollider2D>();
            trigger.isTrigger = true;
            trigger.size = size;
            return trigger;
        }

        /// <summary>
        /// A ground tile is a surface tile when the cell directly above it is not ground.
        /// </summary>
        private static bool IsSurfaceTile(string[] map, int row, int column)
        {
            // The top row of the map is always a surface.
            if (row == 0)
            {
                return true;
            }

            string rowAbove = map[row - 1];
            return column >= rowAbove.Length || rowAbove[column] != GroundSymbol;
        }

        /// <summary>
        /// Adds invisible walls at the left and right edges so the player cannot leave the level.
        /// </summary>
        private static void CreateBoundaryWalls(Rect bounds, Transform parent)
        {
            float wallHeight = bounds.height * 3f;
            float wallCenterY = bounds.center.y + bounds.height;

            CreateWall("LeftWall", new Vector2(bounds.xMin - BoundaryWallWidth * 0.5f, wallCenterY), wallHeight, parent);
            CreateWall("RightWall", new Vector2(bounds.xMax + BoundaryWallWidth * 0.5f, wallCenterY), wallHeight, parent);
        }

        private static void CreateWall(string wallName, Vector2 position, float height, Transform parent)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.SetParent(parent);
            wall.transform.position = position;

            BoxCollider2D wallCollider = wall.AddComponent<BoxCollider2D>();
            wallCollider.size = new Vector2(BoundaryWallWidth, height);
        }
    }
}
