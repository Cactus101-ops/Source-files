using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    public AudioClip worksuccess;
    public AudioClip happymeow;
    public AudioClip angrymeow;
    public AudioClip happybark;
    public AudioClip catworkfail;    
    public AudioClip backgroundmusic;
    public AudioClip alarmclock;
    public AudioClip money;
    public AudioClip levelup;
    private AudioSource audiosource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        audiosource = GetComponent<AudioSource>();
        if (audiosource == null)
        {
            audiosource = gameObject.AddComponent<AudioSource>();
        }
    }

    //=== Play a specific sound effect ===\\
    public void playsound(AudioClip clip)
    {
        if (clip != null)
        {
            audiosource.PlayOneShot(clip);
        }
       
    }
    public void playworksuccess()
    {//happy sounds
        playsound(money);
        playsound(happybark);
        playsound(happymeow);
    }

    public void playworkfail()
    {//sad sounds
        playsound(catworkfail);
        playsound(angrymeow);
    }

    public void playalarmclock()
    {//for when a dog is activated
        playsound(alarmclock);
    }

    public void playlevelup()
    {//self explanatory
        playsound(levelup);
    }

    public void playhappybark()
    {
        playsound(happybark);
    }   
}





