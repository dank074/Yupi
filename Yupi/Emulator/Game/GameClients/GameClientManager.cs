﻿/**
     Because i love chocolat...                                      
                                    88 88  
                                    "" 88  
                                       88  
8b       d8 88       88 8b,dPPYba,  88 88  
`8b     d8' 88       88 88P'    "8a 88 88  
 `8b   d8'  88       88 88       d8 88 ""  
  `8b,d8'   "8a,   ,a88 88b,   ,a8" 88 aa  
    Y88'     `"YbbdP'Y8 88`YbbdP"'  88 88  
    d8'                 88                 
   d8'                  88     
   
   Private Habbo Hotel Emulating System
   @author Claudio A. Santoro W.
   @author Kessiler R.
   @version dev-beta
   @license MIT
   @copyright Sulake Corporation Oy
   @observation All Rights of Habbo, Habbo Hotel, and all Habbo contents and it's names, is copyright from Sulake
   Corporation Oy. Yupi! has nothing linked with Sulake. 
   This Emulator is Only for DEVELOPMENT uses. If you're selling this you're violating Sulakes Copyright.
*/

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Yupi.Emulator.Core.Io.Logger;
using Yupi.Emulator.Data.Base.Adapters.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Users.Messenger.Structs;
using Yupi.Emulator.Messages;
using Yupi.Emulator.Messages.Buffers;
using Yupi.Net;

namespace Yupi.Emulator.Game.GameClients
{
    /// <summary>
    ///     Class GameClientManager..
    /// </summary>
     class GameClientManager
    {
        /// <summary>
        ///     The _badge Queue
        /// </summary>
        private readonly Queue _badgeQueue;

        /// <summary>
        ///     The _broadcast Queue
        /// </summary>
        private readonly ConcurrentQueue<byte[]> _broadcastQueue;

        /// <summary>
        ///     The _id user name register
        /// </summary>
        private readonly HybridDictionary _idUserNameRegister;

        /// <summary>
        ///     The _user identifier register
        /// </summary>
        private readonly HybridDictionary _userIdRegister;

        /// <summary>
        ///     The _user name identifier register
        /// </summary>
        private readonly HybridDictionary _userNameIdRegister;

        /// <summary>
        ///     The _user name register
        /// </summary>
        private readonly HybridDictionary _userNameRegister;

        /// <summary>
        ///     The clients
        /// </summary>
		 ConcurrentDictionary<ISession, GameClient> Clients;
		// TODO Keep reference in Yupi.Net

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameClientManager" /> class.
        /// </summary>
         GameClientManager()
        {
			Clients = new ConcurrentDictionary<ISession, GameClient>();
            
            _badgeQueue = new Queue();
            _broadcastQueue = new ConcurrentQueue<byte[]>();
            
            _userNameRegister = new HybridDictionary();
            _userIdRegister = new HybridDictionary();
            _userNameIdRegister = new HybridDictionary();
            _idUserNameRegister = new HybridDictionary();
        }

        /// <summary>
        ///     Gets the client count.
        /// </summary>
        /// <value>The client count.</value>
         int ClientCount() => Clients.Count;

        /// <summary>
        ///     Gets the client by user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>GameClient.</returns>
         GameClient GetClientByUserId(uint userId) => _userIdRegister.Contains(userId) ? (GameClient) _userIdRegister[userId] : null;

		public GameClient GetClient(ISession session) {
			GameClient client;
			Clients.TryGetValue (session, out client);
			return client;
		}

        /// <summary>
        ///     Gets the name of the client by user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>GameClient.</returns>
         GameClient GetClientByUserName(string userName) => _userNameRegister.Contains(userName.ToLower()) ? (GameClient) _userNameRegister[userName.ToLower()] : null;

        /// <summary>
        ///     Return Online Clients Count
        /// </summary>
        /// <returns>Online Client Count.</returns>
		 int GetOnlineClients() {
			return Clients.Values.Count (client => {
				bool? isOnline = client?.GetHabbo ()?.IsOnline;
				return isOnline.HasValue && (bool)isOnline;
			});
		}

        /// <summary>
        ///     Gets the name by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.String.</returns>
         string GetUserNameByUserId(uint id) => Yupi.GetHabboById(id)?.UserName;

        /// <summary>
        ///     Gets the clients by identifier.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <returns>IEnumerable&lt;GameClient&gt;.</returns>
         IEnumerable<GameClient> GetClientsByUserIds(Dictionary<uint, MessengerBuddy>.KeyCollection users) => users.Select(GetClientByUserId).Where(clientByUserId => clientByUserId != null);

