using UnityEngine;
using UnityEngine.UI;

public class PetFactory : MonoBehaviour
{
    public static PetFactory Instance { get; private set;}

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        Instance.initialize(LogicManager.Instance, UIManager.instance);
    }

    private LogicManager LM;
    public UIManager UI;


    private void initialize(LogicManager logicmanager, UIManager uimanager)
    {
        this.LM = logicmanager;
    }

    public void loadbutton(Button spawnbutton)
    {
        spawnbutton.onClick.RemoveAllListeners(); //this makes sure the game doesn't spam or create multiple dogs or cat's when their spawn buttons are clicked
        
    }

   
    public GameObject createdog()
    {
        if (LM.pawpoints >= 100)
        {
            
            LM.subtractpppoints(100);

            
            if (UI == null)
            {
                UI = FindObjectOfType<UIManager>();
            }
            UI.updatepointstext(); // UIManager reference is initialized to avoid nullref when updating UI.

            GameObject doginstance = Instantiate(LM.dogprefab, LM.spawnpoint.position, Quaternion.identity);
            Dog dogcomponent = doginstance.GetComponent<Dog>();

            string dogname = LM.dognames[UnityEngine.Random.Range(0, LM.dognames.Length)];
            LM.dogname = dogname;
            dogcomponent.initialize(dogname);

            LM.Dog = doginstance;
            Debug.Log($"Dog {dogname} created and added to awake dogs.");

            LM.DoM.activatedog(dogcomponent);
            return doginstance;
        }
        else
        {
            Debug.Log("Not enough Paw-Sative Points to spawn a dog.");
            return null;
        }
    }

    public GameObject createcat()
    {
        string catname = LM.catnames[UnityEngine.Random.Range(0, LM.catnames.Length)]; //This asigns a random name from the catnames list in the logic manager
        

        while (iscatnameinuse(catname))
        {
            catname = LM.catnames[UnityEngine.Random.Range(0, LM.catnames.Length)]; // This code assigns a new random name if the one that was previously picked, was already in use
        }

        Debug.Log($"Spawning cat with name: {catname}");
        Transform spawnpoint = LM.getrandomspawn(LM.occupiedspawnpoints, LM.spawnpoints);
        if (LM.mapspawnpointtocollider.ContainsKey(spawnpoint))
        {


            ActionManager actionManager = LM.mapspawnpointtocollider[spawnpoint];


            GameObject catinstance = Instantiate(LM.catprefab, spawnpoint.position, Quaternion.identity);
            Cat catcomponent = catinstance.GetComponent<Cat>();

            catcomponent.initialize(catname);


            actionManager.setcat(catcomponent);

            activecatadd(catcomponent);
            return catinstance;
        }
        return null;

    }

    public void activecatadd(Cat cat)
    {
        if (!LogicManager.Instance.activecats.Contains(cat))
        {
            LogicManager.Instance.activecats.Add(cat);
        }
    }

    private bool iscatnameinuse(string name)
    {
        foreach (var cat in LogicManager.Instance.activecats)
        {
            if (cat.name == name)
            {
                return true; // the name is already in use
            }
        }
        return false; // the name is available
    }


}

     