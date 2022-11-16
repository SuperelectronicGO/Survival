using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
   
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private AudioSource musicSource;
    private float delayTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        musicSource.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        delayTimer += 1;
        if (delayTimer > 800 && !musicSource.isPlaying)
        {
            musicSource.clip = soundManager.titleScreenMusic;
            musicSource.Play();
        }
        if (delayTimer > 800 && delayTimer < 1901)
        {
            musicSource.volume = (delayTimer - 900) / 1000;
        }
        
    }
}
