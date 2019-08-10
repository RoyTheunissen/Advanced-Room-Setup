using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Tweening
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Shake 
    {
        [SerializeField]
        private List<Wave> waves = new List<Wave>();

        [SerializeField]
        private float masterAmplitude = 1.0f;

        public Shake(params Wave[] waves)
        {
            this.waves = waves.ToList();
        }

        public Vector3 GetOffset()
        {
            return GetOffset(Time.time);
        }

        public Vector3 GetOffset(float time)
        {
            Vector3 totalOffset = Vector3.zero;
            for (int i = 0; i < waves.Count; i++)
                totalOffset += waves[i].GetOffset(time);
            return totalOffset * masterAmplitude;
        }
    }
}
