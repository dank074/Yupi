using System.Collections.Generic;
using System.Linq;
using Yupi.Emulator.Game.Items.Interactions.Enums;
using Yupi.Emulator.Game.Items.Interfaces;
using Yupi.Emulator.Game.Items.Wired.Interfaces;
using Yupi.Emulator.Game.Rooms;

namespace Yupi.Emulator.Game.Items.Wired.Handlers.Conditions
{
     public class FurniHasUsers : IWiredItem
    {
        public FurniHasUsers(RoomItem item, Room room)
        {
            Item = item;
            Room = room;
            Items = new List<RoomItem>();
        }

        public Interaction Type => Interaction.ConditionFurnisHaveUsers;

        public RoomItem Item { get; set; }

        public Room Room { get; set; }

        public List<RoomItem> Items { get; set; }

        public string OtherString
        {
            get { return ""; }
            set { }
        }

        public string OtherExtraString
        {
            get { return ""; }
            set { }
        }

        public string OtherExtraString2
        {
            get { return ""; }
            set { }
        }

        public bool OtherBool
        {
            get { return true; }
            set { }
        }

        public int Delay
        {
            get { return 0; }
            set { }
        }

        public bool Execute(params object[] stuff)
            =>
                !Items.Any() ||
                Items.Where(current => current != null && Room.GetRoomItemHandler().FloorItems.ContainsKey(current.Id))
                    .Where(
                        current =>
                            !current.AffectedTiles.Values.Any(
                                current2 => Room.GetGameMap().SquareHasUsers(current2.X, current2.Y)))
                    .All(current => Room.GetGameMap().SquareHasUsers(current.X, current.Y));
    }
}