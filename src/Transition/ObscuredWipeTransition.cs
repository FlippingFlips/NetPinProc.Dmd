using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <inheritdoc/>
    public class ObscuredWipeTransition : LayerTransitionBase
    {
        DMDBlendMode composite_op;
        TransitionDirection direction;
        Frame obs_frame;

        /// <inheritdoc/>
        public ObscuredWipeTransition(Frame obscuring_frame, DMDBlendMode composite_op, TransitionDirection direction = TransitionDirection.North)
        {
            this.composite_op = composite_op;
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
            this.obs_frame = obscuring_frame;
        }

        /// <summary>
        /// TODO: Improve src_x/y so that it moves at the same speed as ovr_x/y with the midpoint
        /// </summary>
        /// <param name="from_frame"></param>
        /// <param name="to_frame"></param>
        /// <returns></returns>
        public override IFrame TransitionFrame(IFrame from_frame, IFrame to_frame)
        {
            Frame frame = new Frame(((Frame)from_frame).Width, ((Frame)from_frame).Height);
            double prog0 = this.progress;
            double prog1 = this.progress;
            if (this.in_out == false)
                prog0 = 1.0 - prog0;
            else
                prog1 = 1.0 - prog1;

            int src_x, src_y, ovr_x, ovr_y;            
            if (direction == TransitionDirection.North)
            {
                src_x = 0;
                src_y = (int)(prog1 * frame.Height);
                ovr_x = 0;
                ovr_y = (int)(frame.Height - prog0 * (this.obs_frame.Height + 2 * frame.Height));
            }
            else if (direction == TransitionDirection.South)
            {
                src_x = 0;
                src_y = (int)(prog0 * frame.Height);
                ovr_x = 0;
                ovr_y = (int)(frame.Height - prog1 * (this.obs_frame.Height + 2 * frame.Height));
            }
            else if (direction == TransitionDirection.East)
            {
                src_x = (int)(prog0 * frame.Width);
                src_y = 0;
                ovr_x = (int)(frame.Width - prog1 * (this.obs_frame.Width + 2 * frame.Width));
                ovr_y = 0;
            }
            else
            {
                src_x = (int)(prog1 * frame.Width);
                src_y = 0;
                ovr_x = (int)(frame.Width - prog0 * (this.obs_frame.Width + 2 * frame.Width));
                ovr_y = 0;
            }

            if (this.direction == TransitionDirection.East || this.direction == TransitionDirection.South)
            {
                var tmpFrame = from_frame as Frame;
                
                from_frame = to_frame;
                to_frame = tmpFrame;
            }

            Frame.CopyRect(frame, 0, 0, ((Frame)from_frame), 0, 0, ((Frame)from_frame).Width, ((Frame)from_frame).Height, DMDBlendMode.DMDBlendModeCopy);
            Frame.CopyRect(frame, src_x, src_y, (Frame)to_frame, src_x, src_y, ((Frame)from_frame).Width - src_x, ((Frame)from_frame).Height - src_y, DMDBlendMode.DMDBlendModeCopy);
            Frame.CopyRect(frame, ovr_x, ovr_y, obs_frame, 0, 0, this.obs_frame.Width, this.obs_frame.Height, this.composite_op);

            return frame;
        }
    }
}
