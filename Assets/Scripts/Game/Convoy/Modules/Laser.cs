using Game.Foes.FSM;
using Game.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Convoy.Modules
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
        [SerializeField] [Range(0.001f, 1)] private float _raySliceKeenness = 0.1f;
        [SerializeField] private float _sphereCastRadius = 5f;

        [Header("Rendering")]
        public bool EnableLaserRendering;
        [SerializeField] private LineRenderer _beam;
        [SerializeField] private int _lineRendererVertices = 10;
        [SerializeField] private Gradient _beamColor;
        [SerializeField] private float _beamThickness = 1;
        [SerializeField] private float _beamFrequency = 1;
        [SerializeField] private float _beamAmplitude = 1;
        [SerializeField] private float _beamAnimSpeed = 3;

        #region Debug

        private void OnDrawGizmos()
        {
            if (Physics.SphereCast(_canon.position, _sphereCastRadius, _canon.forward, out RaycastHit hit, _range, 1 << LayerMask.NameToLayer("Xenolith")))
            {
                Gizmos.color = Color.green;
                Vector3 sphereCastMidpoint = _canon.position + (_canon.forward * hit.distance);
                Gizmos.DrawWireSphere(sphereCastMidpoint, _sphereCastRadius);
                Gizmos.DrawSphere(hit.point, 0.1f);
                Debug.DrawLine(_canon.position, sphereCastMidpoint, Color.green);
            }
            else
            {
                Gizmos.color = Color.red;
                Vector3 sphereCastMidpoint = _canon.position + (_canon.forward * (_range - _sphereCastRadius));
                Gizmos.DrawWireSphere(sphereCastMidpoint, _sphereCastRadius);
                Debug.DrawLine(_canon.position, sphereCastMidpoint, Color.red);
            }
        }

        #endregion
        
        private bool _firing;

        protected override void Awake()
        {
            base.Awake();
            _firing = false;
        }

        private void Start()
        {
            if (EnableLaserRendering)
            {
                _beam.material = new Material(Shader.Find("Sprites/Default"));
                _beam.positionCount = _lineRendererVertices;
                _beam.widthMultiplier = _beamThickness;
                _beam.colorGradient = _beamColor;
            }
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
                BatteryGauge.value = BatteryCharge;
            }
            else {
                if (EnableLaserRendering) _beam.enabled = false;
            }
        }

        #region Logic

        private void Fire()
        {
            if (EnableLaserRendering) _beam.enabled = true;
            
            Xenolith unit = RaycastForwardTarget();
            if (unit)
                unit.TakeDamage(_damagePerSecond * Time.deltaTime);

            BatteryCharge -= ConsumptionPerSecond * Time.deltaTime;
        }

        private Xenolith RaycastForwardTarget()
        {
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;

            for (float rayHeight = _canon.position.y; rayHeight > 0; rayHeight -= _raySliceKeenness)
            {
                Vector3 rayOrigin = _canon.position - new Vector3(0, rayHeight, 0);

                if (Physics.SphereCast(rayOrigin, _sphereCastRadius,_canon.forward, out RaycastHit hit, _range, _targetLayers))
                {
                    if (hit.distance < closestDistance)
                    {
                        closestDistance = hit.distance;
                        closestCollider = hit.collider;
                    }
                }
            }
            
            if (EnableLaserRendering) UpdateLaserBeam(closestDistance > _range ? _range : closestDistance); // TODO: do not work yet, weird offset
            return closestCollider ? closestCollider.gameObject.GetComponent<Xenolith>() : null;
        }

        private void UpdateLaserBeam(float distance)
        {
            Vector3[] lineVertices = new Vector3[_lineRendererVertices];
            float forwardPos;

            lineVertices[0] = Vector3.zero;
            
            for (int vert = 1; vert < _lineRendererVertices; vert++)
            {
                forwardPos = vert * (distance / _lineRendererVertices);
                lineVertices[vert] = new Vector3(0, Mathf.Sin(_beamFrequency * vert + _beamAnimSpeed * Time.time) * _beamAmplitude, forwardPos);
            }
            
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
            
            _turret.LookAt(_turret.position + new Vector3(value.x,90, value.y));
        }

        public override bool ExitModule(PlayerController currentController)
        {
            _firing = false;
            
            return base.ExitModule(currentController);
        }

        #endregion

        #region Debug

        /* private void DrawLaser()
        {
            if (_range <= 0) {
                Debug.LogWarning("Range must be greater than 0");
                return;
            }
            
            Vector3 endPoint = _canon.position + _canon.forward * _range;
            Debug.DrawLine(_canon.position, endPoint, Color.red, Time.deltaTime);
        } */

        #endregion
    }
}
