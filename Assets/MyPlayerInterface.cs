namespace oojjrs.onet
{
    public interface MyPlayerInterface
    {
        string Id { get; }
        bool IsHost { get; }
        string Nickname { get; set; }

        string GetData(string key);
    }
}
