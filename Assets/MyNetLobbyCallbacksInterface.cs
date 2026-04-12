using System.Collections.Generic;

namespace oojjrs.onet
{
    public interface MyNetLobbyCallbacksInterface : MyNetCallbacksInterface
    {
        void OnOk(IEnumerable<MyNetRoomInterface> rooms);
    }
}
