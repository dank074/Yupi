﻿using System;


namespace Yupi.Messages.Support
{
	public class ModerationToolRoomToolMessageEvent : AbstractHandler
	{
		public override void HandleMessage (Yupi.Net.ISession<GameClient> session, Yupi.Protocol.Buffers.ClientMessage message, Yupi.Protocol.IRouter router)
		{
			if (!session.GetHabbo().HasFuse("fuse_mod"))
				return;

			uint roomId = message.GetUInt32();

			Yupi.Messages.Rooms room = Yupi.GetGame().GetRoomManager().GetRoom(roomId);

			ModerationTool.SerializeRoomTool (data);
		}
	}
}

