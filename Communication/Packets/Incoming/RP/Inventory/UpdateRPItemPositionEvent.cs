using Plus.Communication.Packets.Outgoing.RP.Inventory;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.RP.Inventory;

internal class UpdateRPItemPositionEvent : IPacketEvent
{
    public async Task Parse(GameClient session, IIncomingPacket packet)
    {
        int itemId = packet.ReadInt();
        int position = packet.ReadInt();

        await session.GetHabbo().RPInventory.UpdateItemPosition(itemId, position);
        session.Send(new UpdateRPItemPositionComposer(itemId, position));
    }
}