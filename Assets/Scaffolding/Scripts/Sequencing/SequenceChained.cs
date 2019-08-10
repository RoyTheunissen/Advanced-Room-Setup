using RoyTheunissen.Scaffolding.Routines;
using System.Collections;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Sequencing
{
    /// <summary>
    /// A chain of occurrences to be performed one after the other. Can be chained with
    /// other sequence chains, sequence groups or individual sequencables like tweens.
    /// </summary>
    public sealed class SequenceChained : Sequence
    {
        private bool isPlaying;

        private Coroutine playSequenceablesRoutine;

        public override bool IsDone => !isPlaying;

        public SequenceChained(params ISequenceable[] sequenceables)
            : base(sequenceables)
        {
        }

        public override void PlaySequenceable(Sequence sequence)
        {
            isPlaying = true;
            Routine.Start(ref playSequenceablesRoutine, PlaySequenceablesRoutine());
        }

        public override void StopSequenceable(Sequence sequence)
        {
            base.StopSequenceable(sequence);

            isPlaying = false;
            Routine.Stop(ref playSequenceablesRoutine);
        }

        private IEnumerator PlaySequenceablesRoutine()
        {
            float delay;
            for (int i = 0; i < sequenceables.Count; i++)
            {
                delay = sequenceables[i].GetDelay(this);

                // It's important to make sure that we don't yield if it's already done.
                // We don't want to wait a whole frame if it's not necessary.
                if (delay > 0.0f)
                    yield return new WaitForSeconds(delay);

                sequenceables[i].PlaySequenceable(this);

                while (!sequenceables[i].IsDone)
                    yield return null;
            }
            isPlaying = false;
        }
    }
}
