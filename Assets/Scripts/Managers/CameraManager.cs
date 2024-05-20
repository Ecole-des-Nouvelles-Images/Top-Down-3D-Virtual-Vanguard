using System;
using System.Linq;
using UnityEngine;

using Internal;
using Terrain;
using Cinemachine;

namespace Managers
{
    public class CameraManager: SingletonMonoBehaviour<CameraManager>
    {
        [Header("Camera systems")]
        public GameObject LeftCam;
        public GameObject RightCam;
        public GameObject CenteredCam;

        public GameObject CurrentCamera { get; private set; }

        private void Start()
        {
            SwitchCameraFocus(FindObjectOfType<TerrainManager>().FocusMode, true);
        }

        public void SwitchCameraFocus(FocusMode mode, bool useFarthermostCamera)
        {
            switch (mode)
            {
                case FocusMode.Centered:
                    CenteredCam?.SetActive(true);
                    LeftCam?.SetActive(false);
                    RightCam?.SetActive(false);
                    CurrentCamera = CenteredCam;
                    break;
                case FocusMode.Left:
                    CenteredCam?.SetActive(false);
                    LeftCam?.SetActive(true);
                    RightCam?.SetActive(false);
                    CurrentCamera = LeftCam;
                    break;
                case FocusMode.Right:
                    CenteredCam?.SetActive(false);
                    LeftCam?.SetActive(false);
                    RightCam?.SetActive(true);
                    CurrentCamera = RightCam;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            if (!CurrentCamera)
                throw new NullReferenceException("CameraManager: CurrentCamera is not set at this point to manage virtual cameras");

            if (useFarthermostCamera)
            {
                var farthermostCamera = GetComponentsInChildren<CinemachineVirtualCamera>().OrderByDescending(vc => vc.m_Lens.FieldOfView).FirstOrDefault();
                
                foreach (CinemachineVirtualCamera virtualCamera in CurrentCamera.GetComponentsInChildren<CinemachineVirtualCamera>()) {
                    if (virtualCamera != farthermostCamera) 
                        virtualCamera.gameObject.SetActive(false);
                }
                
                farthermostCamera?.gameObject.SetActive(true);
            }
            else
            {
                foreach (CinemachineVirtualCamera virtualCamera in CurrentCamera.GetComponentsInChildren<CinemachineVirtualCamera>()) {
                    virtualCamera.gameObject.SetActive(true);
                }
            }
        }
    }
}
