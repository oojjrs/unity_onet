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

                internal static void Receive(MyNetRequest request)
                {
                    _requests.Enqueue(request);
                }

                public static void Send(MyNetResponse response)
                {
                    _responses.Enqueue(response);
                }

                public static bool TryDequeue(out MyNetRequest request)
                {
                    return _requests.TryDequeue(out request);
                }

                internal static bool TryDequeue(out MyNetResponse response)
                {
                    return _responses.TryDequeue(out response);
                }
            }
        }
    }
}
