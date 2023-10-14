using NetPinProc.Domain;
using System;
using System.Collections.Generic;
using System.IO;

namespace NetPinProc.Dmd
{
    /// <summary>
    /// An ordered collection of Frame objects
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Ordered collection of Frame objects
        /// </summary>
        public List<Frame> frames;

        /// <summary>
        /// Height of each of the animation frames in dots
        /// </summary>
        public int height = 0;

        /// <summary>
        /// Width of each of the animation frames in dots
        /// </summary>
        public int width = 0;
        /// <inheritdoc/>
        public Animation() => frames = new List<Frame>();

        /// <summary>
        /// Loads the given file from disk. The native animation format is the 'dmd-format' which
        /// can be created using the dmdconvert tool.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="allow_cache"></param>
        public Animation Load(string filename, bool allow_cache = true)
        {
            double t0 = Time.GetTime();
            // Load the file from disk
            if (filename.EndsWith(".dmd"))
            {
                // Load in from DMD file
                this.PopulateFromDmdFile(filename);
            }
            else
            {
                // Load from other image formats (TODO)
            }

            return this;
        }

        /// <summary>
        /// Reads file into frames
        /// </summary>
        /// <param name="filename"></param>
        /// <exception cref="Exception"></exception>
        public void PopulateFromDmdFile(string filename)
        {
            BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open));
            long file_length = br.BaseStream.Length;
            br.BaseStream.Seek(4, SeekOrigin.Begin); // Skip over the 4 byte DMD header
            int frame_count = br.ReadInt32();
            this.width = (int)br.ReadInt32();
            this.height = (int)br.ReadInt32();

            if (file_length != 16 + this.width * this.height * frame_count)
                throw new Exception("File size inconsistent with header information. Old or incompatible file format?");

            for (int frame_index = 0; frame_index < frame_count; frame_index++)
            {
                byte[] frame = br.ReadBytes((int)(this.width * this.height));
                Frame new_frame = new Frame(this.width, this.height);
                new_frame.SetData(frame);
                this.frames.Add(new_frame);
            }
        }

        /// <summary>
        /// Saves the animation as a .dmd file in the given filename
        /// </summary>
        public void Save(string filename)
        {
            if (this.width == 0 || this.height == 0)
                throw new Exception("Width and height must be set on an animation before it can be saved.");

            this.SaveToDmdFile(filename);
        }
        /// <summary>
        /// Saves frames to binary file
        /// </summary>
        /// <param name="filename"></param>
        public void SaveToDmdFile(string filename)
        {
            BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create));
            bw.Write(0x00646D64); // 4 byte DMD header
            bw.Write(this.frames.Count); // Frame count
            bw.Write((int)this.width); // Animation Width
            bw.Write((int)this.height); // Animation Height
            foreach (Frame f in this.frames)
                bw.Write(f.GetData());

            bw.Close();
        }
    }
}
