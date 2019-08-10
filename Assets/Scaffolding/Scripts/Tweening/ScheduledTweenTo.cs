using RoyTheunissen.Scaffolding.Sequencing;

namespace RoyTheunissen.Scaffolding.Tweening
{
    /// <summary>
    /// Tweens to a specific value.
    /// </summary>
    public class ScheduledTweenTo : ISequenceable
    {
        private Tween tween;

        public float target;
        public float? duration;
        public float delay;
        public Eases.Ease ease;

        public bool IsDone => tween.IsDone;

        public ScheduledTweenTo(
            Tween tween, float target, float? duration, float delay, Eases.Ease ease = null)
        {
            this.tween = tween;
            this.target = target;
            this.duration = duration;
            this.delay = delay;
            this.ease = ease;
        }

        public void AddedToSequence(Sequence sequence)
        {
        }

        public float GetDelay(Sequence sequence)
        {
            return 0.0f;
        }

        public void PlaySequenceable(Sequence sequence)
        {
            tween.TweenToAsScheduled(target, duration, delay, ease, sequence);
        }

        public void StopSequenceable(Sequence sequence)
        {
            tween.StopTweeningAsScheduled();
        }
    }
}
