using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] audioClips;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayAudio(int i) {
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = audioClips[i];
        audioSource.Play();
    }
}
