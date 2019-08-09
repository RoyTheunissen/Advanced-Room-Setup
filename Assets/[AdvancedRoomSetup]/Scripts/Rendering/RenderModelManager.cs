using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace RoyTheunissen.AdvancedRoomSetup.Rendering
{
    /// <summary>
    /// Responsible for managing the render models of the various tracked VR objects.
    /// </summary>
    public sealed class RenderModelManager : MonoBehaviour 
    {
        [SerializeField] private Shader shader;

        private Dictionary<uint, TrackingReferenceObject> trackingReferences =
            new Dictionary<uint, TrackingReferenceObject>();

        private void OnEnable()
        {
            SteamVR_Events.NewPoses.AddListener(OnNewPoses);
        }

        private void OnDisable()
        {
            SteamVR_Events.NewPoses.RemoveListener(OnNewPoses);
        }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            if (poses == null)
                return;

            for (uint deviceIndex = 0; deviceIndex < poses.Length; deviceIndex++)
            {
                if (trackingReferences.ContainsKey(deviceIndex) == false)
                {
                    ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass(deviceIndex);

                    if (ShouldVisualize(deviceClass))
                    {
                        TrackingReferenceObject trackingReference = new TrackingReferenceObject();
                        trackingReference.trackedDeviceClass = deviceClass;
                        trackingReference.gameObject = new GameObject("Tracking Reference " + deviceIndex.ToString());
                        trackingReference.gameObject.transform.parent = this.transform;
                        trackingReference.trackedObject = trackingReference.gameObject.AddComponent<SteamVR_TrackedObject>();
                        trackingReference.renderModel = trackingReference.gameObject.AddComponent<SteamVR_RenderModel>();
                        trackingReference.renderModel.createComponents = false;
                        trackingReference.renderModel.updateDynamically = false;
                        trackingReference.renderModel.shader = shader;

                        trackingReferences.Add(deviceIndex, trackingReference);

                        trackingReference.gameObject.SendMessage("SetDeviceIndex", (int)deviceIndex, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        trackingReferences.Add(deviceIndex, new TrackingReferenceObject() { trackedDeviceClass = deviceClass });
                    }
                }
            }
        }

        private bool ShouldVisualize(ETrackedDeviceClass deviceClass)
        {
            switch (deviceClass)
            {
                case ETrackedDeviceClass.HMD:
                case ETrackedDeviceClass.Controller:
                case ETrackedDeviceClass.TrackingReference:
                    return true;
                case ETrackedDeviceClass.Invalid:
                case ETrackedDeviceClass.GenericTracker:
                case ETrackedDeviceClass.DisplayRedirect:
                case ETrackedDeviceClass.Max:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceClass), deviceClass, null);
            }
        }

        private class TrackingReferenceObject
        {
            public ETrackedDeviceClass trackedDeviceClass;
            public GameObject gameObject;
            public SteamVR_RenderModel renderModel;
            public SteamVR_TrackedObject trackedObject;
        }
    }
}
