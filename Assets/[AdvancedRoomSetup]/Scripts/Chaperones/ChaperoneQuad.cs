using System;
using UnityEngine;
using Valve.VR;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Represents a quad of the OpenVR chaperone. Serializable, compatible with Unity's API and
    /// has helper methods for visualizing it in the scene view.
    /// </summary>
    [Serializable]
    public class ChaperoneQuad
    {
        private const float DefaultHeight = 3.0f;
        
        private Vector3 from;
        private Vector3 to;
        private float height;

        private Matrix4x4 Matrix
        {
            get
            {
                Vector3 position = from;
                Quaternion rotation = Quaternion.LookRotation((to - from).normalized);
                Vector3 scale = new Vector3(1.0f, height, Vector3.Distance(from, to));
                return Matrix4x4.TRS(position, rotation, scale);
            }
        }

        public ChaperoneQuad(Vector3 from, Vector3 to, float height = DefaultHeight)
        {
            this.from = from;
            this.to = to;
            this.height = height;
        }

        public void DrawDebug(Color color, float duration)
        {
            Matrix4x4 matrix = Matrix;
            Debug.DrawLine(
                matrix.MultiplyPoint(new Vector3(0, 0, 0)),
                matrix.MultiplyPoint(new Vector3(0, 0, 1)), color, duration);
            Debug.DrawLine(
                matrix.MultiplyPoint(new Vector3(0, 0, 1)),
                matrix.MultiplyPoint(new Vector3(0, 1, 1)), color, duration);
            Debug.DrawLine(
                matrix.MultiplyPoint(new Vector3(0, 1, 1)),
                matrix.MultiplyPoint(new Vector3(0, 1, 0)), color, duration);
            Debug.DrawLine(
                matrix.MultiplyPoint(new Vector3(0, 1, 0)),
                matrix.MultiplyPoint(new Vector3(0, 0, 0)), color, duration);
        }

        public void DrawGizmo()
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix;
            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            Gizmos.DrawLine(new Vector3(0, 0, 1), new Vector3(0, 1, 1));
            Gizmos.DrawLine(new Vector3(0, 1, 1), new Vector3(0, 1, 0));
            Gizmos.DrawLine(new Vector3(0, 1, 0), new Vector3(0, 0, 0));
            Gizmos.matrix = originalMatrix;
        }

        public static implicit operator HmdQuad_t(ChaperoneQuad quad)
        {
            Matrix4x4 matrix = quad.Matrix;

            Vector3 v0 = matrix.MultiplyPoint(new Vector3(0, 0, 0));
            Vector3 v1 = matrix.MultiplyPoint(new Vector3(0, 0, 1));
            Vector3 v2 = matrix.MultiplyPoint(new Vector3(0, 1, 1));
            Vector3 v3 = matrix.MultiplyPoint(new Vector3(0, 1, 0));

            return new HmdQuad_t
            {
                vCorners0 = v0,
                vCorners1 = v1,
                vCorners2 = v2,
                vCorners3 = v3,
            };
        }
        
        public static implicit operator ChaperoneQuad(HmdQuad_t quad)
        {
            Vector3 from = quad.vCorners0;
            float fromHeight = quad.vCorners1.v1 - quad.vCorners0.v1;
            
            Vector3 to = quad.vCorners3;
            float toHeight = quad.vCorners2.v1 - quad.vCorners3.v1;

            float height = Mathf.Max(fromHeight, toHeight);
            
            return new ChaperoneQuad(from, to, height);
        }
    }
}
