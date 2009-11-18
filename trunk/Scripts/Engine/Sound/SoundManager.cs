// Mono Framework
using System;
using System.Collections;

// Unity Framework
using UnityEngine;

public delegate void SoundManagerCallback();

public class FadeAudioSource
{
    public float targetVolume;          // Target volume to go
    public float initialVolume;         // Initial volume
    public float fadeInSecs;            // Complete the fade in the specified seconds
    public float initialTime;
    public float accumTime;             // Current delta time
    public AudioSource audioSrc;        // The affected audiosource
    public SoundManagerCallback fnCb;
}

/// <summary>
/// A SoundManager.
/// 
/// Features:
/// . Background, Music and sounds channels.
/// . A priority system for the sound triggering.
/// . Fade In and Fade Out of music and sounds (event oriented)
/// . Fade out of all sounds (event oriented).
/// . 
/// 
/// Requires:
/// 
/// A GameObject with this MonoBehaviour, SoundList object and a Dont Destroy on Unload if necessary.
/// </summary>
public class SoundManager : MonoBehaviour
{
    public const int CHANNELS = 7;

    // Private Variables
    private static SoundManager _instance;
    private static AudioSource[] _fx;
    private static AudioSource _music;
    private static AudioSource _background;
    private static SoundList _soundList;

    // Not used by now
    private static float _fxVolume;
    private static float _backgroundVolume;
    private static float _masterVolume;
    private static float _musicVolume;

    private static SoundManagerCallback _fadeOutAllSoundsCallback;
    private static bool _fadingAllSounds;

    private static ArrayList _asFades = new ArrayList();

    private static SoundProp _currentBackgroundSndProp = null;
    private static SoundProp _currentMusicSndProp = null;
    
    void Awake()
    {
        _instance = this;
        _soundList = GetComponent(typeof(SoundList)) as SoundList;
       
    }

    void Start()
    {
        // Add an specific audiosource for the music
        _music = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        _music.playOnAwake = false;

        // Add an specific audiosource for the background main sound
        _background = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
        _background.playOnAwake = false;

        // Add many audiosources for multiple sounds
        _fx = new AudioSource[CHANNELS];
        for (int i = 0; i < CHANNELS; i++)
        {
            _fx[i] = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            _fx[i].playOnAwake = false;
        }

        MasterVolume = 1;
        BackgroundVolume = 1;
        MusicVolume = 1;
        FxVolume = 1;

        _music.volume = ConfigParams.musicVolume * MasterVolume;
        _background.volume = ConfigParams.backgroundVolume * MasterVolume;

        _fadingAllSounds = false;

    }

    void OnEnable()
    {
        _instance = this;
    }

    void OnDisable()
    {
        // Stop all sounds now
        StopAll();
    }

    void Update()
    {
        if (_soundList == null)
            _soundList = gameObject.GetComponent(typeof(SoundList)) as SoundList;

        // Update the position of the music and the background main sound
        if (Camera.current != null)
        {
            _music.transform.position = Camera.current.transform.position;
            _background.transform.position = Camera.current.transform.position;
        }

        // Change the volume of the faded sounds
        foreach (FadeAudioSource fas in _asFades)
        {
            fas.accumTime = Time.realtimeSinceStartup - fas.initialTime;

            // Increase the volume
            float totalDelta = fas.targetVolume - fas.initialVolume;
            float deltaVolume = totalDelta * (fas.accumTime / fas.fadeInSecs);

            fas.audioSrc.volume = fas.initialVolume + deltaVolume;

            //Debug.Log(String.Format("totalDelta: {0} deltaVol: {1} vol: {2}", totalDelta, deltaVolume, fas.audioSrc.volume));

            if (fas.accumTime >= fas.fadeInSecs)
            {
                fas.audioSrc.volume = fas.targetVolume;

                // Call the event
                if (fas.fnCb != null)
                    fas.fnCb();

                // Remove the fading
                _asFades.Remove(fas);

                // Return on every remove
                return;
            }
        }

        if (_fadingAllSounds)
        {
            if (_asFades.Count == 0)
            {
                if (_fadeOutAllSoundsCallback != null)
                {
                    _fadeOutAllSoundsCallback();
                    _fadeOutAllSoundsCallback = null;
                }
            }
        }
    }

