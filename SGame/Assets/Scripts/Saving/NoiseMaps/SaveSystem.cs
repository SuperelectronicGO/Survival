using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveSystem
{
    public static bool checkForFile()
    {
        string path = Application.persistentDataPath + "/NoiseMaps.sav";
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void saveMaps(MapGenerator generator)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/NoiseMaps.sav";
        FileStream stream = new FileStream(path, FileMode.Create);
        NoiseMapData data = new NoiseMapData(generator);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static NoiseMapData loadMapData()
    {
        string path = Application.persistentDataPath + "/NoiseMaps.sav";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            NoiseMapData data = formatter.Deserialize(stream) as NoiseMapData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("File not found at " + path);
            return null;
        }
    }
}
