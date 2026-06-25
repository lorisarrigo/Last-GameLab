using System.Collections;
using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    public static Music_Manager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void PlayMusic(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(playmusic(audioClip, volume));
    }

    IEnumerator playmusic(AudioClip audioClip, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length);

        Destroy(audioSource);
    }
}
