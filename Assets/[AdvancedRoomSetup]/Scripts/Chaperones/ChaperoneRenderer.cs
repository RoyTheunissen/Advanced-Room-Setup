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

        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public void Initialize(Chaperone chaperone)
        {
            this.chaperone = chaperone;

            chaperone.PerimeterChangedEvent += HandlePerimeterChangedEvent;
            
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            if (chaperone != null)
                chaperone.PerimeterChangedEvent -= HandlePerimeterChangedEvent;
        }

        private void HandlePerimeterChangedEvent()
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            Vector3[] positions = chaperone.Perimeter.ToArray();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);

            playAreaRectangle.localScale = new Vector3(chaperone.Size.x, 1.0f, chaperone.Size.y);

            playAreaArrow.gameObject.SetActive(
                !Mathf.Approximately(chaperone.Size.x, 0.0f) &&
                !Mathf.Approximately(chaperone.Size.y, 0.0f));
        }
    }
}
