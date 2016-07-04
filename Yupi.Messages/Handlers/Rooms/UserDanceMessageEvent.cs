﻿using System;
using Yupi.Emulator.Game.Rooms;
using Yupi.Emulator.Game.Rooms.User;

namespace Yupi.Messages.Rooms
{
	public class UserDanceMessageEvent : AbstractHandler
	{
		public override void HandleMessage (Yupi.Emulator.Game.GameClients.Interfaces.GameClient session, Yupi.Protocol.Buffers.ClientMessage request, Router router)
		{
			Room room = Yupi.GetGame().GetRoomManager().GetRoom(session.GetHabbo().CurrentRoomId);
			RoomUser roomUserByHabbo = room?.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);

			if (roomUserByHabbo == null)
				return;

			roomUserByHabbo.UnIdle();

			uint danceId = request.GetUInt32();

			if (danceId > 4)
				danceId = 0;

			if (danceId > 0 && roomUserByHabbo.CarryItemId > 0)
				roomUserByHabbo.CarryItem(0);

			roomUserByHabbo.DanceId = danceId;

			router.GetComposer<DanceStatusMessageComposer> ().Compose (room, roomUserByHabbo.VirtualId, danceId);
		}
	}
}
