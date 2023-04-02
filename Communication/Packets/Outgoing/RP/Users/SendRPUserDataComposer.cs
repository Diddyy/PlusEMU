using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.RP.Users;

public class SendRPUserDataComposer : IServerPacket
{
    private readonly Habbo _habbo;

    public uint MessageId => ServerPacketHeader.SendRPUserDataComposer;

    public SendRPUserDataComposer(Habbo habbo)
    {
        _habbo = habbo;
    }

    public void Compose(IOutgoingPacket packet)
    {
        packet.WriteInteger(_habbo.Id);
        packet.WriteInteger(_habbo.RPHabboData.Level);
        packet.WriteInteger(_habbo.RPHabboData.Health);
        packet.WriteInteger(_habbo.RPHabboData.MaxHealth);
        packet.WriteInteger(_habbo.RPHabboData.Stamina);
        packet.WriteInteger(_habbo.RPHabboData.MaxStamina);
        packet.WriteInteger(_habbo.RPHabboData.Aggression);
    }
}
