using NetPinProc.Domain;
using System.Collections.Generic;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Performs a cross-fade between two layers. As one fades out the other fades in.
    /// </summary>
    public class CrossFadeTransition : LayerTransitionBase
    {
        int width, height;
        List<Frame> frames;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public CrossFadeTransition(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.progress_per_frame = 1.0 / 45.0;
            
            // Create the frames that will be used in the composite operations
            this.frames = new List<Frame>();
            for (int i = 0; i < 16; i++)
            {
                Frame frame = new Frame(width, height);
                frame.FillRect(0, 0, width, height, (byte)i);
                this.frames.Add(frame);
            }
        }

        /// <summary>
        /// From one to the other DMD
        /// </summary>
        /// <param name="from_frame"></param>
        /// <param name="to_frame"></param>
        /// <returns></returns>
        public override IFrame TransitionFrame(IFrame from_frame, IFrame to_frame)
        {
            // Calculate the Frame index
            int index = 0;
            if (this.in_out == true)
                index = (int)(this.progress * (frames.Count - 1));
            else
                index = (int)((1.0 - this.progress) * (frames.Count - 1));

            // Subtract the respective reference Frame from each of the input frames
            from_frame = from_frame.Copy();
            Frame.CopyRect((DMDBuffer)from_frame, 0, 0, this.frames[index], 0, 0, this.width, this.height, DMDBlendMode.DMDBlendModeSubtract);
            to_frame = to_frame.Copy();
            Frame.CopyRect((DMDBuffer)to_frame, 0, 0, this.frames[frames.Count - (index + 1)], 0, 0, this.width, this.height, DMDBlendMode.DMDBlendModeSubtract);
            // Add the results together
            Frame.CopyRect((DMDBuffer)from_frame, 0, 0, (DMDBuffer)to_frame, 0, 0, this.width, this.height, DMDBlendMode.DMDBlendModeAdd);
            return from_frame;
        }
    }
}
