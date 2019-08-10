using RoyTheunissen.Scaffolding.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RoyTheunissen.AdvancedRoomSetup.UI
{
    /// <summary>
    /// Helps extend button functionality with things like special hover states.
    /// </summary>
    public sealed class AdvancedCalibrationButton : MonoBehaviour, IPointerEnterHandler,
        IPointerExitHandler
    {
        private const float FadeDuration = 0.1f;
        
        [SerializeField] private Button button;
        public Button Button => button;
        
        [SerializeField] private CanvasGroup brightener;

        private bool isHovered;
        public bool IsHovered => isHovered;

        private Tween brightenerTween;

        public delegate void HoverStateChangedHandler(
            AdvancedCalibrationButton advancedCalibrationButton, bool isHovered);
        public event HoverStateChangedHandler HoverStateChangedEvent;

        private void Awake()
        {
            brightenerTween = brightener.TweenAlpha().SkipToOut().SetContinuous(true);
        }

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovered = true;

            brightenerTween.TweenIn(FadeDuration);
            
            HoverStateChangedEvent?.Invoke(this, isHovered);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            
            brightenerTween.TweenOut(FadeDuration);
            
            HoverStateChangedEvent?.Invoke(this, isHovered);
        }
    }
}
