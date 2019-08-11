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
        [SerializeField] private ChaperoneEditor chaperoneEditor;
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
            chaperoneRendererWorking.Initialize(chaperoneManager.ChaperoneWorking, chaperoneEditor);
            
            chaperoneRendererNew = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererNew.Initialize(chaperoneManager.ChaperoneNew, chaperoneEditor);
            chaperoneRendererNew.FadeTo(0.0f, true);
            
            chaperoneRendererLoad = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererLoad.FadeTo(0.0f, true);

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
            chaperoneRendererLoad.Initialize(to, chaperoneEditor);
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

            chaperoneRendererWorking.FadeTo(
                activeChaperoneRenderer == chaperoneRendererWorking ? 1.0f : 0.0f);
            chaperoneRendererNew.FadeTo(
                activeChaperoneRenderer == chaperoneRendererNew ? PreviewOpacity : 0.0f);
            chaperoneRendererLoad.FadeTo(
                activeChaperoneRenderer == chaperoneRendererLoad ? PreviewOpacity : 0.0f);
        }
    }
}
