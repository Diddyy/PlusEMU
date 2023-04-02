using Plus.Communication.Packets.Outgoing.RP.Users;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.RP.Users;
internal class GetRPUserInfoEvent : IPacketEvent
{
    public Task Parse(GameClient session, IIncomingPacket packet)
    {
        session.Send(new SendRPUserDataComposer(session.GetHabbo()));
        return Task.CompletedTask;
    }
}
