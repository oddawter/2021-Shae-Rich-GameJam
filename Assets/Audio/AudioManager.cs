﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;
using DG.Tweening;

    /// <summary>
    /// Holds an immutable ID.
    /// </summary>
    public class AudioID
    {
        /// <summary>
        /// Used to give each track a unique ID so it can be referenced later.
        /// </summary>
        private static int keyCount = 0;

        public readonly int ID;

        public AudioID()
        {
            ID = keyCount++; // super secure way of generating a unique ID
        }
    }

    public class AudioManager : MonoBehaviour
    {
        //static variables
        private static AudioManager Instance;

        /// <summary>
        /// True if there is an active, initialized AudioManager in the Scene.
        /// If this is false and you aren't okay with that, call AudioManager.Init().
        /// </summary>
        public static bool IsInitialized;

        [Header("---Audio Manager---")]
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField]
        private int sfxAudioSourceCount = 4;

        private AudioSource[] SFXAudioSources;
        private static AudioSource[] SFXSources { get => Instance.SFXAudioSources; }

        [SerializeField] private Vector2 pitchShiftRange = new Vector2(0.75f, 1.25f);
        private static Vector2 PitchShiftRange { get => Instance.pitchShiftRange; }

        //[Header("---Background Music Sources---")]
        private AudioSource backgroundMusicTrackA;
        private static AudioSource BackgroundMusicTrackA
        { get => Instance.backgroundMusicTrackA; }

        private AudioSource backgroundMusicTrackB;
        private static AudioSource BackGroundMusicTrackB
        { get => Instance.backgroundMusicTrackB; }

        private static AudioSource ActiveMusicTrack;

        /// <summary>
        /// This is so the caller can query the sound after the fact, like for interruping a spell.
        /// </summary>
        private static Dictionary<int, AudioSource> sourceDictionary
            = new Dictionary<int, AudioSource>(6);

        protected void Awake()
        {
            InitSingleton(this);
            CreateAudioSources();
            ActiveMusicTrack = backgroundMusicTrackB; // start with b to switch to a
        }

        private void OnDestroy()
        {
            if (Instance == this)
                IsInitialized = false;//no more active audio manager in scene.
        }

        private static void InitSingleton(AudioManager current)
        {
            if (!Instance)
            {
                Instance = current;
                DontDestroyOnLoad(Instance.gameObject); // immortality!
                IsInitialized = true;
            }
            else
            {
                Debug.LogError("[AudioManager] Too many AudioManagers in the Scene! Destroying: " +
                    current.name, current);
                Destroy(current.gameObject); // there can only be one!
            }
        }

        /// <summary>
        /// Create all the AudioSources needed for the whole game.
        /// </summary>
        /// <remarks>Create all at once on same GameObject to help with cache.</remarks>
        private void CreateAudioSources()
        {
            //create SFX audio sources
            SFXAudioSources = new AudioSource[sfxAudioSourceCount];

            for (var i = 0; i < sfxAudioSourceCount; ++i)
            {
                SFXAudioSources[i] = gameObject.AddComponent<AudioSource>(); // new AudioSource
            }

            //create background track sources
            backgroundMusicTrackA = gameObject.AddComponent<AudioSource>();
            backgroundMusicTrackB = gameObject.AddComponent<AudioSource>();

            if (audioMixer)
            {   //this section of code makes a lot of assumptions about the given audio mixer
                var sfxGroup = audioMixer.FindMatchingGroups("SFX")[0];
                foreach (var source in SFXAudioSources)
                {
                    source.outputAudioMixerGroup = sfxGroup;
                }
                var bgGroup = audioMixer.FindMatchingGroups("Background Music")[0];
                backgroundMusicTrackA.outputAudioMixerGroup = bgGroup;
                backgroundMusicTrackB.outputAudioMixerGroup = bgGroup;
            }
        }

        /// <summary>
        /// Track AudioSource with a key so it can be found later.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static AudioID TrackAudio(AudioSource source)
        {
            //TODO - prevent duplicates in dictionary
            //maybe use a List of custom structs
            var key = new AudioID(); // maybe pool these to cut down on "newing"

            sourceDictionary.Add(key.ID, source);

            return key;
        }

        private static float RandomPitchShift()
            => Random.Range(PitchShiftRange.x, PitchShiftRange.y);

        /// <summary>
        /// Load options and play clip from source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="clip"></param>
        /// <param name="options"></param>
        private static void ConfigAudioSource(AudioSource source,
            AudioClip clip, AudioOptions options)
        {
            //load options
            source.loop = options.loop;
            source.priority = options.priority;
            //source.volume = options.volume; // crossfade handles this, even if crossfade is 0
            source.DOFade(options.volume, options.crossfade); // fade in
            source.pitch = options.pitchShift ? RandomPitchShift() : 1.0f;
            source.clip = clip;
            source.Play();
        }

        /// <summary>
        /// Free clip after time.
        /// </summary>
        /// <param name="clipLength"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static IEnumerator RemoveSourceAfterClip(float clipLength, AudioID key)
        {
            yield return new WaitForSeconds(clipLength); // wait until duration over

            if (sourceDictionary.TryGetValue(key.ID, out AudioSource ass))
            {
                ass.Stop(); // stop if still playing
                sourceDictionary.Remove(key.ID);
            }
        }

        #region API

