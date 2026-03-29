using System;
using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace oojjrs.onet
{
    public class MyNetRoomCreator : MonoBehaviour
    {
        public MyNet.Room.CreateConfigInterface Config { get; set; }

        public event Action<LobbyServiceException> OnException;
        public event Action OnFailed;
        public event Action<Lobby> OnOk;

        private async void Start()
        {
            try
            {
                var lobby = await LobbyService.Instance.CreateLobbyAsync(Config.Title, Config.MaxPlayers, new()
                {
                    Data = Config.LobbyFields.ToDictionary(t => t.key, t => ToLobbyDataObject(t)),
                    IsPrivate = Config.IsPrivate,
                    Player = new(id: Config.Account, data: MyNet.ToPlayerData(Config.PlayerFields)),
                });

                if (this != default)
                {
                    if (lobby != default)
                        OnOk?.Invoke(lobby);
                    else
                        OnFailed?.Invoke();
                }
            }
            catch (LobbyServiceException e)
            {
                OnException?.Invoke(e);
            }

            if (this != default)
                MyNet.Room.StopCreate();

            DataObject ToLobbyDataObject(MyNet.Field field)
            {
                return new DataObject(Convert(field.visibility), field.value);

                DataObject.VisibilityOptions Convert(MyNet.Field.VisibilityEnum e)
                {
                    return e switch
                    {
                        MyNet.Field.VisibilityEnum.Public => DataObject.VisibilityOptions.Public,
                        MyNet.Field.VisibilityEnum.Member => DataObject.VisibilityOptions.Member,
                        MyNet.Field.VisibilityEnum.Private => DataObject.VisibilityOptions.Private,
                        _ => HandleException(e),
                    };

                    DataObject.VisibilityOptions HandleException(MyNet.Field.VisibilityEnum e)
                    {
                        Debug.LogWarning($"{name}> UNEXPECTED VALUE: {e}. FALLING BACK TO PUBLIC.");
                        return DataObject.VisibilityOptions.Public;
                    }
                }
            }
        }
    }
}
