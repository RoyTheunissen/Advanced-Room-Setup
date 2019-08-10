using RoyTheunissen.Scaffolding.Tweening;
using UnityEngine;

namespace RoyTheunissen.AdvancedRoomSetup.UI
{
    /// <summary>
    /// Responsible for controlling the on-screen hint.
    /// </summary>
    public sealed class HintController : MonoBehaviour
    {
        private const float TweenDuration = 0.25f;
        
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private AdvancedCalibrationButton newChaperoneButton;

        private Tween tween;

        private void Awake()
        {
            tween = canvasGroup.TweenAlpha().SkipToOut().SetContinuous(true);
        }

        private void Update()
        {
            bool showHint = newChaperoneButton.IsHovered;
            if (showHint)
                tween.TweenIn(TweenDuration);
            else
                tween.TweenOut(TweenDuration);
        }
    }
}
