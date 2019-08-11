using RoyTheunissen.Scaffolding.Tweening;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.Chaperones
{
    /// <summary>
    /// Visualizes chaperone data in the game view.
    /// </summary>
    public sealed class ChaperoneRenderer : MonoBehaviour
    {
        private const float FadeDuration = 0.1f;
        
        [SerializeField] private GameObject validityContainer;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform playAreaRectangle;
        [SerializeField] private Transform playAreaArrow;
        [SerializeField] private float areaSizeMin = 0.5f;
        
        [SerializeField] private float arrowPaddingMin = 0.1f;
        [SerializeField] private float arrowSizeMin = 0.000000001f;
        [SerializeField] private float arrowSizeMax = 0.5f;
        [SerializeField] private List<Renderer> fadeableRenderers = new List<Renderer>();

        private Chaperone chaperone;

        public bool Active
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        private float cachedOpacity = 1.0f;
        private float Opacity
        {
            get => cachedOpacity;
            set
            {
                cachedOpacity = value;

                for (int i = 0; i < fadeableRenderers.Count; i++)
                {
                    fadeableRenderers[i].GetPropertyBlock(materialPropertyBlock);
                    materialPropertyBlock.SetFloat("_Fade", 1.0f - cachedOpacity);
                    fadeableRenderers[i].SetPropertyBlock(materialPropertyBlock);
                }
            }
        }
        
        private Tween opacityTween;

        private MaterialPropertyBlock materialPropertyBlock;

        private void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            
            opacityTween = new Tween(f => Opacity = f).SkipToIn().SetContinuous(true);
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

        public void FadeTo(float opacity, bool instant = false)
        {
            if (instant)
            {
                opacityTween.SkipTo(opacity);
                return;
            }
            opacityTween.TweenTo(opacity, FadeDuration);
        }

        private void UpdateVisuals()
        {
            validityContainer.SetActive(chaperone.Size.x > areaSizeMin &&
                                        chaperone.Size.y > areaSizeMin);
            
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
