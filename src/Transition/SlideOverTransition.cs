using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    ///<inheritdoc/>
    public class SlideOverTransition : LayerTransitionBase
    {
        TransitionDirection direction;

        ///<inheritdoc/>
        public SlideOverTransition(TransitionDirection direction = TransitionDirection.North)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
        }

        ///<inheritdoc/>
        public override IFrame TransitionFrame(IFrame from_frame, IFrame to_frame)
        {
            var frame = (Frame)from_frame.Copy();
            int dst_x = 0;
            int dst_y = 0;

            double prog = this.progress;
            if (this.in_out == true)
            {
                prog = 1.0 - prog;
            }

            if (this.direction == TransitionDirection.North)
            {
                dst_x = 0;
                dst_y = (int)(prog * frame.Height);
            }
            else if (this.direction == TransitionDirection.South)
            {
                dst_x = 0;
                dst_y = (int)(-prog * frame.Height);
            }
            else if (this.direction == TransitionDirection.East)
            {
                dst_x = (int)(-prog * frame.Width);
                dst_y = 0;
            }
            else if (this.direction == TransitionDirection.West)
            {
                dst_x = (int)(prog * frame.Width);
                dst_y = 0;
            }
            Frame.CopyRect(frame, dst_x, dst_y, (Frame)to_frame, 0, 0, ((Frame)from_frame).Width, ((Frame)from_frame).Height, DMDBlendMode.DMDBlendModeCopy);
            return frame;
        }
    }
}
