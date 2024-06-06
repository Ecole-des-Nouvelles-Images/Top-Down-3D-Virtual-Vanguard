using System;
using UnityEngine;

namespace Game
{
    [ExecuteAlways]
    public class AudioManager: MonoBehaviour
    {
        #region Test

        [Header("References")]
        public AudioSource Convoy;

        [Header("Durability Low Alarm")]
        public AudioClip AlarmModerate;
        public AudioClip AlarmHeavy;
        public AudioClip AlarmCritical;

        [ContextMenu("Play/Durability Low/Moderate")]
        public void DurabilityLowModerate()
        {
            ToggleClip(AlarmModerate);
        }
        
        [ContextMenu("Play/Durability Low/Heavy")]
        public void DurabilityLowHeavy()
        {
            ToggleClip(AlarmHeavy);
        }
        
        [ContextMenu("Play/Durability Low/Critical")]
        public void DurabilityLowCritical()
        {
            ToggleClip(AlarmCritical);
        }

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

        #endregion
    }
}
