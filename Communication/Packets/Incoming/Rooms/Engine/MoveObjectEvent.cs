﻿using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine;

internal class MoveObjectEvent : IPacketEvent
{
    private readonly IRoomManager _roomManager;
    private readonly IQuestManager _questManager;

    public MoveObjectEvent(IRoomManager roomManager, IQuestManager questManager)
    {
        _roomManager = roomManager;
        _questManager = questManager;
    }

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.GetHabbo().InRoom)
            return;
        var itemId = packet.PopInt();
        if (itemId == 0)
            return;
        if (!_roomManager.TryGetRoom(session.GetHabbo().CurrentRoomId, out var room))
            return;
        Item item;
        if (room.Group != null)
        {
            if (!room.CheckRights(session, false, true))
            {
                item = room.GetRoomItemHandler().GetItem(itemId);
                if (item == null)
                    return;
                session.SendPacket(new ObjectUpdateComposer(item, room.OwnerId));
                return;
            }
        }
        else
        {
            if (!room.CheckRights(session)) return;
        }
        item = room.GetRoomItemHandler().GetItem(itemId);
        if (item == null)
            return;
        var x = packet.PopInt();
        var y = packet.PopInt();
        var rotation = packet.PopInt();
        if (x != item.GetX || y != item.GetY)
            _questManager.ProgressUserQuest(session, QuestType.FurniMove);
        if (rotation != item.Rotation)
            _questManager.ProgressUserQuest(session, QuestType.FurniRotate);
        if (!room.GetRoomItemHandler().SetFloorItem(session, item, x, y, rotation, false, false, true))
        {
            room.SendPacket(new ObjectUpdateComposer(item, room.OwnerId));
            return;
        }
        if (item.GetZ >= 0.1)
            _questManager.ProgressUserQuest(session, QuestType.FurniStack);
    }
}