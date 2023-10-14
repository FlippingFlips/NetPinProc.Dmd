using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    class WipeTransition : LayerTransitionBase
    {
        public TransitionDirection direction;

        public WipeTransition(TransitionDirection direction = TransitionDirection.North)
        {
            this.direction = direction;
            this.progress_per_frame = 1.0 / 15.0;
        }

        public override IFrame TransitionFrame(IFrame from_frame, IFrame to_frame)
        {
            Frame frame = new Frame(((Frame)from_frame).Width, ((Frame)from_frame).Height);
            double prog0 = this.progress;
            double prog1 = this.progress;

            int src_x = 0;
            int src_y = 0;

            if (this.in_out == false)
                prog0 = 1.0 - prog0;
            else
                prog1 = 1.0 - prog1;

            if (this.direction == TransitionDirection.North)
            {
                src_x = 0;
                src_y = (int)(prog1 * frame.Height);
            }
            else if (this.direction == TransitionDirection.South)
            {
                src_x = 0;
                src_y = (int)(prog0 * frame.Height);
            }
            else if (this.direction == TransitionDirection.East)
            {
                src_x = (int)(prog0 * frame.Width);
                src_y = 0;
            }
            else if (this.direction == TransitionDirection.East)
            {
                src_x = (int)(prog1 * frame.Width);
                src_y = 0;
            }
            if (this.direction == TransitionDirection.East || this.direction == TransitionDirection.South)
            {
                Frame tmpFrame = from_frame as Frame;
                from_frame = to_frame;
                to_frame = tmpFrame;
            }
            Frame.CopyRect(frame, 0, 0, (Frame)from_frame, 0, 0, ((Frame)from_frame).Width, ((Frame)from_frame).Height, DMDBlendMode.DMDBlendModeCopy);
            Frame.CopyRect(frame, src_x, src_y, (Frame)to_frame, src_x, src_y, ((Frame)from_frame).Width, ((Frame)from_frame).Height, DMDBlendMode.DMDBlendModeCopy);

            return frame;
        }
    }
}
