using System.Collections.Generic;

namespace oojjrs.onet
{
    public interface MyRoomInterface
    {
        MyPlayerInterface Host { get; }
        string HostId { get; }
        string Id { get; }
        bool IsPrivate { get; }
        string Name { get; }
        int PlayerCount { get; }
        int PlayerCountMax { get; }
        IEnumerable<MyPlayerInterface> Players { get; }

        string GetData(string key);
    }
}
