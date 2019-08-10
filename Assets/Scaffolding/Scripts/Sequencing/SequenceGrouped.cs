using RoyTheunissen.Scaffolding.Routines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Sequencing
{
    /// <summary>
    /// A group of occurrences to be performed simultaneously. When part of a sequence, by default
    /// the sequence continues when all of the group's sequenceables have finished. The group 
    /// can also be explicitly assigned a duration though as part of a constructor. When adding 
    /// a new sequence to another sequence, you can actually omit the constructor as it implicitly 
    /// creates grouped sequences when left unspecified. See: Sequence.Add
    /// </summary>
    public sealed class SequenceGrouped : Sequence
    {
        private bool isPlaying;

        private Coroutine playSequenceablesRoutine;
        private Dictionary<ISequenceable, Coroutine> delayRoutinesBySequenceable
            = new Dictionary<ISequenceable, Coroutine>();

        private float durationOverride;
        private bool hasDurationOverride;

        public override bool IsDone => !isPlaying;

        public SequenceGrouped(params ISequenceable[] sequenceables)
            : base(sequenceables)
        {
        }

        public SequenceGrouped(float duration, params ISequenceable[] sequenceables)
            : this(sequenceables)
        {
            durationOverride = duration;
            hasDurationOverride = true;
        }

        public override void PlaySequenceable(Sequence tweenSequence)
        {
            isPlaying = true;
            Routine.Start(ref playSequenceablesRoutine, PlaySequenceablesRoutine());
        }

        public override void StopSequenceable(Sequence sequence)
        {
            base.StopSequenceable(sequence);

            isPlaying = false;
            Routine.Stop(ref playSequenceablesRoutine);

            foreach (var delayRoutineSequenceablePair in delayRoutinesBySequenceable)
                Routine.Stop(delayRoutineSequenceablePair.Value);
            delayRoutinesBySequenceable.Clear();
        }

        private IEnumerator PlaySequenceableDelayedRoutine(float delay, ISequenceable sequenceable)
        {
            yield return new WaitForSeconds(delay);

            // Unregister the sequenceable as being delayed.
            delayRoutinesBySequenceable.Remove(sequenceable);

            sequenceable.PlaySequenceable(this);
        }

        private IEnumerator PlaySequenceablesRoutine()
        {
            // Start all of the sequenceables at once, either instantaneously or delayed.
            float delay;
            Coroutine delayedRoutine;
            for (int i = 0; i < sequenceables.Count; i++)
            {
                delay = sequenceables[i].GetDelay(this);

                // If there's no delay, start the sequenceable immediately.
                if (delay == 0.0f)
                {
                    sequenceables[i].PlaySequenceable(this);
                    continue;
                }

                // If there is a delay, play the sequenceable delayed. Note that this delay affects 
                // only this sequenceable and not the others like it would in a chained sequence.
                delayedRoutine = Routine.Start(
                    PlaySequenceableDelayedRoutine(delay, sequenceables[i]));
                delayRoutinesBySequenceable.Add(sequenceables[i], delayedRoutine);
            }

            if (hasDurationOverride)
                yield return new WaitForSeconds(durationOverride);
            else
            {
                // Wait until all sequenceables are both started and done.
                while (delayRoutinesBySequenceable.Count > 0 || sequenceables.Any(s => !s.IsDone))
                    yield return null;
            }

            isPlaying = false;
        }
    }
}
