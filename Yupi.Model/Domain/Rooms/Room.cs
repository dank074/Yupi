﻿using System;
using System.Collections.Generic;
using System.Linq;
using Yupi.Protocol;

namespace Yupi.Model.Domain
{
	[Ignore]
	public class Room : ISender
	{
		public RoomData Data { get; private set; }

		public IList<RoomEntity> Users { get; private set; }

		public Room ()
		{
		}

		public RoomEntity GetEntity(int id) {
			return Users.Single (entity => entity.Id == id);
		}

		public void Send (Yupi.Protocol.Buffers.ServerMessage message)
		{
			foreach (RoomEntity entity in Users) {
				if (entity is UserEntity) {
					entity.Send (message);
				}
			}
		}
	}
}

