using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    [Serializable]
    public class Chaperone
    {
        [SerializeField] private List<Vector3> perimeter = new List<Vector3>();
        public List<Vector3> Perimeter => perimeter;

        private Matrix4x4 origin = Matrix4x4.identity;
        public Matrix4x4 Origin => origin;

        private Vector2 size;
        public Vector2 Size => size;
        
        public delegate void PerimeterChangedHandler();
        public event PerimeterChangedHandler PerimeterChangedEvent;

        public void LoadFromWorkingFile()
        {
            // Get the origin.
            HmdMatrix34_t standingZeroPoseToRawTrackingPose = new HmdMatrix34_t();
            OpenVR.ChaperoneSetup.GetWorkingStandingZeroPoseToRawTrackingPose(
                ref standingZeroPoseToRawTrackingPose);
            origin = standingZeroPoseToRawTrackingPose;
            
            // Load all the quads and convert them to chaperone quads and then to a perimeter.
            HmdQuad_t[] workingColisionBoundsInfo;
            OpenVR.ChaperoneSetup.GetWorkingCollisionBoundsInfo(out workingColisionBoundsInfo);
            ChaperoneQuad[] quads = new ChaperoneQuad[workingColisionBoundsInfo.Length];
            for (int i = 0; i < quads.Length; i++)
            {
                quads[i] = workingColisionBoundsInfo[i];
                quads[i].DrawDebug(Color.magenta, 10.0f);
                
                perimeter.Add(quads[i].From);
            }

            float sizeX = 0.0f, sizeZ = 0.0f;
            OpenVR.ChaperoneSetup.GetWorkingPlayAreaSize(ref sizeX, ref sizeZ);
            size = new Vector2(sizeX, sizeZ);
        }
        
        public void SaveToWorkingFile()
        {
            // Set the origin.
            HmdMatrix34_t matrix = origin;
            OpenVR.ChaperoneSetup.SetWorkingStandingZeroPoseToRawTrackingPose(ref matrix);
            
            // Set collision bounds quads.
            HmdQuad_t[] quads = new HmdQuad_t[perimeter.Count];
            for (int i = 0; i < quads.Length; i++)
            {
                Vector3 from = perimeter[i];
                Vector3 to = perimeter[(i + 1) % perimeter.Count];
                quads[i] = new ChaperoneQuad(from, to);
            }
            OpenVR.ChaperoneSetup.SetWorkingCollisionBoundsInfo(quads);
            
            // Set the play area size as a perimeter of points.
            HmdVector2_t[] points = new HmdVector2_t[perimeter.Count];
            for (int i = 0; i < perimeter.Count; i++)
            {
                points[i] = new HmdVector2_t {v0 = perimeter[i].x, v1 = perimeter[i].z};
            }
            OpenVR.ChaperoneSetup.SetWorkingPerimeter(points);
            
            // Commit the working copy to the live environment.
            OpenVR.ChaperoneSetup.CommitWorkingCopy(EChaperoneConfigFile.Live);
        }

        public void SetViaExtremities(Transform a, Transform b)
        {
            Quaternion rotationA = Quaternion.Euler(new Vector3(0.0f, a.eulerAngles.y, 0.0f));
            Quaternion rotationB = Quaternion.Euler(new Vector3(0.0f, b.eulerAngles.y, 0.0f));
            Quaternion rotation = Quaternion.Slerp(rotationA, rotationB, 0.5f);
            
            Vector3 center = (a.position + b.position) / 2;

            origin = Matrix4x4.TRS(center, rotation, Vector3.one);
            
            Debug.DrawRay(origin.GetPosition(), origin.rotation * Vector3.forward, Color.cyan);

            Vector3 localPosA = origin.inverse.MultiplyPoint(a.position);
            Vector3 localPosB = origin.inverse.MultiplyPoint(b.position);
            
            Vector3 min = Vector3.Min(localPosA, localPosB);
            Vector3 max = Vector3.Max(localPosA, localPosB);
            
            perimeter.Clear();
            perimeter.Add(new Vector3(min.x, 0.0f, min.z));
            perimeter.Add(new Vector3(max.x, 0.0f, min.z));
            perimeter.Add(new Vector3(max.x, 0.0f, max.z));
            perimeter.Add(new Vector3(min.x, 0.0f, max.z));

            size = new Vector2(max.x - min.x, max.z - min.z);
            
            PerimeterChangedEvent?.Invoke();
        }
        
        public void DrawGizmos()
        {
            DrawPose(Matrix4x4.identity, 0.1f);
            DrawPose(origin.inverse, 0.3f);

            Gizmos.color = Color.red;
            DrawPlayArea();
            
            DrawPerimeter();
        }

        private void DrawPerimeter()
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = origin;
            
            // Draw all of the positions.
            Gizmos.color = Color.green;
            Vector3 previous = Vector3.zero;
            if (perimeter.Count > 0)
                previous = perimeter[perimeter.Count - 1];

            for (int i = 0; i < perimeter.Count; i++)
            {
                Gizmos.DrawLine(previous, perimeter[i]);

                previous = perimeter[i];
            }
            
            Gizmos.matrix = originalMatrix;
        }

        private void DrawPose(Matrix4x4 matrix, float length)
        {
            Matrix4x4 originalmatrix = Gizmos.matrix;
            Gizmos.matrix = matrix;
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Vector3.zero, Vector3.right * length);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(Vector3.zero, Vector3.up * length);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(Vector3.zero, Vector3.forward * length);

            Gizmos.matrix = originalmatrix;
        }
        
        public void DrawPlayArea()
        {
            Matrix4x4 originalMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(
                origin.GetPosition(), origin.rotation, new Vector3(size.x, 0.0f, size.y) / 2);
            Gizmos.DrawLine(new Vector3(-1, 0, -1), new Vector3(1, 0, -1));
            Gizmos.DrawLine(new Vector3(-1, 0, 1), new Vector3(1, 0, 1));
            Gizmos.DrawLine(new Vector3(-1, 0, -1), new Vector3(-1, 0, 1));
            Gizmos.DrawLine(new Vector3(1, 0, -1), new Vector3(1, 0, 1));
            Gizmos.matrix = originalMatrix;
        }
    }
}
