using NetPinProc.Domain;
using System;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Expand frames 
    /// </summary>
    public class ExpandTransition : LayerTransitionBase
    {
        TransitionVertical direction;

        /// <summary>
        /// Sets direction and progress_per_frame
        /// </summary>
        /// <param name="direction"></param>
        public ExpandTransition(TransitionVertical direction = TransitionVertical.Vertical)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 11.0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from_frame"></param>
        /// <param name="to_frame"></param>
        /// <returns></returns>
        public override IFrame TransitionFrame(IFrame from_frame, IFrame to_frame)
        {
            Frame frame = new Frame(((Frame)from_frame).Width, ((Frame)from_frame).Height);
            int dst_x = 0;
            int dst_y = 0;
            int width, height;
            double prog = this.progress;

            if (this.in_out == false)
                prog = 1.0 - prog;

            if (this.direction == TransitionVertical.Vertical)
            {
                dst_x = 0;
                dst_y = Convert.ToInt32((frame.Height / 2 - prog * (frame.Height / 2)));

                width = frame.Width;
                height = Convert.ToInt32(prog * frame.Height);
            }
            else
            {
                dst_x = Convert.ToInt32((frame.Width / 2 - prog * (frame.Width / 2)));
                dst_y = 0;

                width = Convert.ToInt32(prog * frame.Width);
                height = frame.Height;
            }
            Frame.CopyRect(frame, dst_x, dst_y, (DMDBuffer)to_frame, dst_x, dst_y, width, height, DMDBlendMode.DMDBlendModeCopy);
            return frame;
        }
    }
}
