using NetPinProc.Domain;
using System;

namespace NetPinProc.Dmd
{
    /// <inheritdoc/>
    public class DMDBuffer
    {
        /// <inheritdoc/>
        public DMDFrame frame;

        /// <inheritdoc/>
        public DMDBuffer(int width, int height)
        {
            frame = DMDGlobals.DMDFrameCreate(new DMDSize() { height = height, width = width });
        }
        /// <inheritdoc/>
        ~DMDBuffer()
        {
            frame.buffer = null;
        }

        /// <inheritdoc/>
        public string Ascii()
        {
            string output = "";
            char[] table = { ' ', '.', '.', '.', ',', ',', ',', '-', '-', '=', '=', '=', '*', '*', '#', '#' };
            byte dot = 0;
            for (int y = 0; y < this.frame.size.height; y++)
            {
                for (int x = 0; x < this.frame.size.width; x++)
                {
                    dot = this.GetDot(x, y);
                    output += table[dot & 0xf];
                }
                output += "\n";
            }
            return output;
        }

        /// <inheritdoc/>
        public void Clear() => Array.Clear(frame.buffer, 0, 0);

        /// <inheritdoc/>
        public void CopyToRect(DMDBuffer dst, int dst_x, int dst_y, int src_x, int src_y, int width, int height, DMDBlendMode mode = DMDBlendMode.DMDBlendModeCopy)
        {
            DMDRect srcRect = DMDGlobals.DMDRectMake(src_x, src_y, width, height);
            DMDPoint dstPoint = DMDGlobals.DMDPointMake(dst_x, dst_y);
            DMDGlobals.DMDFrameCopyRect(ref frame, srcRect, ref dst.frame, dstPoint, mode);
        }

        /// <inheritdoc/>
        public void FillRect(int x, int y, int width, int height, byte value)
        {
            DMDGlobals.DMDFrameFillRect(ref frame,
                DMDGlobals.DMDRectMake(x, y, width, height),
                value);
        }

        /// <summary>
        /// Gets the <see cref="DMDFrame.buffer"/>
        /// </summary>
        /// <returns></returns>
        public byte[] GetData() => frame.buffer;

        /// <summary>
        /// Gets a dot at position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte GetDot(int x, int y)
        {
            if (x >= frame.size.width || y >= frame.size.height)
            {
                throw new Exception("X or Y are out of range");
            }
            return DMDGlobals.DMDFrameGetDot(ref frame, x, y);
        }

        /// <summary>
        /// Set frame data
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="Exception"></exception>
        public void SetData(byte[,] data)
        {
            int frame_size = DMDGlobals.DMDFrameGetBufferSize(ref frame);
            if (data.Length != frame_size)
            {
                throw new Exception("Buffer length is incorrect (" + data.Length.ToString() + " != " + frame_size.ToString() + ")");
            }
            Buffer.BlockCopy(data, 0, frame.buffer, 0, (int)frame_size);
        }

        /// <summary>
        /// Set frame data
        /// </summary>
        /// <param name="data"></param>
        /// <exception cref="Exception"></exception>
        public void SetData(byte[] data)
        {
            int frame_size = DMDGlobals.DMDFrameGetBufferSize(ref frame);
            if (data.Length != frame_size)
            {
                throw new Exception("Buffer length is incorrect");
            }
            Buffer.BlockCopy(data, 0, frame.buffer, 0, (int)frame_size);
        }

        /// <summary>
        /// Sets a dot at a given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        /// <exception cref="Exception"></exception>
        public void SetDot(int x, int y, byte value)
        {
            if (x >= frame.size.width || y >= frame.size.height)
            {
                throw new Exception("X or Y are out of range");
            }
            DMDGlobals.DMDFrameSetDot(ref frame, x, y, value);
        }
    }
}
