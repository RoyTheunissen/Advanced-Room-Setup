using System;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Tweening
{
    [Serializable]
    public class Wave
    {
        [SerializeField]
        private Vector3 perAxisAmplitude = Vector3.one;

        [SerializeField]
        private float amplitude = 1.0f;

        [SerializeField]
        private float frequency = 1.0f;

        [SerializeField]
        private float offset;

        public Wave(float amplitude, float frequency, float offset) : this(
            amplitude, frequency, offset, Vector3.one)
        {
        }

        public Wave(float amplitude, float frequency, float offset, Vector3 perAxisAmplitude)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.offset = offset;
            this.perAxisAmplitude = perAxisAmplitude;
        }

        public Vector3 GetOffset()
        {
            return GetOffset(Time.time);
        }

        public Vector3 GetOffset(float time)
        {
            float wave = Mathf.Sin((time + offset) * Mathf.PI * frequency) * amplitude;
            return wave * perAxisAmplitude;
        }
    }
}
