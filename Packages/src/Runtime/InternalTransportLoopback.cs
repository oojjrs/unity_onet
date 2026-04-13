using UnityEngine;

namespace oojjrs.onet
{
    internal class InternalTransportLoopback : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            {
                while (MyNet.Packets.Server.TryDequeue(out MyNetResponse response))
                    MyNet.Packets.Client.Receive(response);

                MyNet.Packets.Client.HandleResponses();
            }

            // 의도적으로 요청은 응답보다 늦게 처리하는 것이다.
            {
                while (MyNet.Packets.Client.TryDequeue(out MyNetRequest request))
                    MyNet.Packets.Server.Receive(request);

                MyNet.Packets.Server.HandleRequests();
            }
        }
    }
}
