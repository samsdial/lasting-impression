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
    /// </summary>
    public static class LevelData
    {
        public static readonly string[] Map =
        {
            "............................................................",
            "............................................................",
            "..........................S.................................",
            ".....................#####.............S..........T.........",
            "...............T.....................#####.......####.......",
            "..........S...####..........T...............................",
            ".........###...............###.........R...............S..F.",
            "..P.................T................#####.......T....######",
            "######....#######~~~~~#########..#############~~~~##########",
            "######....#######~~~~~#########..#############~~~~##########",
        };
    }
}
