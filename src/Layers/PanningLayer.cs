using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Pans a Frame about on a 128x32 buffer, bouncing when it reaches the boundaries
    /// </summary>
    public class PanningLayer : Layer
    {
        Frame buffer;
        Frame frame;
        Pair<int, int> origin;
        Pair<int, int> original_origin;
        Pair<int, int> translate;
        bool bounce;
        int tick;

        /// <inheritdoc/>
        public PanningLayer(int width, int height, Frame frame, Pair<int, int> origin, Pair<int, int> translate, bool bounce = true)
        {
            this.buffer = new Frame(width, height);
            this.frame = frame;
            this.origin = origin;
            this.original_origin = origin;
            this.translate = translate;
            this.bounce = bounce;
            this.tick = 0;

            // make sure the translate value doesnt cause us to do any strange movements
            if (width == frame.Width)
                this.translate = new Pair<int, int>(0, this.translate.Second);
            if (height == frame.Height)
                this.translate = new Pair<int, int>(this.translate.First, 0);
        }

        /// <inheritdoc/>
        public override void Reset() => this.origin = this.original_origin;

        /// <inheritdoc/>
        public override IFrame NextFrame()
        {
            this.tick += 1;

            if ((this.tick % 6) != 0) return this.buffer;

            Frame.CopyRect(this.buffer, 0, 0, this.frame, this.origin.First, this.origin.Second, this.buffer.Width, this.buffer.Height);
            if (this.bounce && (this.origin.First + this.buffer.Width + this.translate.First > this.frame.Width) ||
                this.origin.First + this.translate.First < 0)
            {
                this.translate = new Pair<int, int>(this.translate.First * -1, this.translate.Second);
            }

            if (this.bounce && (this.origin.Second + this.buffer.Height + this.translate.Second > this.frame.Height) ||
                (this.origin.Second + this.translate.Second < 0))
            {
                this.translate = new Pair<int, int>(this.translate.First, this.translate.Second * -1);
            }

            this.origin = new Pair<int, int>(this.origin.First + this.translate.First, this.origin.Second + this.translate.Second);
            return this.buffer;
        }
    }
}
