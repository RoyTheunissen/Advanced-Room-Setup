using System.Collections;
using System.Collections.Generic;

namespace RoyTheunissen.Scaffolding.Sequencing
{
    /// <summary>
    /// A sequence of occurrences. Could either be played consecutively or simultaneously depending 
    /// on which derivation of this class you are using. See: SequenceChained and SequenceGrouped.
    /// </summary>
    public abstract class Sequence : ISequenceable, IEnumerable<ISequenceable>
    {
        protected List<ISequenceable> sequenceables = new List<ISequenceable>();

        public abstract bool IsDone
        {
            get;
        }

        public Sequence(params ISequenceable[] sequenceables)
        {
            AddSequencedItems(sequenceables);
        }

        public float GetDelay(Sequence sequence)
        {
            // Sequences themselves never have a delay. If you want to insert a delay anywhere in 
            // a sequence, feel free to use a ScheduledDelay object instead.
            return 0.0f;
        }

        /// <summary>
        /// Wraps the specified sequenceables in a new SequenceGrouped and adds that group to our 
        /// sequenced items. Called by the { } collection initializer operator. If you want to nest 
        /// a SequenceChained instead, you can explicitly prefix the braces with new SequenceChained
        /// </summary>
        public void Add(params ISequenceable[] sequenceables)
        {
            AddSequencedItems(new SequenceGrouped(sequenceables));
        }

        protected virtual void AddSequencedItems(params ISequenceable[] sequenceables)
        {
            this.sequenceables.AddRange(sequenceables);

            for (int i = 0; i < sequenceables.Length; i++)
                sequenceables[i].AddedToSequence(this);
        }

        public void AddedToSequence(Sequence sequence)
        {
        }

        public IEnumerator<ISequenceable> GetEnumerator()
        {
            return sequenceables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract void PlaySequenceable(Sequence sequence);

        public void PlaySequenceable()
        {
            // The root sequence is not attached to another sequence.
            PlaySequenceable(null);
        }

        public virtual void StopSequenceable(Sequence sequence)
        {
            for (int i = 0; i < sequenceables.Count; i++)
                sequenceables[i].StopSequenceable(sequence);
        }

        public static void Stop(Sequence sequence)
        {
            if (sequence == null)
                return;

            sequence.StopSequenceable(null);
        }

        public static void Stop(ref Sequence sequence)
        {
            Stop(sequence);
            sequence = null;
        }

        /// <summary>
        /// Plays a sequence and stops it first if it was already playing. Make sure to avoid 
        /// directly passing new Sequence instances into this, because then it doesn't get cached.
        /// It's more efficient to create a sequence once and call Play on it whenever you want.
        /// </summary>
        public static void Play(Sequence sequence)
        {
            // Syntactically it can be nicer to call Sequence.Play and provide a newly constructed 
            // sequence rather than construct it and call its PlaySequenceable, hence this method.
            // This also allows to ensure that it is stopped before playing the new sequence.
            Stop(sequence);
            sequence.PlaySequenceable();
        }

        /// <summary>
        /// Stop a previous cached sequence, start a new sequence instance and cache it.
        /// </summary>
        public static void Play(ref Sequence preExistingSequence, Sequence newSequence)
        {
            // Make sure the pre-existing sequence is stopped before assigning a new one to it.
            Stop(ref preExistingSequence);

            preExistingSequence = newSequence;
            if (newSequence != null)
                newSequence.PlaySequenceable();
        }
    }
}
