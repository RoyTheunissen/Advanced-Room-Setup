using System;
using System.Collections.Generic;
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
        [SerializeField] private LineRenderer dragLinePrefab;

        private Type uiInteractibilityTypeFilter;
        
        private List<ChaperoneSpaceUi> chaperoneSpaceUis = new List<ChaperoneSpaceUi>();
        
        private void Awake()
        {
            FilterUiInteractibilityByType<LightHouseReferenceUi>();
            
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

        public bool IsUiTypeInteractible(Type type)
        {
            return uiInteractibilityTypeFilter.IsAssignableFrom(type);
        }

        public void ClearUiInteractibilityFilter()
        {
            uiInteractibilityTypeFilter = null;
            for (int i = 0; i < chaperoneSpaceUis.Count; i++)
            {
                chaperoneSpaceUis[i].UpdateInteractibility();
            }
        }

        public void FilterUiInteractibilityByType<T>()
        {
            uiInteractibilityTypeFilter = typeof(T);
            for (int i = 0; i < chaperoneSpaceUis.Count; i++)
            {
                chaperoneSpaceUis[i].UpdateInteractibility();
            }
        }

        public Vector3 GetWorldSpacePointerPosition(
            Vector2 screenSpacePointerPosition, float y = 0.0f)
        {
            y += chaperoneManager.ChaperoneWorking.Origin.GetPosition().y;
            return overheadCameraFraming.GetWorldSpacePointerPosition(
                screenSpacePointerPosition, y);
        }

        public void Register(ChaperoneSpaceUi chaperoneSpaceUi)
        {
            chaperoneSpaceUis.Add(chaperoneSpaceUi);
        }
        
        public void Unregister(ChaperoneSpaceUi chaperoneSpaceUi)
        {
            chaperoneSpaceUis.Remove(chaperoneSpaceUi);
        }
    }
}
