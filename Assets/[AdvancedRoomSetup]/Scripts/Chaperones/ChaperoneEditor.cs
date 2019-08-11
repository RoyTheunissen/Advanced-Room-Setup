using RoyTheunissen.AdvancedRoomSetup.UI.ChaperoneSpace;
using UnityEngine;
using Valve.VR;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Responsible for managing the modifications to chaperones and the UI required for that.
    /// </summary>
    public sealed class ChaperoneEditor : MonoBehaviour
    {
        [SerializeField] private OverheadCameraFraming overheadCameraFraming;
        [SerializeField] private ChaperoneManager chaperoneManager;
        
        private void Awake()
        {
            SteamVR_Events.RenderModelLoaded.AddListener(OnRenderModelLoaded);
        }

        private void OnDestroy()
        {
            SteamVR_Events.RenderModelLoaded.RemoveListener(OnRenderModelLoaded);
        }
        
        private void OnRenderModelLoaded(SteamVR_RenderModel renderModel, bool success)
        {
            if (!success)
                return;

            SteamVR_TrackedObject trackedObject = renderModel.GetComponent<SteamVR_TrackedObject>();
            if (trackedObject == null)
                return;

            ETrackedDeviceClass deviceClass =
                OpenVR.System.GetTrackedDeviceClass((uint)trackedObject.index);
            
            if (deviceClass != ETrackedDeviceClass.TrackingReference)
                return;

            LightHouseUi lightHouseUi = renderModel.gameObject.AddComponent<LightHouseUi>();
            lightHouseUi.Initialize(this);
        }

        public Vector3 GetWorldSpacePointerPosition(
            Vector2 screenSpacePointerPosition, float y = 0.0f)
        {
            y += chaperoneManager.ChaperoneWorking.Origin.GetPosition().y;
            return overheadCameraFraming.GetWorldSpacePointerPosition(
                screenSpacePointerPosition, y);
        }
    }
}
