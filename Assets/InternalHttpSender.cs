using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalHttpSender : MonoBehaviour
    {
        private bool _isQuitting;

        public HttpClient HttpClient { get; set; }
        public bool IsLogging { get; set; }
        public MyNetRequest Request { get; set; }
        public string Token { get; set; }
        public Uri Uri { get; set; }

        public event Action OnError;
        public event Action<MyNetResponse> OnReceived;
        public event Action OnUnauthorized;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private async void Start()
        {
            try
            {
                var Bytes = await Task.Run(() => MyNetSerializer.Serialize(Request));
                if (IsAlive())
                {
                    if (IsLogging)
                        Debug.Log($"{name}> SEND: ({Bytes.Length} bytes)");

                    var request = new HttpRequestMessage(HttpMethod.Post, Uri)
                    {
                        Content = new ByteArrayContent(Bytes),
                    };

                    if (string.IsNullOrWhiteSpace(Token) == false)
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

                    var response = await HttpClient.SendAsync(request);
                    if (IsAlive())
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                if (IsAlive())
                                {
                                    if (await Task.Run(() => MyNetDeserializer.Deserialize(stream)) is MyNetResponse r)
                                    {
                                        if (IsAlive())
                                        {
                                            if (IsLogging)
                                                Debug.Log($"{name}> RECV ({stream.Length} bytes)");

                                            OnReceived?.Invoke(r);
                                        }
                                    }
                                    else
                                    {
                                        Debug.LogWarning($"{name}> RECV: UNIDENTIFIED RESPONSE.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"{name}> API TOKEN EXPIRED.");

                            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                                OnUnauthorized?.Invoke();
                            else
                                _ = response.EnsureSuccessStatusCode();
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Debug.LogWarning($"{name}> RECV: {e}");

                if (IsAlive())
                    OnError?.Invoke();
            }

            if (IsAlive())
                Destroy(gameObject);

            bool IsAlive() => (this != default) && (_isQuitting == false);
        }
    }
}
