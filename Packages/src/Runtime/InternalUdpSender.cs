using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalUdpSender : MonoBehaviour
    {
        private bool _isQuitting;

        public byte[] Bytes { get; set; }
        public bool IsLogging { get; set; }
        public IPEndPoint RemoteEndPoint { get; set; }

        public event Action OnError;
        public event Action OnSent;

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
                if (IsLogging)
                    Debug.Log($"{name}> UDP SEND ({Bytes.Length} bytes) to {RemoteEndPoint}");

                using (var udpClient = new UdpClient())
                {
                    var ret = await udpClient.SendAsync(Bytes, Bytes.Length, RemoteEndPoint);
                    if (IsAlive())
                    {
                        if (ret == Bytes.Length)
                            OnSent?.Invoke();
                        else
                            OnError?.Invoke();
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                if (IsAlive())
                    OnError?.Invoke();
            }

            if (IsAlive())
                Destroy(gameObject);

            bool IsAlive() => (this != default) && (_isQuitting == false);
        }
    }
}
