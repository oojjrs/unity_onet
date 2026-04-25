using System.Collections.Generic;
using System.Net;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        // 지워질지도 모르는 일단 임시 코드
        internal static class Udp
        {
            private static Dictionary<IPEndPoint, Queue<byte[]>> Packets { get; } = new();

            public static void Add(IPEndPoint remote, byte[] bytes)
            {
                if (Packets.TryGetValue(remote, out var queue) == false)
                {
                    queue = new();
                    Packets[remote] = queue;
                }

                queue.Enqueue(bytes);
            }

            public static bool Has(IPEndPoint remote)
            {
                return Packets.TryGetValue(remote, out var queue) && (queue.Count > 0);
            }

            public static bool TryDequeue(IPEndPoint remote, out byte[] result)
            {
                if (Packets.TryGetValue(remote, out var queue))
                {
                    return queue.TryDequeue(out result);
                }
                else
                {
                    result = null;
                    return false;
                }
            }
        }
    }
}
