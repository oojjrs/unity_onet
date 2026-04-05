namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static partial class Packets
        {
            private static readonly HashQueue<MyNetRequest> _requests = new();
            private static readonly HashQueue<MyNetResponse> _responses = new();

            public static void Receive(MyNetResponse response)
            {
                _responses.Enqueue(response);
            }

            public static void Send(MyNetRequest request)
            {
                _requests.Enqueue(request);
            }

            public static bool TryDequeue(out MyNetRequest request)
            {
                return _requests.TryDequeue(out request);
            }

            public static bool TryDequeue(out MyNetResponse response)
            {
                return _responses.TryDequeue(out response);
            }
        }
    }
}
