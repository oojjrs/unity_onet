namespace oojjrs.onet
{
    public interface MyNetCallbacksInterface
    {
        public enum FailureEnum
        {
            EmptyCode,
            EmptyPlayerId,
            EmptyRoomId,
            NotFoundRoom,
            NotPermitted,
        }

        void OnBusy();
        void OnException(MyNetSessionException e);
        void OnFailed(FailureEnum e);
    }
}
