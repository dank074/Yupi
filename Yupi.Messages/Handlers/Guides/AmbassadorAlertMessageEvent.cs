﻿using System;
using Yupi.Emulator.Game.GameClients.Interfaces;

namespace Yupi.Messages.Guides
{
	public class AmbassadorAlertMessageEvent : AbstractHandler
	{
		public override void HandleMessage (Yupi.Emulator.Game.GameClients.Interfaces.GameClient session, Yupi.Protocol.Buffers.ClientMessage message, Router router)
		{
			if (session.GetHabbo().Rank < Convert.ToUInt32(Yupi.GetDbConfig().DbData["ambassador.minrank"]))
				return;

			uint userId = message.GetUInt32();

			GameClient user = Yupi.GetGame().GetClientManager().GetClientByUserId(userId);

			user?.SendNotif("${notification.ambassador.alert.warning.message}",
				"${notification.ambassador.alert.warning.title}");
		}
	}
}
