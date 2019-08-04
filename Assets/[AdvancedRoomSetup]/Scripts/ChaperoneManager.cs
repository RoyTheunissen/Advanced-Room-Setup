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

        private ChaperoneRenderer chaperoneRenderer;
        
        private Chaperone chaperone;
        
        private void Awake()
        {
            chaperone = new Chaperone();
            chaperone.LoadFromWorkingFile();

            chaperoneRenderer = Instantiate(chaperoneRendererPrefab);
            chaperoneRenderer.Initialize(chaperone);
        }

        private void OnDrawGizmos()
        {
            chaperone?.DrawGizmos();
        }
    }
}
