using NetPinProc.Domain;
using System;
using System.Collections.Generic;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Variable Width bitmap font.
    /// 
    /// Fonts can be loaded manually, using the Load() method or with the FontName() utility
    /// function which supports searching a font path.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Array of dot widths for each character, 0-indexed from 'space'.
        /// This array is populated by the Load() method
        /// </summary>
        public List<int> char_widths = null;

        /// <summary>
        /// 
        /// </summary>
        public int CharSize = 0;

        /// <summary>
        /// Composite operation used by Draw() when calling DMDBuffer.CopyRect()
        /// </summary>
        public DMDBlendMode composite_op = DMDBlendMode.DMDBlendModeCopy;

        /// <summary>
        /// Number of dots to adjust the horizontal position between characters, in addition to the last chars Width.
        /// </summary>
        public int tracking = 0;
        private Animation _anim;
        Frame bitmap = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        public Font(string filename = "")
        {
            this._anim = new Animation();
            this.CharSize = 0;
            this.bitmap = null;
            if (filename != "")
            {
                this.Load(filename);
            }
        }

        /// <summary>
        /// Uses this fonts characters to Draw the given string at the given position
        /// </summary>
        public void Draw(Frame frame, string text, int x, int y)
        {
            foreach (char ch in text)
            {
                int char_offset = (int)ch - (int)' ';
                if (char_offset < 0 || char_offset >= 96)
                    continue;

                int char_x = this.CharSize * (char_offset % 10);
                int char_y = this.CharSize * (char_offset / 10);
                int width = this.char_widths[(int)char_offset];
                Frame.CopyRect(frame, x, y, this.bitmap, char_x, char_y, width, this.CharSize, this.composite_op);
                x += width + this.tracking;
            }
        }

        /// <summary>
        /// Loads a font from a dmd file. Fonts are stored in .dmd files with Frame 0 containing
        /// the bitmap data and Frame 1 containing the character widths. 96 characters (32..127, ASCII 
        /// printables) are stored in a 10x10 grid starting with space (' ') in the upper left at 0,0
        /// The character widths are stored in the second Frame within the raw bitmap data in bytes 0-95
        /// </summary>
        /// <param name="filename"></param>
        public void Load(string filename)
        {
            this.char_widths = new List<int>();
            this._anim.Load(filename, false);

            if (this._anim.width != this._anim.height)
                throw new Exception("Width != Height");

            if (this._anim.frames.Count == 1)
            {
                // We allow 1 Frame for handmade fonts.
                // This is so that they can be loaded as a basic bitmap, have their char widths modified
                // then be saved (TODO)
                this._anim.frames.Add(new Frame(this._anim.width, this._anim.height));
            }
            else if (this._anim.frames.Count != 2)
            {
                throw new Exception("Expected 2 frames, got " + this._anim.frames.Count.ToString());
            }
            this.CharSize = (this._anim.width / 10);
            this.bitmap = this._anim.frames[0];
            for (int i = 0; i < 96; i++)
            {
                this.char_widths.Add(this._anim.frames[1].GetDot(i % _anim.width, i / _anim.width));
            }
        }

        /// <summary>
        /// Save the font to the given path
        /// </summary>
        public void Save(string filename)
        {
            Animation result = new Animation();
            result.width = this._anim.width;
            result.height = this._anim.height;
            result.frames.Add(this.bitmap);
            result.frames.Add(new Frame(result.width, result.height));
            for (int i = 0; i < 96; i++)
            {
                result.frames[1].SetDot(i % _anim.width, i / _anim.width, (byte)this.char_widths[(int)i]);
            }
        }
        /// <summary>
        /// Returns a tuple of the Width and Height of this text as rendered with this font.
        /// </summary>
        public Pair<int, int> size(string text)
        {
            int x = 0;
            int char_offset = 0;
            foreach (char ch in text)
            {
                char_offset = (int)ch - (int)' ';
                if (char_offset < 0 || char_offset >= 96)
                    continue;

                int width = this.char_widths[(int)char_offset];
                x += width + this.tracking;
            }
            return new Pair<int, int>(x, this.CharSize);
        }
    }
}
