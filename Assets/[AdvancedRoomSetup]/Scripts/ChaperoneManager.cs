using RoyTheunissen.AdvancedRoomSetup.Chaperones;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup
{
    /// <summary>
    /// Helps interpret and manipulate chaperone data as it comes from OpenVR.
    /// </summary>
    public sealed class ChaperoneManager : MonoBehaviour
    {
        [SerializeField] private ChaperoneRenderer chaperoneRendererPrefab;
        [SerializeField] private Transform controllerLeft;
        [SerializeField] private Transform controllerRight;

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
            chaperoneRendererWorking.gameObject.SetActive(false);
            
            chaperoneNew = new Chaperone();
            chaperoneRendererNew = Instantiate(chaperoneRendererPrefab);
            chaperoneRendererNew.Initialize(chaperoneNew);
        }

        private void Update()
        {
            chaperoneNew.SetViaExtremities(controllerLeft.position, controllerRight.position);
        }

        private void OnDrawGizmos()
        {
            chaperoneWorking?.DrawGizmos();
        }
    }
}
