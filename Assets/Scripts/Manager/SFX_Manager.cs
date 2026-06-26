using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.VFX;

public class SFX_Manager : MonoBehaviour
{
    [SerializeField] AudioMixerGroup SFXMixer;
    public static SFX_Manager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySfx(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(playSFX(audioClip, volume));
    }

    IEnumerator playSFX(AudioClip audioClip, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = SFXMixer;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(audioSource);
    }
}
