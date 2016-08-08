﻿using System;
using System.Linq;
using Yupi.Model;


namespace Yupi.Messages.User
{
	public class WardrobeUpdateMessageEvent : AbstractHandler
	{
		public override void HandleMessage (Yupi.Protocol.ISession<Yupi.Model.Domain.Habbo> session, Yupi.Protocol.Buffers.ClientMessage message, Yupi.Protocol.IRouter router)
		{
			int slot = message.GetInteger ();
			string look = message.GetString ();
			string gender = message.GetString ();
			// TODO Filter look & gender

			WardrobeItem item = session.UserData.Info.Wardrobe.FirstOrDefault (x => x.Slot == slot);

			if (item != default(WardrobeItem)) {
				item.Look = look;
				item.Gender = gender;
			}
		}
	}
}

