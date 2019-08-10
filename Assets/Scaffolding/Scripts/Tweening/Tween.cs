using RoyTheunissen.Scaffolding.Routines;
using RoyTheunissen.Scaffolding.Sequencing;
using System;
using System.Collections;
using UnityEngine;

namespace RoyTheunissen.Scaffolding.Tweening
{
    /// <summary>
    /// A utility for interpolating to a target value in a specified duration. It got very tedious 
    /// writing coroutines for it. Now you can create an instance of this class either manually 
    /// or via an extension method on a Unity class and then skip to positions or tween to it 
    /// gradually, optionally with some kind of cool parameterized easing. You can also use 
    /// tweens as part of a larger sequence. See: TweenSequencing
    /// </summary>
    public partial class Tween
    {
        // This is used for being able to pause and resume a tween.
        private IEnumerator enumerator;
        private Coroutine routine;

        protected TweenHandler handler;

        private bool isContinuous;

        private float value;
        private float target;
        private float speed;
        private Eases.Ease ease;

        public float Value => value;

        public float ValueNormalized => Mathf.Clamp01((value - valueMin) / (valueMax - valueMin));

        private float valueMin = 0.0f;
        public float ValueMin
        {
            get { return valueMin; }
            set { valueMin = value; }
        }

        private float valueMax = 1.0f;
        public float ValueMax
        {
            get { return valueMax; }
            set { valueMax = value; }
        }

        private float duration;
        public float Duration => duration;

        private float delay;
        public float Delay => delay;

        private bool debug;
        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        private bool useUnscaledTime;
        
        public float DeltaTime => useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        public delegate void TweenHandler(float fraction);

        public delegate void TweenedInHandler(Tween tween);
        public event TweenedInHandler TweenedInEvent = delegate { };
        private Action tweenedInCallbacks;

        public delegate void TweenedOutHandler(Tween tween);
        public event TweenedOutHandler TweenedOutEvent = delegate { };
        private Action tweenedOutCallbacks;

        public delegate void TweenedToHandler(Tween tween, float target);
        public event TweenedToHandler TweenedToEvent = delegate { };
        public delegate void TweenedToCallbackHandler(float target);
        private TweenedToCallbackHandler tweenedToCallbacks;

        public delegate void StartedTweeningInHandler(Tween tween);
        public event StartedTweeningInHandler StartedTweeningInEvent = delegate { };
        private Action startedTweeningInCallbacks;

        public delegate void StartedTweeningOutHandler(Tween tween);
        public event StartedTweeningOutHandler StartedTweeningOutEvent = delegate { };
        private Action startedTweeningOutCallbacks;

        public delegate void StartedTweeningToHandler(Tween tween, float target);
        public event StartedTweeningToHandler StartedTweeningToEvent = delegate { };
        public delegate void StartedTweeningToCallbackHandler(float target);
        private StartedTweeningToCallbackHandler startedTweeningToCallbacks;

        /// <summary>
        /// An empty tween whose only use it to tween back and forth and expose events and values.
        /// 
        /// NOTE: You most likely want to use the overload with a specific tween handler instead.
        /// </summary>
        public Tween(float duration = 1.0f)
        {
            speed = 1.0f / duration;
        }

        public Tween(TweenHandler handler, float duration = 1.0f)
            : this(duration)
        {
            this.handler = handler;
        }

        public Tween SetContinuous(bool isContinuous)
        {
            if (this.isContinuous == isContinuous)
                return this;

            // TODO:    Maybe it would be nice to cancel an existing tween and start a new one at 
            //          this point, but I don't think one really wants to or should switch between 
            //          being continuous and not continuos on-the-go.

            this.isContinuous = isContinuous;
            return this;
        }

        public Tween SetUseUnscaledTime(bool useUnscaledTime)
        {
            this.useUnscaledTime = useUnscaledTime;
            return this;
        }

        protected void CallHandler()
        {
            CallHandler(Value);
        }

        protected  void CallHandler(float value)
        {
            if (handler == null)
                return;

            if (ease != null)
                handler(ease(value));
            else
                handler(value);
        }

        public Tween AddTweenedInCallback(Action callback)
        {
            tweenedInCallbacks += callback;
            return this;
        }

