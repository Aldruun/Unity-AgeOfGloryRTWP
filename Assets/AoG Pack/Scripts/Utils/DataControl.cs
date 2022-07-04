using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Schema;
using UnityEngine;

[Serializable]
public class DataControl
{
    private static string PERSISTENT_DATA_PATH = Application.persistentDataPath + "/saves/";

    public static void PPSavePositionV3(string dataName, Vector3 v3)
    {
        PlayerPrefs.SetFloat(dataName + "x", v3.x);
        PlayerPrefs.SetFloat(dataName + "y", v3.y);
        PlayerPrefs.SetFloat(dataName + "z", v3.z);
    }

    public static Vector3 PPLoadPositionV3(string dataName)
    {
        var v3 = new Vector3
        {
            x = PlayerPrefs.GetFloat(dataName + "x"),
            y = PlayerPrefs.GetFloat(dataName + "y"),
            z = PlayerPrefs.GetFloat(dataName + "z")
        };


        return v3;
    }

    public XmlSchema GetSchema()
    {
        throw new NotImplementedException();
    }

    public void ReadXml(XmlReader reader)
    {
    }

    public void WriteXml(XmlWriter writer)
    {
    }

    public static void Save<T>(T objectToSave, string key)
    {
        var path = PERSISTENT_DATA_PATH;
        Directory.CreateDirectory(path);
        var formatter = new BinaryFormatter();
        using (var fileStream = new FileStream(path + key + ".txt", FileMode.Create))
        {
            formatter.Serialize(fileStream, objectToSave);
        }
    }

    public static T Load<T>(string key)
    {
        var path = PERSISTENT_DATA_PATH;
        var formatter = new BinaryFormatter();
        T returnValue = default;
        using (var fileStream = new FileStream(path + key + ".txt", FileMode.Open))
        {
            returnValue = (T) formatter.Deserialize(fileStream);
        }

        return returnValue;
    }

    public static bool DataExists(string key)
    {
        var path = PERSISTENT_DATA_PATH + key + ".txt";
        return File.Exists(path);
    }

    public static void DeleteAllSaveFiles()
    {
        var path = PERSISTENT_DATA_PATH;
        var directory = new DirectoryInfo(path);
        directory.Delete();
        Directory.CreateDirectory(path);
    }
}

public class UniqueID
{
}