using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                _instance = go.AddComponent<AudioManager>();
                // _instance.Awake() will be called automatically, taking care of DontDestroyOnLoad and AudioSources.
            }
            return _instance;
        }
    }

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private Dictionary<string, AudioClip> clipCache = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.5f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 0.8f;
    }

    public void PlayBGM(string clipName)
    {
        AudioClip clip = GetClip(clipName);
        if (clip != null)
        {
            if (bgmSource.clip == clip && bgmSource.isPlaying) return; // 이미 재생 중이면 무시
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(string clipName, float volumeMultiplier = 1f)
    {
        AudioClip clip = GetClip(clipName);
        if (clip != null)
        {
            // volumeMultiplier가 1보다 크면, 여러 번 겹쳐 재생해서 실제 볼륨을 증폭시킵니다.
            int playCount = Mathf.Max(1, Mathf.RoundToInt(volumeMultiplier));
            float scale = volumeMultiplier > 1f ? 1f : volumeMultiplier;
            for(int i = 0; i < playCount; i++)
            {
                sfxSource.PlayOneShot(clip, scale);
            }
        }
    }

    private AudioClip GetClip(string clipName)
    {
        if (clipCache.ContainsKey(clipName))
            return clipCache[clipName];

        AudioClip loadedClip = Resources.Load<AudioClip>("Audio/" + clipName);
        if (loadedClip != null)
        {
            clipCache.Add(clipName, loadedClip);
            return loadedClip;
        }
        else
        {
            Debug.LogWarning("AudioClip 못 찾음: Resources/Audio/" + clipName);
            return null;
        }
    }
}
