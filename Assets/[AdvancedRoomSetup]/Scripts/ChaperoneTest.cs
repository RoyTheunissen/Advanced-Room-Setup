using RoyTheunissen.AdvancedRoomSetup.Chaperones;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup
{
    /// <summary>
    /// Helps interpret and manipulate chaperone data as it comes from OpenVR.
    /// </summary>
    public sealed class ChaperoneTest : MonoBehaviour
    {
        private Chaperone chaperone;
        
        private void Awake()
        {
            chaperone = new Chaperone();
            chaperone.LoadFromWorkingFile();
        }

        private void OnDrawGizmos()
        {
            chaperone?.DrawGizmos();
        }
    }
}
