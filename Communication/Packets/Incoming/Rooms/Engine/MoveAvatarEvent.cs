using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.Chat.Commands.User.RP.Corporations;

namespace Plus.Communication.Packets.Incoming.Rooms.Engine;

internal class MoveAvatarEvent : IPacketEvent
{
    private readonly SellCommand _sellCommand;

    public MoveAvatarEvent(SellCommand sellCommand)
    {
        _sellCommand = sellCommand;
    }

    public async Task Parse(GameClient session, IIncomingPacket packet)
    {
        if (!session.GetHabbo().InRoom)
            return;
        var room = session.GetHabbo().CurrentRoom;
        if (room == null)
            return;
        var user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
        if (user == null || !user.CanWalk)
            return;
        var moveX = packet.ReadInt();
        var moveY = packet.ReadInt();
        if (moveX == user.X && moveY == user.Y)
            return;
        if (user.RidingHorse)
        {
            var horse = room.GetRoomUserManager().GetRoomUserByVirtualId(user.HorseId);
            if (horse != null)
                horse.MoveTo(moveX, moveY);
        }
        user.MoveTo(moveX, moveY);

        if (user.HasActiveOffer)
        {
            foreach (var offerId in _sellCommand._itemOffers.Keys)
            {
                var offer = _sellCommand._itemOffers[offerId];
                if (offer.SenderId == user.UserId || offer.TargetId == user.UserId)
                {
                    await _sellCommand.OnUserMoved(session, offerId);
                }
            }
        }
    }
}