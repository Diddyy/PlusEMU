using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.RP.Users;

public class SendRPUserInventoryComposer : IServerPacket
{
    private readonly Habbo _habbo;
    public uint MessageId => ServerPacketHeader.SendRPUserInventoryComposer;

    public SendRPUserInventoryComposer(Habbo habbo) => _habbo = habbo;

    public void Compose(IOutgoingPacket packet)
    {
        var inventoryItems = _habbo.RPInventory.AllItems;

        packet.WriteInteger(_habbo.Id);
        packet.WriteInteger(inventoryItems.Count());

        foreach (var item in inventoryItems)
        {
            packet.WriteInteger(item.ItemSlot);
            packet.WriteInteger(item.ItemId);
        }
    }
}