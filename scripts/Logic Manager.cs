using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;






public class LogicManager : MonoBehaviour
{
    

    

    [Header("public variables")]
    public int pawpoints;
    public GameObject Dog;
    public Transform spawnpoint;

    [SerializeField] public GameObject dogprefab;
    [SerializeField] public GameObject catprefab;
    [SerializeField] public Transform[] spawnpoints;
    [SerializeField] public Transform[] points;

    public string[] dognames;
    public string dogname;
    public string[] catnames;

    

    public Dictionary<Transform, ActionManager> mapspawnpointtocollider = new Dictionary<Transform, ActionManager>();

    [Header("Lists")]
    public List<Dog> dogsawake = new List<Dog>();
    public List<Dog> dogssleeping = new List<Dog>();
    public List<Cat> activecats = new List<Cat>();
    private Transform targetpoint;
    public List<ActionManager> colliders;
    public GameObject selectedCollider; 

    public List<Transform> occupiedspawnpoints = new List<Transform>();

    [Header("Script References")]
    private UIManager UI;
    private GameData GD;
    private ActionManager AM;
    private SaveLoadManager SLM;
    private PetFactory PF;
    public DogManager DoM;
    public DogListUIManager DLUI;
    
    

    [Header("Misc References")]
    public Button dogbutton;  
    public Button catbutton;
    public Slider progressbar;



    public static LogicManager Instance { get; private set; }

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        initialize();

        string dogfilepath = System.IO.Path.Combine(Application.persistentDataPath, "Dogs.txt");
       string catfilepath = System.IO.Path.Combine(Application.persistentDataPath, "Cats.txt");
       string pointsfilepath = System.IO.Path.Combine(Application.persistentDataPath, "PawSativePoints.txt");

}


    private void Start()
    {
        initializespawnpointmap();
        PF = PetFactory.Instance;
        PetFactory.Instance.loadbutton(dogbutton);
        PetFactory.Instance.loadbutton(catbutton);


        DoM = new DogManager();
    }
   
    private void initialize()
    {
        if (UI == null)
        {
            UI = FindObjectOfType<UIManager>();
            
        }

        colliders = new List<ActionManager>();
        
        activecats = new List<Cat>();
        
    }

    //=== spawn dogs and cats ===\\
    public void createdogbuttonclicked()
    {
        
        GameObject dog = PF.createdog();
        AudioManager.Instance.playhappybark();
    }

    public void createcatbuttonclicked()
    {
        
        GameObject cat = PF.createcat();
        
    }

    //=== maps cat spawans to colliders ===\\
    private void initializespawnpointmap()
    {
        foreach (Transform spawnpoint in spawnpoints)
        {
            var link = spawnpoint.GetComponent<ColliderLink>();// gets whatever collider is being held in the colliderlink script for each instance of a cat spawn point.

            mapspawnpointtocollider[spawnpoint] = link.associatedcollider;
        }
    }

    //=== gets random spawn point for cats ===\\

    public Transform getrandomspawn(List<Transform> occupiedspawnpoints, Transform[] spawnpoints)
    {
        

        List<Transform> freespawnpoints = new List<Transform>();
        foreach (Transform sp in spawnpoints)
        {
            if (!occupiedspawnpoints.Contains(sp))
            {
                freespawnpoints.Add(sp);
            }
        }


        if (freespawnpoints.Count == 0)
        {
            Debug.LogError("All spawn points are occupied! Can't spawn more cats!");
            return null;
        }

        Transform selectedSpawnPoint = freespawnpoints[UnityEngine.Random.Range(0, freespawnpoints.Count)];
        occupiedspawnpoints.Add(selectedSpawnPoint);
        return selectedSpawnPoint;
    }
    

    

    

    //=== For Setting the Collider the player clicks on ===\\
    public void setcollider(GameObject collider)
    {
        selectedCollider = collider;
        Debug.Log($"Selected collider set to: {collider.name}");
    }

    //=== Get collider's target point for moving the dog ===//
    public void settarget(Transform point)
    {
        targetpoint = point;
        Debug.Log($"set to: {point.position}");
    }

    public Transform gettarget()
    {
        return targetpoint;
    }



    //=== stat comparison system that compares dog stats against cat stats ===\\
    public static bool comparestats(Dictionary<string, int> dogstats, Dictionary<string, int> catstats, string stattocompare, LogicManager logicmanager, Dog dog, Cat cat)
    {

        if (dogstats.ContainsKey(stattocompare) && catstats.ContainsKey(stattocompare))
        {
            int catvalue = catstats[stattocompare];
            int dogvalue = dogstats[stattocompare];
            Debug.Log($"Comparing {stattocompare}: Dog = {dogvalue}, Cat = {catvalue}");

            if (dogvalue >= catvalue)
            {
                Debug.Log($"Good job, {dog.Name}! You earned bonus Paw-Sative Points!");
                logicmanager.worksuccess(dog.gameObject);
                cat.calmdown();
                
            }
            else
            {
                Debug.LogError($"{cat.name} scared off a dog. Better luck next time!");
                logicmanager.dogsawake.Remove(dog);
                UnityEngine.Object.Destroy(dog.gameObject);
                AudioManager.Instance.playworkfail();
                cat.becomeangry();
                LogicManager.Instance.DLUI.filldoglist();
                return false;
            }
        }return true;
    }

    public void worksuccess(GameObject Dog)
    {
        
        Dog dogComponent = Dog.GetComponent<Dog>();
        pawpoints += 100;
        UI.updatepointstext();
        Debug.Log("Paw-Sative Points added for good work!");
        AudioManager.Instance.playworksuccess();
        DoM.deactivatedog(dogComponent);
        LogicManager.Instance.DLUI.filldoglist();

    }
    
   
    public void subtractpppoints(int amount)
    {
        pawpoints -= amount;
    }

    //=== Helper Methods For the SaveLoadManager ===\\
    public void clearcatsanddogs()
    {
        foreach (var dog in dogsawake)
        {
            if (dog?.gameObject != null)
            {
                Destroy(dog.gameObject);
            }
        }
        dogsawake.Clear();

        foreach (var dog in dogssleeping)
        {
            if (dog?.gameObject != null)
            {
                Destroy(dog.gameObject);
            }
        }
        dogssleeping.Clear();

        

        Dog = null;
        
        Debug.Log("Cleared all existing cats and dogs.");
    }

    public Cat findcatbyname(string name)
    {
        foreach (var cat in activecats)
        {
            if (cat.name == name)
                return cat; 
        }
        return null; 
    }



}



