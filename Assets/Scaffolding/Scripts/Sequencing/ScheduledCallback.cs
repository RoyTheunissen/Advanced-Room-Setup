using System;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Sequencing
{
    /// <summary>
    /// Schedules a callback to be executed as part of a sequence.
    /// </summary>
    public class ScheduledCallback : ISequenceable
    {
        private Action callback;

        private float startTime;

        private float delay;
        private float duration;

        public bool IsDone => Time.time >= startTime + duration;

        private Coroutine playSequenceablesRoutine;

        public ScheduledCallback(Action callback, float delay = 0.0f, float duration = 0.0f)
        {
            this.callback = callback;
            this.delay = delay;
            this.duration = duration;
        }

        public float GetDelay(Sequence sequence)
        {
            return delay;
        }

        public void AddedToSequence(Sequence sequence)
        {
        }

        public void PlaySequenceable(Sequence tweenSequence)
        {
            startTime = Time.time;
            callback();
        }

        public void StopSequenceable(Sequence sequence)
        {
        }
    }
}
