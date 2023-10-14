using NetPinProc.Domain;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// 
    /// </summary>
    public class ScoreLayer : GroupedLayer
    {
        ScoreDisplay _mode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="mode"></param>
        public ScoreLayer(int width, int height, ScoreDisplay mode)
            : base(width, height)
        {
            this._mode = mode;
        }

        ///<inheritdoc/>
        public override IFrame NextFrame()
        {
            this._mode.UpdateLayer();
            return base.NextFrame();
        }
    }
}