public class DogManager
{
    //=== class for controling a dogs state ===\\
    
    public void activatedog(Dog dog)
    {
        if (!LogicManager.Instance.dogsawake.Contains(dog))
        {
            AudioManager.Instance.playalarmclock();
            LogicManager.Instance.dogsawake.Add(dog);
            LogicManager.Instance.dogssleeping.Remove(dog);
            LogicManager.Instance.DLUI.filldoglist();
        }
    }

    public void deactivatedog(Dog dog)
    {
        if (LogicManager.Instance.dogsawake.Contains(dog))
        {
            LogicManager.Instance.DLUI.filldoglist();
            LogicManager.Instance.dogsawake.Remove(dog);
            LogicManager.Instance.dogssleeping.Add(dog);
            LogicManager.Instance.StartCoroutine(reactivatedog(dog, 20f));
        }

        teleportdogtospawn(dog);
    }
    public IEnumerator reactivatedog(Dog dog, float delay)//9 out of 10 cats love countdown.
    {
        float returntime = 0f;
        
        while(returntime < delay)
        {

            returntime += Time.deltaTime;
            if (LogicManager.Instance.progressbar != null)
            {
                LogicManager.Instance.progressbar.value = returntime / delay;
            }

            yield return null;
        }
        LogicManager.Instance.DoM.activatedog(dog);
        Debug.Log($"{dog.Name} reactivated after {delay} seconds.");
        LogicManager.Instance.progressbar.value = 0f;
    }
    public void teleportdogtospawn(Dog dog)
    {
        dog.transform.position = LogicManager.Instance.spawnpoint.position;
        
    }

   

}




   

