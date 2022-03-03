using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace DetectionService
{
    /// <summary>
    ///     <para>A compound handle to edit a capsule-shaped bounding volume in the Scene view.</para>
    /// </summary>
    public class EllipseBoundsHandle : PrimitiveBoundsHandle
    {
        /// <summary>
        ///     <para>An enumeration for specifying which axis on a EllipseBoundsHandle object maps to the capsuleHeight parameter.</para>
        /// </summary>
        private enum HeightAxis
        {
            X,
            Y,
            Z
        }

        private const int KDirectionX = 0;
        private const int KDirectionY = 1;
        private const int KDirectionZ = 2;

        private static readonly Vector3[] SHeightAxes = new Vector3[3]
        {
            Vector3.right,
            Vector3.up,
            Vector3.forward
        };

        private static readonly int[] SNextAxis = new int[3]
        {
            1,
            2,
            0
        };

        private int _mHeightAxis = 1;

        /// <summary>
        ///     <para>Create a new instance of the EllipseBoundsHandle class.</para>
        /// </summary>
        /// <param name="controlIDHint">
        ///     An integer value used to generate consistent control IDs for each control handle on this
        ///     instance. Avoid using the same value for all of your EllipseBoundsHandle instances.
        /// </param>
        [Obsolete("Use parameterless constructor instead.")]
        public EllipseBoundsHandle(int controlIDHint)
            : base(controlIDHint)
        {
        }

        /// <summary>
        ///     <para>Create a new instance of the EllipseBoundsHandle class.</para>
        /// </summary>
        public EllipseBoundsHandle()
        {
        }

        /// <summary>
        ///     <para>
        ///         Returns or specifies the axis in the handle's space to which capsuleHeight maps. The capsuleRadius maps to the remaining
        ///         axes.
        ///     </para>
        /// </summary>
        private HeightAxis heightAxis
        {
            get => (HeightAxis) _mHeightAxis;
            set
            {
                var index = (int) value;
                if (_mHeightAxis == index)
                    return;
                var size = Vector3.one * radius * 2f;
                size[index] = GetSize()[_mHeightAxis];
                _mHeightAxis = index;
                SetSize(size);
            }
        }

        private float _mThickness = 1;

        /// <summary>
        ///     <para>
        ///         Returns or specifies the axis in the handle's space to which capsuleHeight maps. The capsuleRadius maps to the remaining
        ///         axes.
        ///     </para>
        /// </summary>
        public float thickness
        {
            get => _mThickness;
            set => _mThickness = value;
        }

        /// <summary>
        ///     <para>Returns or specifies the capsuleHeight of the capsule bounding volume.</para>
        /// </summary>
        public float height
        {
            get => !IsAxisEnabled(_mHeightAxis) ? 0.0f : Mathf.Max(GetSize()[_mHeightAxis], 2f * radius);
            set
            {
                value = Mathf.Max(Mathf.Abs(value), 2f * radius);
                if (height == (double) value)
                    return;
                var size = GetSize();
                size[_mHeightAxis] = value;
                SetSize(size);
            }
        }

        /// <summary>
        ///     <para>Returns or specifies the capsuleRadius of the capsule bounding volume.</para>
        /// </summary>
        public float radius
        {
            get
            {
                int radiusAxis;
                return GetRadiusAxis(out radiusAxis) || IsAxisEnabled(_mHeightAxis)
                    ? 0.5f * GetSize()[radiusAxis]
                    : 0.0f;
            }
            set
            {
                var size = GetSize();
                var b = 2f * value;
                for (var index = 0; index < 3; ++index)
                    size[index] = index == _mHeightAxis ? Mathf.Max(size[index], b) : b;
                SetSize(size);
            }
        }

        public Bounds bounds;

        /// <summary>
        ///     <para>Draw a wireframe capsule for this instance.</para>
        /// </summary>
        protected override void DrawWireframe()
        {
            var vector3Axis1 = HeightAxis.Y;
            var vector3Axis2 = HeightAxis.Z;

            switch (heightAxis)
            {
                case HeightAxis.Y:
                    vector3Axis1 = HeightAxis.Z;
                    vector3Axis2 = HeightAxis.X;
                    break;
                case HeightAxis.Z:
                    vector3Axis1 = HeightAxis.X;
                    vector3Axis2 = HeightAxis.Y;
                    break;
                case HeightAxis.X:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var flag1 = IsAxisEnabled((int) heightAxis);
            var flag2 = IsAxisEnabled((int) vector3Axis1);
            var flag3 = IsAxisEnabled((int) vector3Axis2);
            var heightAx1 = SHeightAxes[_mHeightAxis];
            var heightAx2 = SHeightAxes[SNextAxis[_mHeightAxis]];
            var heightAx3 = SHeightAxes[SNextAxis[SNextAxis[_mHeightAxis]]];

            var center1 = center + heightAx1 * (height * 0.5f - radius);
            var center2 = center - heightAx1 * (height * 0.5f - radius);
            if (flag1)
            {
                if (flag3)
                {
                    Handles.DrawWireArc(center1, heightAx2, heightAx3, 180f, radius, thickness);
                    Handles.DrawWireArc(center2, heightAx2, heightAx3, -180f, radius, thickness);
                    Handles.DrawLine(center1 + heightAx3 * radius, center2 + heightAx3 * radius, thickness);
                    Handles.DrawLine(center1 - heightAx3 * radius, center2 - heightAx3 * radius, thickness);
                }

                if (flag2)
                {
                    Handles.DrawWireArc(center1, heightAx3, heightAx2, -180f, radius, thickness);
                    Handles.DrawWireArc(center2, heightAx3, heightAx2, 180f, radius, thickness);
                    Handles.DrawLine(center1 + heightAx2 * radius, center2 + heightAx2 * radius, thickness);
                    Handles.DrawLine(center1 - heightAx2 * radius, center2 - heightAx2 * radius, thickness);
                }
            }

            if (!(flag2 & flag3))
                return;
            Handles.DrawWireArc(center1, heightAx1, heightAx2, 360f, radius, thickness);
            Handles.DrawWireArc(center2, heightAx1, heightAx2, -360f, radius, thickness);
        }

        protected override Bounds OnHandleChanged(
            HandleDirection handle,
            Bounds boundsOnClick,
            Bounds newBounds)
        {
            var index1 = 0;
            switch (handle)
            {
                case HandleDirection.PositiveY:
                case HandleDirection.NegativeY:
                    index1 = 1;
                    break;
                case HandleDirection.PositiveZ:
                case HandleDirection.NegativeZ:
                    index1 = 2;
                    break;
            }

            var max = newBounds.max;
            var min = newBounds.min;
            if (index1 == _mHeightAxis)
            {
                GetRadiusAxis(out var radiusAxis);
                var num = max[radiusAxis] - min[radiusAxis];
                if (max[_mHeightAxis] - min[_mHeightAxis] < (double) num)
                {
                    if (handle == HandleDirection.PositiveX || handle == HandleDirection.PositiveY ||
                        handle == HandleDirection.PositiveZ)
                        max[_mHeightAxis] = min[_mHeightAxis] + num;
                    else
                        min[_mHeightAxis] = max[_mHeightAxis] - num;
                }
            }
            else
            {
                max[_mHeightAxis] = boundsOnClick.center[_mHeightAxis] + 0.5f * boundsOnClick.size[_mHeightAxis];
                ref var local1 = ref min;
                var heightAxis = _mHeightAxis;
                double num1 = boundsOnClick.center[_mHeightAxis];
                var vector3 = boundsOnClick.size;
                var num2 = 0.5 * vector3[_mHeightAxis];
                var num3 = num1 - num2;
                local1[heightAxis] = (float) num3;
                var b = (float) (0.5 * (max[index1] - (double) min[index1]));
                var a = (float) (0.5 * (max[_mHeightAxis] - (double) min[_mHeightAxis]));
                for (var index2 = 0; index2 < 3; ++index2)
                    if (index2 != index1)
                    {
                        var num4 = index2 == _mHeightAxis ? Mathf.Max(a, b) : b;
                        ref var local2 = ref min;
                        var index3 = index2;
                        vector3 = center;
                        var num5 = vector3[index2] - (double) num4;
                        local2[index3] = (float) num5;
                        ref var local3 = ref max;
                        var index4 = index2;
                        vector3 = center;
                        var num6 = vector3[index2] + (double) num4;
                        local3[index4] = (float) num6;
                    }
            }

            bounds = new Bounds((max + min) * 0.5f, max - min);
            return bounds;
        }

        private bool GetRadiusAxis(out int radiusAxis)
        {
            radiusAxis = SNextAxis[_mHeightAxis];
            if (IsAxisEnabled(radiusAxis))
                return IsAxisEnabled(SNextAxis[radiusAxis]);
            radiusAxis = SNextAxis[radiusAxis];
            return false;
        }
    }
}