    public static float MasterVolume
    {
        set
        {
            ConfigParams.masterVolume = value;
        }
        get
        {
            return ConfigParams.masterVolume;
        }
    }

    public static float BackgroundVolume
    {
        set
        {
            ConfigParams.backgroundVolume = value;
            _background.volume = value * MasterVolume * (_currentBackgroundSndProp != null ? (_currentBackgroundSndProp.volume / 100.0f) : 1);
        }
        get
        {
            return ConfigParams.backgroundVolume;
        }
    }

    public static float MusicVolume
    {
        set
        {
            ConfigParams.musicVolume = value;
            _music.volume = value * MasterVolume * (_currentMusicSndProp != null ? (_currentMusicSndProp.volume / 100.0f) : 1);
        }
        get
        {
            return ConfigParams.musicVolume;
        }
    }

    public static float FxVolume
    {
        set
        {
            ConfigParams.fxVolume = value;

            for (int i = 0; i < CHANNELS; i++)
            {
                _fx[i].volume = value * MasterVolume;
            }
        }
        get
        {
            return ConfigParams.fxVolume;
        }
    }

    public static void PauseAll(bool pauseThem)
    {
        if (pauseThem)
        {
            // Pause the background, music and all sounds
            _background.Pause();
            _music.Pause();
            for (int i = 0; i < CHANNELS; i++)
                _fx[i].Pause();

            // Finish all the fades
            foreach (FadeAudioSource fas in _asFades)
                fas.audioSrc.volume = fas.targetVolume;

            _asFades.Clear();
        }
        else
        {
            // Resume the background, music and all sounds if there were playing in the pause
            
            if (_background.time > 0)
                _background.Play();
            
            if (_music.time > 0)
                _music.Play();
            
            for (int i = 0; i < CHANNELS; i++)
            {
                if (_fx[i].time > 0)
                    _fx[i].Play();
            }
           

        }
    }

    /// <summary>
    /// Play the background sound
    /// </summary>
    /// <param name="sndId"></param>
    public static void PlayBackground(SndId sndId)
    {
        SoundProp sp = _soundList.GetSoundProp(sndId);

        if (sp != null)
        {
            _currentBackgroundSndProp = sp;

            // Set the position of the current camera in order to play the sound balanced
            if (Camera.current != null)
                _background.transform.position = Camera.current.transform.position;

            _background.clip = sp.audioClip;
            _background.loop = sp.loop;
            _background.volume = BackgroundVolume * MasterVolume * (sp.volume / 100.0f);
            _background.Play();
        }
    }

    /// <summary>
    /// Stop the background sound
    /// </summary>
    public static void StopBackground()
    {
        _background.Stop();
    }

    /// <summary>
    /// Play the music
    /// </summary>
    /// <param name="sndId"></param>
    public static void PlayMusic(SndId sndId)
    {
        PlayMusic(sndId, false);

        
    }

    public static void PlayMusic(SndId sndId, bool fadeOutCurrent)
    {
        if (_instance == null) return;

        SoundProp sp = _soundList.GetSoundProp(sndId);

        if (sp != null)
        {

            _currentMusicSndProp = sp;

            if (fadeOutCurrent && _music.isPlaying)
            {
                FadeOutMusic(1, playMusicAfterFadeOut);
            }
            else
            {
                playMusicAfterFadeOut();
            }
        }
    }

    public static bool IsPlayingMusic()
    {
        return _music.isPlaying;
    }

    private static void playMusicAfterFadeOut()
    {
        // Set the position of the current camera in order to play the sound balanced
        if (Camera.current != null)
            _music.transform.position = Camera.current.transform.position;

        _music.clip = _currentMusicSndProp.audioClip;
        _music.loop = _currentMusicSndProp.loop;

        _music.volume = MusicVolume * MasterVolume * (_currentMusicSndProp.volume / 100.0f);
        _music.Play();
    }

    /// <summary>
    /// Stop the music
    /// </summary>
    public static void StopMusic()
    {
        _music.Stop();
    }

