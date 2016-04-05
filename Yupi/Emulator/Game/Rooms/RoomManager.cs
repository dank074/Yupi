﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using Yupi.Emulator.Core.Io.Logger;
using Yupi.Emulator.Data.Base.Adapters.Interfaces;
using Yupi.Emulator.Game.Browser.Models;
using Yupi.Emulator.Game.Events;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Rooms.Competitions;
using Yupi.Emulator.Game.Rooms.Data;
using Yupi.Emulator.Game.Rooms.Data.Models;
using Yupi.Emulator.Game.Rooms.User;

namespace Yupi.Emulator.Game.Rooms
{
    /// <summary>
    ///     Class RoomManager.
    /// </summary>
     class RoomManager
    {
        /// <summary>
        ///     The _active rooms
        /// </summary>
        private readonly Dictionary<RoomData, uint> _activeRooms;

        /// <summary>
        ///     The _active rooms add queue
        /// </summary>
        private readonly Queue _activeRoomsAddQueue;

        /// <summary>
        ///     The _active rooms update queue
        /// </summary>
        private readonly Queue _activeRoomsUpdateQueue;

        /// <summary>
        ///     The _event manager
        /// </summary>
        private readonly EventManager _eventManager;

        /// <summary>
        ///     The _room models
        /// </summary>
        private readonly HybridDictionary _roomModels;

        /// <summary>
        ///     The _voted rooms
        /// </summary>
        private readonly Dictionary<RoomData, int> _votedRooms;

        /// <summary>
        ///     The _voted rooms add queue
        /// </summary>
        private readonly Queue _votedRoomsAddQueue;

        /// <summary>
        ///     The _voted rooms remove queue
        /// </summary>
        private readonly Queue _votedRoomsRemoveQueue;

         readonly ConcurrentDictionary<uint, RoomData> LoadedRoomData;

        private RoomCompetitionManager _competitionManager;

        /// <summary>
        ///     The _ordered active rooms
        /// </summary>
        private IEnumerable<KeyValuePair<RoomData, uint>> _orderedActiveRooms;

        /// <summary>
        ///     The _ordered voted rooms
        /// </summary>
        private IEnumerable<KeyValuePair<RoomData, int>> _orderedVotedRooms;

        /// <summary>
        ///     The active rooms remove queue
        /// </summary>
        public Queue ActiveRoomsRemoveQueue;

        /// <summary>
        ///     The loaded rooms
        /// </summary>
         ConcurrentDictionary<uint, Room> LoadedRooms;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RoomManager" /> class.
        /// </summary>
         RoomManager()
        {
            LoadedRooms = new ConcurrentDictionary<uint, Room>();
            _roomModels = new HybridDictionary();
            LoadedRoomData = new ConcurrentDictionary<uint, RoomData>();
            _votedRooms = new Dictionary<RoomData, int>();
            _activeRooms = new Dictionary<RoomData, uint>();
            _votedRoomsRemoveQueue = new Queue();
            _votedRoomsAddQueue = new Queue();
            ActiveRoomsRemoveQueue = new Queue();
            _activeRoomsUpdateQueue = new Queue();
            _activeRoomsAddQueue = new Queue();
            _eventManager = new EventManager();
        }

        /// <summary>
        ///     Gets the loaded rooms count.
        /// </summary>
        /// <value>The loaded rooms count.</value>
         int LoadedRoomsCount => LoadedRooms.Count;

         RoomCompetitionManager GetCompetitionManager() => _competitionManager;

         void LoadCompetitionManager() => _competitionManager = new RoomCompetitionManager();

        /// <summary>
        ///     Gets the active rooms.
        /// </summary>
        /// <returns>KeyValuePair&lt;GetPublicRoomData, System.UInt32&gt;[].</returns>
         KeyValuePair<RoomData, uint>[] GetActiveRooms() => _orderedActiveRooms?.ToArray();

        /// <summary>
        ///     Gets the voted rooms.
        /// </summary>
        /// <returns>KeyValuePair&lt;GetPublicRoomData, System.Int32&gt;[].</returns>
         KeyValuePair<RoomData, int>[] GetVotedRooms() => _orderedVotedRooms?.ToArray();

