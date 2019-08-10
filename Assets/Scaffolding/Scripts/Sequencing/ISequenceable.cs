namespace RoyTheunissen.Scaffolding.Sequencing
{
    public interface ISequenceable 
    {
        // The reason these are no longer properties is because a single tween instance may be 
        // scheduled to perform different kinds of tweens for different kinds of sequences. But 
        // at scheduling time we want to ask what its duration is, but it doesn't have one duration,
        // it has several depending on which sequence is inquiring. I prefer to make this explicit 
        // rather than having a Duration property pretending to have the right duration at the 
        // right time. Duration, to me, suggests what is its *current* duration, nothing more.
        float GetDelay(Sequence sequence);
        bool IsDone
        {
            get;
        }

        void PlaySequenceable(Sequence sequence);
        void StopSequenceable(Sequence sequence);

        void AddedToSequence(Sequence sequence);
    }
}
