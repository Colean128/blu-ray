// Blu-Ray Discord Bot
//
// Copyright © 2020, The Blu-Ray authors 
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;

namespace Bot.Structures
{
    public class ColorDetection
    {
        public static DiscordColor GetMostUsedColor(string url)
        {
            WebClient client    = new WebClient();
            MemoryStream stream = new MemoryStream(client.DownloadData(url));
            Bitmap bmp          = new Bitmap(Image.FromStream(stream));

            List<int> colors = new List<int>();
            for (int row = 0; row < bmp.Size.Width; row++) for (int col = 0; col < bmp.Size.Height; col++)
            {
                Color c = bmp.GetPixel(row, col);
                if (c.R == 0 && c.G == 0 && c.B == 0 && c.A == 0) continue;

                colors.Add(c.ToArgb());
            }

            Dictionary<int, int> values = new Dictionary<int, int>();
            foreach (int x in colors) if (values.ContainsKey(x)) values[x]++; else values.Add(x, 1);

            Color color = Color.FromArgb(Convert.ToInt32(values.Where(x => x.Value == values.Values.Max()).FirstOrDefault().Key));

            bmp.Dispose();
            stream.Close();
            client.Dispose();

            return new DiscordColor(color.R, color.G, color.B);
        }
    }
}
