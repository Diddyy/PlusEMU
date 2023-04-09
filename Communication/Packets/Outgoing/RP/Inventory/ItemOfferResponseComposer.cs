using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Outgoing.RP.Inventory;
public class ItemOfferResponseComposer : IServerPacket
{
    private readonly int _offerId;
    private readonly bool _accepted;


    public uint MessageId => ServerPacketHeader.ItemOfferResponseComposer;

    public ItemOfferResponseComposer(int offerId, bool accepted)
    {
        _offerId = offerId;
        _accepted = accepted;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteInt(_offerId);
        packet.WriteBool(_accepted);
    }
}