using System;
using System.Numerics;
using Foes;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using Unity.VisualScripting;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Convoy.Modules
{
    public class Laser: Module
    {
        [Header("Laser specifics")]
        [SerializeField] private Transform _turret;
        [SerializeField] private Transform _canon;
        [SerializeField] private float _torque;
        [SerializeField] private int _range;
        [SerializeField] private int _damagePerSecond;
        
        [Header("Raycast settings")]
        [SerializeField] private LayerMask _targetLayers;
        [Tooltip("Keenness of the rays below the original one.\n/!\\ Increase the number of raycast made until the floor is reached.")]
        [SerializeField] [Range(0.001f, 1)] private float _raySlicePrecision = 0.1f;

        [Header("Rendering")]
        [SerializeField] private LineRenderer _beam;
        [SerializeField] private int _lineRendererVertices = 10;
        
        private bool _firing;

        protected override void Awake()
        {
            base.Awake();
            _firing = false;
        }

        private void Start()
        {
            _beam.material = new Material(Shader.Find("Sprites/Default"));
            _beam.widthMultiplier = 0.2f;
            _beam.positionCount = _lineRendererVertices;

            // A simple 2 color gradient with a fixed alpha of 1.0f.
            float alpha = 1.0f;
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
            );
            _beam.colorGradient = gradient;
        }

        protected override void Update()
        {
            if (!Online) return;
            
            if (BatteryCharge <= 0) {
                _firing = false;
                return;
            }

            if (_firing) {
                Fire();
            }
            else {
                _beam.enabled = false;
            }
        }

        #region Logic

        private void Fire()
        {
            _beam.enabled = true;
            
            Xenolith unit = RaycastForwardTarget();
            if (unit)
                unit.TakeDamage(_damagePerSecond * Time.deltaTime);
        }

        private Xenolith RaycastForwardTarget()
        {
            RaycastHit hit;
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;

            for (float rayHeight = _canon.position.y; rayHeight > 0; rayHeight -= _raySlicePrecision)
            {
                Vector3 rayOrigin = _canon.position - new Vector3(0, rayHeight, 0);

                if (Physics.Raycast(rayOrigin, _canon.forward, out hit, _range, _targetLayers))
                {
                    if (hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestCollider = hit.collider;
                    }
                }
                
                Debug.DrawRay(rayOrigin, _canon.forward * _range, Color.magenta, Time.deltaTime * 2); // TODO: Position good in space but weird to the camera ?
            }
            
            // UpdateLaserBeam(_canon.forward, closestDistance > _range ? _range : closestDistance); // TODO: do not work yet, weird offset
            return closestCollider ? closestCollider.gameObject.GetComponent<Xenolith>() : null;
        }

        private void UpdateLaserBeam(Vector3 direction, float distance)
        {
            Vector3[] lineVertices = new Vector3[_lineRendererVertices];
            float forwardPos;
            
            for (int vert = 0; vert < _lineRendererVertices; vert++)
            {
                forwardPos = vert * (distance / _lineRendererVertices);
                lineVertices[vert] = new Vector3(0, Mathf.Sin(vert + Time.time), forwardPos);
            }
            
            Debug.Log(lineVertices);
            _beam.SetPositions(lineVertices);
        }

        #endregion

        #region Actions

        public override void Operate(PlayerController currentController)
        {
            _firing = !_firing;
        }

        public override void Aim(InputValue input)
        {
            Vector2 value = input.Get<Vector2>();
            
            _turret.LookAt(_turret.position + new Vector3(value.x,0, value.y));
        }

        public override bool ExitModule(PlayerController currentController)
        {
            _firing = false;
            
            return base.ExitModule(currentController);
        }

        #endregion

        #region Debug

        private void DrawLaser()
        {
            if (_range <= 0) {
                Debug.LogWarning("Range must be greater than 0");
                return;
            }
            
            Vector3 endPoint = _canon.position + _canon.forward * _range;
            Debug.DrawLine(_canon.position, endPoint, Color.red, Time.deltaTime);
        }

        #endregion
    }
}
