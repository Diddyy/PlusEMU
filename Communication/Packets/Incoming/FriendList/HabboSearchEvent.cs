﻿using Plus.Communication.Packets.Outgoing.FriendList;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Messenger;
using Plus.Utilities;

namespace Plus.Communication.Packets.Incoming.FriendList;

internal class HabboSearchEvent : IPacketEvent
{
    public Task Parse(GameClient session, IIncomingPacket packet)
    {
        var query = StringCharFilter.Escape(packet.ReadString().Replace("%", ""));
        if (query.Length < 1 || query.Length > 100)
            return Task.CompletedTask;
        var friends = new List<SearchResult>();
        var othersUsers = new List<SearchResult>();
        var results = SearchResultFactory.GetSearchResult(query);
        foreach (var result in results.ToList())
        {
            if (session.GetHabbo().GetMessenger().FriendshipExists(result.UserId))
                friends.Add(result);
            else
                othersUsers.Add(result);
        }
        session.Send(new HabboSearchResultComposer(friends, othersUsers));
        return Task.CompletedTask;
    }
}