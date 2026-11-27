namespace GuardiansOfTheWetland.Level
{
    /// <summary>
    /// Text-based layout of the level. Each character is one 1x1 tile.
    /// The first string is the TOP row of the level and the last one is the BOTTOM row.
    ///
    /// Legend:
    ///   '.'  empty space
    ///   '#'  ground or platform (solid)
    ///   'P'  player spawn point
    ///   '~'  polluted water   (hazard: lose a drop and return to the start)
    ///   'R'  gorse "retamo"   (hazard: lose a drop and get pushed back)
    ///   'T'  trash            (collectible, all of it is required to restore)
    ///   'S'  native seed      (collectible)
    ///   'F'  finish / restoration point
    ///   'M'  moving platform  (machine mechanic: travels 5 tiles to the right and back)
    ///
    /// Unknown characters are ignored by the builder.
    ///
    /// Design decisions (see doc: docs/aportes/mariana.md):
    ///   - 11 rows x 60 columns, every row the same length.
    ///   - Player jump reaches 3.2 tiles (PlayerController.jumpHeight),
    ///     so vertical rises between platforms stay <= 3 and gaps <= 4.
    ///   - 8 trash (T), 5 seeds (S), 1 spawn (P), 1 finish (F).
    ///   - The three ground gaps are polluted water (~) and two gorse bushes (R)
    ///     guard the path, so every hazard sound and subtitle appears in the level.
    ///   - Difficulty grows from left to right: low, wide platforms near the
    ///     spawn become smaller and higher platforms toward the finish.
    ///   - The moving platform (M) starts on the shore at column 47 and crosses the
    ///     last water gap (columns 48-51). The gap is still 4 tiles wide, so it can
    ///     also be jumped: the platform is a second, slower way through the same
    ///     obstacle (WCAG 2.2 / UDL, multiple means of action), not a wall.
    /// </summary>
    public static class LevelData
    {
        public static readonly string[] Map =
        {
            "............................................................",
            ".....................................................T.SF...",
            ".................................................T..########",
            ".........................................ST...####..########",
            "...................................T....####..####..........",
            "...........................S.....####...####................",
            "......................T...####...####.......................",
            "...........S........####..####..............................",
            ".......S..###.......####....................................",
            "...P.T###T###....T......R...............R......M............",
            "############~~~~##############~~~~##############~~~~########",
        };
    }
}