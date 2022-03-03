using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DetectionService.Editor
{
    [CustomEditor(typeof(SensorEditorHelper3D))]
    [CanEditMultipleObjects]
    public class SensorEditorHelper3DEditor : UnityEditor.Editor
    {
        private DetectionSensor _mComp;
        private EllipseBoundsHandle _paddingModifier;

        private ArcHandle _mArcHandleLatN;
        private ArcHandle _mArcHandleLatS;
        private ArcHandle _mArcHandleLon;

        private static float DefaultMidpointHandleSizeFunction(Vector3 position)
        {
            return HandleUtility.GetHandleSize(position) * 0.1f;
        }

        private void OnEnable()
        {
            _paddingModifier = new EllipseBoundsHandle();
            _paddingModifier.SetColor(Color.white);
            _paddingModifier.center = Vector3.zero;
            _paddingModifier.handleColor = Color.red;
            _paddingModifier.midpointHandleSizeFunction = DefaultMidpointHandleSizeFunction;
            _paddingModifier.thickness = 3;

            _mArcHandleLatN = new ArcHandle();
            _mArcHandleLatN.SetColorWithoutRadiusHandle(new Color32(152, 237, 67, 255), 0.1f);
            _mArcHandleLatN.radiusHandleColor = Color.white;

            _mArcHandleLatS = new ArcHandle();
            _mArcHandleLatS.SetColorWithoutRadiusHandle(new Color32(152, 237, 67, 255), 0.1f);
            _mArcHandleLatS.radiusHandleColor = Color.white;

            _mArcHandleLon = new ArcHandle();
            _mArcHandleLon.SetColorWithoutRadiusHandle(new Color32(237, 67, 30, 255), 0.1f);
            _mArcHandleLon.radiusHandleColor = Color.white;
        }

        private void OnSceneGUI()
        {
            _mComp ??= ((SensorEditorHelper3D) target).GetSensorComponent();
            if (!_mComp) return;
            if (!_mComp.showAngleGizmos) return;
            
            DrawCapsule();
            DrawHandles();
        }

        private void DrawCapsule()
        {
            var tf = _mComp.transform;

            var fwd = tf.right;
            var normal = Vector3.Cross(fwd, tf.up);
            var matrix = Matrix4x4.TRS(
                tf.position,
                Quaternion.LookRotation(fwd, normal),
                Vector3.one
            );

            _paddingModifier.center = Vector3.zero;
            using (new Handles.DrawingScope(matrix))
            {
                EditorGUI.BeginChangeCheck();
                {
                    _paddingModifier.height = _mComp.capsuleHeight;
                    _paddingModifier.radius = _mComp.capsuleRadius;
                    _paddingModifier.DrawHandle();
                }

                if (!EditorGUI.EndChangeCheck()) return;
                if (Math.Abs(_mComp.capsuleHeight - _paddingModifier.height) > 0.01f)
                {
                    _mComp.capsuleHeight = _paddingModifier.height;
                    _mComp.OnValidate();
                }
                else if (Math.Abs(_mComp.capsuleRadius - _paddingModifier.radius) > 0.01f)
                {
                    _mComp.capsuleRadius = _paddingModifier.radius;
                    _mComp.OnValidate();
                }
            }
        }
        
        private void DrawHandles()
        {
            var tf = _mComp.transform;

            var fwd = tf.forward;
            var normal = Vector3.Cross(fwd, tf.up);
            var matrix = Matrix4x4.TRS(
                tf.position,
                Quaternion.LookRotation(fwd, -normal),
                Vector3.one
            );

            using (new Handles.DrawingScope(matrix))
            {
                EditorGUI.BeginChangeCheck();
                {
                    _mArcHandleLatN.angle = _mComp.angleNorth;
                    _mArcHandleLatN.radius = _mComp.rayDistance;
                    _mArcHandleLatN.DrawHandle();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (Math.Abs(_mComp.angleNorth - _mArcHandleLatN.angle) > 0.01f)
                    {
                        _mComp.angleNorth = _mArcHandleLatN.angle;
                        _mComp.OnValidate();
                    }
                    else if (Math.Abs(_mComp.rayDistance - _mArcHandleLatN.radius) > 0.01f)
                    {
                        _mComp.rayDistance = _mArcHandleLatN.radius;
                        _mComp.OnValidate();
                    }
                }
            }

            // Latitude South & Max. Distance.

            normal = Vector3.Cross(fwd, -tf.up);
            matrix = Matrix4x4.TRS(
                tf.position,
                Quaternion.LookRotation(fwd, normal),
                Vector3.one
            );

            using (new Handles.DrawingScope(matrix))
            {
                EditorGUI.BeginChangeCheck();
                {
                    _mArcHandleLatS.angle = _mComp.angleSouth;
                    _mArcHandleLatS.radius = _mComp.rayDistance;
                    _mArcHandleLatS.DrawHandle();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (Math.Abs(_mComp.angleSouth - _mArcHandleLatS.angle) > 0.01f)
                    {
                        _mComp.angleSouth = _mArcHandleLatS.angle;
                        _mComp.OnValidate();
                    }
                    else if (Math.Abs(_mComp.rayDistance - _mArcHandleLatS.radius) > 0.01f)
                    {
                        _mComp.rayDistance = _mArcHandleLatS.radius;
                        _mComp.OnValidate();
                    }
                }
            }

            // Longitude & Max. Distance.

            normal = Vector3.Cross(fwd, tf.right);
            matrix = Matrix4x4.TRS(
                tf.position,
                Quaternion.LookRotation(fwd, normal),
                Vector3.one
            );

            using (new Handles.DrawingScope(matrix))
            {
                EditorGUI.BeginChangeCheck();
                {
                    _mArcHandleLon.angle = _mComp.angleLon;
                    _mArcHandleLon.radius = _mComp.rayDistance;
                    _mArcHandleLon.DrawHandle();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    if (Math.Abs(_mComp.angleLon - _mArcHandleLon.angle) > 0.01f)
                    {
                        _mArcHandleLon.angle = _mArcHandleLon.angle > 120
                            ? 120
                            : _mArcHandleLon.angle = _mArcHandleLon.angle;
                        _mArcHandleLon.angle =
                            _mArcHandleLon.angle < 0 ? 0 : _mArcHandleLon.angle = _mArcHandleLon.angle;
                        _mComp.angleLon = _mArcHandleLon.angle;
                        _mComp.OnValidate();
                    }
                    else if (Math.Abs(_mComp.rayDistance - _mArcHandleLon.radius) > 0.01f)
                    {
                        _mComp.rayDistance = _mArcHandleLon.radius;
                        _mComp.OnValidate();
                    }
                }
            }
        }
    }
}