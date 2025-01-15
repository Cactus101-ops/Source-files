using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


[System.Serializable]
public class Cat : BaseCat
{
    public TextMeshPro angrytext;
    public TextMeshPro nametext; 
    public bool isangry { get; set; } 
    public enum catstate { Neutral, Angry }
    public catstate currentstate = catstate.Neutral;
    
    public Dictionary<string, int> catstats { get; set; }
   

    private static readonly Dictionary<string, Dictionary<string, int>> cattemplates = new()
    {
        { "Fat Cat", new Dictionary<string, int> { { "laziness", 5 }, { "playfulness", 1 }, { "boisterous", 1 }, { "smarts", 1 } } },
        { "Zorp Glorp Cat", new Dictionary<string, int> { { "laziness", 2 }, { "playfulness", 5 }, { "boisterous", 4 }, { "smarts", 2 } } },
        { "Professor Cats", new Dictionary<string, int> { { "laziness", 4 }, { "playfulness", 2 }, { "boisterous", 5 }, { "smarts", 4 } } },
        { "Neuromancer Cat", new Dictionary<string, int> { { "laziness", 2 }, { "playfulness", 3 }, { "boisterous", 4 }, { "smarts", 5 } } },
        { "Caffine Cat", new Dictionary<string, int> { { "laziness", 1 }, { "playfulness", 2 }, { "boisterous", 2 }, { "smarts", 5 } } },
    };//all about figuring a way to work with the cats, using Laziness on the Fat Cat early on would be a bad idea etc 

    
    private void Awake()
    {
        if (catstats == null)
        {
            catstats = new Dictionary<string, int>(); 
           
            isangry = false; 
        }
    }
    public void becomeangry()
    {
        isangry = true;
        if (currentstate == catstate.Angry)
        {
            return;
        }
        Debug.Log($"{name} is now Angry!");
        
        currentstate = catstate.Angry;
        angrytext.gameObject.SetActive(true);//activates the UI element to show if the cat is angry
        


        foreach (var key in catstats.Keys.ToList())
        {
            catstats[key]++;
        }
    }

    public void calmdown()
    {
        isangry = false;
        if (currentstate != catstate.Angry)
        {
            return;
        }

        currentstate = catstate.Neutral;
        
        Debug.Log($"{name} has calmed down.");
        angrytext.gameObject.SetActive(false);//disables the UI element to show if a cat is angry

        foreach (var key in catstats.Keys.ToList())
         {
            catstats[key]--;
         }
    }

    //=== Activates UI elements for when the car
    public void Update()
    {
        if (isangry == true)
        {
           

        }
        else
        {
           
        }
    }

    //=== Method to initialize the cat with its name and stats ===\\
    public void initialize(string catname)
    {
       
        this.name = catname;
        nametext.text = catname; // display the cats name on the ui
        this.catstats = new Dictionary<string, int>();
        whencatspawn();
        
    }
    public void whencatspawn()
    {

        if (cattemplates.TryGetValue(name, out var stats))
        {
            catstats = new Dictionary<string, int>(stats);
            
        }
       

    }
    
    
    

    
}


