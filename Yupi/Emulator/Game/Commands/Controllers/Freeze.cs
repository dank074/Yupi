﻿using Yupi.Emulator.Game.Commands.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Rooms.User;

namespace Yupi.Emulator.Game.Commands.Controllers
{
    /// <summary>
    ///     Class Freeze. This class cannot be inherited.
    /// </summary>
     sealed class Freeze : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Freeze" /> class.
        /// </summary>
        public Freeze()
        {
            MinRank = 7;
            Description = "Makes the user can't walk. To let user can walk again, execute this command again.";
            Usage = ":freeze [USERNAME]";
            MinParams = 1;
        }

        public override bool Execute(GameClient session, string[] pms)
        {
            RoomUser user = session.GetHabbo()
                .CurrentRoom.GetRoomUserManager()
                .GetRoomUserByHabbo(pms[0]);
            if (user == null) session.SendWhisper(Yupi.GetLanguage().GetVar("user_not_found"));
            else user.Frozen = !user.Frozen;

            return true;
        }
    }
}