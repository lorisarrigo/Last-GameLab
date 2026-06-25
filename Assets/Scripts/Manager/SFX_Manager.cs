using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class SFX_Manager : MonoBehaviour
{
    public static SFX_Manager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void PlaySfx(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(playSFX(audioClip, volume));
    }

    IEnumerator playSFX(AudioClip audioClip, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(audioSource);
    }
}
