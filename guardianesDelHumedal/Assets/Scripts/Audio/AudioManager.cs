using System;
using System.Collections;
using System.Collections.Generic;
using GuardiansOfTheWetland.Player;
using GuardiansOfTheWetland.UI;
using UnityEngine;

namespace GuardiansOfTheWetland.Audio
{
    /// <summary>
    /// Plays the music and sound effects of the game.
    /// - Two music tracks (tension and calm) play in sync and are crossfaded on restoration.
    /// - Every sound effect also shows its subtitle (accessibility: sound + text).
    /// - Music and effects have separate volumes, adjustable with the keyboard and saved between sessions.
    /// - Effects are normalized on load because the source files have very different loudness.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        /// <summary>Single active audio manager, reachable from any gameplay script.</summary>
        public static AudioManager Instance { get; private set; }

        private const string AudioFolder = "Audio/";

        // Loudest sample of every effect after normalization (0..1). Leaves headroom to avoid clipping.
        private const float TargetPeak = 0.8f;

        // Effects quieter than this are treated as silence and are not amplified.
        private const float SilenceThreshold = 0.0001f;

        private const float DefaultMusicVolume = 0.5f;
        private const float DefaultSfxVolume = 0.9f;
        private const float VolumeStep = 0.1f;

        // PlayerPrefs keys used to remember the volume settings.
        private const string MusicVolumeKey = "GuardiansOfTheWetland.MusicVolume";
        private const string SfxVolumeKey = "GuardiansOfTheWetland.SfxVolume";

        private readonly Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

        private AudioSource tensionMusicSource;
        private AudioSource calmMusicSource;
        private AudioSource sfxSource;

        // 0 = only tension music is heard, 1 = only calm music is heard.
        private float calmBlend;

        /// <summary>Music volume from 0 to 1.</summary>
        public float MusicVolume { get; private set; }

        /// <summary>Sound effects volume from 0 to 1.</summary>
        public float SfxVolume { get; private set; }

        /// <summary>Raised when the player changes a volume (used by the HUD).</summary>
        public event Action VolumesChanged;

