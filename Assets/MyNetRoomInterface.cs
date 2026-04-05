using System.Collections.Generic;

namespace oojjrs.onet
{
    public interface MyNetRoomInterface
    {
        MyNetPlayerInterface Host { get; }
        string HostId { get; }
        string Id { get; }
        bool IsPrivate { get; }
        string Name { get; }
        int PlayerCount { get; }
        int PlayerCountMax { get; }
        IEnumerable<MyNetPlayerInterface> Players { get; }

        string GetData(string key);
    }
}
