using System;
using Internal;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Game
{
    public class AudioManager: SingletonMonoBehaviour<AudioManager>
    {
        [Header("References")]
        public AudioMixer Master;
        public Scrollbar MusicVolume;
        public Scrollbar FXVolume;
        
        [Header("Sources")]
        public AudioSource MusicSource;
        public AudioSource FXConvoy;
        public AudioSource FXDrone;
        public AudioSource FXXenolith;

        [Header("Music Clips")]
        public AudioClip TransitClip;
        public AudioClip StopClip;

        [Header("Convoy FXs")]
        public AudioClip AlarmModerate;
        public AudioClip AlarmHeavy;
        public AudioClip AlarmCritical;

        private void Start()
        {
            SetBackgroundVolume();
            SetSFXVolume();
            SwitchToTransit();
        }

        public void SwitchToStopZone(bool loop = true)
        {
            MusicSource.Stop();
            MusicSource.clip = StopClip;
            MusicSource.loop = loop;
            MusicSource.Play();
        }
        
        public void SwitchToTransit(bool loop = true)
        {
            MusicSource.Stop();
            MusicSource.clip = TransitClip;
            MusicSource.loop = loop;
            MusicSource.Play();
        }

        public void SetBackgroundVolume()
        {
            float decibels = -80 * (1 - MusicVolume.value);
            Master.SetFloat("BackgroundVolume", decibels);
        }
        
        public void SetSFXVolume()
        {
            float decibels = -80 * (1 - FXVolume.value);
            Master.SetFloat("SFXVolume", decibels);
        }
        
        #region Low Durability Alarm SFX
        
        [ContextMenu("Stop")]
        public void StopConvoyFX()
        {
            FXConvoy.Stop();
            FXConvoy.clip = null;
        }
        
        public void SwitchConvoyFX(AudioClip clip)
        {
            FXConvoy.Stop();
            FXConvoy.clip = clip;
            FXConvoy.loop = true;
            FXConvoy.Play();
        }
        
        
        [ContextMenu("Play/Durability Low/Moderate")]
        public void StartDurabilityLowModerateAlarm()
        {
            if (FXConvoy.clip != AlarmModerate)
                SwitchConvoyFX(AlarmModerate);
        }
        
        [ContextMenu("Play/Durability Low/Heavy")]
        public void StartDurabilityLowHeavyAlarm()
        {
            if (FXConvoy.clip != AlarmHeavy)
                SwitchConvoyFX(AlarmHeavy);
        }
        
        [ContextMenu("Play/Durability Low/Critical")]
        public void StartDurabilityLowCriticalAlarm()
        {
            if (FXConvoy.clip != AlarmCritical)
                SwitchConvoyFX(AlarmCritical);
        }
        
        #endregion
    }
}
