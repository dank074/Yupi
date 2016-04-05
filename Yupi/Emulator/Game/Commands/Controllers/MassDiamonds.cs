﻿using Yupi.Emulator.Game.Commands.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Users;

namespace Yupi.Emulator.Game.Commands.Controllers
{
    /// <summary>
    ///     Class MassDiamonds. This class cannot be inherited.
    /// </summary>
     sealed class MassDiamonds : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MassDiamonds" /> class.
        /// </summary>
        public MassDiamonds()
        {
            MinRank = 8;
            Description = "Gives all the users online Diamonds.";
            Usage = ":massdiamonds [AMOUNT]";
            MinParams = 1;
        }

        public override bool Execute(GameClient session, string[] pms)
        {
            uint amount;

            if (!uint.TryParse(pms[0], out amount))
            {
                session.SendNotif(Yupi.GetLanguage().GetVar("enter_numbers"));

                return true;
            }

            foreach (GameClient client in Yupi.GetGame().GetClientManager().Clients.Values)
            {
                if (client?.GetHabbo() == null)
                    continue;

                Habbo habbo = client.GetHabbo();

                habbo.Diamonds += amount;

                client.GetHabbo().UpdateSeasonalCurrencyBalance();

                client.SendNotif(Yupi.GetLanguage().GetVar("command_diamonds_one_give") + amount + Yupi.GetLanguage().GetVar("command_diamonds_two_give"));
            }

            Yupi.GetGame().GetModerationTool().LogStaffEntry(session.GetHabbo().UserName, string.Empty, "Diamonds", string.Concat("RoomDiamonds in room [", session.GetHabbo().CurrentRoom.RoomId, "] with amount [", pms[0], "]"));

            return true;
        }
    }
}