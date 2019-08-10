using RoyTheunissen.Scaffolding.Tweening;
using UnityEngine.UI;

namespace UnityEngine
{
    public class TextTween : Tween
    {
        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                CallHandler();
            }
        }

        private Text textComponent;

        public TextTween(Text textComponent, float duration = 1) : base(duration)
        {
            this.textComponent = textComponent;
            text = textComponent.text;
            handler = Handler;
        }

        private void Handler(float fraction)
        {
            textComponent.text = text.Substring(0, Mathf.RoundToInt(fraction * text.Length));
        }
    }
}
