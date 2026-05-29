using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource sfxSource;
    
    public static AudioManager Instance;
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if(musicSource) musicSource.loop = true;
    }
    
    public void PlaySfx(AudioClip clip, float volume)
    {
        //Allows for SFX to be played in other scripts
        if(sfxSource)
        {
            sfxSource.volume=volume;
            sfxSource.PlayOneShot(clip);
        }
    }

    public void StopSfx()
    {
        if(sfxSource) sfxSource.Stop();
    }
}