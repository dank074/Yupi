﻿using System;
using Yupi.Protocol.Buffers;
using Yupi.Model.Domain.Components;

namespace Yupi.Messages.User
{
	public class LoadVolumeMessageComposer : Yupi.Messages.Contracts.LoadVolumeMessageComposer
	{
		public override void Compose ( Yupi.Protocol.ISender session, UserPreferences preferences)
		{
			using (ServerMessage message = Pool.GetMessageBuffer (Id)) {
				message.AppendInteger(preferences.Volume1); 
				message.AppendInteger(preferences.Volume2);
				message.AppendInteger(preferences.Volume3); 
				message.AppendBool(preferences.PreferOldChat);
				message.AppendBool(preferences.IgnoreRoomInvite);
				message.AppendBool(preferences.DisableCameraFollow);
				// TODO Add to preferences
				message.AppendInteger(3); // collapse friends (3 = no) 
				message.AppendInteger(preferences.ChatColor); //bubble

				session.Send(message);
			}
		}
	}
}

