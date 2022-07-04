public interface IDataService
{
    bool SaveData<T>(string relativePath, T Data, bool encrypted);

    T LoadData<T>(string relativePath, bool encrypted);
}