using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MenuAudioManager: MonoBehaviour
    {
        [Header("References")] 
        public AudioSource MusicSource;
        public AudioSource FXSource;

        public Scrollbar MusicVolume;
        public Scrollbar FXVolume;

        private void Start()
        {
            MusicSource.volume = MusicVolume.value;
            MusicSource.loop = true;
            FXSource.volume = FXVolume.value;
            FXSource.loop = false;
        }

        public void PlayFX(AudioClip fx)
        {
            FXSource.Stop();
            FXSource.clip = fx;
            FXSource.Play();
        }
    }
}
