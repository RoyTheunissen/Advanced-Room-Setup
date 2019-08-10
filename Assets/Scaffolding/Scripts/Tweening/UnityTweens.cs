using RoyTheunissen.Scaffolding.Tweening;
using UnityEngine.UI;

namespace UnityEngine
{
    // A bunch of convenient default tweens. Intended to replace the DefaultTweens.cs file.

    public static class ExtensionTweens
    {
        public static Tween TweenScale(this Transform transform, float duration = 1.0f)
        {
            return TweenScale(transform, duration, Vector3.one);
        }

        public static Tween TweenScale(this Transform transform, float duration, Vector3 axis)
        {
            Vector3 inverse = Vector3.one - axis;
            return new Tween(
                fraction => transform.localScale =
                    inverse + axis.Scale(Mathf.Max(float.Epsilon, fraction)), duration);
        }

        public static Tween TweenScaleTo(
            this Transform transform, float scale = 1.0f, float duration = 1.0f)
        {
            return new Tween(
                fraction => transform.Scale(fraction, 0.0f, scale), duration);
        }

        public static Tween TweenScaleFromTo(this Transform transform,
            float scaleFrom, float scaleTo, float duration = 1.0f)
        {
            return new Tween(
                fraction => transform.Scale(fraction, scaleFrom, scaleTo),
                duration);
        }

        public static void Scale(
            this Transform transform, float fraction, float minimum = 0.0f, float maximum = 1.0f)
        {
            transform.localScale = Vector3.one.Scale(
                Mathf.LerpUnclamped(
                    Mathf.Max(float.Epsilon, minimum), Mathf.Max(float.Epsilon, maximum),
                    fraction));
        }

        public static Tween TweenAnchorOffset(
            this RectTransform transform, Vector3 offset, float duration = 1.0f)
        {
            return new Tween(
                fraction => transform.anchoredPosition3D = offset * (1.0f - fraction), duration);
        }
        
        public static Tween TweenSizeDelta(this RectTransform transform, float duration = 1.0f)
        {
            Vector3 original = transform.sizeDelta;
            return new Tween(
                fraction => transform.sizeDelta = original * fraction, duration);
        }
        
        public static Tween TweenSizeDelta(
            this RectTransform transform, Vector2 size, float duration = 1.0f)
        {
            return new Tween(
                fraction => transform.sizeDelta = size * fraction, duration);
        }

        public static Tween TweenAlpha(this CanvasGroup canvasGroup, float duration = 1.0f)
        {
            return new Tween(fraction => canvasGroup.alpha = fraction, duration);
        }

        public static Tween TweenAlphaTo(this CanvasGroup canvasGroup,
            float alpha, float duration = 1.0f)
        {
            return new Tween(fraction => canvasGroup.alpha = alpha * fraction, duration);
        }

        public static Tween TweenVolume(this AudioSource audioSource,
            float volume = 1.0f, float duration = 1.0f)
        {
            return new Tween(fraction =>
            {
                if (audioSource != null)
                    audioSource.volume = fraction * volume;
            }, duration);
        }

        public static ShakeTween TweenShake(
            this Transform transform, float amplitude = 1.0f, float duration = 1.0f)
        {
            return new ShakeTween(transform, amplitude, duration);
        }

        public static TextTween TweenText(this Text text, float duration = 1.0f)
        {
            return new TextTween(text, duration);
        }
    }
}
