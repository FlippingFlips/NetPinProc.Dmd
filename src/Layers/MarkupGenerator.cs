using System;
using System.Collections.Generic;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// Renders a Frame object for given text-based markup.
    /// <para/>
    /// The markup format presently uses three markup tokens:
    /// '#' for headlines and '[' and ']' for plain text. The markup tokens indicate
    /// justification. Lines with no markup or a leading '#' or '[' will be left-justified.
    /// Lines with a training '#' or ']' will be right justified. Lines with both will be centered.
    /// <para/>
    /// The Width and min-Height are specified with instantiation.
    /// <para/>
    /// Fonts can be adjusted by assigning 'font_plain' and 'font_bold' member values.
    /// </summary>
    public class MarkupGenerator
    {
        Font font_plain = null;
        Font font_bold = null;
        Frame frame;
        int width, min_height;

        ///<inheritdoc/>
        public MarkupGenerator(int width = 128, int min_height = 32)
        {
            this.width = width;
            this.min_height = min_height;
            this.frame = null;
            this.font_plain = FontManager.instance.FontName("Font07x5.dmd");
            this.font_bold = FontManager.instance.FontName("Font09Bx7.dmd");
        }

        /// <summary>
        /// Returns a Frame with the given markup rendered within it.
        /// 
        /// The Frame Width is fixed, but the Height will be adjusted to fit the contents while respecting
        /// min_height.
        /// 
        /// The Y offset can be configured by supplying 'y_offset'
        /// </summary>
        public Frame FrameForMarkup(string markup, int y_offset = 0)
        {
            markup = markup.Replace("\r", "");
            string[] lines = markup.Split('\n');
            foreach (bool draw in new bool[] { false, true })
            {
                int y = y_offset;
                foreach (string line in lines)
                {
                    if (line.StartsWith("#") && line.EndsWith("#")) // Centered headline
                        y = this.DrawText(y, line.Substring(1, line.Length - 2), font_bold, FontJustify.Center, draw);
                    else if (line.StartsWith("#")) // Left justified headline
                        y = this.DrawText(y, line.Substring(1), font_bold, FontJustify.Left, draw);
                    else if (line.EndsWith("#")) // Right justified headline
                        y = this.DrawText(y, line.Substring(0, line.Length - 2), font_bold, FontJustify.Right, draw);
                    else if (line.StartsWith("[") && line.EndsWith("]")) // Centered text
                        y = this.DrawText(y, line.Substring(1, line.Length - 2), font_plain, FontJustify.Center, draw);
                    else if (line.EndsWith("]")) // Right justified text
                        y = this.DrawText(y, line.Substring(0, line.Length - 2), font_plain, FontJustify.Right, draw);
                    else if (line.StartsWith("[")) // Left justified text
                        y = this.DrawText(y, line.Substring(1), font_plain, FontJustify.Left, draw);
                    else // Left justified but nothing to clip off
                        y = this.DrawText(y, line, font_plain, FontJustify.Left, draw);
                }
                if (!draw)
                    this.frame = new Frame(this.width, Math.Max(this.min_height, y));
            }
            return this.frame;
        }

        private int DrawText(int y, string text, Font font, FontJustify justify, bool draw)
        {
            if (GetMaxValueInList(font.char_widths) * text.Length > this.width)
            {
                // We need to do some word wrapping
                string line = "";
                int w = 0;
                foreach (char ch in text)
                {
                    line += ch;
                    w += font.size(ch.ToString()).First;
                    if (w > this.width)
                    {
                        // Too much! We need to back-track for the last space. If possible...
                        int idx = line.LastIndexOf(' ');
                        if (idx == -1)
                        {
                            // No space, we'll have to break before this char and continue
                            y = this.DrawLine(y, line.Substring(0, line.Length - 1), font, justify, draw);
                            line = ch.ToString();
                        }
                        else
                        {
                            // We found a space!
                            y = this.DrawLine(y, line.Substring(0, idx), font, justify, draw);
                            line = line.Substring(idx + 1, line.Length - idx - 1);
                        }
                        // Recalculate w
                        w = font.size(line).First;
                    }
                }
                if (line.Length > 0) // Left-over text we have to Draw
                    y = this.DrawLine(y, line, font, justify, draw);
                return y;
            }
            else
                return this.DrawLine(y, text, font, justify, draw);
        }

        /// <summary>
        /// Draw a line without concern for word wrapping
        /// </summary>
        private int DrawLine(int y, string text, Font font, FontJustify justify, bool draw)
        {
            int w = 0;
            if (draw)
            {
                int x = 0; // TODO: x should be set based on justify
                if (justify != FontJustify.Left)
                {
                    w = font.size(text).First;
                    if (justify == FontJustify.Center)
                        x = (this.frame.Width - w) / 2;
                    else
                        x = (this.frame.Width - w);
                }
                font.Draw(this.frame, text, x, y);
            }
            y += font.CharSize;
            return y;
        }

        private int GetMaxValueInList(List<int> list)
        {
            int[] listArr = list.ToArray();
            Array.Sort(listArr);
            return listArr[listArr.Length - 1];
        }
    }
}
