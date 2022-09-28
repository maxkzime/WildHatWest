using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class SaveLoadSystem : MonoBehaviour
{

    [ContextMenu("Save.")]
    public void Save()
    {
        // Null or previous save.
        var state = LoadFile();

        SaveState(state);

        SaveFile(state);
    }

    [ContextMenu("Load.")]
    public void Load()
    {
        var state = LoadFile();
        LoadState(state);
    }


    public string SavePath => $"{Application.persistentDataPath}/save.txt";

    public void SaveFile(object state)
    {
        using (var stream = File.Open(SavePath, FileMode.Create))
        {
            BinaryFormatter formatter = new();
            formatter.Serialize(stream, state);
        }
    }



    Dictionary<string, object> LoadFile()
    {
        if(!File.Exists(SavePath))
        {
            // New file if file doesn't exists.
            //Debug.Log("No save file found");
            return new Dictionary<string, object>();
        }

        using (FileStream stream = File.Open(SavePath, FileMode.Open))
        {
            BinaryFormatter formatter = new();
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }
    }


    // Telling every objects with saveable entity, to save their state
    void SaveState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<SaveableEntity>())
        {
            state[saveable.Id] = saveable.SaveState();
        }
    }


    void LoadState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<SaveableEntity>())
        {
            if(state.TryGetValue(saveable.Id, out object savedState))
            {
                saveable.LoadState(savedState);
            }
        }
    }
}
