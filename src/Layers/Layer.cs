using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// The Layer class is the basis for the pyprocgame display architecture.
	/// Subclasses override NextFrame() to provide a Frame for the current moment in time.
	/// Handles compositing of provided frames and applying transitions within a DisplayController context.
    /// </summary>
    public class Layer : ILayer
    {
        /// <inheritdoc/>
        public bool Opaque { get; set; }

        /// <summary>
        /// Base 'x' component of the coordinates at which this layer will be composited upon a target buffer
        /// </summary>
        public int target_x = 0;

        /// <summary>
        /// Base 'y' component of the coordinates at which this layer will be composited upon a target buffer
        /// </summary>
        public int target_y = 0;

        /// <summary>
        /// Translation component used in addition to 'target_x' as this layer's final compositing position
        /// </summary>
        public int target_x_offset = 0;

        /// <summary>
        /// Translation component used in addition to 'target_y' as this layer's final compositing position
        /// </summary>
        public int target_y_offset = 0;

        /// <inheritdoc/>
        public bool Enabled { get; set; }

        /// <summary>
        /// The composite operation used by CompositeNext when calling DMDBuffer.CopyRect
        /// </summary>
        public DMDBlendMode composite_op = DMDBlendMode.DMDBlendModeCopy;

        /// <summary>
        /// Transition which CompositeNext() applies to the result of NextFrame prior to compositing upon the target buffer
        /// </summary>
        public LayerTransitionBase transition = null;

        /// <summary>
        /// Sets position to 0,0
        /// </summary>
        /// <param name="opaque"></param>
        public Layer(bool opaque = false)
        {
            this.Opaque = opaque;
            this.SetTargetPosition(0, 0);
        }

        /// <summary>
        /// Does nothing
        /// </summary>
        public virtual void Reset() { }

        /// <inheritdoc/>
        public virtual void SetTargetPosition(int x, int y)
        {
            this.target_x = x;
            this.target_y = y;
        }

        /// <inheritdoc/>
        public virtual IFrame NextFrame() => null;

        /// <inheritdoc/>
        public virtual IFrame CompositeNext(IFrame target)
        {
            var src = NextFrame() as Frame;
            if (src != null)
            {
                if (transition != null)
                {
                    src = this.transition.NextFrame(target, src) as Frame;
                }
                // src not all zeroes
                // Target = all zeros here
                Frame.CopyRect(target as DMDBuffer, (int)(this.target_x + this.target_x_offset), (int)(this.target_y + this.target_y_offset), src, 0, 0, src.Width, src.Height, this.composite_op);
            }
            return src;
        }
    }
}