#if UNITY_EDITOR // because this file probably isn't in an Editor folder.
        [MenuItem("Tools/Audio Manager/Create Instance in Scene")]
#endif
        /// <summary>
        /// Create an AudioManager in the Scene.
        /// </summary>
        public static void Init()
            => new GameObject("AudioManager").AddComponent<AudioManager>();

        /// <summary>
        /// Different way to play a SFX if you don't want to use AudioOptions.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="loop"></param>
        /// <param name="pitchShift"></param>
        /// <param name="crossfade"></param>
        /// <param name="priority"></param>
        /// <param name="volume"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static AudioID PlaySFX(AudioClip clip, bool loop = false,
            bool pitchShift = true, float crossfade = 0.0f, int priority = 128,
            float volume = 1.0f, float duration = 0.0f)
        {   //no default constructor
            if (!clip) return null;//for safety
            var audioOptions = new AudioOptions();

            audioOptions.pitchShift = pitchShift;
            audioOptions.loop = loop;
            audioOptions.crossfade = crossfade;
            audioOptions.priority = priority;
            audioOptions.volume = volume;
            audioOptions.duration = duration == 0.0f ? clip.length : duration; //validate

            return PlaySFX(clip, audioOptions);
        }

        /// <summary>
        /// Play the given clip.
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="options"></param>
        public static AudioID PlaySFX(AudioClip clip, AudioOptions options)
        {
            if (!clip) return null;//for safety

            //default values for options, iff none included.
            if (options.priority <= 0) //clear sign this wasn't init'd
            {
                options.duration = clip.length;
                options.loop = false;
                options.pitchShift = true;
                options.crossfade = 0.0f;
                options.priority = 128;
                options.volume = 1.0f;//max
            }

            //find an audio source with the lowest volume, or that is not playing
            var size = SFXSources.Length;
            var sources = SFXSources;
            var lowestVolume = float.MaxValue;// 1 would also suffice
            var lowestIndex = 0;
            AudioSource source = null;

            for (var i = 0; i < size; ++i)
            {
                source = sources[i];
                if (!source.isPlaying) // if this one isn't busy
                {
                    // use this one
                    lowestIndex = i;
                    break;
                }

                //try to override quietest volume
                else if (source.volume < lowestVolume)
                {
                    lowestVolume = source.volume;
                    lowestIndex = i;
                }
            }

            source = sources[lowestIndex];

            var key = TrackAudio(source); // return a key so audio can be interrupted later
            ConfigAudioSource(source, clip, options);

            // if looping with duration < 0, it's up to caller to stop the clip.
            if (!options.loop || options.duration > 0) //stop clip if not looping, or if looping for a specified duration
                Instance.StartCoroutine(RemoveSourceAfterClip(options.duration, key)); // free source after time
            return key;
        }

        public static AudioID PlayBackgroundTrack(AudioClip clip,
            AudioOptions options = default)
        {
            if (!clip) return null;//for safety

            //default values for options, iff none included.
            if (options.priority <= 0) //clear sign this wasn't init'd
            {
                options.loop = true;
                options.pitchShift = false;
                options.crossfade = 2.5f;
                options.priority = 127;
                options.volume = 1;
            }
            //Debug.Log("Crossfade Time: " + options.crossfade);

            //
            ActiveMusicTrack.DOFade(0, options.crossfade); // fade out current track
            ActiveMusicTrack = ActiveMusicTrack == BackgroundMusicTrackA ? // switch active track
                BackGroundMusicTrackB : BackgroundMusicTrackA; // just alternate tracks

            var key = TrackAudio(ActiveMusicTrack);
            if (!options.loop) // if it's looping, it's up to caller to stop the clip.
                Instance.StartCoroutine(RemoveSourceAfterClip(clip.length, key)); // free source after time
            ConfigAudioSource(ActiveMusicTrack, clip, options);
            return key;
        }

        /// <summary>
        /// Stop clip from playing and free up AudioSource.
        /// </summary>
        /// <param name="key"></param>
        public static void StopSFX(AudioID key)
        {
            if (key == null) return;

            var id = key.ID;
            if (id < 0) return;//check for invalid key

            var found = sourceDictionary.TryGetValue(id, out AudioSource source);

            if (found && source)
            {
                source.Stop();
                sourceDictionary.Remove(id);
            }
        }

        #endregion
    }

