using GuardiansOfTheWetland.Core;
using UnityEngine;

namespace GuardiansOfTheWetland.Level
{
    /// <summary>
    /// Information returned after the level is built.
    /// </summary>
    public struct LevelBuildResult
    {
        // World position where the player appears.
        public Vector3 SpawnPoint;

        // Rectangle that contains the whole level, used by the camera.
        public Rect Bounds;
    }

    /// <summary>
    /// Converts the text map from <see cref="LevelData"/> into GameObjects:
    /// sprites for the visuals and colliders for the physics.
    /// </summary>
    public static class LevelBuilder
    {
        // Map symbols handled in this block.
        public const char GroundSymbol = '#';
        public const char SpawnSymbol = 'P';

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
            result.SpawnPoint = Vector3.zero;

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

                        CreateGroundRun(map, row, runStart, column - runStart, worldY, parent);
                        continue;
                    }

                    if (symbol == SpawnSymbol)
                    {
                        result.SpawnPoint = new Vector3(column * GameConfig.TileSize, worldY * GameConfig.TileSize, 0f);
                    }

                    column++;
                }
            }

            // Tiles are centered on integer coordinates, so the level starts at -0.5.
            result.Bounds = new Rect(-0.5f, -0.5f, maxWidth * GameConfig.TileSize, rowCount * GameConfig.TileSize);

            CreateBoundaryWalls(result.Bounds, parent);
            return result;
        }

        /// <summary>
        /// Creates a horizontal run of ground tiles with one shared collider.
        /// </summary>
        private static void CreateGroundRun(string[] map, int row, int startColumn, int length, int worldY, Transform parent)
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

                GameObject tile = new GameObject($"Tile_{column}");
                tile.transform.SetParent(run.transform);
                tile.transform.position = new Vector3(column, worldY, 0f);

                SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();

                // Surface tiles (grass edge) and fill tiles (soil) use different art.
                renderer.sprite = isSurface
                    ? SpriteFactory.Get("ground_top", GameConfig.GroundTopPolluted)
                    : SpriteFactory.Get("ground_fill", GameConfig.GroundFillPolluted);
            }
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
