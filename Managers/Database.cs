﻿// Blu-Ray Discord Bot
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

using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class Database
    {
        private static SqliteConnection sqlite;

        public static async Task ConnectAsync(string databasePath)
        {
            sqlite = new SqliteConnection($"Data Source={databasePath}");
            await sqlite.OpenAsync();
        }

        public static void Disconnect()
        {
            sqlite.Close();
            sqlite.Dispose();
        }

        public static SqliteCommand CreateCommand() => sqlite.CreateCommand();
    }
}
