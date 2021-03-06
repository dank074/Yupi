﻿// ---------------------------------------------------------------------------------
// <copyright file="SaveRoomBackgroundTonerMessageEvent.cs" company="https://github.com/sant0ro/Yupi">
//   Copyright (c) 2016 Claudio Santoro, TheDoctor
// </copyright>
// <license>
//   Permission is hereby granted, free of charge, to any person obtaining a copy
//   of this software and associated documentation files (the "Software"), to deal
//   in the Software without restriction, including without limitation the rights
//   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//   copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//
//   The above copyright notice and this permission notice shall be included in
//   all copies or substantial portions of the Software.
//
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//   THE SOFTWARE.
// </license>
// ---------------------------------------------------------------------------------
namespace Yupi.Messages.Items
{
    using System;

    public class SaveRoomBackgroundTonerMessageEvent : AbstractHandler
    {
        #region Methods

        public override void HandleMessage(Yupi.Model.Domain.Habbo session, Yupi.Protocol.Buffers.ClientMessage request,
            Yupi.Protocol.IRouter router)
        {
            /*
            Yupi.Messages.Rooms room = Yupi.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (room == null || !room.CheckRights(session, true))
                return;

            RoomItem item = room.GetRoomItemHandler().GetItem(room.TonerData.ItemId);

            if (item == null || item.GetBaseItem().InteractionType != Interaction.RoomBg)
                return;

            // TODO Unused
            request.GetInteger();

            int data1 = request.GetInteger();
            int data2 = request.GetInteger();
            int data3 = request.GetInteger();

            if (data1 > 255 || data2 > 255 || data3 > 255)
                return;

            using (IQueryAdapter queryReactor = Yupi.GetDatabaseManager().GetQueryReactor()) {
                queryReactor.SetQuery ("UPDATE items_toners SET enabled = @enabled, data1 = @data1, data2 = @data2, data3 = @data3 WHERE id = @id");
                queryReactor.AddParameter ("enabled", 1);
                queryReactor.AddParameter ("data1", data1);
                queryReactor.AddParameter ("data2", data2);
                queryReactor.AddParameter ("data3", data3);
                queryReactor.AddParameter ("id", item.Id);
            }

            room.TonerData.Data1 = data1;
            room.TonerData.Data2 = data2;
            room.TonerData.Data3 = data3;

            // TODO Enabled is int?!
            room.TonerData.Enabled = 1;

            router.GetComposer<UpdateRoomItemMessageComposer> ().Compose (room, item);
            item.UpdateState();
            */
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}