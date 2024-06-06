using System;
using Internal;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class AudioManager: SingletonMonoBehaviour<AudioManager>
    {
        [Header("References")]
        public AudioSource Convoy;

        [Header("Durability Low Alarm")]
        public AudioClip AlarmModerate;
        public AudioClip AlarmHeavy;
        public AudioClip AlarmCritical;

        public void ToggleClip(AudioClip clip)
        {
            if (Convoy.isPlaying) {
                Convoy.Stop();
            }

            if (Convoy.clip == clip) {
                Convoy.clip = null;
                return;
            }

            Convoy.clip = clip;
            Convoy.loop = true;
            Convoy.Play();
        }
        
        [ContextMenu("Stop")]
        public void Stop()
        {
            Convoy.Stop();
            Convoy.clip = null;
        }
        
        #region Low Durability Alarm SFX
        
        [ContextMenu("Play/Durability Low/Moderate")]
        public void StartDurabilityLowModerateAlarm()
        {
            if (Convoy.clip != AlarmModerate)
                ToggleClip(AlarmModerate);
        }
        
        [ContextMenu("Play/Durability Low/Heavy")]
        public void StartDurabilityLowHeavyAlarm()
        {
            if (Convoy.clip != AlarmHeavy)
                ToggleClip(AlarmHeavy);
        }
        
        [ContextMenu("Play/Durability Low/Critical")]
        public void StartDurabilityLowCriticalAlarm()
        {
            if (Convoy.clip != AlarmCritical)
                ToggleClip(AlarmCritical);
        }
        
        #endregion
    }
}
