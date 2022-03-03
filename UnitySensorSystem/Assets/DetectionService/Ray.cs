using UnityEngine;

namespace DetectionService
{
    public struct DetectionRay
    {
        public readonly Ray Ray;
        public readonly Vector3 StartPoint;
        public readonly Vector3 Direction;
        public readonly Vector3 EndPoint;

        public DetectionRay(Ray ray, Vector3 startPoint, Vector3 endPoint, Vector3 direction)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            Direction = direction;
            Ray = ray;
        }
    }
}