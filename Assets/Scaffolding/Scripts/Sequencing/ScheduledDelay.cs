namespace RoyTheunissen.Scaffolding.Sequencing
{
    /// <summary>
    /// A useful sequencable that does nothing but contribute a specified delay to the sequence.
    /// </summary>
    public class ScheduledDelay : ISequenceable
    {
        private float delay;

        public bool IsDone => true;

        public ScheduledDelay(float delay)
        {
            this.delay = delay;
        }

        public float GetDelay(Sequence tweenSequence)
        {
            return delay;
        }

        public void AddedToSequence(Sequence sequence)
        {
        }

        public void PlaySequenceable(Sequence sequence)
        {
        }

        public void StopSequenceable(Sequence tweenSequence)
        {
        }
    }
}
