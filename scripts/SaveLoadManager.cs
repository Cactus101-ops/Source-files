using System.Collections.Generic;
using System.IO;
using UnityEngine;




public class SaveLoadManager : MonoBehaviour
{
    
    private LogicManager LM;
    private GameObject dogprefab;
    private GameObject catprefab;
    public UIManager UI;
    public DogListUIManager DLUI;


    public List<ActionManager> actionmanagers;
    
    public void initialize(LogicManager logicmanager, DogListUIManager doglistui)
    {
        this.LM = logicmanager;
        this.DLUI = doglistui;
    }
    //=== Game Saving and Loading ===\\
    public void savegame()
    {
        //Serialize Dogs
        var dogmementos = new List<DogMemento>();

        

        foreach (var dog in LogicManager.Instance.dogsawake)
        {

            var memento = new DogMemento
            {
                savename = dog.dogname,
                

                isactive = true
            };

            memento.serialization(); // Convert the dictionary to a serializable list of key-value pairs because Unity can't serialize dictionaries with JSONUtility!!! THANKS FOR WASTING MY TIME!!!!
            dogmementos.Add(memento);

            Debug.Log($"Saving active dog: {dog.dogname}");
        }


        
        foreach (var dog in LogicManager.Instance.dogssleeping)
        {
            

            var memento = new DogMemento
            {
                savename = dog.dogname,
                
                isactive = false
            };
            memento.serialization();
            dogmementos.Add(memento);
        }

        string dogJson = JsonUtility.ToJson(new Wrapper<DogMemento> { saveitems = dogmementos }, true);
        string dogffilepath = System.IO.Path.Combine(Application.persistentDataPath, "Dogs.txt");
        File.WriteAllText(dogffilepath, dogJson);
        Debug.Log($"Saved {dogmementos.Count} dogs.");



        //Serialize Cats
        var catangerstates = new List<catangerstate>();
        foreach (var cat in LogicManager.Instance.activecats)
        {
            var state = new catangerstate //I only need to save these two things as cat's are permenant and don't get chased off from the game.
            {
                Name = cat.name,
                isangry = cat.isangry
            };

            catangerstates.Add(state);
        }

        string catJson = JsonUtility.ToJson(new Wrapper<catangerstate> { saveitems = catangerstates });
        string catFilePath = System.IO.Path.Combine(Application.persistentDataPath, "Cats.txt");
        File.WriteAllText(catFilePath, catJson);
       
        Debug.Log("Game saved.");


        //Serialize Points
        PPPointsWrapper ppWrapper = new PPPointsWrapper
        {
            pawpoints = LogicManager.Instance.pawpoints
        };

        string json = JsonUtility.ToJson(ppWrapper, true);
        string pointsfilepath = System.IO.Path.Combine(Application.persistentDataPath, "PawSatviePoints.txt");
        File.WriteAllText(pointsfilepath, json);

        Debug.Log($"Paw-Sative Points saved: {LogicManager.Instance.points}");

        Debug.Log("Game saved successfully!");
    }

    public void LoadGame()
    {
        string dogfilepath = System.IO.Path.Combine(Application.persistentDataPath, "Dogs.txt");
        string catfilepath = System.IO.Path.Combine(Application.persistentDataPath, "Cats.txt");
        string pointsfilepath = System.IO.Path.Combine(Application.persistentDataPath, "PawSatviePoints.txt");
        LogicManager.Instance.clearcatsanddogs(); //clears all lists first in order to prevent duplicates or "ghost dogs"...spooky

        if (!File.Exists(dogfilepath) || !File.Exists(catfilepath) || !File.Exists(pointsfilepath))
        {
            Debug.LogError("Save files not found!");
            return;
        }

        if (File.Exists(dogfilepath))
        {
            string dogJson = File.ReadAllText(dogfilepath);
            var wrapper = JsonUtility.FromJson<Wrapper<DogMemento>>(dogJson);
            if (wrapper?.saveitems != null)
            {
                LogicManager.Instance.dogssleeping.Clear();
                

                foreach (var memento in wrapper.saveitems)
                {
                    memento.revertserialization(); // Converts serialized list back to the dictionary for dogs 


                    GameObject doginstance = Instantiate(LogicManager.Instance.dogprefab);
                    Dog dogcomponent = doginstance.GetComponent<Dog>();
                    if (memento.isactive == true)
                    {
                        LogicManager.Instance.dogsawake.Add(dogcomponent);
                        LogicManager.Instance.Dog = doginstance; // Update's the reference here
                    }
                    else
                    {
                        LogicManager.Instance.dogssleeping.Add(dogcomponent);
                    }



                    dogcomponent.initialize(memento.savename, memento.savestats); // Initialize a dog with the stats that were saved
                    //if I give the new dog the exact same stats, is it still the same dog?


                    Debug.Log($"Loaded dog: {memento.savename}");
                }

                Debug.Log($"Loaded {LogicManager.Instance.dogsawake.Count} active dogs and {LogicManager.Instance.dogssleeping.Count} inactive dogs.");

            }
            else
            {
                Debug.LogWarning("No dog data found to load.");
            }
        }

        // Deserialize Cats
        string catsJson = File.ReadAllText(catfilepath);
        var catangerstates = JsonUtility.FromJson<Wrapper<catangerstate>>(catsJson).saveitems;

        foreach (var state in catangerstates)
        {
            var cat = LogicManager.Instance.findcatbyname(state.Name); //findcatbyname does as it says on the tin
            if (cat != null)
            {
                if (state.isangry == true)
                { 
                    cat.becomeangry(); 
                
                }    
                else
                {
                    cat.calmdown();
                }//resets a cat to neutral or angry depending on their state when the memento was saved
                   



            }
        }
            // Deserialize Points
            string pawsativepointsJson = File.ReadAllText(pointsfilepath);
            PPPointsWrapper loadedpointswrapper = JsonUtility.FromJson<PPPointsWrapper>(pawsativepointsJson);
            LogicManager.Instance.pawpoints = loadedpointswrapper.pawpoints;

            Debug.Log("Game loaded successfully!");

            UI.updatepointstext();
            DLUI.filldoglist();
        

    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> saveitems;
    }//the gameshark of JSON Utility
}

