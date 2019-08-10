using RoyTheunissen.Scaffolding.Sequencing;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Tweening
{
    /// <summary>
    /// Contains the functionality for scheduling tweens as part of a larger sequence. Separated 
    /// into its own partial class for readability as these responsibilities are quite independent 
    /// of the regular functionality of a tween.
    /// </summary>
    public partial class Tween
    {
        private Sequence currentTweenSequence;

        private bool isWaitingForCompletion;

        private Coroutine playSequenceablesRoutine;

        public bool IsDone => !isWaitingForCompletion;

        private void CancelSequence()
        {
            currentTweenSequence = null;
        }

        public void StopTweeningAsScheduled()
        {
            Stop();
        }

        public void TweenToAsScheduled(
            float target, float? duration, float delay, Eases.Ease ease, Sequence sequence)
        {
            TweenToInternal(target, duration, delay, ease, sequence);
        }

        private ISequenceable ScheduleTweenToInternal(
            float target, float? duration, float delay, Eases.Ease ease = null)
        {
            return new ScheduledTweenTo(this, target, duration, delay, ease);
        }

        public ISequenceable ScheduleTweenTo(float target)
        {
            return new ScheduledTweenTo(this, target, null, 0.0f);
        }

        public ISequenceable ScheduleTweenTo(
            float target, float duration, float delay = 0.0f, Eases.Ease ease = null)
        {
            return new ScheduledTweenTo(this, target, duration, delay, ease);
        }

        public ISequenceable ScheduleTweenIn()
        {
            return new ScheduledTweenTo(this, valueMax, null, 0.0f);
        }

        public ISequenceable ScheduleTweenIn(Eases.Ease ease)
        {
            return new ScheduledTweenTo(this, valueMax, null, 0.0f, ease);
        }

        public ISequenceable ScheduleTweenIn(Eases.Ease ease, float duration, float delay = 0.0f)
        {
            return new ScheduledTweenTo(this, valueMax, duration, delay, ease);
        }

        public ISequenceable ScheduleTweenIn(float duration, float delay = 0.0f, Eases.Ease ease = null)
        {
            return new ScheduledTweenTo(this, valueMax, duration, delay, ease);
        }

        public ISequenceable ScheduleTweenOut()
        {
            return new ScheduledTweenTo(this, valueMin, null, 0.0f);
        }

        public ISequenceable ScheduleTweenOut(Eases.Ease ease)
        {
            return new ScheduledTweenTo(this, valueMin, null, 0.0f, ease);
        }

        public ISequenceable ScheduleTweenOut(Eases.Ease ease, float duration, float delay = 0.0f)
        {
            return new ScheduledTweenTo(this, valueMin, duration, delay, ease);
        }

        public ISequenceable ScheduleTweenOut(float duration, float delay = 0.0f, Eases.Ease ease = null)
        {
            return new ScheduledTweenTo(this, valueMin, duration, delay, ease);
        }
        
        public ISequenceable ScheduleSkipTo(float target)
        {
            return new ScheduledTweenSkip(this, target);
        }

        public ISequenceable ScheduleSkipToIn()
        {
            return new ScheduledTweenSkip(this, valueMax);
        }

        public ISequenceable ScheduleSkipToOut()
        {
            return new ScheduledTweenSkip(this, valueMin);
        }

        private bool IsDelayHandledBySequence()
        {
            return currentTweenSequence != null && currentTweenSequence is SequenceChained;
        }
    }
}