        private void Awake()
        {
            Instance = this;

            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);
            SfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume);

            tensionMusicSource = CreateSource("TensionMusic", true);
            calmMusicSource = CreateSource("CalmMusic", true);
            sfxSource = CreateSource("SoundEffects", false);
        }

        private void Start()
        {
            // Both tracks start together so the crossfade keeps them in rhythm.
            tensionMusicSource.clip = LoadClip(SoundNames.MusicTension, false);
            calmMusicSource.clip = LoadClip(SoundNames.MusicCalm, false);
            ApplyMusicVolumes();

            if (tensionMusicSource.clip != null)
            {
                tensionMusicSource.Play();
            }

            if (calmMusicSource.clip != null)
            {
                calmMusicSource.Play();
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            // Keys 1/2 lower/raise the music, keys 3/4 lower/raise the effects.
            if (PlayerInput.MusicDownPressed())
            {
                SetMusicVolume(MusicVolume - VolumeStep);
            }

            if (PlayerInput.MusicUpPressed())
            {
                SetMusicVolume(MusicVolume + VolumeStep);
            }

            if (PlayerInput.SfxDownPressed())
            {
                SetSfxVolume(SfxVolume - VolumeStep);
            }

            if (PlayerInput.SfxUpPressed())
            {
                SetSfxVolume(SfxVolume + VolumeStep);
            }
        }

        /// <summary>
        /// Plays a sound effect once and shows its subtitle.
        /// The subtitle appears even if the audio file is missing or the volume is zero.
        /// </summary>
        /// <param name="soundName">One of the constants in <see cref="SoundNames"/>.</param>
        public void PlaySfx(string soundName)
        {
            AudioClip clip = LoadClip(soundName, true);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, SfxVolume);
            }

            if (SubtitleManager.Instance != null)
            {
                SubtitleManager.Instance.ShowForSound(soundName);
            }
        }

        /// <summary>
        /// Gradually replaces the tension music with the calm music.
        /// </summary>
        public void CrossfadeToCalmMusic(float duration)
        {
            StartCoroutine(CrossfadeRoutine(duration));
        }

        private IEnumerator CrossfadeRoutine(float duration)
        {
            float elapsed = 0f;

            // Each frame moves the blend a little closer to the calm track.
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                calmBlend = Mathf.Clamp01(elapsed / duration);
                ApplyMusicVolumes();
                yield return null;
            }

            calmBlend = 1f;
            ApplyMusicVolumes();
        }

        /// <summary>
        /// Changes the music volume (clamped to 0..1) and saves it.
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            MusicVolume = RoundToStep(Mathf.Clamp01(volume));
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            ApplyMusicVolumes();
            VolumesChanged?.Invoke();
        }

        /// <summary>
        /// Changes the effects volume (clamped to 0..1) and saves it.
        /// </summary>
        public void SetSfxVolume(float volume)
        {
            SfxVolume = RoundToStep(Mathf.Clamp01(volume));
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
            VolumesChanged?.Invoke();
        }

        /// <summary>
        /// Splits the music volume between the two tracks according to the crossfade.
        /// </summary>
        private void ApplyMusicVolumes()
        {
            tensionMusicSource.volume = MusicVolume * (1f - calmBlend);
            calmMusicSource.volume = MusicVolume * calmBlend;
        }

        private AudioSource CreateSource(string sourceName, bool loop)
        {
            GameObject sourceObject = new GameObject(sourceName);
            sourceObject.transform.SetParent(transform);

            AudioSource source = sourceObject.AddComponent<AudioSource>();
            source.loop = loop;
            source.playOnAwake = false;

            // 2D sound: the same volume no matter where the camera is.
            source.spatialBlend = 0f;
            return source;
        }

        /// <summary>
        /// Loads a clip from Resources/Audio once and keeps it in a cache.
        /// </summary>
        /// <param name="soundName">File name without extension.</param>
        /// <param name="normalize">True for short effects that must be leveled.</param>
        private AudioClip LoadClip(string soundName, bool normalize)
        {
            if (clipCache.TryGetValue(soundName, out AudioClip cachedClip))
            {
                return cachedClip;
            }

            AudioClip clip = Resources.Load<AudioClip>(AudioFolder + soundName);
            if (clip == null)
            {
                Debug.LogWarning($"[AudioManager] Missing audio file: Resources/{AudioFolder}{soundName}");
            }
            else if (normalize)
            {
                NormalizePeak(clip);
            }

            // Missing clips are cached as null so the warning appears only once.
            clipCache[soundName] = clip;
            return clip;
        }

        /// <summary>
        /// Scales the samples of a clip so its loudest point equals <see cref="TargetPeak"/>.
        /// This fixes effects that were exported too quiet or too loud.
        /// </summary>
        private static void NormalizePeak(AudioClip clip)
        {
            try
            {
                // Sample data is only readable once the clip is loaded in memory.
                if (clip.loadState != AudioDataLoadState.Loaded)
                {
                    clip.LoadAudioData();
                }

                float[] samples = new float[clip.samples * clip.channels];
                if (!clip.GetData(samples, 0))
                {
                    return;
                }

                // First pass: find the loudest sample.
                float peak = 0f;
                for (int i = 0; i < samples.Length; i++)
                {
                    peak = Mathf.Max(peak, Mathf.Abs(samples[i]));
                }

                // Silent clips are left untouched to avoid amplifying noise.
                if (peak < SilenceThreshold)
                {
                    return;
                }

                // Second pass: apply the same gain to every sample.
                float gain = TargetPeak / peak;
                for (int i = 0; i < samples.Length; i++)
                {
                    samples[i] *= gain;
                }

                clip.SetData(samples, 0);
            }
            catch (Exception exception)
            {
                // If the clip cannot be edited (for example, it is streamed), it plays as it is.
                Debug.LogWarning($"[AudioManager] Could not normalize '{clip.name}': {exception.Message}");
            }
        }

        private static float RoundToStep(float volume)
        {
            return Mathf.Round(volume / VolumeStep) * VolumeStep;
        }
    }
}
