namespace GuardiansOfTheWetland.Audio
{
    /// <summary>
    /// File names (without extension) of every audio clip inside Assets/Resources/Audio.
    /// Using constants avoids typos when other scripts request a sound.
    /// </summary>
    public static class SoundNames
    {
        public const string MusicTension = "music_tension";
        public const string MusicCalm = "music_calm";

        public const string Trash = "sfx_trash";
        public const string Seed = "sfx_seed";
        public const string Damage = "sfx_damage";
        public const string Water = "sfx_water";
        public const string Restore = "sfx_restore";
        public const string Species = "sfx_species";
    }
}
