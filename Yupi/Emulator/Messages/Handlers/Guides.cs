﻿using System;
using Yupi.Emulator.Game.GameClients.Interfaces;
using Yupi.Emulator.Game.Rooms;
using Yupi.Emulator.Game.Users.Guides;
using Yupi.Emulator.Messages.Buffers;

namespace Yupi.Emulator.Messages.Handlers
{
    /// <summary>
    ///     Class GameClientMessageHandler.
    /// </summary>
    internal partial class GameClientMessageHandler
    {
        /// <summary>
        ///     Calls the guide.
        /// </summary>
        internal void GuideMessage()
        {
            Request.GetBool();

            int userId = Request.GetIntegerFromString();
            string message = Request.GetString();

            GuideManager guideManager = Yupi.GetGame().GetGuideManager();

            if (guideManager.GuidesCount <= 0)
            {
                Response.Init(PacketLibraryManager.SendRequest("OnGuideSessionError"));
                Response.AppendInteger(0);
                SendResponse();
                return;
            }

            GameClient guide = guideManager.GetRandomGuide();

            if (guide == null)
            {
                Response.Init(PacketLibraryManager.SendRequest("OnGuideSessionError"));
                Response.AppendInteger(0);
                SendResponse();
                return;
            }

            SimpleServerMessageBuffer onGuideSessionAttached =
                new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionAttachedMessageComposer"));
            onGuideSessionAttached.AppendBool(false);
            onGuideSessionAttached.AppendInteger(userId);
            onGuideSessionAttached.AppendString(message);
            onGuideSessionAttached.AppendInteger(30);
            Session.SendMessage(onGuideSessionAttached);

            SimpleServerMessageBuffer onGuideSessionAttached2 =
                new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionAttachedMessageComposer"));
            onGuideSessionAttached2.AppendBool(true);
            onGuideSessionAttached2.AppendInteger(userId);
            onGuideSessionAttached2.AppendString(message);
            onGuideSessionAttached2.AppendInteger(15);
            guide.SendMessage(onGuideSessionAttached2);
            guide.GetHabbo().GuideOtherUser = Session;
            Session.GetHabbo().GuideOtherUser = guide;
        }

        /// <summary>
        ///     Answers the guide request.
        /// </summary>
        internal void GetGuideDetached()
        {
            bool state = Request.GetBool();

            if (!state)
                return;

            GameClient requester = Session.GetHabbo().GuideOtherUser;
            SimpleServerMessageBuffer messageBuffer = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionStartedMessageComposer"));

            messageBuffer.AppendInteger(requester.GetHabbo().Id);
            messageBuffer.AppendString(requester.GetHabbo().UserName);
            messageBuffer.AppendString(requester.GetHabbo().Look);
            messageBuffer.AppendInteger(Session.GetHabbo().Id);
            messageBuffer.AppendString(Session.GetHabbo().UserName);
            messageBuffer.AppendString(Session.GetHabbo().Look);
            requester.SendMessage(messageBuffer);
            Session.SendMessage(messageBuffer);
        }

        /// <summary>
        ///     Opens the guide tool.
        /// </summary>
        internal void GetHelperTool()
        {
            GuideManager guideManager = Yupi.GetGame().GetGuideManager();
            bool onDuty = Request.GetBool();

            Request.GetBool();
            Request.GetBool();
            Request.GetBool();

            if (onDuty)
                guideManager.AddGuide(Session);
            else
                guideManager.RemoveGuide(Session);

            Session.GetHabbo().OnDuty = onDuty;
            Response.Init(PacketLibraryManager.SendRequest("HelperToolConfigurationMessageComposer"));
            Response.AppendBool(onDuty);
            Response.AppendInteger(guideManager.GuidesCount);
            Response.AppendInteger(guideManager.HelpersCount);
            Response.AppendInteger(guideManager.GuardiansCount);
            SendResponse();
        }