        /// <summary>
        ///     Gets the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>RoomModel.</returns>
         RoomModel GetModel(string model, uint roomId)
        {
            if (model == "custom" && _roomModels.Contains($"custom_{roomId}"))
                return (RoomModel) _roomModels[$"custom_{roomId}"];

            if (_roomModels.Contains(model))
                return (RoomModel) _roomModels[model];

            return null;
        }

        /// <summary>
        ///     Generates the nullable room data.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>GetPublicRoomData.</returns>
         RoomData GenerateNullableRoomData(uint roomId)
        {
            if (GenerateRoomData(roomId) != null)
                return GenerateRoomData(roomId);
            RoomData roomData = new RoomData();
            roomData.FillNull(roomId);
            return roomData;
        }

        private bool IsRoomLoaded(uint roomId)
        {
            return LoadedRooms.ContainsKey(roomId);
        }

        /// <summary>
        ///     Generates the room data.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>GetPublicRoomData.</returns>
         RoomData GenerateRoomData(uint roomId)
        {
            if (LoadedRoomData.ContainsKey(roomId))
            {
                LoadedRoomData[roomId].LastUsed = DateTime.Now;
                return LoadedRoomData[roomId];
            }

            if (IsRoomLoaded(roomId))
                return GetRoom(roomId).RoomData;

            RoomData roomData = new RoomData();
            using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery($"SELECT * FROM rooms_data WHERE id = {roomId} LIMIT 1");

                DataRow dataRow = queryReactor.GetRow();
                if (dataRow == null)
                    return null;

                roomData.Fill(dataRow);
                LoadedRoomData.TryAdd(roomId, roomData);
            }

            return roomData;
        }

        /// <summary>
        ///     Gets the event rooms.
        /// </summary>
        /// <returns>KeyValuePair&lt;GetPublicRoomData, System.UInt32&gt;[].</returns>
         KeyValuePair<RoomData, uint>[] GetEventRooms()
        {
            return _eventManager.GetRooms();
        }

        /// <summary>
        ///     Loads the room.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="forceLoad"></param>
        /// <returns>Room.</returns>
         Room LoadRoom(uint id, bool forceLoad = false)
        {
            if (IsRoomLoaded(id))
                return GetRoom(id);

            RoomData roomData = GenerateRoomData(id);

            if (roomData == null)
                return null;

            Room room = new Room();

            LoadedRooms.AddOrUpdate(id, room, (key, value) => room);

            room.Start(roomData, forceLoad);

            YupiWriterManager.WriteLine($"Room #{id} was loaded", "Yupi.Room", ConsoleColor.DarkCyan);

            room.InitBots();
            room.InitPets();
            return room;
        }

         void RemoveRoomData(uint id)
        {
            RoomData dataJunk;
            LoadedRoomData.TryRemove(id, out dataJunk);
        }

        /// <summary>
        ///     Fetches the room data.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="dRow">The d row.</param>
        /// <returns>GetPublicRoomData.</returns>
         RoomData FetchRoomData(uint roomId, DataRow dRow)
        {
            if (LoadedRoomData.ContainsKey(roomId))
            {
                LoadedRoomData[roomId].LastUsed = DateTime.Now;

                return LoadedRoomData[roomId];
            }

            RoomData roomData = new RoomData();
            roomData.Fill(dRow);
            LoadedRoomData.TryAdd(roomId, roomData);

            return roomData;
        }

        /// <summary>
        ///     Gets the room.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <returns>Room.</returns>
         Room GetRoom(uint roomId)
        {
            Room result;

            return LoadedRooms.TryGetValue(roomId, out result) ? result : null;
        }

