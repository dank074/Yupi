using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Yupi.Emulator.Data.Base.Adapters.Interfaces;
using Yupi.Emulator.Game.SoundMachine.Songs;

namespace Yupi.Emulator.Game.SoundMachine
{
    /// <summary>
    ///     Class SongManager.
    /// </summary>
     class SoundMachineSongManager
    {
        /// <summary>
        ///     The songs
        /// </summary>
         static Dictionary<uint, SongData> Songs;

        /// <summary>
        ///     The _cache timer
        /// </summary>
        private static Dictionary<uint, double> _cacheTimer;

        /// <summary>
        ///     Gets the song identifier.
        /// </summary>
        /// <param name="codeName">Name of the code.</param>
        /// <returns>System.UInt32.</returns>
         static uint GetSongId(string codeName)
            => (from current in Songs.Values where current.CodeName == codeName select current.Id).FirstOrDefault();

        /// <summary>
        ///     Gets the song.
        /// </summary>
        /// <param name="codeName">Name of the code.</param>
        /// <returns>SongData.</returns>
         static SongData GetSong(string codeName)
            => Songs.Values.FirstOrDefault(current => current.CodeName == codeName);

        /// <summary>
        ///     Gets the song by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>SongData.</returns>
         static SongData GetSongById(uint id) => Songs.Values.FirstOrDefault(current => current.Id == id);

        /// <summary>
        ///     Gets the code by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>String.</returns>
         static string GetCodeById(uint id)
            => (from current in Songs.Values where current.Id == id select current.CodeName).FirstOrDefault();

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
         static void Load()
        {
            Songs = new Dictionary<uint, SongData>();
            _cacheTimer = new Dictionary<uint, double>();

            Songs.Clear();

            using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("SELECT * FROM items_songs_data ORDER BY id");
                DataTable table = queryReactor.GetTable();

                foreach (SongData songFromDataRow in from DataRow dRow in table.Rows select GetSongFromDataRow(dRow))
                    Songs.Add(songFromDataRow.Id, songFromDataRow);
            }
        }

        /// <summary>
        ///     Processes the thread.
        /// </summary>
         static void ProcessThread()
        {
            double num = Yupi.GetUnixTimeStamp();

            List<uint> list = (from current in _cacheTimer where num - current.Value >= 180.0 select current.Key).ToList();

            foreach (uint current2 in list)
            {
                Songs.Remove(current2);
                _cacheTimer.Remove(current2);
            }
        }

        /// <summary>
        ///     Gets the song from data row.
        /// </summary>
        /// <param name="dRow">The d row.</param>
        /// <returns>SongData.</returns>
         static SongData GetSongFromDataRow(DataRow dRow)
            =>
                new SongData(Convert.ToUInt32(dRow["id"]), dRow["codename"].ToString(), (string) dRow["name"],
                    (string) dRow["artist"], (string) dRow["song_data"], (double) dRow["length"]);

        /// <summary>
        ///     Gets the song.
        /// </summary>
        /// <param name="songId">The song identifier.</param>
        /// <returns>SongData.</returns>
         static SongData GetSong(uint songId)
        {
            SongData result;

            Songs.TryGetValue(songId, out result);

            return result;
        }
    }
}