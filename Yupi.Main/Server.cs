﻿// ---------------------------------------------------------------------------------
// <copyright file="Server.cs" company="https://github.com/sant0ro/Yupi">
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
namespace Yupi.Main
{
    using System;

    using Yupi.Controller;
    using Yupi.Crypto;
    using Yupi.Crypto.Utils;
    using Yupi.Messages;
    using Yupi.Messages.Achievements;
    using Yupi.Model;
    using Yupi.Model.Domain;
    using Yupi.Net;
    using Yupi.Protocol.Buffers;
    using Yupi.Rest;
    using Yupi.Util;
    using Yupi.Util.Settings;

    using log4net;
    using log4net.Appender;
    using log4net.Core;
    using log4net.Layout;
    using log4net.Config;
    using log4net.Repository.Hierarchy;

    public class Server
    {
        #region Fields

        private ClientManager ClientManager;
        private IServer<Habbo> TCPServer;
        private RestServer RestServer;

        #endregion Fields

        #region Constructors

        public Server()
        {
            SetupLogger();

            if (CryptoSettings.Enabled)
            {
                Encryption.GetInstance(new Crypto.Cryptography.RSACParameters(RSACUtils.Base64ToBigInteger(CryptoSettings.RsaD), RSACUtils.Base64ToBigInteger(CryptoSettings.RsaN), RSACUtils.Base64ToBigInteger(CryptoSettings.RsaE)), CryptoSettings.DHKeysSize);
            }

            var factory = ModelHelper.CreateFactory();

            // TODO: Close Session & Multiple sessions!
            var dbSession = factory.OpenSession();

            DependencyFactory.RegisterInstance(dbSession);

            // TODO: Don't run this if DB is not new!
            ModelHelper.Populate();

            Router.Default = new Router(GameSettings.Release, "../../../Config/",
                typeof(AchievementProgressMessageComposer).Assembly
            );

            ClientManager = DependencyFactory.Resolve<ClientManager>();
            RestServer = new RestServer();

            SetupTCP();
        }

        #endregion Constructors

        #region Methods

        public void Run()
        {
            TCPServer.Start();
            RestServer.Start();
        }

        private void SetupLogger()
        {
            Hierarchy hierarchy = (Hierarchy) LogManager.GetRepository();
            hierarchy.Root.Level = Level.Debug;
            hierarchy.RaiseConfigurationChanged(EventArgs.Empty);

            AppenderSkeleton consoleAppender, fileAppender;

            if (MonoUtil.IsRunningOnMono())
            {
                consoleAppender = new ConsoleAppender();
            }
            else
            {
                consoleAppender = new ColoredConsoleAppender();
            }

            consoleAppender.Layout = new PatternLayout(@"[%date][%c][%level] %message %newline");
            consoleAppender.Threshold = Level.Debug;
            consoleAppender.ActivateOptions();

            // fileAppender = new FileAppender();
            // fileAppender.AppendToFile = true;
            // fileAppender.Layout = new PatternLayout(@"[%date][%c][%level] %message %newline");
            // fileAppender.Threshold = Level.Debug;
            // fileAppender.File = "log.txt";
            // fileAppender.ActivateOptions();

            BasicConfigurator.Configure(consoleAppender);
            // BasicConfigurator.Configure(FileAppender);
        }

        private void SetupTCP()
        {
            TCPServer = ServerFactory<Habbo>.CreateServer(GameSettings.GamePort);

            TCPServer.OnConnectionOpened += ClientManager.AddClient; // TODO: Connection security!
            TCPServer.OnConnectionClosed += ClientManager.RemoveClient;
            TCPServer.OnMessageReceived += (ISession<Habbo> session, byte[] body) =>
            {
                // using(global::Yupi.Emulator.Messages.Buffers.SimpleClientMessageBuffer message = ClientMessageFactory.GetClientMessage()) {
                // TODO: When using message pool the SimpleClientMessageBuffer becomes invalid (after several messages) -> DEBUG
                ClientMessage message = new ClientMessage();
                message.Setup(body);

                Router.Default.Handle(session.UserData, message);
            };
        }

        #endregion Methods
    }
}
