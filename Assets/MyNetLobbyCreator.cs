using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetLobbyCreator : MonoBehaviour
    {
        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.CreateLobbyAsync(MyNet.Lobby.Config.Title, MyNet.Lobby.Config.MaxPlayers, new()
                {
                    Data = MyNet.Lobby.Config.LobbyFields.ToDictionary(t => t.key, t => ToLobbyDataObject(t)),
                    IsPrivate = MyNet.Lobby.Config.IsPrivate,
                    Player = new(id: MyNet.Lobby.Config.Account, data: MyNet.Lobby.Config.PlayerFields.ToDictionary(t => t.key, t => ToPlayerDataObject(t))),
                });

                if (this != default)
                {
                    if (lobby != default)
                        MyNet.Lobby.RaiseCreated(lobby);
                    else
                        MyNet.Lobby.RaiseCreateFailed();
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning(e);

                MyNet.Lobby.RaiseException(e);
            }

            if (this != default)
                MyNet.Lobby.StopCreate();

            DataObject ToLobbyDataObject(MyNet.Lobby.ConfigInterface.Field field)
            {
                return new DataObject(Convert(field.visibility), field.value);

                DataObject.VisibilityOptions Convert(MyNet.Lobby.ConfigInterface.Field.VisibilityEnum e)
                {
                    return e switch
                    {
                        MyNet.Lobby.ConfigInterface.Field.VisibilityEnum.Public => DataObject.VisibilityOptions.Public,
                        MyNet.Lobby.ConfigInterface.Field.VisibilityEnum.Member => DataObject.VisibilityOptions.Member,
                        MyNet.Lobby.ConfigInterface.Field.VisibilityEnum.Private => DataObject.VisibilityOptions.Private,
                        _ => HandleException(e),
                    };

                    DataObject.VisibilityOptions HandleException(MyNet.Lobby.ConfigInterface.Field.VisibilityEnum e)
                    {
                        Debug.LogWarning($"{name}> UNEXPECTED VALUE: {e}. FALLING BACK TO PUBLIC.");
                        return DataObject.VisibilityOptions.Public;
                    }
                }
            }

            PlayerDataObject ToPlayerDataObject(MyNet.Lobby.ConfigInterface.Field field)
            {
                return new PlayerDataObject(Convert(field.visibility), field.value);

                PlayerDataObject.VisibilityOptions Convert(MyNet.Lobby.ConfigInterface.Field.VisibilityEnum e)
                {
                    return e switch
                    {
                        MyNet.Lobby.ConfigInterface.Field.VisibilityEnum.Public => PlayerDataObject.VisibilityOptions.Public,
                        MyNet.Lobby.ConfigInterface.Field.VisibilityEnum.Member => PlayerDataObject.VisibilityOptions.Member,
                        MyNet.Lobby.ConfigInterface.Field.VisibilityEnum.Private => PlayerDataObject.VisibilityOptions.Private,
                        _ => HandleException(e),
                    };

                    PlayerDataObject.VisibilityOptions HandleException(MyNet.Lobby.ConfigInterface.Field.VisibilityEnum e)
                    {
                        Debug.LogWarning($"{name}> UNEXPECTED VALUE: {e}. FALLING BACK TO PUBLIC.");
                        return PlayerDataObject.VisibilityOptions.Public;
                    }
                }
            }
        }
    }
}
