﻿using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun;

internal class SummonCommand : IChatCommand
{
    private readonly IGameClientManager _gameClientManager;
    public string Key => "summon";
    public string PermissionRequired => "command_summon";

    public string Parameters => "%username%";

    public string Description => "Bring another user to your current room.";

    public SummonCommand(IGameClientManager gameClientManager)
    {
        _gameClientManager = gameClientManager;
    }

    public void Execute(GameClient session, Room room, string[] parameters)
    {
        if (parameters.Length == 1)
        {
            session.SendWhisper("Please enter the username of the user you wish to summon.");
            return;
        }
        var targetClient = _gameClientManager.GetClientByUsername(parameters[1]);
        if (targetClient == null)
        {
            session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
            return;
        }
        if (targetClient.GetHabbo() == null)
        {
            session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
            return;
        }
        if (targetClient.GetHabbo().Username == session.GetHabbo().Username)
        {
            session.SendWhisper("Get a life.");
            return;
        }
        targetClient.SendNotification("You have been summoned to " + session.GetHabbo().Username + "!");
        if (!targetClient.GetHabbo().InRoom)
            targetClient.Send(new RoomForwardComposer(session.GetHabbo().CurrentRoomId));
        else
            targetClient.GetHabbo().PrepareRoom(session.GetHabbo().CurrentRoomId, "");
    }
}