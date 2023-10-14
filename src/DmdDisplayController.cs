using NetPinProc.Domain;
using NetPinProc.Domain.PinProc;
using System;
using System.Collections.Generic;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="frame"></param>
    public delegate void DMDFrameHandler(Frame frame);
    /// <summary>
    /// Manages the process of obtaining DMD frames from active modes and compositing them together for
    /// display on the DMD.
    /// 
    /// 1. Add a DisplayController instance to your class (GameController subclass)
    /// 
    /// 2. In your subclass' method dmd_event(), call DisplayController.Update() (this.dmd.Update();)
    /// </summary>
    public class DmdDisplayController : IDisplayController
    {
        /// <summary>
        /// If set, frames obtained by Update() will be sent to the functions in this list
        /// with the Frame as the only parameter.
        /// 
        /// This list is initialized to contain only dmd_draw()
        /// </summary>
        public List<DMDFrameHandler> frame_handlers;

        private IGameController game;
        private TextLayer message_layer;
        private int width = 0;
        private int height = 0;

        /// <summary>
        /// Creates DMD 128x32
        /// </summary>
        /// <param name="game"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="message_font"></param>
        public DmdDisplayController(IGameController game, int width = 128, int height = 32, Font message_font = null)
        {
            this.game = game;
            this.width = width;
            this.height = height;
            if (message_font != null)
                this.message_layer = new TextLayer(1, 1, message_font, FontJustify.Center);

            // Do two updates to get the "pump primed" ? -- Yeah.
            for (int i = 0; i < 2; i++)
                this.Update();

            this.frame_handlers = new List<DMDFrameHandler>
            {
                new DMDFrameHandler(game.PROC.DmdDraw)
            };
        }

        /// <inheritdoc/>
        public void SetMessage(string message, int seconds)
        {
            if (this.message_layer == null)
                throw new Exception("message_font must be specified in constructor to enable message layer.");

            game.Logger.Log("Setting message layer on DC", LogLevel.Debug);
            this.message_layer.SetText(message, seconds);
        }

        /// <summary>
        /// Iterates over 'GameController.Modes' from lowest to highest and composites a DMD image for this
        /// point in time by checking for a layer attribute on each Mode class.
        /// 
        /// If the mode has a layer attribute, that layer's CompositeNext method is called to apply that layer's
        /// next Frame to the Frame in progress.
        /// </summary>
        public void Update()
        {
            List<Layer> layers = new List<Layer>();
            foreach (IMode mode in this.game.Modes.Modes)
            {
                if (mode.Layer != null)
                {
                    layers.Add(mode.Layer as Layer);
                    if (mode.Layer.Opaque) break;
                }
            }

            Frame frame = new Frame(this.width, this.height);
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                if (layers[i].Enabled)
                    layers[i].CompositeNext(frame);
            }
            if (this.message_layer != null)
            {
                this.message_layer.CompositeNext(frame);
            }
            if (frame != null && this.frame_handlers != null)
            {
                foreach (DMDFrameHandler handler in this.frame_handlers)
                {
                    handler.DynamicInvoke(frame);
                }
            }
        }
    }
}