        /// <summary>
        ///     Invites to room.
        /// </summary>
        internal void InviteGuide()
        {
            GameClient requester = Session.GetHabbo().GuideOtherUser;

            Room room = Session.GetHabbo().CurrentRoom;

            SimpleServerMessageBuffer messageBuffer =
                new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionInvitedToGuideRoomMessageComposer"));

            if (room == null)
            {
                messageBuffer.AppendInteger(0);
                messageBuffer.AppendString(string.Empty);
            }
            else
            {
                messageBuffer.AppendInteger(room.RoomId);
                messageBuffer.AppendString(room.RoomData.Name);
            }

            requester.SendMessage(messageBuffer);
            Session.SendMessage(messageBuffer);
        }

        /// <summary>
        ///     Visits the room.
        /// </summary>
        internal void VisitRoomGuide()
        {
            if (Session.GetHabbo().GuideOtherUser == null)
                return;

            GameClient requester = Session.GetHabbo().GuideOtherUser;
            SimpleServerMessageBuffer visitRoom = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("RoomForwardMessageComposer"));
            visitRoom.AppendInteger(requester.GetHabbo().CurrentRoomId);
            Session.SendMessage(visitRoom);
        }

        /// <summary>
        ///     Guides the speak.
        /// </summary>
        internal void MessageFromAGuy()
        {
            string message = Request.GetString();
            GameClient requester = Session.GetHabbo().GuideOtherUser;
            SimpleServerMessageBuffer messageBufferC = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionMsgMessageComposer"));
            messageBufferC.AppendString(message);
            messageBufferC.AppendInteger(Session.GetHabbo().Id);
            requester.SendMessage(messageBufferC);
            Session.SendMessage(messageBufferC);
        }

        /// <summary>
        ///     BETA
        ///     Closes the guide request.
        /// </summary>
        internal void GuideEndSession()
        {
            GameClient requester = Session.GetHabbo().GuideOtherUser;

            SimpleServerMessageBuffer messageBuffer = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionDetachedMessageComposer"));

            /* guide - close session  */
            messageBuffer.AppendInteger(2);
            requester.SendMessage(messageBuffer);

            /* user - close session */
            SimpleServerMessageBuffer message2 = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionDetachedMessageComposer"));
            messageBuffer.AppendInteger(0);
            Session.SendMessage(message2);

            /* user - detach session */
            SimpleServerMessageBuffer message3 = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionDetachedMessageComposer"));
            Session.SendMessage(message3);

            /* guide - detach session */
            SimpleServerMessageBuffer message4 = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionDetachedMessageComposer"));
            requester.SendMessage(message4);

            requester.GetHabbo().GuideOtherUser = null;
            Session.GetHabbo().GuideOtherUser = null;
        }

        /// <summary>
        ///     Cancels the call guide.
        ///     BETA
        /// </summary>
        internal void CancelCallGuide()
        {
            // @TODO what packet is this?
            //Response.Load(3485);

            /* user - cancell session */
            SimpleServerMessageBuffer messageBuffer = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionDetachedMessageComposer"));
            messageBuffer.AppendInteger(2);
            Session.SendMessage(messageBuffer);

            /* achievement */
            SimpleServerMessageBuffer messageBuffer2 = new SimpleServerMessageBuffer(PacketLibraryManager.SendRequest("OnGuideSessionDetachedMessageComposer"));
            Session.SendMessage(messageBuffer2);
            Yupi.GetGame().GetAchievementManager().ProgressUserAchievement(Session, "ACH_GuideFeedbackGiver", 1);
        }

        /// <summary>
        ///     Ambassadors the alert.
        /// </summary>
        internal void AmbassadorAlert()
        {
            if (Session.GetHabbo().Rank < Convert.ToUInt32(Yupi.GetDbConfig().DbData["ambassador.minrank"]))
                return;

            uint userId = Request.GetUInteger();

            GameClient user = Yupi.GetGame().GetClientManager().GetClientByUserId(userId);

            user?.SendNotif("${notification.ambassador.alert.warning.message}",
                "${notification.ambassador.alert.warning.title}");
        }
    }
}