using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.RP.Inventory;

public class UpdateRPItemPositionComposer : IServerPacket
{
    private readonly int _itemId;
    private readonly int _newPosition;

    public uint MessageId => ServerPacketHeader.UpdateRPItemPositionComposer;

    public UpdateRPItemPositionComposer(int itemId, int newPosition)
    {
        _itemId = itemId;
        _newPosition = newPosition;
    }

    public void Compose(IOutgoingPacket packet)
    {
        Console.WriteLine("UpdateRPItemPositionComposer");
        packet.WriteInteger(_itemId);
        packet.WriteInteger(_newPosition);
    }
}