        /// <summary>
        ///     Creates the room.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="name">The name.</param>
        /// <param name="desc">The desc.</param>
        /// <param name="model">The model.</param>
        /// <param name="category">The category.</param>
        /// <param name="maxVisitors">The maximum visitors.</param>
        /// <param name="tradeState">State of the trade.</param>
        /// <returns>GetPublicRoomData.</returns>
         RoomData CreateRoom(GameClient session, string name, string desc, string model, int category,
            int maxVisitors, int tradeState)
        {
            if (!_roomModels.Contains(model))
            {
                session.SendNotif(Yupi.GetLanguage().GetVar("user_room_model_error"));

                return null;
            }

            uint roomId;

            using (IQueryAdapter dbClient = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery(
                    "INSERT INTO rooms_data (roomtype,caption,description,owner,model_name,category,users_max,trade_state) VALUES ('private',@caption,@desc,@UserId,@model,@cat,@usmax,@tstate)");
                dbClient.AddParameter("caption", name);
                dbClient.AddParameter("desc", desc);
                dbClient.AddParameter("UserId", session.GetHabbo().Id);
                dbClient.AddParameter("model", model);
                dbClient.AddParameter("cat", category);
                dbClient.AddParameter("usmax", maxVisitors);
                dbClient.AddParameter("tstate", tradeState.ToString());
                roomId = (uint) dbClient.InsertQuery();
            }

            RoomData data = GenerateRoomData(roomId);

            if (data == null)
                return null;

            session.GetHabbo().UsersRooms.Add(data);

            return data;
        }

