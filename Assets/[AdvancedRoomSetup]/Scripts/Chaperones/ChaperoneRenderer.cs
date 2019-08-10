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
        
        [SerializeField] private float arrowPaddingMin = 0.1f;
        [SerializeField] private float arrowSizeMin = 0.000000001f;
        [SerializeField] private float arrowSizeMax = 0.5f;
        [SerializeField] private Renderer[] fadeableRenderers;

        private Chaperone chaperone;

        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        private float cachedOpacity = 1.0f;
        public float Opacity
        {
            get => cachedOpacity;
            set
            {
                cachedOpacity = value;

                for (int i = 0; i < fadeableRenderers.Length; i++)
                {
                    fadeableRenderers[i].material.color =
                        fadeableRenderers[i].material.color.Fade(cachedOpacity);
                }
            }
        }

        public void Initialize(Chaperone chaperone)
        {
            if (this.chaperone != null)
            {
                this.chaperone.PerimeterChangedEvent -= HandlePerimeterChangedEvent;
            }
            
            this.chaperone = chaperone;

            if (chaperone == null)
                return;
            
            chaperone.PerimeterChangedEvent += HandlePerimeterChangedEvent;
            
            UpdateVisuals();
        }

        private void OnDestroy()
        {
            if (chaperone != null)
                chaperone.PerimeterChangedEvent -= HandlePerimeterChangedEvent;
        }

        private void HandlePerimeterChangedEvent(Chaperone chaperone)
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            transform.position = chaperone.Origin.MultiplyPoint(Vector3.zero);
            transform.rotation = chaperone.Origin.rotation;
            
            Vector3[] positions = chaperone.Perimeter.ToArray();
            lineRenderer.positionCount = positions.Length;
            lineRenderer.SetPositions(positions);

            playAreaRectangle.localScale = new Vector3(chaperone.Size.x, 1.0f, chaperone.Size.y);

            float arrowSize = Mathf.Min(
                chaperone.Size.x - arrowPaddingMin, chaperone.Size.y - arrowPaddingMin,
                arrowSizeMax);
            arrowSize = Mathf.Max(arrowSize, arrowSizeMin);
            
            playAreaArrow.localScale = Vector3.one * arrowSize;
            playAreaArrow.gameObject.SetActive(
                !Mathf.Approximately(chaperone.Size.x, 0.0f) &&
                !Mathf.Approximately(chaperone.Size.y, 0.0f));
        }
    }
}
