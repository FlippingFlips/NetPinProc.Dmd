using NetPinProc.Domain;
using System.Collections.Generic;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Layer subclass that composites several sublayers together.
    /// </summary>
    public class GroupedLayer : Layer
    {
        /// <summary>
        /// List of layers to be composited together whenever this layer's NextFrame() is called.
        /// <para/>
        /// Layers are composited first to last using each layer's CompositeNext() method.
        /// </summary>
        public List<Layer> layers;

        private Frame buffer;

        /// <inhertdoc/>
        public GroupedLayer(int width, int height, List<Layer> layers = null)
        {
            this.buffer = new Frame(width, height);
            if (layers == null)
                this.layers = new List<Layer>();
            else
                this.layers = layers;
        }

        /// <summary>
        /// Resets all layers
        /// </summary>
        public override void Reset()
        {
            foreach (Layer layer in this.layers)
                layer.Reset();
        }

        /// <summary>
        /// Composites next frame
        /// </summary>
        /// <returns></returns>
        public override IFrame NextFrame()
        {
            this.buffer.Clear();
            int composited_count = 0;
            foreach (Layer layer in this.layers)
            {
                Frame frame = null;
                if (layer.Enabled)
                    frame = (Frame)layer.CompositeNext(this.buffer);
                if (frame != null)
                    composited_count++;
                if (frame != null && layer.Opaque) // If an Opaque layer doesn't Draw anything, dont Stop
                    break;
            }

            if (composited_count == 0)
                return null;

            return this.buffer;
        }
    }
}