        public Tween AddTweenedOutCallback(Action callback)
        {
            tweenedOutCallbacks += callback;
            return this;
        }

        public Tween AddTweenedToCallback(TweenedToCallbackHandler callback)
        {
            tweenedToCallbacks += callback;
            return this;
        }

        public Tween AddStartedTweeningInCallback(Action callback)
        {
            startedTweeningInCallbacks += callback;
            return this;
        }

        public Tween AddStartedTweeningOutCallback(Action callback)
        {
            startedTweeningOutCallbacks += callback;
            return this;
        }

        public Tween AddStartedTweeningToCallback(StartedTweeningToCallbackHandler callback)
        {
            startedTweeningToCallbacks += callback;
            return this;
        }

        private void Log(string format, params object[] parameters)
        {
            if (!debug)
                return;

            UnityEngine.Debug.LogFormat(format, parameters);
        }

        private void FinishedTween()
        {
            Log("Finished tween.");

            // Allow the sequence it's currently in to continue.
            isWaitingForCompletion = false;

            TweenedToEvent(this, target);

            if (tweenedToCallbacks != null)
            {
                tweenedToCallbacks(target);
                tweenedToCallbacks = null;
            }
        }

        private IEnumerator TweenRoutine(float delay)
        {
            float startValue = value;

            Log("Starting tween at value {0} up to {1} with speed {2} and delay {3}",
                value, target, speed, delay);

            if (startValue == target)
                yield break;

            // Don't wait for our delay if our current sequence is already handling our delay. 
            // Otherwise, we want to wait for our delay ourselves.
            if (!IsDelayHandledBySequence())
            {
                if (useUnscaledTime)
                    yield return new WaitForSecondsRealtime(delay);
                else
                    yield return new WaitForSeconds(delay);
            }

            if (delay > 0)
                Log("\tFinished delay.");

            // Increment towards the target.
            if (startValue < target)
            {
                Log("\tWill increment towards target.");
                while (value < target)
                {
                    value = Mathf.Min(value + DeltaTime * speed, target);
                    Log("\t\tIncrement to {0}.", value);
                    CallHandler(value);
                    yield return null;
                }
            }

            // Decrement to the target.
            if (startValue > target)
            {
                Log("\tWill decrement towards target.");
                while (value > target)
                {
                    value = Mathf.Max(value - DeltaTime * speed, target);
                    Log("\t\tDecrement to {0}.", value);
                    CallHandler(value);
                    yield return null;
                }
            }

            value = target;
            CallHandler(value);

            if (target <= valueMin)
            {
                TweenedOutEvent(this);

                if (tweenedOutCallbacks != null)
                {
                    tweenedOutCallbacks();
                    tweenedOutCallbacks = null;
                }
            }

            if (target >= valueMax)
            {
                TweenedInEvent(this);

                if (tweenedInCallbacks != null)
                {
                    tweenedInCallbacks();
                    tweenedInCallbacks = null;
                }
            }

            FinishedTween();
        }

        private IEnumerator TweenContinuouslyRoutine()
        {
            float previousValue;
            while (true)
            {
                previousValue = value;

                // Increment towards the target.
                if (value < target)
                {
                    value = Mathf.Min(value + DeltaTime * speed, target);

                    CallHandler(value);
                }

                // Increment away from the target.
                if (value > target)
                {
                    value = Mathf.Max(value - DeltaTime * speed, target);

                    CallHandler(value);
                }

                if (!Mathf.Approximately(previousValue, target)
                    && Mathf.Approximately(value, target))
                {
                    FinishedTween();
                }

                yield return null;
            }
        }

