using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Displays a single Frame
    /// </summary>
    public class FrameLayer : Layer
    {
        /// <summary>
        /// Number of Frame times to turn on and off
        /// </summary>
        public int blink_frames = 0;

        private int blink_frames_counter = 0;
        private Frame frame_old = null;
        private Frame frame;

        /// <inheritdoc/>
        public FrameLayer(bool opaque = false, Frame frame = null)
            : base(opaque) => this.frame = frame;

        /// <summary>
        /// Blink frames
        /// </summary>
        /// <returns></returns>
        public override IFrame NextFrame()
        {
            if (this.blink_frames > 0)
            {
                if (this.blink_frames_counter == 0)
                {
                    this.blink_frames_counter = this.blink_frames;
                    if (this.frame == null)
                        this.frame = this.frame_old;
                    else
                    {
                        this.frame_old = this.frame;
                        this.frame = null;
                    }
                }
                else
                    this.blink_frames_counter--;
            }
            return this.frame;
        }
    }
}
