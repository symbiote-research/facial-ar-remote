﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace FacialRemote
{
    /// <inheritdoc cref = "IStreamSettings" />
    /// <summary>
    /// Holds the data needed to process facial data to and from a byte stream.
    /// This data needs to match on the server and client side.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "Stream Settings", menuName = "Facial Remote/Stream Settings")]
    public class StreamSettings : ScriptableObject, IStreamSettings
    {
        [SerializeField]
        [Tooltip("Error check byte value.")]
        byte m_ErrorCheck = 42;

        [SerializeField]
        [Tooltip("Size of blend shapes in the byte array.")]
        int m_BlendShapeSize;

        [SerializeField]
        [Tooltip("Size of frame number value in byte array.")]
        int m_FrameNumberSize;

        [SerializeField]
        [Tooltip("Size of frame time value in byte array.")]
        int m_FrameTimeSize;

        [SerializeField]
        [Tooltip("Location of head pose in byte array.")]
        int m_HeadPoseOffset;

        [SerializeField]
        [Tooltip("Location of camera pose in byte array.")]
        int m_CameraPoseOffset;

        [SerializeField]
        [Tooltip("Location of frame number value in byte array.")]
        int m_FrameNumberOffset;

        [SerializeField]
        [Tooltip("Location of frame time value in byte array.")]
        int m_FrameTimeOffset;

        [SerializeField]
        [Tooltip("Total size of buffer of byte array for single same of data.")]
        int m_BufferSize;

        [SerializeField]
        [Tooltip("String names of the blend shapes in the stream with their index in the array being their relative location.")]
        string[] m_Locations = { };

        [SerializeField]
        [Tooltip("Rename mapping values to apply blend shape locations to a blend shape controller.")]
        Mapping[] m_Mappings = { };

        public byte ErrorCheck { get { return m_ErrorCheck; } }
        public int BlendShapeSize { get { return m_BlendShapeSize; } }
        public int FrameNumberSize { get { return m_FrameNumberSize; } }
        public int FrameTimeSize { get { return m_FrameTimeSize; } }
        public int HeadPoseOffset { get { return m_HeadPoseOffset; } }
        public int CameraPoseOffset { get { return m_CameraPoseOffset; } }
        public int FrameNumberOffset { get { return m_FrameNumberOffset; } }
        public int FrameTimeOffset { get { return m_FrameTimeOffset; } }

        // ARKit 1.5 buffer layout
        // 0 - Error check
        // 1-204 - Blend Shapes
        // 205-232 - Head Pose
        // 233-260 - Camera Pose
        // 261-264 - Frame Number
        // 265-268 - Frame Time
        // 269 - Active state

        // ARKit 2.0 buffer layout
        // 0 - Error check
        // 1-208 - Blend Shapes
        // 209-236 - Head Pose
        // 237-264 - Camera Pose
        // 265-268 - Frame Number
        // 269-273 - Frame Time
        // 274 - Active state
        public int bufferSize { get { return m_BufferSize; } }

        public Mapping[] mappings { get { return m_Mappings; } }

        public string[] locations { get { return m_Locations; } }

        void OnValidate()
        {
            UpdateOffsets();
        }

        void UpdateOffsets()
        {
            const int poseSize = BlendShapeUtils.PoseSize;
            var blendShapeCount = m_Locations.Length;
            m_BlendShapeSize = sizeof(float) * blendShapeCount;
            m_FrameNumberSize = sizeof(int);
            m_FrameTimeSize = sizeof(float);
            m_HeadPoseOffset = BlendShapeSize + 1;
            m_CameraPoseOffset = HeadPoseOffset + poseSize;
            m_FrameNumberOffset = CameraPoseOffset + poseSize;
            m_FrameTimeOffset = FrameNumberOffset + FrameNumberSize;

            // Error check + Blend Shapes + HeadPose + CameraPose + FrameNumber + FrameTime + Active
            m_BufferSize = 1 + BlendShapeSize + poseSize * 2 + FrameNumberSize + FrameTimeSize + 1;
        }

        void Reset()
        {
            var locationStrings = new List<string>();
            foreach (var location in BlendShapeUtils.Locations)
            {
                locationStrings.Add(location);
            }

            locationStrings.Sort();
            m_Locations = locationStrings.ToArray();
            UpdateOffsets();
        }
    }
}
