using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop = false;
        [HideInInspector]
        public AudioSource source;
    }

    public List<Sound> sounds;
    private Sound currentBGM;
    private const float fadeTime = 1f;
    private const float MIN_VOLUME = 0f;
    private const float MAX_VOLUME = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Play("BGM");
        currentBGM = sounds[0];
    }
    private void InitializeSounds()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0)
        {
            string newBGMName = GetBGMForScene(scene.name);
            ChangeBGM(newBGMName);
        }
    }

    private string GetBGMForScene(string sceneName)
    {
        // 여기에서 각 씬에 맞는 BGM 이름을 반환하도록 구현
        // 예: switch 문을 사용하여 씬 이름에 따라 다른 BGM 반환
        switch (sceneName)
        {
            case "1.LoginScene": return "BGM";
            case "2.MainScene": return "BGM";
            case "3.CardGameScene": return "CardBGM";
            case "4.ColorGame": return "ColorBGM";
            case "5.1_StoreGame": return "StoreBGM";
            case "6.2_StoreGame": return "StoreBGM";
            case "7.3_StoreGame": return "StoreBGM";
            default: return "BGM";
        }
    }

    public void ChangeBGM(string newBGMName)
    {
        //StartCoroutine(SmoothBGMTransition(newBGMName));
        if (currentBGM.name != newBGMName)
        {
            currentBGM.source.Stop();
            Sound newBGM = sounds.Find(sound => sound.name == newBGMName);
            currentBGM = newBGM;
            currentBGM.source.Play();
        }
    }

    private IEnumerator SmoothBGMTransition(string newBGMName)
    {
        if (newBGMName != currentBGM.name)
        {
            if (currentBGM != null)
            {
                // 현재 BGM 페이드 아웃
                yield return StartCoroutine(FadeOut(currentBGM.source));
                currentBGM.source.Stop();
            }

            Sound newBGM = sounds.Find(sound => sound.name == newBGMName);
            if (newBGM == null)
            {
                Debug.LogWarning($"BGM: {newBGMName} not found!");
                yield break;
            }

            currentBGM = newBGM;
            currentBGM.source.Play();
            yield return StartCoroutine(FadeIn(currentBGM.source));
        }

    }

    private IEnumerator FadeOut(AudioSource audioSource)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = 0;
    }

    private IEnumerator FadeIn(AudioSource audioSource)
    {
        float targetVolume = audioSource.volume;
        audioSource.volume = 0;

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // 기존 메서드들 (Play, Stop, SetVolume 등)은 그대로 유지

    public void Play(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        s.source.Stop();
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return;
        }
        s.source.volume = Mathf.Clamp(volume, MIN_VOLUME, MAX_VOLUME);
    }

    public void SetMasterVolume(float volume)
    {
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * volume;
        }
    }

    public float GetVolume(string name)
    {
        Sound s = sounds.Find(sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound: {name} not found!");
            return 0;
        }
        return s.source.volume;
    }
    public void PlayOneShot(string clip)
    {
        Play(clip);
    }
}