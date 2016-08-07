﻿using System;
using System.Drawing;
using Yupi.Protocol.Buffers;

namespace Yupi.Messages.Rooms
{
	public class GetFloorPlanUsedCoordsMessageComposer : Yupi.Messages.Contracts.GetFloorPlanUsedCoordsMessageComposer
	{
		public override void Compose ( Yupi.Protocol.ISender session, Point[] coords)
		{
			using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
				message.AppendInteger(coords.Length);

				foreach (Point point in coords)
				{
					message.AppendInteger(point.X);
					message.AppendInteger(point.Y);
				}

				session.Send (message);
			}
		}
	}
}

