﻿using Yupi.Emulator.Core.Security.BlackWords;
using Yupi.Emulator.Game.Commands.Interfaces;
using Yupi.Emulator.Game.GameClients.Interfaces;

namespace Yupi.Emulator.Game.Commands.Controllers
{
    /// <summary>
    ///     Class Enable. This class cannot be inherited.
    /// </summary>
     sealed class DeleteBlackWord : Command
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteBlackWord" /> class.
        /// </summary>
        public DeleteBlackWord()
        {
            MinRank = 8;
            Description = "Delete a word from filter list.";
            Usage = ":deleteblackword type(hotel|insult|all) word";
            MinParams = 2;
        }

        public override bool Execute(GameClient session, string[] pms)
        {
            string type = pms[0];
            string word = pms[1];

            if (string.IsNullOrEmpty(word))
            {
                session.SendWhisper("Palabra inválida.");
                return true;
            }
            BlackWordsManager.DeleteBlackWord(type, word);
            return true;
        }
    }
}