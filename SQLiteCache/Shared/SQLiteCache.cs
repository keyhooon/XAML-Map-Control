// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// � 2019 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Data.SQLite;
using System.Diagnostics;

namespace MapControl.Caching
{
    /// <summary>
    /// Image cache implementation based on SqLite.
    /// </summary>
    public sealed partial class SQLiteCache : IDisposable
    {
        private readonly SQLiteConnection connection;

        private static SQLiteConnection Open(string path)
        {
            var connection = new SQLiteConnection("Data Source=" + path);
            connection.Open();

            using (var command = new SQLiteCommand("create table if not exists items (key text primary key, expiration integer, buffer blob)", connection))
            {
                command.ExecuteNonQuery();
            }

            Debug.WriteLine("SQLiteCache: Opened database " + path);

            return connection;
        }

        public void Dispose()
        {
            connection.Dispose();
        }

        public void Clean()
        {
            using (var command = new SQLiteCommand("delete from items where expiration < @exp", connection))
            {
                command.Parameters.AddWithValue("@exp", DateTime.UtcNow.Ticks);
                command.ExecuteNonQuery();
            }
#if DEBUG
            using (var command = new SQLiteCommand("select changes()", connection))
            {
                var deleted = (long)command.ExecuteScalar();
                if (deleted > 0)
                {
                    Debug.WriteLine("SQLiteCache: Deleted {0} expired items.", deleted);
                }
            }
#endif
        }

        private SQLiteCommand RemoveItemCommand(string key)
        {
            var command = new SQLiteCommand("delete from items where key = @key", connection);
            command.Parameters.AddWithValue("@key", key);
            return command;
        }

        private SQLiteCommand GetItemCommand(string key)
        {
            var command = new SQLiteCommand("select expiration, buffer from items where key = @key", connection);
            command.Parameters.AddWithValue("@key", key);
            return command;
        }

        private SQLiteCommand SetItemCommand(string key, DateTime expiration, byte[] buffer)
        {
            var command = new SQLiteCommand("insert or replace into items (key, expiration, buffer) values (@key, @exp, @buf)", connection);
            command.Parameters.AddWithValue("@key", key);
            command.Parameters.AddWithValue("@exp", expiration.Ticks);
            command.Parameters.AddWithValue("@buf", buffer ?? new byte[0]);
            return command;
        }
    }
}
