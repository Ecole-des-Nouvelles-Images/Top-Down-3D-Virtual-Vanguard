using System;
using Cinemachine;
using Internal;
using UnityEngine;

namespace Game
{
    public class CameraManager: SingletonMonoBehaviour<CameraManager>
    {
        [Header("Camera systems")]
        public GameObject LeftCam;
        public GameObject RightCam;
        public GameObject CenteredCam;
        public GameObject FollowCam;

        [Header("Settings")]
        public float TransitShakeAmplitude;
        
        public GameObject CurrentCamera { get; private set; }

        private void Start()
        {
            CinemachineVirtualCamera vcam = FollowCam.GetComponent<CinemachineVirtualCamera>();
            vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = TransitShakeAmplitude;
        }

        public void SwitchCameraFocus(bool switchToFollowCamera, Side mode)
        {
            try
            {
                switch (mode)
                {
                    case Side.Centered:
                        CenteredCam?.SetActive(true);
                        LeftCam?.SetActive(false);
                        RightCam?.SetActive(false);
                        CurrentCamera = CenteredCam;
                        break;
                    case Side.Left:
                        CenteredCam?.SetActive(false);
                        LeftCam?.SetActive(true);
                        RightCam?.SetActive(false);
                        CurrentCamera = LeftCam;
                        break;
                    case Side.Right:
                        CenteredCam?.SetActive(false);
                        LeftCam?.SetActive(false);
                        RightCam?.SetActive(true);
                        CurrentCamera = RightCam;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                }
            }
            catch (Exception e)
            {
                if (e is NullReferenceException)
                    Debug.LogError($"Null Reference Exception raised from {e.Source}: {e.Message}");

                if (CurrentCamera is null)
                    Debug.LogWarning("CurrentCamera is not set at this point");

                throw;
            }

            FollowCam.SetActive(switchToFollowCamera);

            /* if (useFarthermostCamera)
                FindCameraByFOV(false).SetActive(false);
            else
                FindCameraByFOV(true).SetActive(false); */
        }

        private GameObject FindCameraByFOV(bool findFarthermost)
        {
            if (CurrentCamera is null) return null;

            GameObject cameraToFind = null;
            float fov = (findFarthermost) ? Mathf.NegativeInfinity : Mathf.Infinity;
            
            foreach (Transform child in CurrentCamera.transform)
            {
                CinemachineVirtualCamera cam = child.GetComponent<CinemachineVirtualCamera>();

                if (!cam) continue;

                if (findFarthermost)
                {
                    if (!(cam.m_Lens.FieldOfView > fov)) continue;
                    
                    cameraToFind = cam.gameObject;
                    fov = cam.m_Lens.FieldOfView;
                }
                else
                {
                    if (!(cam.m_Lens.FieldOfView < fov)) continue;
                    
                    cameraToFind = cam.gameObject;
                    fov = cam.m_Lens.FieldOfView;
                }
            }
            
            return cameraToFind;
        }
    }
}
