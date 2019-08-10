using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Tweening
{
    [Serializable]
    public class ShakeCollection
    {
        [SerializeField]
        private List<Shake> shakes = new List<Shake>();

        [SerializeField]
        private float masterAmplitude = 1.0f;

        public ShakeCollection(params Shake[] shakes)
        {
            this.shakes = shakes.ToList();
        }

        public Vector3 GetOffset()
        {
            return GetOffset(Time.time);
        }

        public Vector3 GetOffset(float time)
        {
            Vector3 totalOffset = Vector3.zero;
            for (int i = 0; i < shakes.Count; i++)
                totalOffset += shakes[i].GetOffset(time);
            return totalOffset * masterAmplitude;
        }
    }
}