        /// <summary>
        ///     Initializes the voted rooms.
        /// </summary>
         void InitVotedRooms()
        {
            using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery(
                    "SELECT * FROM rooms_data WHERE score > 0 AND roomtype = 'private' ORDER BY score DESC LIMIT 40");

                DataTable table = queryReactor.GetTable();

                foreach (
                    RoomData data in
                        from DataRow dataRow in table.Rows
                        select FetchRoomData(Convert.ToUInt32(dataRow["id"]), dataRow))
                    QueueVoteAdd(data);
            }
        }

        /// <summary>
        ///     Loads the models.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
        /// <param name="loadedModel">The loaded model.</param>
         void LoadModels(IQueryAdapter dbClient, out uint loadedModel)
        {
            LoadModels(dbClient);
            loadedModel = (uint) _roomModels.Count;
        }

        /// <summary>
        ///     Loads the models.
        /// </summary>
        /// <param name="dbClient">The database client.</param>
         void LoadModels(IQueryAdapter dbClient)
        {
            _roomModels.Clear();

            dbClient.SetQuery("SELECT * FROM rooms_models");
            DataTable table = dbClient.GetTable();

            if (table == null)
                return;

            foreach (DataRow dataRow in table.Rows)
            {
                string key = (string) dataRow["id"];

                if (key.StartsWith("model_floorplan_"))
                    continue;

                string staticFurniMap = (string) dataRow["public_items"];

                _roomModels.Add(key,
                    new RoomModel((int) dataRow["door_x"], (int) dataRow["door_y"], (double) dataRow["door_z"],
                        (int) dataRow["door_dir"], (string) dataRow["heightmap"], staticFurniMap,
                        Yupi.EnumToBool(dataRow["club_only"].ToString()), (string) dataRow["poolmap"]));
            }

            dbClient.SetQuery("SELECT * FROM rooms_models_customs");

            DataTable dataCustom = dbClient.GetTable();

            if (dataCustom == null) return;

            foreach (DataRow row in dataCustom.Rows)
            {
                string modelName = $"custom_{row["roomid"]}";
                _roomModels.Add(modelName,
                    new RoomModel((int) row["door_x"], (int) row["door_y"], (double) row["door_z"],
                        (int) row["door_dir"],
                        (string) row["heightmap"], "", false, ""));
            }
        }

        /// <summary>
        ///     Update the existent model.
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="modelData"></param>
         void UpdateCustomModel(uint roomId, RoomModel modelData)
        {
            string modelId = $"custom_{roomId}";

            if (_roomModels.Contains(modelId))
                _roomModels[modelId] = modelData;
            else
                _roomModels.Add(modelId, modelData);
        }

        /// <summary>
        ///     Called when [cycle].
        /// </summary>
         void OnCycle()
        {
            try
            {
                bool flag = WorkActiveRoomsAddQueue();
                bool flag2 = WorkActiveRoomsRemoveQueue();
                bool flag3 = WorkActiveRoomsUpdateQueue();

                if (flag || flag2 || flag3)
                    SortActiveRooms();

                bool flag4 = WorkVotedRoomsAddQueue();
                bool flag5 = WorkVotedRoomsRemoveQueue();

                if (flag4 || flag5)
                    SortVotedRooms();

                Yupi.GetGame().RoomManagerCycleEnded = true;
            }
            catch (Exception ex)
            {
                YupiLogManager.LogException(ex, "Registered Room YupiDatabaseManager Crashing.");
            }
        }

        /// <summary>
        ///     Queues the vote add.
        /// </summary>
        /// <param name="data">The data.</param>
         void QueueVoteAdd(RoomData data)
        {
            lock (_votedRoomsAddQueue.SyncRoot)
            {
                _votedRoomsAddQueue.Enqueue(data);
            }
        }

        /// <summary>
        ///     Queues the vote remove.
        /// </summary>
        /// <param name="data">The data.</param>
         void QueueVoteRemove(RoomData data)
        {
            lock (_votedRoomsRemoveQueue.SyncRoot)
            {
                _votedRoomsRemoveQueue.Enqueue(data);
            }
        }

        /// <summary>
        ///     Queues the active room update.
        /// </summary>
        /// <param name="data">The data.</param>
         void QueueActiveRoomUpdate(RoomData data)
        {
            lock (_activeRoomsUpdateQueue.SyncRoot)
                _activeRoomsUpdateQueue.Enqueue(data);
        }

        /// <summary>
        ///     Queues the active room add.
        /// </summary>
        /// <param name="data">The data.</param>
         void QueueActiveRoomAdd(RoomData data)
        {
            lock (_activeRoomsAddQueue.SyncRoot)
            {
                _activeRoomsAddQueue.Enqueue(data);
            }
        }

        /// <summary>
        ///     Queues the active room remove.
        /// </summary>
        /// <param name="data">The data.</param>
         void QueueActiveRoomRemove(RoomData data)
        {
            lock (ActiveRoomsRemoveQueue.SyncRoot)
            {
                ActiveRoomsRemoveQueue.Enqueue(data);
            }
        }

        /// <summary>
        ///     Removes all rooms.
        /// </summary>
         void RemoveAllRooms()
        {
            lock (LoadedRooms)
            {
                foreach (Room current in LoadedRooms.Values)
                    Yupi.GetGame().GetRoomManager().UnloadRoom(current, "RemoveAllRooms void called");
            }

            YupiWriterManager.WriteLine("RoomManager Destroyed", "Yupi.Room", ConsoleColor.DarkYellow);
        }

        /// <summary>
        ///     Unloads the room.
        /// </summary>
        /// <param name="room">The room.</param>
        /// <param name="reason">The reason.</param>
         void UnloadRoom(Room room, string reason)
        {
            if (room?.RoomData == null || room.Disposed)
                return;

            room.Disposed = true;

            if (Yupi.GetGame().GetNavigator().PrivateCategories.Contains(room.RoomData.Category))
                ((PublicCategory) Yupi.GetGame().GetNavigator().PrivateCategories[room.RoomData.Category]).UsersNow -=
                    room.UserCount;

            room.RoomData.UsersNow = 0;

            string state = "open";

            if (room.RoomData.State == 1)
                state = "locked";
            else if (room.RoomData.State > 1)
                state = "password";

            uint roomId = room.RoomId;

            using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("UPDATE rooms_data SET " +
                                      "caption = @roomcaption," +
                                      "description = @description," +
                                      "password = @password," +
                                      "category = @category," +
                                      "state = @state," +
                                      "tags = @tags," +
                                      "users_now = @usersnow," +
                                      "users_max = @usersmax," +
                                      "allow_pets = @allowpets," +
                                      "allow_pets_eat = @allowpetseat," +
                                      "allow_walkthrough = @allowwalk," +
                                      "hidewall = @hidewall," +
                                      "floorthick = @floorthick," +
                                      "wallthick = @wallthick," +
                                      "mute_settings = @whocanmute," +
                                      "kick_settings = @kicksettings," +
                                      "ban_settings = @bansettings," +
                                      "walls_height = @wallheight," +
                                      "chat_type = @chat_t," +
                                      "chat_balloon = @chat_b," +
                                      "chat_speed = @chat_s," +
                                      "chat_max_distance = @chat_m," +
                                      "chat_flood_protection = @chat_f," +
                                      "trade_state = @tradestate " +
                                      "WHERE id = " + roomId);

                queryReactor.AddParameter("usersnow", room.RoomData.UsersNow);
                queryReactor.AddParameter("roomcaption", room.RoomData.Name);
                queryReactor.AddParameter("usersmax", room.RoomData.UsersMax);
                queryReactor.AddParameter("allowpets", Yupi.BoolToEnum(room.RoomData.AllowPets));
                queryReactor.AddParameter("allowpetseat", Yupi.BoolToEnum(room.RoomData.AllowPetsEating));
                queryReactor.AddParameter("allowwalk", Yupi.BoolToEnum(room.RoomData.AllowWalkThrough));
                queryReactor.AddParameter("hidewall", Yupi.BoolToEnum(room.RoomData.HideWall));
                queryReactor.AddParameter("floorthick", room.RoomData.FloorThickness);
                queryReactor.AddParameter("wallthick", room.RoomData.WallThickness);
                queryReactor.AddParameter("whocanmute", room.RoomData.WhoCanMute);
                queryReactor.AddParameter("kicksettings", room.RoomData.WhoCanKick);
                queryReactor.AddParameter("bansettings", room.RoomData.WhoCanBan);
                queryReactor.AddParameter("wallheight", room.RoomData.WallHeight);
                queryReactor.AddParameter("tradestate", room.RoomData.TradeState);
                queryReactor.AddParameter("category", room.RoomData.Category);
                queryReactor.AddParameter("state", state);
                queryReactor.AddParameter("description", room.RoomData.Description);
                queryReactor.AddParameter("password", room.RoomData.PassWord);
                queryReactor.AddParameter("tags", string.Join(",", room.RoomData.Tags));
                queryReactor.AddParameter("chat_t", room.RoomData.ChatType);
                queryReactor.AddParameter("chat_b", room.RoomData.ChatBalloon);
                queryReactor.AddParameter("chat_s", room.RoomData.ChatSpeed);
                queryReactor.AddParameter("chat_m", room.RoomData.ChatMaxDistance);
                queryReactor.AddParameter("chat_f", room.RoomData.ChatFloodProtection);

                queryReactor.RunQuery();
            }

            if (room.GetRoomUserManager() != null && room.GetRoomUserManager().UserList != null)
            {
                foreach (RoomUser current in room.GetRoomUserManager().UserList.Values.Where(current => current != null))
                {
                    if (current.IsPet)
                    {
                        if (current.PetData == null)
                            continue;

                        using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
                        {
                            queryReactor.SetQuery("UPDATE pets_data SET x=@x, y=@y, z=@z WHERE id=@id LIMIT 1");
                            queryReactor.AddParameter("x", current.X);
                            queryReactor.AddParameter("y", current.Y);
                            queryReactor.AddParameter("z", current.Z);
                            queryReactor.AddParameter("id", current.PetData.PetId);
                            queryReactor.RunQuery();
                        }

                        if (current.BotAi == null)
                            continue;

                        current.BotAi.Dispose();
                    }
                    else if (current.IsBot)
                    {
                        if (current.BotData == null)
                            continue;

                        using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor())
                        {
                            queryReactor.SetQuery(
                                "UPDATE bots_data SET x=@x, y=@y, z=@z, name=@name, motto=@motto, look=@look, rotation=@rotation, dance=@dance WHERE id=@id LIMIT 1");
                            queryReactor.AddParameter("name", current.BotData.Name);
                            queryReactor.AddParameter("motto", current.BotData.Motto);
                            queryReactor.AddParameter("look", current.BotData.Look);
                            queryReactor.AddParameter("rotation", current.BotData.Rot);
                            queryReactor.AddParameter("dance", current.BotData.DanceId);
                            queryReactor.AddParameter("x", current.X);
                            queryReactor.AddParameter("y", current.Y);
                            queryReactor.AddParameter("z", current.Z);
                            queryReactor.AddParameter("id", current.BotData.BotId);
                            queryReactor.RunQuery();
                        }

                        current.BotAi?.Dispose();
                    }
                    else
                    {
                        if (current.GetClient() != null)
                        {
                            room.GetRoomUserManager().RemoveUserFromRoom(current.GetClient(), true, false);

                            current.GetClient().CurrentRoomUserId = -1;
                        }
                    }
                }
            }

            room.SaveRoomChatlog();

            Room junkRoom;

            LoadedRooms.TryRemove(room.RoomId, out junkRoom);

            YupiWriterManager.WriteLine(string.Format("Room #{0} was unloaded, reason: " + reason, room.RoomId),
                "Yupi.Room", ConsoleColor.DarkGray);

            room.Destroy();
        }

        /// <summary>
        ///     Sorts the active rooms.
        /// </summary>
        private void SortActiveRooms()
        {
            _orderedActiveRooms = _activeRooms.OrderByDescending(t => t.Value).Take(40);
        }

        /// <summary>
        ///     Sorts the voted rooms.
        /// </summary>
        private void SortVotedRooms()
        {
            _orderedVotedRooms = _votedRooms.OrderByDescending(t => t.Value).Take(40);
        }

        /// <summary>
        ///     Works the active rooms update queue.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool WorkActiveRoomsUpdateQueue()
        {
            if (_activeRoomsUpdateQueue.Count <= 0) return false;
            lock (_activeRoomsUpdateQueue.SyncRoot)
            {
                while (_activeRoomsUpdateQueue.Count > 0)
                {
                    RoomData roomData = (RoomData) _activeRoomsUpdateQueue.Dequeue();
                    if (roomData == null || roomData.ModelName.Contains("snowwar")) continue;
                    if (!_activeRooms.ContainsKey(roomData)) _activeRooms.Add(roomData, roomData.UsersNow);
                    else _activeRooms[roomData] = roomData.UsersNow;
                }
            }
            return true;
        }

        /// <summary>
        ///     Works the active rooms add queue.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool WorkActiveRoomsAddQueue()
        {
            if (_activeRoomsAddQueue.Count <= 0) return false;
            lock (_activeRoomsAddQueue.SyncRoot)
            {
                while (_activeRoomsAddQueue.Count > 0)
                {
                    RoomData roomData = (RoomData) _activeRoomsAddQueue.Dequeue();
                    if (!_activeRooms.ContainsKey(roomData) && !roomData.ModelName.Contains("snowwar"))
                        _activeRooms.Add(roomData, roomData.UsersNow);
                }
            }
            return true;
        }

        /// <summary>
        ///     Works the active rooms remove queue.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool WorkActiveRoomsRemoveQueue()
        {
            if (ActiveRoomsRemoveQueue.Count <= 0) return false;
            lock (ActiveRoomsRemoveQueue.SyncRoot)
            {
                while (ActiveRoomsRemoveQueue.Count > 0)
                {
                    RoomData key = (RoomData) ActiveRoomsRemoveQueue.Dequeue();
                    _activeRooms.Remove(key);
                }
            }
            return true;
        }

        /// <summary>
        ///     Works the voted rooms add queue.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool WorkVotedRoomsAddQueue()
        {
            if (_votedRoomsAddQueue.Count <= 0) return false;
            lock (_votedRoomsAddQueue.SyncRoot)
            {
                while (_votedRoomsAddQueue.Count > 0)
                {
                    RoomData roomData = (RoomData) _votedRoomsAddQueue.Dequeue();
                    if (!_votedRooms.ContainsKey(roomData)) _votedRooms.Add(roomData, roomData.Score);
                    else _votedRooms[roomData] = roomData.Score;
                }
            }
            return true;
        }

        /// <summary>
        ///     Works the voted rooms remove queue.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool WorkVotedRoomsRemoveQueue()
        {
            if (_votedRoomsRemoveQueue.Count <= 0) return false;
            lock (_votedRoomsRemoveQueue.SyncRoot)
            {
                while (_votedRoomsRemoveQueue.Count > 0)
                {
                    RoomData key = (RoomData) _votedRoomsRemoveQueue.Dequeue();
                    _votedRooms.Remove(key);
                }
            }
            return true;
        }
    }
}