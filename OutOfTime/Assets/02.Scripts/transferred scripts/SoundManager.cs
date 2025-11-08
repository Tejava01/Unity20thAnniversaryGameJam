using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource soundtrackFirstLoop;
    [SerializeField] private AudioSource soundtrackLoop;
    [SerializeField] private GameObject audiosourcePrefab;


    private const int poolSize = 20;
    private AudioSource[] pool;


    private Dictionary<AudioClip, float> playedRecently;           // list of sound effects playing within the timeframe indicated below, prevents duplicates of same sound playing too frequently
    private const float sfxCooldown = 0.1f;


    private Coroutine transition;                 // controls transitions in soundtrack source, prevents interupting transition

    public static SoundManager instance { get; private set; }

    private const float SFXVolume = 1f;
    private const float musicVolume = 1f;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        playedRecently = new Dictionary<AudioClip, float>();

        initializeASPool();
    }

    private void initializeASPool()
    {
        pool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = Instantiate(audiosourcePrefab, transform).GetComponent<AudioSource>();
            pool[i] = source;
        }
    }



    // methods for playing soundtrack
    public void playSoundtrack(AudioClip soundtrack)
    {
        if (transition != null) StopCoroutine(transition);
        transition = StartCoroutine(soundtrackTransition(soundtrack, 1, 0));
    }

    public void playSoundtrack(AudioClip firstLoop, AudioClip loop)
    {
        StartCoroutine(soundtrackTransition(firstLoop, loop, 1, 0));
    }



    private IEnumerator soundtrackTransition(AudioClip loop, float fade, float pause)
    {
        float time = 0;

        while (time < fade)
        {
            soundtrackLoop.volume = Mathf.Clamp01(Mathf.Lerp(musicVolume, 0, time / fade));
            soundtrackFirstLoop.volume = Mathf.Clamp01(Mathf.Lerp(musicVolume, 0, time / fade));
            time += Time.deltaTime;
            yield return null;
        }
        soundtrackLoop.volume = 0;
        soundtrackFirstLoop.volume = 0;
        soundtrackLoop.Stop();
        soundtrackFirstLoop.Stop();
        yield return new WaitForSeconds(pause);
        time = 0;


        soundtrackLoop.clip = loop;
        soundtrackLoop.Play();

        while (time < fade)
        {
            soundtrackLoop.volume = Mathf.Clamp01(Mathf.Lerp(0, musicVolume, time / fade));
            time += Time.deltaTime;
            yield return null;
        }

        soundtrackLoop.volume = musicVolume;
        transition = null;
    }

    private IEnumerator soundtrackTransition(AudioClip firstLoop, AudioClip loop, float fade, float pause)
    {
        float time = 0;

        while (time < fade)
        {
            soundtrackLoop.volume = Mathf.Clamp01(Mathf.Lerp(musicVolume, 0, time / fade));
            soundtrackFirstLoop.volume = Mathf.Clamp01(Mathf.Lerp(musicVolume, 0, time / fade));
            time += Time.deltaTime;
            yield return null;
        }
        soundtrackLoop.volume = 0;
        soundtrackFirstLoop.volume = 0;
        soundtrackLoop.Stop();              // stop the clip that was prevoiusly playing, this should be immediate and before any scheduled clip starts
        soundtrackFirstLoop.Stop();


        yield return new WaitForSeconds(pause);
        time = 0;
        double preciseTime = AudioSettings.dspTime + firstLoop.length;
        soundtrackLoop.clip = loop;
        soundtrackLoop.PlayScheduled(preciseTime);                      // play exact moment first loop ends


        soundtrackFirstLoop.clip = firstLoop;
        soundtrackFirstLoop.loop = false;
        soundtrackFirstLoop.PlayScheduled(AudioSettings.dspTime);           // play immediately

        while (time < fade)
        {
            soundtrackLoop.volume = Mathf.Clamp01(Mathf.Lerp(0, musicVolume, time / fade));
            time += Time.deltaTime;
            yield return null;
        }

        soundtrackLoop.volume = musicVolume;
        soundtrackFirstLoop.volume = musicVolume;
        transition = null;
    }



    // methods for playing SFX

    public AudioSource playClip(AudioClip sound)
    {
        if (sound == null) return null;
        return playClip(sound, 1, 1);
    }

    public AudioSource playClip(AudioClip sound, float volume)
    {
        if (sound == null) return null;
        return playClip(sound, volume, 1);
    }

    public AudioSource playClipRandomPitch(AudioClip sound)
    {
        if (sound == null) return null;
        return playClip(sound, 1, getRandomPitch());
    }

    public AudioSource playClipRandomPitch(AudioClip sound, float volume)
    {
        if (sound == null) return null;
        return playClip(sound, volume, getRandomPitch());
    }

    private AudioSource playClip(AudioClip sound, float volume, float pitch)       // parameter volume is relative to SFXVolume, which is applied to all SFX
    {
        if (playedRecently.ContainsKey(sound)) return null;
        playedRecently.Add(sound, 0);
        AudioSource source = getNewSource();

        source.volume = volume * SFXVolume;
        source.pitch = pitch;
        source.clip = sound;
        source.Play();

        return source;
    }

    // update recently played list
    private void LateUpdate()
    {
        AudioClip[] keys = playedRecently.Keys.ToArray();
        List<AudioClip> expired = new List<AudioClip>();
        for (int i = 0; i < keys.Length; i++)
        {
            AudioClip clip = keys[i];
            playedRecently[clip] += Time.deltaTime;
            if (playedRecently[clip] > sfxCooldown)
                expired.Add(clip);
        }

        foreach (AudioClip key in expired)
            playedRecently.Remove(key);
    }





    // methods for modifying SFX
    public IEnumerator fade(AudioSource source, float duration)
    {
        if (source == null) yield break;
        if (!source.isPlaying) yield break;
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(SFXVolume, 0, time / duration);
            yield return null;
        }

        source.Stop();
    }

    public void fadeAll(float duration)
    {
        foreach (AudioSource source in pool)
        {
            StartCoroutine(fade(source, duration));
        }
    }






    // private utility methods
    private AudioSource getNewSource()
    {
        foreach (AudioSource source in pool)
        {
            if (!source.isPlaying) return source;
        }

        Debug.Log("all sources are playing, override oldest");


        AudioSource bestSource = pool[0];
        float shortestTimeRemaining = int.MaxValue;

        foreach (AudioSource source in pool)
        {
            float timeRemaining = source.clip.length - source.time;
            if (timeRemaining < shortestTimeRemaining)
            {
                bestSource = source;
                shortestTimeRemaining = timeRemaining;
            }
        }
        return bestSource;
    }


    private float getRandomPitch() {
        int shift = Random.Range(-2, 3);
        return 1 + shift * 0.14f;
    }

    


}
