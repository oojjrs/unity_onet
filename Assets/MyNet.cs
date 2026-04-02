using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

namespace oojjrs.onet
{
    public static partial class MyNet
    {
        // TODO: 아직 indexing 기능은 지원하지 않...
        public struct Field
        {
            public enum VisibilityEnum
            {
                Public,
                Member,
                Private,
            }

            public string key;
            public string value;
            public VisibilityEnum visibility;
        }

        internal const string PlayerPropertyNickname = "__Nickname__";

        internal static MyNetException ToException(LobbyServiceException e)
        {
            return new(e.Reason.ToString(), (int)e.Reason, e.Message, e);
        }

        internal static Dictionary<string, PlayerDataObject> ToPlayerData(IEnumerable<Field> fields)
        {
            return ToPlayerData(fields, string.Empty);
        }

        internal static Dictionary<string, PlayerDataObject> ToPlayerData(IEnumerable<Field> fields, string nickname)
        {
            var dic = fields.ToDictionary(t => t.key, t => ToPlayerDataObject(t));
            if (string.IsNullOrWhiteSpace(nickname) == false)
                dic.Add(MyNet.PlayerPropertyNickname, new(PlayerDataObject.VisibilityOptions.Public, nickname));

            return dic;

            static PlayerDataObject ToPlayerDataObject(MyNet.Field field)
            {
                return new PlayerDataObject(Convert(field.visibility), field.value);

                static PlayerDataObject.VisibilityOptions Convert(MyNet.Field.VisibilityEnum e)
                {
                    return e switch
                    {
                        MyNet.Field.VisibilityEnum.Public => PlayerDataObject.VisibilityOptions.Public,
                        MyNet.Field.VisibilityEnum.Member => PlayerDataObject.VisibilityOptions.Member,
                        MyNet.Field.VisibilityEnum.Private => PlayerDataObject.VisibilityOptions.Private,
                        _ => HandleException(e),
                    };

                    static PlayerDataObject.VisibilityOptions HandleException(MyNet.Field.VisibilityEnum e)
                    {
                        UnityEngine.Debug.LogWarning($"{typeof(MyNet).Namespace}> UNEXPECTED VALUE: {e}. FALLING BACK TO PUBLIC.");
                        return PlayerDataObject.VisibilityOptions.Public;
                    }
                }
            }
        }

        // 사실 사용처는 룸 밖에 없는데, 모양 맞추느라 여기 갖다놨다.
        internal static Dictionary<string, DataObject> ToRoomData(IEnumerable<Field> fields)
        {
            return fields.ToDictionary(t => t.key, t => ToRoomDataObject(t));

            static DataObject ToRoomDataObject(MyNet.Field field)
            {
                return new DataObject(Convert(field.visibility), field.value);

                static DataObject.VisibilityOptions Convert(MyNet.Field.VisibilityEnum e)
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
                        UnityEngine.Debug.LogWarning($"{typeof(MyNet).Namespace}> UNEXPECTED VALUE: {e}. FALLING BACK TO PUBLIC.");
                        return DataObject.VisibilityOptions.Public;
                    }
                }
            }
        }
    }
}