    public static void FadeOutMusic(float inSecs, SoundManagerCallback cbfn)
    {
        FadeAudioSource fas = new FadeAudioSource();

        fas.initialTime = Time.realtimeSinceStartup;
        fas.accumTime = 0;
        fas.initialVolume = _music.volume;
        fas.targetVolume = 0;
        fas.audioSrc = _music;
        fas.fadeInSecs = inSecs;
        fas.fnCb += cbfn;

        _asFades.Add(fas);
    }

    public static void FadeInMusic(SndId sndId, float inSecs, SoundManagerCallback cbfn)
    {
        SoundProp sp = _soundList.GetSoundProp(sndId);

        if (sp != null)
        {
            _currentMusicSndProp = sp;

            // Set the position of the current camera in order to play the sound balanced
            if (Camera.current != null)
                _music.transform.position = Camera.current.transform.position;

            _music.clip = sp.audioClip;
            _music.loop = sp.loop;
            _music.volume = 0;
            _music.Play();

            FadeAudioSource fas = new FadeAudioSource();

            fas.initialTime = Time.realtimeSinceStartup;
            fas.accumTime = 0;
            fas.initialVolume = 0;
            fas.targetVolume = MusicVolume * MasterVolume * (sp.volume / 100.0f);
            fas.audioSrc = _music;
            fas.fadeInSecs = inSecs;

            

            if (cbfn != null)
                fas.fnCb += cbfn;
            else
                fas.fnCb = null;

            _asFades.Add(fas);

        }
    }

    public static void FadeOutBackground(float inSecs, SoundManagerCallback cbfn)
    {
        FadeAudioSource fas = new FadeAudioSource();

        fas.initialTime = Time.realtimeSinceStartup;
        fas.accumTime = 0;
        fas.initialVolume = _background.volume;
        fas.targetVolume = 0;
        fas.audioSrc = _background;
        fas.fadeInSecs = inSecs;
        fas.fnCb += cbfn;

        _asFades.Add(fas);
    }


	
    public static int PlaySound(SndId sndId)
    {
        if (Camera.current != null)
        {
            return PlaySound(Camera.current.transform.position, sndId);
        }
        else
            return -1;
    }

    /// <summary>
    /// Play an specific sound
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="sndId"></param>
    /// <returns></returns>
    public static int PlaySound(Vector3 pos, SndId sndId)
    {
        if (_fx == null) return -1;
        SoundProp sp = _soundList.GetSoundProp(sndId);

        if (sp != null)
        {
            // The specified sound should be marked as FX (the default value)
            if (sp.type == SndType.SND_FX)
            {
                int channeldIdx = getChannelIdx(sp);

                if (channeldIdx != -1)
                {
                    playThisSoundOnSource(channeldIdx, sp, pos);
                    return channeldIdx;
                }
                else
                    Debug.Log("All audiosource are busy. Cannot play sound: " + sp.name);
            }
            else
                Debug.Log(String.Format("Trying to play a sound that is not a FX ({0})", sp.name));
        }
        return -1;
    }

    private static void playThisSoundOnSource(int idx, SoundProp sp, Vector3 pos)
    {
        _fx[idx].Stop();
        _fx[idx].clip = sp.audioClip;
        _fx[idx].loop = sp.loop;
        _fx[idx].volume = FxVolume * MasterVolume * (sp.volume / 100.0f);

        _fx[idx].transform.position = pos;
        _fx[idx].Play();
    }

    /// <summary>
    /// Returns a channeld idx to play a sound.
    /// Could be:
    /// 1. An empty channel (not yet used)
    /// 2. An IDLE channel
    /// 3. A busy channel but with less priority
    /// 4. A busy channel with the same priority
    /// 
    /// If there isn't a channel that satisfy these conditions, returns -1
    /// 
    /// </summary>
    /// <returns></returns>
    private static int getChannelIdx(SoundProp sp)
    {
        for (int i = 0; i < CHANNELS; i++)
        {
            if (_fx[i].clip != null)
            {
                if (!_fx[i].isPlaying)
                {
                    // Found a audiosource that is not currently being played

                    if (_fx[i].clip != sp.audioClip)
                        return i;
                }
            }
            else
            {
                return i;
            }
        }

        // No audiosource idle. Find a busy audiosource with less priority than the new one
        for (int i = 0; i < CHANNELS; i++)
        {
            SoundProp prop = _soundList.getSoundPropByName(_fx[i].clip.name);
            if (sp.priority > prop.priority)
                return i;
        }

        // Try something with the same priority
        for (int i = 0; i < CHANNELS; i++)
        {
            SoundProp prop = _soundList.getSoundPropByName(_fx[i].clip.name);
            if (sp.priority == prop.priority)
                return i;
        }

        // Cannot find a suitable channel
        return -1;
    }

