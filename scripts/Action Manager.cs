using UnityEngine;


public class ActionManager : MonoBehaviour
{
    [Header("class references")]
    public UIManager UI;

    [SerializeReference]
    public Cat linkedcat; 

    public DogMover DM; 
    public LogicManager LM;
    public ColliderLink colliderlink; 

    public Transform correspondingPoint;

   
    
        //=== player clicking on a cat ===\\

        public void setcat(Cat cat) //because there are 3 instances of this script, each action manager has it's own linked cat for each container
        { 

         linkedcat = cat;
        
        }


    private void OnMouseDown()
    {
        if (linkedcat == null)
        {
            Debug.LogError("No cat is linked to this container!");
            return;
        }

        if (UI.inuse == false) // prevents any overlap issues when a UI interface is active
        {
            LM.settarget(correspondingPoint);
            UIManager.instance.catselected(linkedcat);
            LogicManager.Instance.setcollider(this.gameObject);
        }
        
    }
   
   


    
}

