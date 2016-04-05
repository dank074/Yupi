﻿using Yupi.Emulator.Game.Commands.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;

namespace Yupi.Emulator.Game.Commands.Controllers
{
    /// <summary>
    ///     Class MassEnable. This class cannot be inherited.
    /// </summary>
     sealed class GoInvisible : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MassEnable" /> class.
        /// </summary>
        public GoInvisible()
        {
            MinRank = 7;
            Description = "Invisible";
            Usage = ":invisible";
            MinParams = 0;
        }

        public override bool Execute(GameClient session, string[] pms)
        {
            session.GetHabbo().SpectatorMode = true;
            session.SendNotif("In next room you enter you will be invisible.");

            return true;
        }
    }
}