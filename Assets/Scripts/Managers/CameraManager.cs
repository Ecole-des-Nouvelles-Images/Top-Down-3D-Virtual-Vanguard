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
        
        public Transform CurrentCamera => _currentCamera.GetComponentsInChildren<CinemachineVirtualCamera>().First(vcam => vcam.enabled).transform;

        private GameObject _currentCamera;
        
        public void SwitchCameraFocus(FocusMode mode, bool useFarthermostCamera)
        {
            switch (mode)
            {
                case FocusMode.Centered:
                    CenteredCam.SetActive(true);
                    LeftCam.SetActive(false);
                    RightCam.SetActive(false);
                    _currentCamera = CenteredCam;
                    break;
                case FocusMode.Left:
                    CenteredCam.SetActive(false);
                    LeftCam.SetActive(true);
                    RightCam.SetActive(false);
                    _currentCamera = LeftCam;
                    break;
                case FocusMode.Right:
                    CenteredCam.SetActive(false);
                    LeftCam.SetActive(false);
                    RightCam.SetActive(true);
                    _currentCamera = RightCam;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            if (useFarthermostCamera)
            {
                var farthermostCamera = GetComponentsInChildren<CinemachineVirtualCamera>().OrderByDescending(vc => vc.m_Lens.FieldOfView).FirstOrDefault();
                
                foreach (CinemachineVirtualCamera vcam in _currentCamera.GetComponentsInChildren<CinemachineVirtualCamera>()) {
                    if (vcam != farthermostCamera) 
                        vcam.gameObject.SetActive(false);
                }
                
                farthermostCamera?.gameObject.SetActive(true);
            }
            else
            {
                foreach (CinemachineVirtualCamera vcam in _currentCamera.GetComponentsInChildren<CinemachineVirtualCamera>()) {
                    vcam.gameObject.SetActive(true);
                }
            }
        }
    }
}
