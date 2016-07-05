﻿using System;

namespace Yupi.Messages.Other
{
	public class PongMessageEvent : AbstractHandler
	{
		public override bool RequireUser {
			get { 
				return false; 
			}
		}

		public override void HandleMessage (Yupi.Net.ISession<GameClient> session, Yupi.Protocol.Buffers.ClientMessage request, Yupi.Protocol.IRouter router)
		{
			session.TimePingedReceived = DateTime.Now;
		}
	}
}

