﻿// ---------------------------------------------------------------------------------
// <copyright file="ModerationLockTradeMessageEvent.cs" company="https://github.com/sant0ro/Yupi">
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
namespace Yupi.Messages.Support
{
    using System;

    using Yupi.Controller;
    using Yupi.Messages.Notification;
    using Yupi.Model;
    using Yupi.Model.Domain;
    using Yupi.Model.Repository;

    public class ModerationLockTradeMessageEvent : AbstractHandler
    {
        #region Fields

        private ClientManager ClientManager;
        private IRepository<UserInfo> UserRepository;

        #endregion Fields

        #region Constructors

        public ModerationLockTradeMessageEvent()
        {
            UserRepository = DependencyFactory.Resolve<IRepository<UserInfo>>();
            ClientManager = DependencyFactory.Resolve<ClientManager>();
        }

        #endregion Constructors

        #region Methods

        public override void HandleMessage(Yupi.Model.Domain.Habbo session, Yupi.Protocol.Buffers.ClientMessage request,
            Yupi.Protocol.IRouter router)
        {
            if (!session.Info.HasPermission("fuse_lock_trade"))
                return;

            int userId = request.GetInteger();
            string message = request.GetString();
            int hours = request.GetInteger();

            UserInfo user = UserRepository.Find(userId);

            user.TradeLocks.Add(new TradeLock()
            {
                ExpiresAt = DateTime.Now.AddHours(hours)
            });

            UserRepository.Save(user);

            var target = ClientManager.GetByInfo(user);
            if (target != null)
            {
                target.Router.GetComposer<AlertNotificationMessageComposer>().Compose(target, message);
            }
        }

        #endregion Methods
    }
}