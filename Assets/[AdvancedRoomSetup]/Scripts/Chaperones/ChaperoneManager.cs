using RoyTheunissen.AdvancedRoomSetup.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Helps interpret and manipulate chaperone data as it comes from OpenVR.
    /// </summary>
    public sealed class ChaperoneManager : MonoBehaviour
    {
        [SerializeField] private ChaperoneRenderer chaperoneRendererPrefab;
        [SerializeField] private Transform controllerLeft;
        [SerializeField] private Transform controllerRight;
        
        [SerializeField] private AdvancedCalibrationButton newChaperoneButton;

        private ChaperoneRenderer chaperoneRendererWorking;
        private ChaperoneRenderer chaperoneRendererNew;
        
        private Chaperone chaperoneWorking;
        private Chaperone chaperoneNew;
        
        private void Awake()
        {
            chaperoneWorking = new Chaperone();
            chaperoneWorking.LoadFromWorkingFile();

            chaperoneRendererWorking = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererWorking.Initialize(chaperoneWorking);
            chaperoneRendererWorking.Active = false;
            
            chaperoneNew = new Chaperone();
            chaperoneRendererNew = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererNew.Initialize(chaperoneNew);
        }

        private void Update()
        {
            chaperoneNew.SetViaExtremities(controllerLeft.position, controllerRight.position);

            chaperoneRendererWorking.Active = !newChaperoneButton.IsHovered;
            chaperoneRendererNew.Active = newChaperoneButton.IsHovered;
        }

        private void OnDrawGizmos()
        {
            chaperoneWorking?.DrawGizmos();
        }
    }
}
