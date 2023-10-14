using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// DMDBuffer Frame
    /// </summary>
    public class Frame : DMDBuffer, IFrame
    {
        /// <inheritdoc/>
        public int Width { get; private set; }
        /// <inheritdoc/>
        public int Height { get; private set; }

        DMDFrame IFrame.Frame { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Frame(int width, int height)
            : base(width, height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// See <see cref="DMDBuffer.CopyToRect"/>
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="dst_x"></param>
        /// <param name="dst_y"></param>
        /// <param name="src"></param>
        /// <param name="src_x"></param>
        /// <param name="src_y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        public static void CopyRect(DMDBuffer dst, int dst_x, int dst_y, DMDBuffer src, int src_x, int src_y, int width, int height, DMDBlendMode mode = DMDBlendMode.DMDBlendModeCopy)
            => src.CopyToRect(dst, dst_x, dst_y, src_x, src_y, width, height, mode);

        /// <inheritdoc/>
        public IFrame SubFrame(int x, int y, int width, int height)
        {
            var subframe = new Frame(width, height);
            CopyRect(subframe, 0, 0, this, x, y, width, height, DMDBlendMode.DMDBlendModeCopy);
            return subframe;
        }

        /// <inheritdoc/>
        public IFrame Copy()
        {
            var frame = new Frame(Width, Height);
            frame.SetData(this.GetData());
            return frame;
        }
    }
}
