using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <inheritdoc/>
    public class PushTransition : LayerTransitionBase
    {
        TransitionDirection direction;

        /// <inheritdoc/>
        public PushTransition(TransitionDirection direction = TransitionDirection.North)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
        }

        /// <inheritdoc/>
        public override IFrame TransitionFrame(IFrame from_frame, IFrame to_frame)
        {
            Frame frame = new Frame(((Frame)from_frame).Width, ((Frame)from_frame).Height);
            int dst_x = 0;
            int dst_y = 0;
            int dst_x1 = 0;
            int dst_y1 = 0;

            double prog = this.progress;
            double prog1 = this.progress;

            if (this.in_out == true)
                prog = 1.0 - prog;
            else
                prog1 = 1.0 - prog1;

            if (direction == TransitionDirection.North)
            {
                dst_x = 0;
                dst_y = (int)(prog * frame.Height);
                dst_x1 = 0;
                dst_y1 = (int)(-prog1 * frame.Height);
            }
            else if (direction == TransitionDirection.South)
            {
                dst_x = 0;
                dst_y = (int)(-prog * frame.Height);
                dst_x1 = 0;
                dst_y1 = (int)(prog1 * frame.Height);
            }
            else if (direction == TransitionDirection.East)
            {
                dst_x = (int)(-prog * frame.Width);
                dst_y = 0;
                dst_x1 = (int)(prog1 * frame.Width);
                dst_y1 = 0;
            }
            else if (direction == TransitionDirection.West)
            {
                dst_x = (int)(prog * frame.Width);
                dst_y = 0;
                dst_x1 = (int)(-prog1 * frame.Width);
                dst_y1 = 0;
            }
            Frame.CopyRect(frame, dst_x, dst_y, (Frame)to_frame, 0, 0, ((Frame)from_frame).Width, ((Frame)from_frame).Height, DMDBlendMode.DMDBlendModeCopy);
            Frame.CopyRect(frame, dst_x1, dst_y1, (Frame)from_frame, 0, 0, ((Frame)from_frame).Width, ((Frame)from_frame).Height, DMDBlendMode.DMDBlendModeCopy);

            return frame;
        }
    }
}
