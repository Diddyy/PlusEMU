﻿using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator;

internal class KickCommand : IChatCommand
{
    private readonly IGameClientManager _gameClientManager;
    private readonly IRoomManager _roomManager;
    public string Key => "kick";
    public string PermissionRequired => "command_kick";

    public string Parameters => "%username% %reason%";

    public string Description => "Kick a user from a room and send them a reason.";

    public KickCommand(IGameClientManager gameClientManager, IRoomManager roomManager)
    {
        _gameClientManager = gameClientManager;
        _roomManager = roomManager;
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
        if (!targetClient.GetHabbo().InRoom)
        {
            session.SendWhisper("That user currently isn't in a room.");
            return;
        }
        Room targetRoom;
        if (!_roomManager.TryGetRoom(targetClient.GetHabbo().CurrentRoomId, out targetRoom))
            return;
        if (parameters.Length > 2)
            targetClient.SendNotification("A moderator has kicked you from the room for the following reason: " + CommandManager.MergeParams(parameters, 2));
        else
            targetClient.SendNotification("A moderator has kicked you from the room.");
        targetRoom.GetRoomUserManager().RemoveUserFromRoom(targetClient, true);
    }
}