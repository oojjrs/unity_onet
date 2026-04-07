using System;
using System.Net.Http;
using System.Threading;
using UnityEngine;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        public static partial class Packets
        {
            private static HttpClient HttpClient { get; } = new();
            private static readonly HashQueue<MyNetRequest> _requests = new();
            private static readonly HashQueue<MyNetResponse> _responses = new();
            public static string Token { get; set; }
            public static long UserId { get; set; }

            internal static InternalHttpSender CreateNew(MyNetRequest request, Uri uri)
            {
                var go = new GameObject(request.GetType().Name, typeof(InternalHttpSender));
                var c = go.GetComponent<InternalHttpSender>();
                c.HttpClient = HttpClient;
                c.IsLogging = true;
                c.Request = request;
                //c.Token = Token;
                c.Uri = uri;

                c.OnReceived += Receive;

                if (System.Diagnostics.Debugger.IsAttached)
                    HttpClient.Timeout = Timeout.InfiniteTimeSpan;

                return c;
            }

            public static bool HasRequest()
            {
                return _requests.Count > 0;
            }

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
