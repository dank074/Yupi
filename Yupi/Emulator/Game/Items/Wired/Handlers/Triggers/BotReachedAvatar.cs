using System.Collections.Generic;
using System.Linq;
using Yupi.Emulator.Game.Items.Interactions.Enums;
using Yupi.Emulator.Game.Items.Interfaces;
using Yupi.Emulator.Game.Items.Wired.Interfaces;
using Yupi.Emulator.Game.Rooms;
using Yupi.Emulator.Game.Rooms.User;

namespace Yupi.Emulator.Game.Items.Wired.Handlers.Triggers
{
    public class BotReachedAvatar : IWiredItem
    {
        public BotReachedAvatar(RoomItem item, Room room)
        {
            Item = item;
            Room = room;
        }

        public Interaction Type => Interaction.TriggerBotReachedAvatar;

        public RoomItem Item { get; set; }

        public Room Room { get; set; }

        public List<RoomItem> Items
        {
            get { return new List<RoomItem>(); }
            set { }
        }

        public int Delay
        {
            get { return 0; }
            set { }
        }

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

        public bool Execute(params object[] stuff)
        {
            List<IWiredItem> conditions = Room.GetWiredHandler().GetConditions(this);
            List<IWiredItem> effects = Room.GetWiredHandler().GetEffects(this);

            if (conditions.Any())
            {
                foreach (IWiredItem current in conditions)
                {
                    if (!current.Execute(null))
                        return false;

                    WiredHandler.OnEvent(current);
                }
            }

            if (effects.Any())
            {
                foreach (IWiredItem current2 in effects)
                {
                    foreach (RoomUser current3 in Room.GetRoomUserManager().UserList.Values)
                        current2.Execute(current3, Type);

                    WiredHandler.OnEvent(current2);
                }
            }

            WiredHandler.OnEvent(this);
            return true;
        }
    }
}