        /// <summary>
        ///     Sends the super notif.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="notice">The notice.</param>
        /// <param name="picture">The picture.</param>
        /// <param name="client">The client.</param>
        /// <param name="link">The link.</param>
        /// <param name="linkTitle">The link title.</param>
        /// <param name="broadCast">if set to <c>true</c> [broad cast].</param>
        /// <param name="Event">if set to <c>true</c> [event].</param>
         void SendSuperNotif(string title, string notice, string picture, GameClient client, string link, string linkTitle, bool broadCast, bool Event)
        {
            SimpleServerMessageBuffer simpleServerMessageBuffer = new SimpleServerMessageBuffer(PacketLibraryManager.OutgoingHandler("SuperNotificationMessageComposer"));

            simpleServerMessageBuffer.AppendString(picture);
            simpleServerMessageBuffer.AppendInteger(4);
            simpleServerMessageBuffer.AppendString("title");
            simpleServerMessageBuffer.AppendString(title);
            simpleServerMessageBuffer.AppendString("message");

            simpleServerMessageBuffer.AppendString(broadCast ? (Event ? $"<b>{Yupi.GetLanguage().GetVar("ha_event_one")} {client.GetHabbo().CurrentRoom.RoomData.Owner}!</b>\r\n {Yupi.GetLanguage().GetVar("ha_event_two")} .\r\n<b>{Yupi.GetLanguage().GetVar("ha_event_three")}</b>\r\n{notice}" : $"<b>{Yupi.GetLanguage().GetVar("ha_title")}</b>\r\n{notice}\r\n- <i>{client.GetHabbo().UserName}</i>") : notice);

            if (!string.IsNullOrWhiteSpace(link))
            {
                simpleServerMessageBuffer.AppendString("linkUrl");
                simpleServerMessageBuffer.AppendString(link);
                simpleServerMessageBuffer.AppendString("linkTitle");
                simpleServerMessageBuffer.AppendString(linkTitle);
            }
            else
            {
                simpleServerMessageBuffer.AppendString("linkUrl");
                simpleServerMessageBuffer.AppendString("event:");
                simpleServerMessageBuffer.AppendString("linkTitle");
                simpleServerMessageBuffer.AppendString("ok");
            }

            if (broadCast)
            {
                QueueBroadcaseMessage(simpleServerMessageBuffer);

                return;
            }

            client.SendMessage(simpleServerMessageBuffer);
        }

        /// <summary>
        ///     Called when [cycle].
        /// </summary>
         void OnCycle()
        {
            try
            {
                GiveBadges();

                BroadcastPackets();

                Yupi.GetGame().ClientManagerCycleEnded = true;
            }
            catch (Exception ex)
            {
                YupiLogManager.LogException(ex, "Registered HabboHotel Thread Exception.");
            }
        }

        /// <summary>
        ///     Staffs the alert.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exclude">The exclude.</param>
         void StaffAlert(SimpleServerMessageBuffer message, uint exclude = 0u)
        {
            foreach (GameClient current in Clients.Values.Where(x => x.GetHabbo() != null && x.GetHabbo().Rank >= Yupi.StaffAlertMinRank && x.GetHabbo().Id != exclude))
                current.SendMessage(message);
        }

        /// <summary>
        ///     Ambassador the alert.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exclude">The exclude.</param>
         void AmbassadorAlert(SimpleServerMessageBuffer message, uint exclude = 0u)
        {
            foreach (GameClient current in Clients.Values.Where(x => x.GetHabbo() != null && x.GetHabbo().Rank >= Convert.ToUInt32(Yupi.GetDbConfig().DbData["ambassador.minrank"]) && x.GetHabbo().Id != exclude))
                current.SendMessage(message);
        }

        /// <summary>
        ///     Mods the alert.
        /// </summary>
        /// <param name="message">The message.</param>
         void ModAlert(SimpleServerMessageBuffer message)
        {
            byte[] bytes = message.GetReversedBytes();

            foreach (GameClient current in Clients.Values.Where(current => current?.GetHabbo() != null).Where(current => current.GetHabbo().Rank >= 4u))
                current.GetConnection().Send(bytes);
        }

        /// <summary>
        ///     Creates the and start client.
        /// </summary>
        /// <param name="clientAddress">The client identifier.</param>
        /// <param name="connection">The connection.</param>
         void AddClient(ISession connection)
        {
            GameClient gameClient = new GameClient(connection);

			Clients.TryAdd(connection, gameClient);
        }

        /// <summary>
        ///     Disposes the connection.
        /// </summary>
        /// <param name="clientAddress">The client identifier.</param>
		 void RemoveClient(ISession connection)
        {
            GameClient client;
			Clients.TryRemove(connection, out client);   
        }

        /// <summary>
        ///     Queues the broadcase message.
        /// </summary>
        /// <param name="message">The message.</param>
         void QueueBroadcaseMessage(SimpleServerMessageBuffer message) => _broadcastQueue.Enqueue(message.GetReversedBytes());

