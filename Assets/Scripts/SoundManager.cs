using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    // Sound Effects
    public AudioSource dropItemSound;

    public AudioSource pickUpItemSound;
    public AudioSource craftingSound;

    public AudioSource toolSwingSound;
    public AudioSource chopSound;
    public AudioSource grassWalkSound;
    public AudioSource pullaxeSound;
    public AudioSource choppabletreeSound;

    //Music
    public AudioSource startingZoneBGMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlaySound(AudioSource soundToPlay)
    {
        if (!soundToPlay.isPlaying)
        {
            soundToPlay.Play();
        }
    }

}
