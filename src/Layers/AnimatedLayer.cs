using System;
using System.Collections.Generic;
using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Collection of frames displayed sequentially as an animation. Optionally holds the last Frame on-screen
    /// </summary>
    public class AnimatedLayer : Layer
    {
        /// <summary>
        /// Index of the next Frame to display. Incremented by 'NextFrame()'
        /// </summary>
        public int frame_pointer = 0;

        /// <summary>
        /// Number of Frame times each Frame should be shown on screen before advancing to the next Frame.
        /// </summary>
        public int frame_time = 1;

        /// <summary>
        /// True if the last Frame of the animation should be held on-screen indefinitely
        /// </summary>
        public bool hold = true;

        /// <summary>
        /// True if the animation should be repeated indefinitely
        /// </summary>
        public bool repeat = false;
        private List<Pair<int, Delegate>> frame_listeners;
        private int frame_time_counter = 0;
        private Frame[] frames;
        /// <summary>
        /// Initializes then calls <see cref="Reset"/>
        /// </summary>
        /// <param name="opaque"></param>
        /// <param name="hold"></param>
        /// <param name="repeat"></param>
        /// <param name="frame_time"></param>
        /// <param name="frames"></param>
        public AnimatedLayer(bool opaque = false, bool hold = true, bool repeat = false, int frame_time = 1, Frame[] frames = null)
            : base(opaque)
        {
            this.hold = hold;
            this.repeat = repeat;

            this.frames = frames;

            this.frame_time = frame_time;
            this.frame_time_counter = frame_time;
            this.frame_listeners = new List<Pair<int, Delegate>>();
            this.Reset();
        }

        /// <summary>
        /// Registers a listener to be called when a specific Frame number (frame_index) in the
        /// animation has been reached.
        /// Negative numbers indicate a number of frames from the last Frame. That is a Frame
        /// index of -1 will trigger on the last Frame of the animation.
        /// </summary>
        /// <param name="frame_index"></param>
        /// <param name="listener"></param>
        public void AddFrameListener(int frame_index, Delegate listener)
        {
            Pair<int, Delegate> v = new Pair<int, Delegate>(frame_index, listener);
            frame_listeners.Add(v);
        }

        /// <summary>
        /// Returns the Frame to be shown, or null if there is no Frame.
        /// </summary>
        /// <returns></returns>
        public override IFrame NextFrame()
        {
            if (frame_pointer >= frames.Length) return null;

            // Important: Notify the Frame listeners before the frame_pointer
            // has been advanced. Only notify the listeners if this is the first time
            // this Frame has been shown (such as if frame_time is > 1)
            if (frame_time_counter == frame_time)
                notify_frame_listeners();

            Frame frame = frames[frame_pointer];
            frame_time_counter--;

            if (frames.Length > 1 && frame_time_counter == 0)
            {
                if (frame_pointer == frames.Length - 1)
                {
                    if (repeat)
                        frame_pointer = 0;
                    else if (!hold)
                        frame_pointer++;
                }
                else
                    frame_pointer++;
            }

            if (frame_time_counter == 0)
                frame_time_counter = frame_time;

            return frame;
        }

        /// <summary>
        /// Resets the animation back to the first Frame
        /// </summary>
        public override void Reset() => frame_pointer = 0;
        private void notify_frame_listeners()
        {
            for (int i = 0; i < frame_listeners.Count; i++)
            {
                Pair<int, Delegate> v = frame_listeners[i];
                if (v.First >= 0 && frame_pointer == v.First)
                    v.Second.DynamicInvoke();
                else if (frame_pointer == (frames.Length + v.First))
                    v.Second.DynamicInvoke();
            }
        }
    }
}
