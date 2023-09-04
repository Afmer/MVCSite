namespace MVCSite.Interfaces;
public interface ISearchCacheUpdater
{
    public void UpdateCache(TimeSpan lifeTime);
}