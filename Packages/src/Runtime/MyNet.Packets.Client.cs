using System;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static partial class Packets
        {
            public static class Client
            {
                private static readonly HashQueue<MyNetRequest> _requests = new();
                private static readonly HashQueue<MyNetResponse> _responses = new();

                public static event Action OnFinishThisHandling;
                public static event Action<MyNetResponse> OnReceived;

                internal static void HandleResponses()
                {
                    if (_responses.Count > 0)
                    {
                        while (_responses.TryDequeue(out var response))
                            OnReceived?.Invoke(response);

                        OnFinishThisHandling?.Invoke();
                    }
                }

                internal static bool HasRequest()
                {
                    return _requests.Count > 0;
                }

                internal static void Receive(MyNetResponse response)
                {
                    _responses.Enqueue(response);
                }

                public static void Send(MyNetRequest request)
                {
                    _requests.Enqueue(request);
                }

                internal static bool TryDequeue(out MyNetRequest request)
                {
                    return _requests.TryDequeue(out request);
                }
            }
        }
    }
}
