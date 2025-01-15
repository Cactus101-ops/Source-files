using System;
using System.Collections.Generic;





[Serializable]
public class DogMemento
{
    public string savename;
    public bool isactive;

    
    [NonSerialized]
    public Dictionary<string, int> savestats;

    // A serializable representation of the dictionary because Unity is fussy
    public List<serializableKVP> serializedstats;

    
    public void serialization() //cearal-izes the dictionary
    {
        if (savestats == null) 
        {
            return;

        }

        serializedstats = new List<serializableKVP>();
        foreach (var kvp in savestats)
        {
            serializedstats.Add(new serializableKVP { Key = kvp.Key, Value = kvp.Value });
        }
    }

    // Convert serializable list back to dictionary because Unity is fussy
    public void revertserialization()
    {
        if (serializedstats == null) 
        {
            return;
        }
        savestats = new Dictionary<string, int>();
        foreach (var kvp in serializedstats)
        {
            savestats[kvp.Key] = kvp.Value;
        }
    }
}

[Serializable]
public class serializableKVP
{
    public string Key;
    public int Value;
}






[Serializable]
public class catangerstate
{//serializes the cat's name and the state that it was
    public string Name;
    
    public bool isangry; 


}

[System.Serializable]
public class GameData
{//holds the list of all things saved.
    public List<DogMemento> Dogs;
    public List<catangerstate> Cats;
    public int PlayerPPPoints;
    
    
    public string SelectedDogName;
    public string SelectedCatName;

}





[System.Serializable]
public class PPPointsWrapper
{
    public int pawpoints;
} //I don't actually need to use a wrapper for the points, I was just having some issues getting the int to serialize so I added this in rage. - either way it works lol


