using System.Collections;
using System.Net;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalUdpSenderHandler : MonoBehaviour
    {
        private bool _isQuitting;

        private InternalUdpSender CurrentSender { get; set; }
        public bool IsLogging { get; set; }
        public float NetworkCooldownTimeSeconds { get; set; } = 1;
        private byte[] PendingPacket { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }
        public int RetryCount { get; set; } = 5;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void Start()
        {
            _ = StartCoroutine(Func());

            IEnumerator Func()
            {
                if (IsLogging)
                    Debug.Log($"{name}> {RemoteEndPoint}를 담당하고 있습니다.");

                while (true)
                {
                    yield return new WaitUntil(() => (PendingPacket != default) || MyNet.Udp.Has(RemoteEndPoint));

                    if (_isQuitting)
                        break;

                    byte[] bytes;
                    if (PendingPacket != default)
                    {
                        bytes = PendingPacket;
                        PendingPacket = default;
                    }
                    else
                    {
                        MyNet.Udp.TryDequeue(RemoteEndPoint, out bytes);
                    }

                    if (bytes != default)
                    {
                        for (int i = 0; i < RetryCount; ++i)
                        {
                            var go = new GameObject(bytes.GetType().Name, typeof(InternalUdpSender));
                            CurrentSender = go.GetComponent<InternalUdpSender>();
                            CurrentSender.Bytes = bytes;
                            CurrentSender.RemoteEndPoint = RemoteEndPoint;

                            CurrentSender.OnError += () =>
                            {
                                Debug.LogWarning($"UDP SEND ERROR> {CurrentSender.name}");

                                PendingPacket = bytes;
                                CurrentSender = default;
                            };
                            CurrentSender.OnSent += () => CurrentSender = default;

                            yield return new WaitUntil(() => CurrentSender == default);

                            if (PendingPacket == default)
                                break;
                        }
                    }

                    if (PendingPacket != default)
                        yield return new WaitForSeconds(NetworkCooldownTimeSeconds);
                }

                Destroy(gameObject);
            }
        }
    }
}
