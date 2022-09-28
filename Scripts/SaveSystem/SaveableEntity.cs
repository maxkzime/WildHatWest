using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
    // Attach this script on any gameObject that has data we want to save.

    [SerializeField] private string id;

    public string Id => id;

    [ContextMenu("Generate ID.")]
    private void GenerateId()
    {
        id = Guid.NewGuid().ToString();
    }


    public object SaveState()
    {
        var state = new Dictionary<string, object>();

        foreach(ISaveable saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.SaveState();
        }

        return state;
    }



    public void LoadState(object state)
    {
        Dictionary<string, object> stateDictionary = (Dictionary<string, object>)state;

        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();

            if( stateDictionary.TryGetValue(typeName, out object savedState))
            {
                saveable.LoadState(savedState);
            }
        }
    }
}
