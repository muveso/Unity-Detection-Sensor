using UnityEngine;

namespace DetectionService
{
    public class SensorEditorHelper3D : MonoBehaviour
    {
        public DetectionSensor GetSensorComponent()
        {
            return TryGetComponent(out DetectionSensor comp) ? comp : null;
        }
    }
    
}