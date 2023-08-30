namespace MVCSite.Interfaces;

public interface ISphinxConnector
{
    public List<object[]> GetData(string command);
}
