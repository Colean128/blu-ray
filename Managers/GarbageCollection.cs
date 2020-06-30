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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Threading;

namespace Bot.Managers
{
    public class GarbageCollection : IDisposable
    {
        private Thread thread;

        private void threadFunction()
        {
            while (true)
            {
                GC.Collect();
                Thread.Sleep(5 * 1000 * 60);
            }
        }

        public GarbageCollection()
        {
            thread = new Thread(threadFunction);
            thread.Start();
        }

        public void Dispose() => thread.Abort();
    }
}
