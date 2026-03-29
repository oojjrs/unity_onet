using Unity.Services.Lobbies;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetLobbyUpdater : MonoBehaviour
    {
        private float _nextLobbyUpdateAtSeconds;

        private async void Update()
        {
            // time scale이나 프레임 영향이 없어야 하므로 Time.time을 사용할 수 없다.
            var time = Time.realtimeSinceStartup;
            if (MyNet.Lobby.UpdateRequested || (time >= _nextLobbyUpdateAtSeconds))
            {
                _nextLobbyUpdateAtSeconds = time + MyNet.Lobby.UpdateIntervalSeconds;

                MyNet.Lobby.UpdateRequested = false;

                try
                {
                    var response = await LobbyService.Instance.QueryLobbiesAsync();
                    if (this != default)
                        MyNet.Lobby.Update(response.Results);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogWarning(e);

                    MyNet.Lobby.RaiseException(e);
                }
            }
        }
    }
}
