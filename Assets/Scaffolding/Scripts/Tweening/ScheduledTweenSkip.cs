using RoyTheunissen.Scaffolding.Sequencing;

namespace RoyTheunissen.Scaffolding.Tweening
{
    /// <summary>
    /// Skips to a specific tween value.
    /// </summary>
    public class ScheduledTweenSkip : ISequenceable
    {
        private Tween tween;
        private float valueToSkipTo;

        public bool IsDone => true;

        public ScheduledTweenSkip(Tween tween, float valueToSkipTo)
        {
            this.tween = tween;
            this.valueToSkipTo = valueToSkipTo;
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
            tween.SkipTo(valueToSkipTo);
        }

        public void StopSequenceable(Sequence sequence)
        {
        }
    }
}
