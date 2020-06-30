using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace Bot.Managers
{
    public class Database
    {
        private static SqliteConnection sqlite;

        public static async Task ConnectAsync()
        {
            sqlite = new SqliteConnection("Data Source=blu-ray.db");
            await sqlite.OpenAsync();
        }

        public static void Disconnect()
        {
            sqlite.Close();
            sqlite.Dispose();
        }
    }
}
