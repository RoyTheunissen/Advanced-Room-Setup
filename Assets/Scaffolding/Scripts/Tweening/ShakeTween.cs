using RoyTheunissen.Scaffolding.Tweening;

namespace UnityEngine
{
    public class ShakeTween : Tween
    {
        private float amplitude;
        public float Amplitude
        {
            get { return amplitude; }
            set
            {
                amplitude = value;
                CallHandler();
            }
        }

        private Transform transform;
        private Shake shake;

        public ShakeTween(Transform transform, float amplitude = 1.0f, float duration = 1) : base(duration)
        {
            shake = new Shake(
                new Wave(1.0f / 3, 3.156f, 0.0f, Vector3.right),
                new Wave(1.0f / 3, 6.895f, 0.125645f, Vector3.up),
                new Wave(1.0f / 3, 9.32456f, 0.8546f, Vector3.forward)
            );

            this.transform = transform;
            this.amplitude = amplitude;
            handler = Handler;
        }

        private void Handler(float fraction)
        {
            float curve = 
                    //Mathf.Sin(fraction * Mathf.PI)
                    1.0f - Mathf.Pow(1.0f - fraction * 2.0f, 2)
                ;
                    
            if (transform != null)
                transform.localPosition = shake.GetOffset(fraction) * curve * amplitude;
        }
    }
}
