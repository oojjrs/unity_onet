namespace oojjrs.onet
{
    public interface MyNetPlayerInterface
    {
        string Id { get; }
        bool IsHost { get; }
        string Nickname { get; }

        string GetData(string key);
    }
}
