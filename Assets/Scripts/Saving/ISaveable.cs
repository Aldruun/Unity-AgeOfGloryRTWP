namespace AoG.Serialization
{
    public interface ISaveable
    {
        object CollectData();
        //void RestoreState(object state);
    }
}