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

namespace Bot.Managers
{
    public class ColorDetection
    {
        public static DiscordColor GetMostUsedColor(string url)
        {
            WebClient client    = new WebClient();
            MemoryStream stream = new MemoryStream(client.DownloadData(url));
            Bitmap bmp          = new Bitmap(Image.FromStream(stream));

            List<int> colors = new List<int>();
            for (int row = 0; row < bmp.Size.Width; row++) for (int col = 0; col < bmp.Size.Height; col++) colors.Add(bmp.GetPixel(row, col).ToArgb());

            Color color = Color.FromArgb(Convert.ToInt32(colors.Average()));

            bmp.Dispose();
            stream.Close();
            client.Dispose();

            return new DiscordColor(color.R, color.G, color.B);
        }
    }
}
