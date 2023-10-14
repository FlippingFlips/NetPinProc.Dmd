using NetPinProc.Domain;
using System;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Transition base class
    /// </summary>
    public class LayerTransitionBase
    {
        /// <summary>
        /// Function to be called once the transition has completed
        /// </summary>
        public Delegate completed_handler = null;

        /// <summary>
        /// If true, transition is moving from 'from' to 'to', if false, the transition is moving from 'to' to 'from'
        /// </summary>
        public bool in_out = true;

        /// <summary>
        /// Transition progress from 0.0 (100% from Frame, 0% to Frame) to 1.0 (0% from Frame, 100% to Frame)
        /// updated by 'NextFrame()'
        /// </summary>
        public double progress = 0.0;

        ///<inheritdoc/>
        public int progress_mult = 0;

        /// <summary>
        /// Progress increment for each Frame. Default to 1/60 or 60fps
        /// </summary>
        public double progress_per_frame = 1.0 / 60.0;

        /// <summary>
        /// Applies the transition and increments the progress if the transition is running.
        /// Returns the resulting Frame object.
        /// </summary>
        public virtual IFrame NextFrame(IFrame from_frame, IFrame to_frame)
        {
            this.progress = Math.Max(0.0, Math.Min(1.0, this.progress + this.progress_mult * this.progress_per_frame));
            if (this.progress <= 0.0)
            {
                if (this.in_out == true)
                    return from_frame;
                else
                    return to_frame;
            }
            if (this.progress >= 1.0)
            {
                if (this.completed_handler != null)
                    this.completed_handler.DynamicInvoke();
                if (this.in_out == true)
                    return to_frame;
                else
                    return from_frame;
            }
            return this.TransitionFrame(from_frame, to_frame);
        }

        /// <summary>
        /// Pauses the transition at the current position
        /// </summary>
        public void Pause()
        {
            this.progress_mult = 0;
        }

        /// <summary>
        /// Reset the transition to the beginning
        /// </summary>
        public void Reset()
        {
            this.progress_mult = 0;
            this.progress = 0;
        }

        // Not moving, -1 for B to A, 1 for A to B. play/Pause manipulates this
        /// <summary>
        /// Start the transition
        /// </summary>
        public void Start()
        {
            this.Reset();
            this.progress_mult = 1;
        }
        /// <summary>
        /// Applies the transition at the current progress value.
        /// 
        /// Subclasses should override this method to provide more interesting transition effects.
        /// base implementation simply returns the from_frame
        /// </summary>
        public virtual IFrame TransitionFrame(IFrame from_frame, IFrame to_frame)
        {
            return from_frame;
        }
    }
}
