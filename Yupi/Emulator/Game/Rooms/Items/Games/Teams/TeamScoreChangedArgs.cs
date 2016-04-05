using System;
using Yupi.Emulator.Game.Rooms.Items.Games.Teams.Enums;
using Yupi.Emulator.Game.Rooms.User;

namespace Yupi.Emulator.Game.Rooms.Items.Games.Teams
{
    /// <summary>
    ///     Class TeamScoreChangedArgs.
    /// </summary>
    public class TeamScoreChangedArgs : EventArgs
    {
        /// <summary>
        ///     The points
        /// </summary>
         readonly int Points;

        /// <summary>
        ///     The team
        /// </summary>
         readonly Team Team;

        /// <summary>
        ///     The user
        /// </summary>
         readonly RoomUser User;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TeamScoreChangedArgs" /> class.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="team">The team.</param>
        /// <param name="user">The user.</param>
        public TeamScoreChangedArgs(int points, Team team, RoomUser user)
        {
            Points = points;
            Team = team;
            User = user;
        }
    }
}