        private void TweenToInternal(
            float target, float? duration, float delay, Eases.Ease ease = null,
            Sequence tweenSequence = null)
        {
            isWaitingForCompletion = value != target;

            this.target = target;
            if (duration != null)
                speed = 1.0f / (float)duration;
            this.ease = ease;

            // Calculate what the current duration should be.
            this.duration = Mathf.Abs(target - value) / speed;
            this.delay = delay;

            if (isContinuous)
            {
                currentTweenSequence = tweenSequence;

                if (routine == null)
                {
                    enumerator = TweenContinuouslyRoutine();
                    routine = Routine.Start(enumerator);
                }
                return;
            }

            Stop();

            // Do this after Stop because Stop resets it.
            currentTweenSequence = tweenSequence;

            enumerator = TweenRoutine(delay);
            routine = Routine.Start(enumerator);

            // Fire an event that we started tweening to something.
            StartedTweeningToEvent(this, target);
            if (startedTweeningToCallbacks != null)
            {
                startedTweeningToCallbacks(target);
                startedTweeningToCallbacks = null;
            }
        }

        private void TweenInInternal(
            float target, float? duration, float delay, Eases.Ease ease = null,
            Sequence tweenSequence = null)
        {
            TweenToInternal(target, duration, delay, ease, tweenSequence);

            // Fire an event that we started tweening in.
            StartedTweeningInEvent(this);
            if (startedTweeningInCallbacks != null)
            {
                startedTweeningInCallbacks();
                startedTweeningInCallbacks = null;
            }
        }

        private void TweenOutInternal(
            float target, float? duration, float delay, Eases.Ease ease = null,
            Sequence tweenSequence = null)
        {
            TweenToInternal(target, duration, delay, ease, tweenSequence);

            // Fire an event that we started tweening out.
            StartedTweeningOutEvent(this);
            if (startedTweeningOutCallbacks != null)
            {
                startedTweeningOutCallbacks();
                startedTweeningOutCallbacks = null;
            }
        }

        // There's a lot of overloads for the actual tweening methods themselves. This is very 
        // useful but it makes the class harder to read. Normally use of #region is often a red 
        // flag to me that a class has too much responsibility, but in this case that's not true.
        // Alternatively we could move these to a partial class.
        #region Tween Overloads
        public Tween TweenTo(float target)
        {
            TweenToInternal(target, null, 0.0f);
            return this;
        }

        public Tween TweenTo(float target, float duration, float delay = 0.0f, Eases.Ease ease = null)
        {
            TweenToInternal(target, duration, delay, ease);
            return this;
        }

        public Tween TweenIn()
        {
            TweenInInternal(valueMax, null, 0.0f);
            return this;
        }

        public Tween TweenIn(Eases.Ease ease)
        {
            TweenInInternal(valueMax, null, 0.0f, ease);
            return this;
        }

        public Tween TweenIn(Eases.Ease ease, float duration, float delay = 0.0f)
        {
            TweenInInternal(valueMax, duration, delay, ease);
            return this;
        }

        public Tween TweenIn(float duration, float delay = 0.0f, Eases.Ease ease = null)
        {
            TweenInInternal(valueMax, duration, delay, ease);
            return this;
        }

        public Tween TweenOut()
        {
            TweenOutInternal(valueMin, null, 0.0f);
            return this;
        }

        public Tween TweenOut(Eases.Ease ease)
        {
            TweenOutInternal(valueMin, null, 0.0f, ease);
            return this;
        }

        public Tween TweenOut(Eases.Ease ease, float duration, float delay = 0.0f)
        {
            TweenOutInternal(valueMin, duration, delay, ease);
            return this;
        }

        public Tween TweenOut(float duration, float delay = 0.0f, Eases.Ease ease = null)
        {
            TweenOutInternal(valueMin, duration, delay, ease);
            return this;
        }
        #endregion Tween Overloads
        
        public Tween SkipTo(float target)
        {
            Routine.Stop(routine);
            enumerator = null;
            routine = null;

            target = Mathf.Clamp(target, valueMin, valueMax);
            this.target = target;
            value = target;
            CallHandler(target);

            return this;
        }

        public Tween SkipToIn()
        {
            SkipTo(valueMax);
            return this;
        }

        public Tween SkipToOut()
        {
            SkipTo(valueMin);
            return this;
        }

        public Tween Stop()
        {
            if (routine == null)
                return this;

            Routine.Stop(routine);
            enumerator = null;
            routine = null;

            CancelSequence();

            return this;
        }

        private void Pause()
        {
            Routine.Stop(routine);
        }

        private void Resume()
        {
            routine = Routine.Start(enumerator);
        }
    }
}
