using UnityEngine;

namespace RoyTheunissen.Scaffolding.Tweening
{
    /// <summary>
    /// Lots of easy to use eases.
    /// </summary>
    public static class Eases
    {
        public delegate float Ease(float value);

        public static class Values
        {
            public static float AnimationCurveIn(float value, AnimationCurve animationCurve)
            {
                return animationCurve.Evaluate(value);
            }

            public static float AnimationCurveOut(float value, AnimationCurve animationCurve)
            {
                return 1.0f - AnimationCurveIn(1.0f - value, animationCurve);
            }
            
            public static float Square(float value, float power = 2.0f)
            {
                return Mathf.Pow(value, power);
            }

            public static float SquareInverse(float value, float power = 2.0f)
            {
                return 1.0f - Square(1.0f - value, power);
            }
            
            public static float Exponential(float value, float @base = 0.01f)
            {
                return 1.0f - Mathf.Pow(@base, value);
            }
            
            public static float ExponentialInverse(float value, float @base = 0.01f)
            {
                return 1.0f - Exponential(1.0f - value, @base);
            }

            public static float EaseIn(float value)
            {
                return 1.0f - EaseOut(1.0f - value);
            }

            public static float EaseOut(float value)
            {
                return Mathf.Sin(Mathf.PI * value * 0.5f);
            }

            public static float EaseInOut(float value)
            {
                return 0.5f - Mathf.Cos(Mathf.PI * value) * 0.5f;
            }
            
            public static float SmoothstepInOut(float value)
            {
                return Mathf.SmoothStep(0.0f, 1.0f, value);
            }

            public static float ElasticIn(
                float value, int bounces = DefaultElasticBounces, float rigidity = DefaultRigidity)
            {
                return 1.0f - ElasticOut(1.0f - value, bounces, rigidity);
            }

            public static float ElasticOut(
                float value, int bounces = DefaultElasticBounces, float rigidity = DefaultRigidity)
            {
                return 1.0f - Mathf.Pow(1.0f - value, rigidity)
                    * Mathf.Cos(value * Mathf.PI * 2 * (bounces + 0.25f));
            }

            public static float BounceIn(float value, int bounces = DefaultBounces)
            {
                return 1.0f - Mathf.Abs(Mathf.Cos(Square(value, 1) * Mathf.PI * bounces)
                    * Mathf.Pow(1.0f - value, 2));
            }

            public static float BounceOut(float value, int bounces = DefaultBounces)
            {
                return 1.0f - BounceIn(1.0f - value, bounces);
            }

            public static float EaseBackIn(float value, float amplitude = 1.0f)
            {
                return Mathf.Pow(value, 3) - value * amplitude * Mathf.Sin(value * Mathf.PI);
            }

            public static float EaseBackOut(float value, float amplitude)
            {
                return 1.0f - EaseBackIn(1.0f - value, amplitude);
            }
            
            public static float ZigZag(float value)
            {
                return 1.0f - Mathf.Abs(value * 2.0f - 1.0f);
            }
        }

        public static Ease AnimationCurveIn(AnimationCurve animationCurve)
        {
            return f => Values.AnimationCurveIn(f, animationCurve);
        }
        
        public static Ease AnimationCurveOut(AnimationCurve animationCurve)
        {
            return f => Values.AnimationCurveOut(f, animationCurve);
        }

        public static Ease Square(float power = 2.0f)
        {
            return f => { return Values.Square(f, power); };
        }
        
        public static Ease Exponential(float @base = 0.01f)
        {
            return f => { return Values.Exponential(f, @base); };
        }

        public static Ease SquareInverse(float power = 2.0f)
        {
            return f => { return Values.SquareInverse(f, power); };
        }

        public static Ease EaseIn()
        {
            return f => { return Values.EaseIn(f); };
        }

        public static Ease EaseOut()
        {
            return f => { return Values.EaseOut(f); };
        }

        public static Ease EaseInOut()
        {
            return f => { return Values.EaseInOut(f); };
        }
        
        public static Ease SmoothstepInOut()
        {
            return f => { return Values.SmoothstepInOut(f); };
        }
        
        public static Ease ZigZag()
        {
            return f => { return Values.ZigZag(f); };
        }

        private const int DefaultElasticBounces = 4;
        private const float DefaultRigidity = 6.0f;

        public static Ease ElasticIn(
            int bounces = DefaultElasticBounces, float rigidity = DefaultRigidity)
        {
            return f => { return Values.ElasticIn(f, bounces, rigidity); };
        }

        public static Ease ElasticOut(
            int bounces = DefaultElasticBounces, float rigidity = DefaultRigidity)
        {
            return f => { return Values.ElasticOut(f, bounces, rigidity); };
        }

        private const int DefaultBounces = 3;

        public static Ease BounceIn(int bounces = DefaultBounces)
        {
            return f => { return Values.BounceIn(f, bounces); };
        }

        public static Ease BounceOut(int bounces = DefaultBounces)
        {
            return f => { return Values.BounceOut(f, bounces); };
        }

        public static Ease EaseBackIn(float amplitude = 1.0f)
        {
            return f => { return Values.EaseBackIn(f, amplitude); };
        }

        public static Ease EaseBackOut(float amplitude = 1.0f)
        {
            return f => { return Values.EaseBackOut(f, amplitude); };
        }
    }
}