        /// <summary>
        ///     Logs the clones out.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
         void LogClonesOut(uint userId)
        {
            GameClient clientByUserId = GetClientByUserId(userId);

            clientByUserId?.Disconnect("User is Clone.");
        }

        /// <summary>
        ///     Registers the client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="userName">Name of the user.</param>
         void RegisterClient(GameClient client, uint userId, string userName)
        {
            if (_userNameRegister.Contains(userName.ToLower()))
                _userNameRegister[userName.ToLower()] = client;
            else
                _userNameRegister.Add(userName.ToLower(), client);

            if (_userIdRegister.Contains(userId))
                _userIdRegister[userId] = client;
            else
                _userIdRegister.Add(userId, client);

            if (!_userNameIdRegister.Contains(userName))
                _userNameIdRegister.Add(userName, userId);

            if (!_idUserNameRegister.Contains(userId))
                _idUserNameRegister.Add(userId, userName);

			using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor()) 
			{
                queryReactor.SetQuery("UPDATE users SET online='1' WHERE id = @user");
				queryReactor.AddParameter("user", userId);
				queryReactor.RunQuery ();
			}
        }

        /// <summary>
        ///     Unregisters the client.
        /// </summary>
        /// <param name="userid">The userid.</param>
        /// <param name="userName">The username.</param>
         void UnregisterClient(uint userid, string userName)
        {
            _userIdRegister.Remove(userid);
            _userNameRegister.Remove(userName.ToLower());

			using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor()) 
			{
                queryReactor.SetQuery("UPDATE users SET online = '0' WHERE id = @user LIMIT 1");
				queryReactor.AddParameter("user", userid);
				queryReactor.RunQuery ();
			}
        }

        /// <summary>
        ///     Closes all.
        /// </summary>
         void CloseAll()
        {
            YupiWriterManager.WriteLine("Saving Inventary Content....", "Yupi.Data", ConsoleColor.DarkCyan);

            foreach (GameClient current2 in Clients.Values.Where(current2 => current2?.GetHabbo() != null))
                current2.GetHabbo().GetInventoryComponent().RunDbUpdate();

            foreach (GameClient current3 in Clients.Values.Where(current3 => current3?.GetHabbo() != null))
                current3.GetHabbo().RunDbUpdate();

            YupiWriterManager.WriteLine("Inventary Content Saved!", "Yupi.Data", ConsoleColor.DarkCyan);

            YupiWriterManager.WriteLine("Closing YupiDatabase Manager...", "Yupi.Data", ConsoleColor.DarkMagenta);

            foreach (GameClient current4 in Clients.Values.Where(current4 => current4?.GetConnection() != null))
                current4.Disconnect("Server Shutdown.");
                
            YupiWriterManager.WriteLine("Yupi DataBase Manager Closed!", "Yupi.Data", ConsoleColor.DarkMagenta);

            Clients.Clear();

            YupiWriterManager.WriteLine("Connection Manager Closed!", "Yupi.Data", ConsoleColor.DarkYellow);
        }

        /// <summary>
        ///     Updates the client.
        /// </summary>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
         void UpdateClient(string oldName, string newName)
        {
            if (!_userNameRegister.Contains(oldName.ToLower()))
                return;

            GameClient old = (GameClient) _userNameRegister[oldName.ToLower()];

            _userNameRegister.Remove(oldName.ToLower());

            _userNameRegister.Add(newName.ToLower(), old);
        }

        /// <summary>
        ///     Gives the badges.
        /// </summary>
        private void GiveBadges()
        {
            if (_badgeQueue.Count == 0)
                    return;
                    
            lock (_badgeQueue.SyncRoot)
            {
                while (_badgeQueue.Count > 0)
                {
                    string badge = (string) _badgeQueue.Dequeue();

                    foreach (GameClient current in Clients.Values.Where(current => current?.GetHabbo() != null))
                        current.GetHabbo().GetBadgeComponent().GiveBadge(badge, true, current);
                    
                    foreach (GameClient current2 in Clients.Values.Where(current2 => current2?.GetHabbo() != null))            
                        current2.SendNotif(Yupi.GetLanguage().GetVar("user_earn_badge"));
                }
            }
        }

        /// <summary>
        ///     Broadcasts the packets.
        /// </summary>
        private void BroadcastPackets()
        {
            if (!_broadcastQueue.Any())
                return;
                
            byte[] bytes;
                        
            _broadcastQueue.TryDequeue(out bytes);

            foreach (GameClient current in Clients.Values.Where(current => current?.GetConnection() != null))
                current.GetConnection().Send(bytes);
        }
    }
}
