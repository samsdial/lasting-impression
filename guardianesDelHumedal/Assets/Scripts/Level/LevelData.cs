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
    ///   '~'  polluted water   (hazard)       - implemented in block 2
    ///   'R'  gorse "retamo"   (hazard)       - implemented in block 2
    ///   'T'  trash            (collectible)  - implemented in block 2
    ///   'S'  native seed      (collectible)  - implemented in block 2
    ///   'F'  finish / restoration point      - implemented in block 2
    ///
    /// Characters that are not implemented yet are simply ignored by the builder,
    /// so the level can be designed completely before every system exists.
    ///
    /// Design decisions (see doc: docs/aportes/mariana.md):
    ///   - 11 rows x 60 columns, every row the same length.
    ///   - Player jump reaches 3.2 tiles (PlayerController.jumpHeight),
    ///     so vertical rises between platforms stay <= 3 and gaps <= 4.
    ///   - 8 trash (T), 5 seeds (S), 1 spawn (P), 1 finish (F).
    ///   - Difficulty grows from left to right: low, wide platforms near the
    ///     spawn become smaller and higher platforms toward the finish.
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
            "...P.T###T###....T..........................................",
            "############....##############....##############....########",
        };
    }
}
