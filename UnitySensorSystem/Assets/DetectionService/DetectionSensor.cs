using UnityEngine;

namespace DetectionService
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(SensorEditorHelper3D))]
    [ExecuteInEditMode]
    public class DetectionSensor : MonoBehaviour 
    {
        [SerializeField] public bool showAngleGizmos = true;
        
        public float rayDistance = 5f;
        
        [Range(0, 50)] 
        public int resolutionX = 7;
        [Range(0, 50)] 
        public int resolutionY = 5;

        [Range(0.05f, 5)] public float gizmoRadius = 1;
        
        [SerializeField, HideInInspector] public float angleNorth = 10;
        [SerializeField, HideInInspector] public float angleSouth = -10;
        [SerializeField, HideInInspector] public float angleLon = 60;
        [SerializeField, HideInInspector] public float capsuleHeight = 7;
        [SerializeField, HideInInspector] public float capsuleRadius = 3;
        
        public DetectionRay[] Rays;
        private CapsuleCollider _checkCapsule;
        
        private void Awake()
        {
            _checkCapsule ??= GetComponent<CapsuleCollider>();
            _checkCapsule.hideFlags = HideFlags.HideInInspector;
            
            CreateRays();
            Destroy(_checkCapsule);
        }
        
#if UNITY_EDITOR
        public void OnValidate()
        {
            _checkCapsule ??= GetComponent<CapsuleCollider>();
            _checkCapsule.hideFlags = HideFlags.HideInInspector;
            
            _checkCapsule.center = Vector3.zero;
            _checkCapsule.radius = capsuleRadius;
            _checkCapsule.height = capsuleHeight;
            _checkCapsule.direction = 2;
            
            rayDistance = Mathf.Max(0f, rayDistance);
            
            CreateRays();
        }
#endif  

        private void CreateRays()
        {
            Rays = new DetectionRay[resolutionX * resolutionY];
            
            for (var y = 0; y < resolutionY; y++)
            {
                for (var x = 0; x < resolutionX; x++)
                {
                    var i = resolutionY * x + y;
                    var ax = (resolutionY > 1) ? Mathf.Lerp(angleSouth, angleNorth, (y / (float)(resolutionY - 1))): 0;
                    var ay = (resolutionX > 1) ? Mathf.Lerp(-angleLon, angleLon, x / (float) (resolutionX - 1)) : 0;
                    
                    var dir = Quaternion.Euler(ax, ay, 0f) * Vector3.forward;
                    var virtualPoint = transform.position + (dir * 99999);
                    
                    var startPoint = _checkCapsule.ClosestPoint(virtualPoint);
                    var endPoint = startPoint + (dir * rayDistance);
                    Rays[i] = new DetectionRay(new Ray(startPoint, dir), startPoint, endPoint, dir);
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            for (var i = 0; i < Rays.Length; i++)
            {
                var ray = Rays[i];
                var startPoint = ray.StartPoint;
                var endPoint = ray.EndPoint;
                
                Gizmos.color = new Color(1f, 0.5f, 1f, 0.75f);
                Gizmos.DrawSphere(startPoint, gizmoRadius * .05f);
                
                if (Physics.Raycast(ray.Ray, out var hit))
                {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
                    Gizmos.DrawLine(startPoint, hit.point);

                    Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
                    Gizmos.DrawSphere(hit.point, gizmoRadius);
                }
                else
                {
                    Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
                    Gizmos.DrawLine(startPoint, endPoint);

                    Gizmos.color = new Color(0f, 0.5f, 1f, 0.75f);
                    Gizmos.DrawSphere(endPoint, gizmoRadius);
                }
            }
        }
#endif
        
    }
}