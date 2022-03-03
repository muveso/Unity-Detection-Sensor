using DetectionService;
using UnityEngine;

namespace Test.Scripts
{
    public class TestMono : MonoBehaviour
    {
        [SerializeField] public DetectionSensor detectionSensor;
        private DetectionRay[] _sensorRays;
        
        private void Start()
        {
            _sensorRays = detectionSensor.Rays;
        }

        private void FixedUpdate()
        {
            for (var i = 0; i < _sensorRays.Length; i++)
            {
                var ray = _sensorRays[i];
                if (Physics.Raycast(ray.Ray))
                {
                    Debug.Log("Hit!");
                }
            }
        }
    }
}
