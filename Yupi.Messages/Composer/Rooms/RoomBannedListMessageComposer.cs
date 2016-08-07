﻿using System;
using System.Collections.Generic;
using Yupi.Protocol.Buffers;

namespace Yupi.Messages.Rooms
{
	public class RoomBannedListMessageComposer : Yupi.Messages.Contracts.RoomBannedListMessageComposer
	{
		public override void Compose ( Yupi.Protocol.ISender session, uint roomId, List<uint> bannedUsers)
		{
			using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
				message.AppendInteger (roomId);
				message.AppendInteger (bannedUsers.Count);

				foreach (uint current in bannedUsers) {
					message.AppendInteger (current); 
					// TODO What happens if the user is not loaded?
					message.AppendString (Yupi.GetHabboById (current) != null ? Yupi.GetHabboById (current).UserName : "Undefined");
				}
				session.Send (message);
			}
		}
	}
}

