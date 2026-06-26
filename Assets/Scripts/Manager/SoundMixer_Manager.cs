using UnityEngine;
using UnityEngine.Audio;

public class SoundMixer_Manager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    public static SoundMixer_Manager instance;
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
    }
    public void SetMasterVolume (float level)
    {
        audioMixer.SetFloat("Master_Volume", Mathf.Log10(level) * 20f);
    }
    public void SetMusicVolume (float level)
    {
        audioMixer.SetFloat("Music_Volume", Mathf.Log10(level) * 20f);
    }
    public void SetSFXVolume (float level)
    {
        audioMixer.SetFloat("SFX_Volume", Mathf.Log10(level) * 20f);
    }
}
