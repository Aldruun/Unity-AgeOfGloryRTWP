using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonDataService : IDataService
{
    public bool SaveData<T>(string relativePath, T data, bool encrypted)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Auto;
        string path = Application.persistentDataPath + relativePath;

        try
        {
            if(File.Exists(path))
            {
                Debug.Log("Save data exists. Deleting old file and writing a new one");

                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing file for the first time");
            }

            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented, settings));
            return true;
        }
        catch(Exception e)
        {
            Debug.LogError($"Unable to save due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }

    public T LoadData<T>(string relativePath, bool encrypted)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Auto;
        string path = Application.persistentDataPath + relativePath;

        if(File.Exists(path) == false)
        {
            Debug.LogError($"Unable to load file at path {path} - File does not exist");
            throw new FileNotFoundException($"Path {path} does not exist");
        }

        try
        {
            T data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), settings);
            Debug.Log("Loaded data");
            return data;
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message} {e.StackTrace}");
            throw;
        }
    }
}
