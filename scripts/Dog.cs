using System.Collections.Generic;
using UnityEngine;


public class Dog : BaseDog
{
    
    public DogMover DM;

    public DogStats statsref;

    private void Awake()
    {

        statsref = GetComponent<DogStats>();
        
    }

    //=== Initialize a dog with default stats WHEN SPAWNING ===\\
    public void initialize(string name)
    {
        dogname = name;

        if (statsref.stats == null || statsref.stats.Count == 0)
        {
            Debug.Log($"Initializing default stats for dog {dogname}.");
            statsref.setstats("laziness", Laziness);
            statsref.setstats("playfulness", Playfulness);
            statsref.setstats("boisterous", Boisterous);
            statsref.setstats("smarts", Smarts);
        }
       
    }

    //=== Initialize a dog's EXISTING stats WHEN LOADED ===\\
    public void initialize(string name, Dictionary<string, int> stats)
    {
        dogname = name;

        
        if (stats != null && stats.Count > 0)
        {
            statsref.stats = stats;
        }
        else
        {
            Debug.Log($"Can't find stats for {dogname} giving them defualt stats instead.");
            statsref.setstats("laziness", Laziness);
            statsref.setstats("playfulness", Playfulness);
            statsref.setstats("boisterous", Boisterous);
            statsref.setstats("smarts", Smarts);
        }
    }//method overloading my beloved <3

    
    public void movetopoint(Transform point)
    {
        transform.position = point.position;// <-- the point!
    }

    
}
