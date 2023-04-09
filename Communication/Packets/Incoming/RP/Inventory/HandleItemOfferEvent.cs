using Plus.Communication.Packets.Outgoing.RP.Inventory;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Inventory;

internal class HandleItemOfferEvent : IPacketEvent
{
    public Task Parse(GameClient session, IIncomingPacket packet)
    {
        var offerId = packet.ReadInt();
        var room = session.GetHabbo().CurrentRoom;
        var user = room.GetRoomUserManager().GetRoomUserByHabbo(packet.ReadInt());
        int itemId = packet.ReadInt();
        int price = packet.ReadInt();

        session.Send(new ItemOfferComposer(offerId, user.GetClient().GetHabbo(), itemId, price));
        return Task.CompletedTask;
    }
}