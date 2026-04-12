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
            internal static string Token { get; set; }
            internal static long UserId { get; set; }

            internal static InternalHttpSender CreateNew(MyNetRequest request, Uri uri)
            {
                var go = new GameObject(request.GetType().Name, typeof(InternalHttpSender));
                var c = go.GetComponent<InternalHttpSender>();
                c.HttpClient = HttpClient;
                c.IsLogging = true;
                c.Request = request;
                //c.Token = Token;
                c.Uri = uri;

                c.OnReceived += MyNet.Packets.Client.Receive;

                if (System.Diagnostics.Debugger.IsAttached)
                    HttpClient.Timeout = Timeout.InfiniteTimeSpan;

                return c;
            }
        }
    }
}
