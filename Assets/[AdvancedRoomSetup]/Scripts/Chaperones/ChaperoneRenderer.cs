using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Visualizes chaperone data in the game view.
    /// </summary>
    public sealed class ChaperoneRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform playAreaRectangle;
        [SerializeField] private Transform playAreaArrow;

        private Chaperone chaperone;

        public void Initialize(Chaperone chaperone)
        {
            this.chaperone = chaperone;
            
            Vector3[] positions = chaperone.Perimeter.ToArray();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);

            playAreaRectangle.localScale = new Vector3(chaperone.Size.x, 1.0f, chaperone.Size.y);
        }
    }
}