    /// <summary>
    /// Stop an specific sound immediatelly
    /// </summary>
    /// <param name="channelIdx"></param>
    public static void StopSound(int channelIdx)
    {
        if (channelIdx != -1 && channelIdx < CHANNELS)
        {
            if (_fx[channelIdx] != null && _fx[channelIdx].clip != null)
            {
                _fx[channelIdx].Stop();
            }
        }
    }

    /// <summary>
    /// Fade out an specific sound.
    /// Do not call other function of the sound manager that requires a callback function before the first
    /// one finishes and call the callback.
    /// </summary>
    /// <param name="channelIdx"></param>
    /// <param name="fncb"></param>
    public static void FadeOutSound(int channelIdx, float inSecs, SoundManagerCallback cbfn)
    {
        FadeAudioSource fas = new FadeAudioSource();

        fas.initialTime = Time.realtimeSinceStartup;
        fas.accumTime = 0;
        fas.initialVolume = _fx[channelIdx].volume;
        fas.targetVolume = 0;
        fas.audioSrc = _fx[channelIdx];
        fas.fadeInSecs = inSecs;
        fas.fnCb += cbfn;

        _asFades.Add(fas);

    }

    public static void FadeInSound(SndId sndId, float inSecs, SoundManagerCallback cbfn)
    {
        SoundProp sp = _soundList.GetSoundProp(sndId);

        if (sp != null)
        {

            int channeldIdx = getChannelIdx(sp);

            if (channeldIdx != -1)
            {
                // Set the position of the current camera in order to play the sound balanced
                if (Camera.current != null)
                    _fx[channeldIdx].transform.position = Camera.current.transform.position;

                _fx[channeldIdx].clip = sp.audioClip;
                _fx[channeldIdx].loop = sp.loop;
                _fx[channeldIdx].volume = 0;
                _fx[channeldIdx].Play();

                FadeAudioSource fas = new FadeAudioSource();

                fas.initialTime = Time.realtimeSinceStartup;
                fas.accumTime = 0;
                fas.initialVolume = 0;
                fas.targetVolume = FxVolume * MasterVolume * (sp.volume / 100.0f);
                fas.audioSrc = _fx[channeldIdx];
                fas.fadeInSecs = inSecs;

                if (cbfn != null)
                    fas.fnCb += cbfn;
                else
                    fas.fnCb = null;

                _asFades.Add(fas);
            }

        }
    }

    /// <summary>
    /// Stops all the sounds immediatelly
    /// </summary>
    public static void StopAll()
    {
        if (_background)
            _background.Stop();

        if (_music)
            _music.Stop();

        for (int i = 0; i < CHANNELS; i++)
        {
            if (_fx != null)
                if (_fx[i] != null)
                    _fx[i].Stop();
        }
    }

    /// <summary>
    /// Fade out all the sounds that are currently being played.
    /// </summary>
    /// <param name="fncb"></param>
    public static void FadeOutAll(float inSecs, SoundManagerCallback cbfn)
    {
        // Finish all the fades
        foreach (FadeAudioSource fas in _asFades)
            fas.audioSrc.volume = fas.targetVolume;

        _asFades.Clear();

        FadeOutMusic(inSecs, null);

        FadeOutBackground(inSecs, null);

        for (int i=0; i<CHANNELS; i++)
        {
            if (_fx[i].isPlaying)
                FadeOutSound(i, inSecs, null);
        }

        _fadingAllSounds = true;
        _fadeOutAllSoundsCallback += cbfn;
    }

    /// <summary>
    /// Return the channel
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static AudioSource GetChannelById(int id)
    {
        return _fx[id];
    }
}

