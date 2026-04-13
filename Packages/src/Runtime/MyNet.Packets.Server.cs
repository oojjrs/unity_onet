using System;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static partial class Packets
        {
            public static class Server
            {
                private static readonly HashQueue<MyNetRequest> _requests = new();
                private static readonly HashQueue<MyNetResponse> _responses = new();

                public static event Action OnFinishThisHandling;
                public static event Action<MyNetRequest> OnReceived;

                public static void HandleRequests()
                {
                    if (_requests.Count > 0)
                    {
                        while (_requests.TryDequeue(out var request))
                            OnReceived?.Invoke(request);

                        // 사실 암 것도 안하긴 하는데 클라쪽하고 모양새 맞추려고 추가해둠
                        OnFinishThisHandling?.Invoke();
                    }
                }

                internal static void Receive(MyNetRequest request)
                {
                    _requests.Enqueue(request);
                }

                public static void Send(MyNetResponse response)
                {
                    _responses.Enqueue(response);
                }

                internal static bool TryDequeue(out MyNetResponse response)
                {
                    return _responses.TryDequeue(out response);
                }
            }
        }
    }
}
