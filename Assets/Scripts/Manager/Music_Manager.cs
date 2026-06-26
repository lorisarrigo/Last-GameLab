using UnityEngine;
using UnityEngine.Audio;


public class Music_Manager : MonoBehaviour
{
    private AudioSource musicSource;
    [SerializeField] AudioClip mainGameMusic;
    [SerializeField] AudioMixerGroup musicMixer;
    public static Music_Manager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }
    private void Start()
    {
        if(mainGameMusic != null)
        {
            PlayMusic(mainGameMusic);
        }
    }
    public void PlayMusic(AudioClip audioClip, float volume = 1f)
    {
        musicSource.outputAudioMixerGroup = musicMixer;
        musicSource.clip = audioClip;
        musicSource.volume = volume;
        musicSource.Play();
    }
    public void StopMusic() {  musicSource.Pause(); }
    public void ResumeMusic() { musicSource.UnPause(); }
}
