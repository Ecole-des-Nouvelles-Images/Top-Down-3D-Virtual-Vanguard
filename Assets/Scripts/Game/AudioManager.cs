﻿using System.Collections;
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

        private float Volume { get; set; }
        
        [Header("Sources")]
        public AudioSource MusicSourceTransit;
        public AudioSource MusicSourceStop;
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

        [Header("Parameters")]
        public float TransitionDuration = 3f;

        private void Start()
        {
            Volume = MusicVolume.value;
            MusicSourceTransit.clip = TransitClip;
            MusicSourceTransit.loop = true;
            MusicSourceStop.clip = StopClip;
            MusicSourceStop.loop = true;
            
            SetVolumeManual("BackgroundStopVolume", 0);
            SetVolumeManual("BackgroundTransitVolume");
            
            MusicSourceTransit.Play();
            MusicSourceStop.Play();
        }

        public void SwitchToStopZone(float transitionDuration = 3f)
        {
            StopAllCoroutines();
            StartCoroutine(SwitchToBackgroundStop(transitionDuration));
        }
        
        public void SwitchToTransit(float transitionDuration = 3f)
        {
            StopAllCoroutines();
            StartCoroutine(SwitchToBackgroundTransit(transitionDuration));
        }

        public void SetVolumeManual(string volumeParameter, float manualOverride = -1)
        {
            float decibels = -80 * (1 - Volume);
            float manual = -80 * (1 - manualOverride);
            
            if (manualOverride < 0)
            {
                Master.SetFloat(volumeParameter, decibels);
            }
            else
            {
                Master.SetFloat(volumeParameter, manual);
            }
        }
        
        public void SetSFXVolume()
        {
            float decibels = -80 * (1 - FXVolume.value);
            Master.SetFloat("SFXVolume", decibels);
        }

        private IEnumerator SwitchToBackgroundTransit(float duration)
        {
            float t = 0f;
            
            Master.GetFloat("BackgroundStopVolume", out float initialStop);
            float normalVolume = -80 * (1 - Volume);

            while (t < 1)
            {
                t += Time.unscaledDeltaTime / duration;
                float modifiedT = Mathf.Pow(t, 2); // Use the square of t as the input to SmoothStep
                float smoothT = Mathf.SmoothStep(0f, 1f, modifiedT);
                Master.SetFloat("BackgroundTransitVolume", Mathf.Lerp(-80, normalVolume, smoothT));
                Master.SetFloat("BackgroundStopVolume", Mathf.Lerp(initialStop, 0, smoothT));
                yield return null;
            }
        }
        
        private IEnumerator SwitchToBackgroundStop(float duration)
        {
            float t = 0f;

            if (duration == 0f) duration = 5f;
            
            Master.GetFloat("BackgroundTransitVolume", out float initialTransit);
            float normalVolume = -80 * (1 - Volume);
            
            while (t < 1)
            {
                t += Time.unscaledDeltaTime / duration;
                float modifiedT = Mathf.Pow(t, 2); // Use the square of t as the input to SmoothStep
                float smoothT = Mathf.SmoothStep(0f, 1f, modifiedT);
                Master.SetFloat("BackgroundTransitVolume", Mathf.Lerp(initialTransit, -80, smoothT));
                Master.SetFloat("BackgroundStopVolume", Mathf.Lerp(-80, normalVolume, smoothT));
                yield return null;
            }
        }

        public void UpdateMusicVolume()
        {
            Volume = MusicVolume.value;
            float decibels = -80 * (1 - Volume);
            
            if (GameManager.Instance.IsInTransit)
                Master.SetFloat("BackgroundTransitVolume", decibels);
            else
                Master.SetFloat("BackgroundStopVolume", decibels);
        }

        public void UpdateSFXVolume()
        {
            Master.SetFloat("SFXVolume", FXVolume.value);
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
