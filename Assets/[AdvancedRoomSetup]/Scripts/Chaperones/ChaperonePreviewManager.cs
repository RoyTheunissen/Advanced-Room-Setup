using RoyTheunissen.AdvancedRoomSetup.UI;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Responsible for managing the previews of chaperones.
    /// </summary>
    public sealed class ChaperonePreviewManager : MonoBehaviour
    {
        private const float PreviewOpacity = 0.25f;
        
        [SerializeField] private ChaperoneManager chaperoneManager;
        [SerializeField] private ChaperoneRenderer chaperoneRendererPrefab;
        
        [Space]
        [SerializeField] private OverheadCameraFraming overheadCameraFraming;
        [SerializeField] private ChaperoneManagingUi chaperoneManagingUi;
        
        private ChaperoneRenderer chaperoneRendererWorking;
        private ChaperoneRenderer chaperoneRendererNew;
        private ChaperoneRenderer chaperoneRendererLoad;
        
        private void Start()
        {
            // Use Start for now to ensure ChaperoneManager is fully initialized.
            
            chaperoneRendererWorking = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererWorking.Initialize(chaperoneManager.ChaperoneWorking);
            
            chaperoneRendererNew = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererNew.Initialize(chaperoneManager.ChaperoneNew);
            chaperoneRendererNew.Opacity = PreviewOpacity;
            
            chaperoneRendererLoad = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererLoad.Active = false;
            chaperoneRendererLoad.Opacity = PreviewOpacity;

            chaperoneManagingUi.HoveredChaperoneChangedEvent +=
                HandleHoveredChaperoneToLoadChangedEvent;
        }

        private void OnDestroy()
        {
            chaperoneManagingUi.HoveredChaperoneChangedEvent -=
                HandleHoveredChaperoneToLoadChangedEvent;
        }

        private void HandleHoveredChaperoneToLoadChangedEvent(
            ChaperoneManagingUi chaperoneManagingUi, Chaperone from, Chaperone to)
        {
            chaperoneRendererLoad.Initialize(to);
        }

        private void Update()
        {
            overheadCameraFraming.transform.rotation = Quaternion.Euler(
                0.0f, chaperoneManager.ChaperoneWorking.Origin.rotation.eulerAngles.y, 0.0f);

            ChaperoneRenderer activeChaperoneRenderer = chaperoneRendererWorking;
            if (chaperoneManagingUi.NewChaperoneButton.IsHovered)
                activeChaperoneRenderer = chaperoneRendererNew;
            else if (chaperoneManagingUi.HoveredChaperoneToLoad != null)
                activeChaperoneRenderer = chaperoneRendererLoad;

            chaperoneRendererWorking.Active = activeChaperoneRenderer == chaperoneRendererWorking;
            chaperoneRendererNew.Active = activeChaperoneRenderer == chaperoneRendererNew;
            chaperoneRendererLoad.Active = activeChaperoneRenderer == chaperoneRendererLoad;
        }
    }
}
