using System;
using System.Collections.Generic;
using RoyTheunissen.Scaffolding.Tweening;
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

        [SerializeField] private Matrix4x4 origin = Matrix4x4.identity;
        public Matrix4x4 Origin => origin;

        [SerializeField] private Vector2 size;
        public Vector2 Size => size;

        [SerializeField] private string name;
        public string Name => name;
        
        [SerializeField] private List<Matrix4x4> lightHouseReferences = new List<Matrix4x4>();
        public List<Matrix4x4> LightHouseReferences => lightHouseReferences;

        [NonSerialized] private string filePath;
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public delegate void PerimeterChangedHandler(Chaperone chaperone);
        public event PerimeterChangedHandler PerimeterChangedEvent;

        public Chaperone(string name)
        {
            this.name = name;
        }
        
        public Chaperone(string name, Chaperone other) : this(name)
        {
            // Metadata: Cache where the lighthouses were when creating new chaperones so we can
            // re-contextualize chaperones later even when lighthouses have been adjusted.
            CacheWorkingLighthousePositionsAsReference();
            
            CopySettingsFrom(other);
        }

        public void CopySettingsFrom(Chaperone other)
        {
            perimeter.Clear();
            perimeter.AddRange(other.perimeter);

            origin = other.origin;
            size = other.size;
            
            PerimeterChangedEvent?.Invoke(this);
        }
        
        public void CopyMetaDataFrom(Chaperone other)
        {
            FilePath = other.FilePath;
            
            LightHouseReferences.Clear();
            LightHouseReferences.AddRange(other.LightHouseReferences);
        }

        public void LoadFromWorkingPlayArea()
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
                quads[i].DrawDebug(standingZeroPoseToRawTrackingPose, Color.green, 5.0f);
                
                perimeter.Add(quads[i].From);
            }

            float sizeX = 0.0f, sizeZ = 0.0f;
            OpenVR.ChaperoneSetup.GetWorkingPlayAreaSize(ref sizeX, ref sizeZ);
            size = new Vector2(sizeX, sizeZ);
        }

        private void CacheWorkingLighthousePositionsAsReference()
        {
            TrackedDevicePose_t[] poses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];
            OpenVR.System.GetDeviceToAbsoluteTrackingPose(
                ETrackingUniverseOrigin.TrackingUniverseRawAndUncalibrated, 0.0f, poses);

            lightHouseReferences.Clear();
            for (int i = 0; i < poses.Length; i++)
            {
                ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass((uint)i);
                Matrix4x4 pose = poses[i].mDeviceToAbsoluteTracking;

                if (deviceClass != ETrackedDeviceClass.TrackingReference || !poses[i].bPoseIsValid)
                    continue;

                lightHouseReferences.Add(pose);
            }
        }

        public void CommitToLivePlayArea()
        {
            // Calculate quads for our perimeter.
            HmdQuad_t[] quads = new HmdQuad_t[perimeter.Count];
            for (int i = 0; i < quads.Length; i++)
            {
                Vector3 from = perimeter[i];
                Vector3 to = perimeter[(i + 1) % perimeter.Count];
                ChaperoneQuad quad = new ChaperoneQuad(from, to);
                quad.DrawDebug(origin, Color.red, 5.0f);
                quads[i] = quad;
            }
            
            // Set the origin.
            HmdMatrix34_t matrix = origin;
            OpenVR.ChaperoneSetup.SetWorkingStandingZeroPoseToRawTrackingPose(ref matrix);
            
            // Set collision bounds quads.
            OpenVR.ChaperoneSetup.SetWorkingCollisionBoundsInfo(quads);
            
            // Set the play area size.
            OpenVR.ChaperoneSetup.SetWorkingPlayAreaSize(size.x, size.y);
            
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
            
            PerimeterChangedEvent?.Invoke(this);
        }

        public void Realign(Matrix4x4 from, Matrix4x4 to)
        {
            // TODO: I'm sure there's a more efficient way to do this, I thought I could multiply
            // 'to' by the inverse of 'from' but that didn't do what I expected.
            Matrix4x4 movement = Matrix4x4.TRS(
                to.GetPosition() - from.GetPosition(),
                Quaternion.Inverse(from.rotation) * to.rotation, Vector3.one);

            // Apply the movement to the origin and our light house references.
            origin = movement * origin;
            for (int i = 0; i < lightHouseReferences.Count; i++)
            {
                lightHouseReferences[i] = movement * lightHouseReferences[i];
            }
            
            // TODO: Re-commit to the live play area. Also save to a file?
            
            PerimeterChangedEvent?.Invoke(this);
        }

        public bool IsPlaySpaceTheSame(Chaperone other)
        {
            if (origin != other.origin)
                return false;

            if (size != other.size)
                return false;

            if (perimeter.Count != other.perimeter.Count)
                return false;

            for (int i = 0; i < perimeter.Count; i++)
            {
                if (perimeter[i] != other.perimeter[i])
                    return false;
            }

            return true;
        }
        
        public void DrawGizmos()
        {
            if (!Application.isPlaying)
                return;

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
        
        private void DebugPose(Matrix4x4 matrix, float length, float duration = 0.0f)
        {
            Debug.DrawRay(
                matrix.MultiplyPoint(Vector3.zero), matrix * Vector3.right * length, Color.red,
                duration);
            Debug.DrawRay(
                matrix.MultiplyPoint(Vector3.zero), matrix * Vector3.up * length, Color.green,
                duration);
            Debug.DrawRay(
                matrix.MultiplyPoint(Vector3.zero), matrix * Vector3.forward * length, Color.blue,
                duration);
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
