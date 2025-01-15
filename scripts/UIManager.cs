using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }

    public bool inuse = false;
    public bool forlevelup = false;//signifies whether the stat screen is being used for working or for leveling up.
    public bool dupecheck = false; //checks for dupe

    [Header("Dog References")]
    public GameObject dogbuttonprefab;
    public Transform doglistcontainer_D1;
    public Transform doglistcontainer_D2;

    [Header("UI")]
    public GameObject dogselectionUI_D1;
    public GameObject dogselectionUI_D2;
    public GameObject statscreen_D1;
    public GameObject statscreen_D2;
    public GameObject closebutton;
    
    [Header("Paw-Sative Points")]
    public TMP_Text PPpointsText;

    [Header("Manager References")]
    public List<ActionManager> AMS;
    public LogicManager LM;
    public DogManager DM;

    [Header("Selected dog/cat")]
    public Dog selecteddog_D1;//D1 means dog 1 for work
    public Dog selecteddog_D2;//D2 means dog 2 for leveling up shtuff!
    public Cat selectedcat;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        initializemanagers();
        
        
    }

    private void Start()
    {
        updatepointstext();
    }
    public void ResetGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void initializemanagers()
    {
        AMS = FindObjectsOfType<ActionManager>().ToList();
        LM = FindObjectOfType<LogicManager>();
        
    }
    public void Closebutton()
    {
        hideallUI();
    }   
    private void hideallUI()
    {
        dogselectionUI_D1?.SetActive(false);
        dogselectionUI_D2?.SetActive(false);
        statscreen_D1?.SetActive(false);
        statscreen_D2?.SetActive(false);
        closebutton?.SetActive(false);
        inuse = false;
        selecteddog_D1.transform.position = LogicManager.Instance.spawnpoint.position;
    }

   

    //=== WORK SYSTEM (D1) ===//

    public void catselected(Cat cat)
    {
        inuse = true;
        selectedcat = cat;
        Debug.Log($"Cat selected: {selectedcat.name}");
        forlevelup = false;
        closebutton?.SetActive(true);
        dogselectionUI_D1.SetActive(true);
        doglistcontainer_D1.gameObject.SetActive(true);
        
        filldoglist();
      }
    public void dogselected(Dog dog) //when the player selects a dog for work
    {
        
        selecteddog_D1 = dog;
        Debug.Log($"Dog selected: {selecteddog_D1.dogname}");

        closebutton?.SetActive(false);
        dogselectionUI_D1.SetActive(false);
        doglistcontainer_D1.gameObject.SetActive(false);
        
        statscreen_D1.SetActive(true);//opens the stat screen

        if (selectedcat == null)
        {
            Debug.LogError("No cat selected stat screen can't be loaded");
            return;
        }

        fillstatselectionUI(selectedcat);
        dog.movetopoint(LM.gettarget());
    }

    public void fillstatselectionUI(Cat cat) //populates the stat screen with the stat buttons of whatever stats a cat has
    {
        Transform buttoncontainer = statscreen_D1?.transform.Find("StatButtonsContainer");//searches the stat screen for the container in it's heirarchy

        fillstatbuttons(buttoncontainer, cat.catstats, false);
    }

    public void statbuttonclicked(string statname) 
    {
        inuse = false;
        bool success = LogicManager.comparestats(selecteddog_D1.GetComponent<DogStats>().stats, selectedcat.catstats, statname, LM, selecteddog_D1, selectedcat); //this calls the stat comparison system to compare stats
        if (success == true)
        {
            Debug.Log("Stat comparison succeeded.");
        }
        else
        {
            Debug.Log("Stat comparison failed.");
        }

        statscreen_D1.SetActive(false);
        closebutton?.SetActive(false);

    }

    //=== STAT LEVELING (D2) ===//

    public void openlevelupscreen() 
    {
        inuse = true;
        forlevelup = true; //sets the flag to true to signify the stat screen is being used to level up.
        closebutton?.SetActive(true);
        dogselectionUI_D2?.SetActive(true);
        doglistcontainer_D2?.gameObject.SetActive(true);
        filldoglistforlevelup();
    }

    public void dogselectedforlevelUp(Dog dog)
    { 
        selecteddog_D2 = dog;
        Debug.Log($"Dog selected for leveling up: {selecteddog_D2.dogname}");
        closebutton?.SetActive(false);
        dogselectionUI_D2.SetActive(false);
        doglistcontainer_D2.gameObject.SetActive(false);
        statscreen_D2.SetActive(true);
        
        fillstatlevelUpUI(selecteddog_D2);
    }

    public void fillstatlevelUpUI(Dog dog)
    {
        DogStats dogStats = dog?.GetComponent<DogStats>();
        Transform buttonContainer = statscreen_D2?.transform.Find("StatButtonsContainer");



        fillstatbuttons(buttonContainer, dogStats.stats, true);
    }

    public void statlevelupclicked(string statName)
    {
        if (dupecheck == true)
        {
            return;
        }
        
        if (LM.pawpoints < 50)
        {
            Debug.LogError("Insufficient Paw-Sawtive Points.");
            return;
        }

        dupecheck = true;
        inuse = false;
        
        LM.subtractpppoints(50);
        
        updatepointstext();
        AudioManager.Instance.playlevelup();

        if (!updatestat(selecteddog_D2, statName))//calls the updatestat bool method
        {
            return;
        }
        Debug.Log($"Leveled up {statName} for {selecteddog_D2.dogname}.");

        statscreen_D2?.SetActive(false);
        closebutton?.SetActive(false);
        StartCoroutine(ResetLevelUpFlag()); //I discovered last min (literally 1 day before submission that 100 points were subtracted instead of 50 because this method was being called twice
                                            //so I added this as a bandage fix to prevent that 


    }
    private IEnumerator ResetLevelUpFlag()
    {
        yield return new WaitForSeconds(0.1f); 
        dupecheck = false;
    }  
    
       //=== SHARED FUNCTIONS ===//
        public void filldoglist() //populates the UI for any dogs that can work.
    {
        filldogbuttons(doglistcontainer_D1, LogicManager.Instance.dogsawake);
    }

    private void filldoglistforlevelup() //populates the UI for any dogs that can be leveled up
    {
        filldogbuttons(doglistcontainer_D2, LogicManager.Instance.dogsawake);
    }

    private void filldogbuttons(Transform container, List<Dog> dogs) //as above.
    {
        clearcontainer(container);

        foreach (Dog dog in dogs)
        {
            createdogbutton(container, dog);
        }
    }

    private void createdogbutton(Transform container, Dog dog)
    {
        GameObject buttonObj = Instantiate(dogbuttonprefab, container);
        Button button = buttonObj.GetComponent<Button>();
        TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (text != null)
        {
            text.text = dog.dogname;
            text.color = Color.black;
            text.fontSize = 20;
        }


        if (forlevelup == true)//signifies if this is for leveling up a dog or for working
        {
            button.onClick.AddListener(() => dogselectedforlevelUp(dog));
        }
        else
        {
            button.onClick.AddListener(() => dogselected(dog));
        }
    }
    private void fillstatbuttons(Transform container, Dictionary<string, int> stats, bool forlevelup) //for the stats screen
    {
        
        foreach (Transform child in container)
        {
            Button button = child.GetComponent<Button>();
            if (button == null)
               continue; 
            
                

            
            string statName = child.name.Replace("Button", "").ToLower();

          
            button.gameObject.SetActive(stats.ContainsKey(statName));
            button.onClick.RemoveAllListeners();

            if (stats.ContainsKey(statName))
            {

                if (forlevelup == true)
                {
                    
                    button.onClick.AddListener(() => statlevelupclicked(statName));  
                }
                else
                {
                    button.onClick.AddListener(() => statbuttonclicked(statName));  
                }
            }
        }
    }



    private bool updatestat(Dog dog, string statName) //levels up a stat based on the string value of the button the player clicked
    {
        DogStats dogStats = dog?.GetComponent<DogStats>();
        
        dogStats.stats[statName] += 1;
        return true;
    }

   
    private void clearcontainer(Transform container)//updates the dog selection UI everytime one is opened to remove any old references
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    

    public void updatepointstext()
    {
       
        PPpointsText.text = $"{LM.pawpoints}";
    }

}







