﻿using System;
using Yupi.Protocol.Buffers;

namespace Yupi.Messages.Rooms
{
	public class OutOfRoomMessageComposer : AbstractComposer<short>
	{
		public override void Compose (Yupi.Protocol.ISender session, short code = 0)
		{
			using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
				message.AppendShort(code);
				session.Send (message);
			}
		}
	}
}

