﻿using System;
using Yupi.Emulator.Game.Rooms;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Rooms.User;

namespace Yupi.Messages.User
{
	public class GetUserBadgesMessageEvent : AbstractHandler
	{
		public override void HandleMessage (GameClient session, Yupi.Protocol.Buffers.ClientMessage message, Router router)
		{
			// TODO Refactor
			Room room = Yupi.GetGame().GetRoomManager().GetRoom(session.UserData.CurrentRoomId);

			uint userId = message.GetUInt32 ();

			RoomUser roomUserByHabbo = room?.GetRoomUserManager().GetRoomUserByHabbo(userId);

			if (roomUserByHabbo != null && !roomUserByHabbo.IsBot && roomUserByHabbo.GetClient() != null &&
				roomUserByHabbo.GetClient().GetHabbo() != null)
			{
				session.UserData.LastSelectedUser = roomUserByHabbo.UserId;

				router.GetComposer<UserBadgesMessageComposer> ().Compose (session, roomUserByHabbo.GetClient ().GetHabbo ().Id);
			}
		}
	}
}

