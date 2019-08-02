using System.Collections.Generic;
using RoyTheunissen.AdvancedRoomSetup.Chaperones;
using UnityEngine;
using Valve.VR;

namespace RoyTheunissen.AdvancedRoomSetup
{
    /// <summary>
    /// Helps interpret chaperone data as it comes from OpenVR.
    /// </summary>
    public sealed class ChaperoneTest : MonoBehaviour
    {
        [SerializeField] private Transform testTransform;

        private List<ChaperoneQuad> quads = new List<ChaperoneQuad>();
        
        private void Awake()
        {
            HmdQuad_t[] liveColisionBoundsInfo;
            OpenVR.ChaperoneSetup.GetWorkingCollisionBoundsInfo(out liveColisionBoundsInfo);

            HmdMatrix34_t standingZeroPoseToRawTrackingPose = new HmdMatrix34_t();
            OpenVR.ChaperoneSetup.GetWorkingStandingZeroPoseToRawTrackingPose(
                ref standingZeroPoseToRawTrackingPose);
            
            DrawPose(standingZeroPoseToRawTrackingPose, 0.25f, Color.yellow, 60.0f);
            
            DrawPose(testTransform.localToWorldMatrix, 0.25f, Color.red, 60.0f);
            
            for (int i = 0; i < liveColisionBoundsInfo.Length; i++)
            {
                quads.Add(liveColisionBoundsInfo[i]);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < quads.Count; i++)
            {
                quads[i].DrawGizmo();
            }
            
            Gizmos.matrix = testTransform.localToWorldMatrix;
            
            float size = 0.5f;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(Vector3.zero, Vector3.forward * size);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Vector3.zero, Vector3.right * size);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(Vector3.zero, Vector3.up * size);
        }

        private void DrawPose(Matrix4x4 matrix, float length, Color color, float duration)
        {
            Vector3 position = matrix.MultiplyPoint(Vector3.zero);
            Quaternion rotation = matrix.rotation;

            Vector3 tip = position + rotation * Vector3.forward * length;
            Debug.DrawLine(position, tip, color, duration);
            DrawArrowHead(tip, rotation, length / 3, color, duration);
        }

        private void DrawArrowHead(
            Vector3 from, Quaternion rotation, float size, Color color, float duration)
        {
            Quaternion rotLeft = Quaternion.Euler(0.0f, -90.0f - 45.0f, 0.0f);
            Quaternion rotRight = Quaternion.Euler(0.0f, 90.0f + 45.0f, 0.0f);
            Debug.DrawRay(from, rotation * rotLeft * Vector3.forward * size, color, duration);
            Debug.DrawRay(from, rotation * rotRight * Vector3.forward * size, color, duration);
        }
    }
}
