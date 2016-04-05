﻿using Yupi.Emulator.Game.Commands.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Rooms;
using Yupi.Emulator.Game.Rooms.User;

namespace Yupi.Emulator.Game.Commands.Controllers
{
    /// <summary>
    ///     Class SayAll. This class cannot be inherited.
    /// </summary>
     sealed class SayAll : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Alert" /> class.
        /// </summary>
        public SayAll()
        {
            MinRank = 5;
            Description = "Todos dicen..";
            Usage = ":sayall";
            MinParams = -1;
        }

        /// <summary>
        ///     Executes the specified session.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="pms">The PMS.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public override bool Execute(GameClient session, string[] pms)
        {
            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return true;
            string str = string.Join(" ", pms);
            if (str == "")
                return true;
            foreach (RoomUser user in room.GetRoomUserManager().GetRoomUsers())
                user.Chat(user.GetClient(), str, false, 0);
            return true;
        }
    }
}