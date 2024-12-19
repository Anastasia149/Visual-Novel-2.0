using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public const string music_vol_param_name = "Music";
    public const string sfx_vol_param_name = "SFX";
    public const float muted_volume_level = -80f;

    private const string sfx_parent_name = "sfx";
    private const string sfx_name_format = "sfx - [{0}]";
    public const float track_transition_speed = 1;
    public static AudioManager instance { get; private set; }

    public Dictionary <int, AudioChannel> channels = new Dictionary<int, AudioChannel>();

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup sfxMixer;
    public AudioMixerGroup voiceMixer;

    public AnimationCurve audioFalloffCurve;

    private Transform sfxRoot;

    private void Awake()
    {
        if (instance == null)
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }

        sfxRoot = new GameObject(sfx_parent_name).transform;
        sfxRoot.SetParent(transform);

    }
    #region Sound effect
    public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(filePath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filePath}'.");
            return null;
        }

        return PlaySoundEffect(clip, mixer, volume, pitch, loop, filePath);
    }

    public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, string filePath = "")
    {
        string fileName = clip.name;
        if (filePath != string.Empty)
            fileName = filePath;

        AudioSource effectSource = new GameObject(string.Format(sfx_name_format, fileName)).AddComponent<AudioSource>();
        effectSource.transform.SetParent(sfxRoot);
        effectSource.transform.position = sfxRoot.position;

        effectSource.clip = clip;

        if (mixer == null)
            mixer = sfxMixer;

        effectSource.outputAudioMixerGroup = mixer;
        effectSource.volume = volume;
        effectSource.spatialBlend = 0;
        effectSource.pitch = pitch;
        effectSource.loop = loop;

        effectSource.Play();

        if (!loop)
            Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

        return effectSource;
    }

    public void SetSFXVolume(float volume, bool muted)
    {
        volume = muted ? muted_volume_level : audioFalloffCurve.Evaluate(volume);
        sfxMixer.audioMixer.SetFloat(sfx_vol_param_name, volume);
    }
    //--------------------------------------------------------------------------------------------------------------------------\\
    public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);
    public void StopSoundEffect(string soundName)
    {
        soundName = soundName.ToLower();

        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach(var source in sources)
        {
            if (source.clip.name == soundName)
            {
                Destroy(source.gameObject);
                return;
            }
        }
    }

    public void StopAllSoundEffects()
    {
        AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
        foreach(var source in sources)
        {
            Destroy(source.gameObject);
              
        }
    }
    #endregion

    #region Music / ambiance
    public AudioTrack PlayTrack(string filepath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f)
    {
        AudioClip clip = Resources.Load<AudioClip>(filepath);

        if (clip == null)
        {
            Debug.LogError($"Could not load audio file '{filepath}'. Please make sure this exists in the resources directory!");
            return null;
        }

        return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch, filepath);
    }

    public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = "")
    {
        AudioChannel audioChannel = TryGetChannel(channel, createIfDoesNotExist: true);
        AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);
        return track;
    }

    public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist = false)
    {
        AudioChannel channel = null;

        if (channels.TryGetValue(channelNumber, out channel))
        {
            return channel;
        }

        else if (createIfDoesNotExist)
        {
            channel = new AudioChannel(channelNumber);
            channels.Add(channelNumber, channel);
            return channel;
        }
        return null;
    }

    public void StopTrack(int channel)
    {
        AudioChannel c = TryGetChannel(channel, createIfDoesNotExist: false);

        if (c == null)
            return;

        c.StopTrack();
    }

    public void StopTrack(string trackName)
    {
        trackName = trackName.ToLower();

        foreach (var channel in channels.Values)
        {
            if (channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName)
            {
                channel.StopTrack();
                return;
            }
        }
    }

    public void SetMusicVolume(float volume, bool muted)
    {
        volume = muted ? muted_volume_level : audioFalloffCurve.Evaluate(volume);
        musicMixer.audioMixer.SetFloat(music_vol_param_name, volume);
    }

    public void StopAllTracks()
    {
        foreach (AudioChannel channel in channels.Values)
        {
            channel.StopTrack();
        }
    }
    #endregion

 
  

}
