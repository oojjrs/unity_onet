namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static partial class Packets
        {
            private static readonly HashQueue<MyRequest> _requests = new();
            private static readonly HashQueue<MyResponse> _responses = new();

            public static void Receive(MyResponse response)
            {
                _responses.Enqueue(response);
            }

            public static void Send(MyRequest request)
            {
                _requests.Enqueue(request);
            }

            public static bool TryDequeue(out MyRequest request)
            {
                return _requests.TryDequeue(out request);
            }

            public static bool TryDequeue(out MyResponse response)
            {
                return _responses.TryDequeue(out response);
            }
        }
    }
}
