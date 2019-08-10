using RoyTheunissen.AdvancedRoomSetup.UI;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Responsible for managing the previews of chaperones.
    /// </summary>
    public sealed class ChaperonePreviewManager : MonoBehaviour 
    {
        [SerializeField] private ChaperoneManager chaperoneManager;
        [SerializeField] private ChaperoneRenderer chaperoneRendererPrefab;
        
        [Space]
        [SerializeField] private OverheadCameraFraming overheadCameraFraming;
        [SerializeField] private ChaperoneManagingUi chaperoneManagingUi;
        
        private ChaperoneRenderer chaperoneRendererWorking;
        private ChaperoneRenderer chaperoneRendererNew;
        
        private bool showNewChaperonePreview;
        
        private void Start()
        {
            // Use Start for now to ensure ChaperoneManager is fully initialized.
            
            chaperoneRendererWorking = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererWorking.Initialize(chaperoneManager.ChaperoneWorking);
            
            chaperoneRendererNew = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererNew.Initialize(chaperoneManager.ChaperoneNew);
        }

        private void Update()
        {
            overheadCameraFraming.transform.rotation = Quaternion.Euler(
                0.0f, chaperoneManager.ChaperoneWorking.Origin.rotation.eulerAngles.y, 0.0f);

            showNewChaperonePreview = chaperoneManagingUi.NewChaperoneButton.IsHovered;
            chaperoneRendererWorking.Opacity = showNewChaperonePreview ? 0.05f : 1.0f;
            chaperoneRendererNew.Active = showNewChaperonePreview;
        }
    }
}
