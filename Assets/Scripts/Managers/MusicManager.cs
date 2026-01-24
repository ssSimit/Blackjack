using UnityEngine;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    [SerializeField] private AudioClip bgmClip;

    [SerializeField] private List<AudioClip> audioClips = new List<AudioClip>();

    public bool isMuted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Get or add the BGM AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }

    public void PlayAudio(string clipName)
    {
        foreach (AudioClip clip in audioClips)
        {
            if (clip != null && clip.name == clipName)
            {
                audioSource.PlayOneShot(clip);
                return;
            }
        }
        Debug.LogWarning($"Audio clip '{clipName}' not found in MusicManager!");
    }

    public void PlayBGM()
    {
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
    }


    public void MuteUnmuteAudio()
    {
        isMuted = !isMuted;
        audioSource.mute = isMuted;
    }


}