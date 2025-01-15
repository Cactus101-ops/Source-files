using TMPro;
using UnityEngine;


public class DogListUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject doglistprefab; 
    [SerializeField] private Transform contentcontainer;   

    private void Start()
    {
        filldoglist();
    }

    //=== Method to populate the dog list ===\\
    public void filldoglist()
    {

        clearlist(); 

        foreach (Dog dog in LogicManager.Instance.dogsawake)
        {
            addtolist(dog.dogname);
        }
    }

    private void addtolist(string dogname)
    {
        
        GameObject text = new GameObject("dognametext");
        text.transform.SetParent(contentcontainer, false);

        
        TextMeshProUGUI textComponent = text.AddComponent<TextMeshProUGUI>();

       
        textComponent.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");

        textComponent.text = dogname;
        textComponent.fontSize = 24;
        textComponent.alignment = TextAlignmentOptions.Left;

        
    }



    //=== Clear the dog list ===\\
    private void clearlist()
    {
       
        foreach (Transform child in contentcontainer)
        {
            Destroy(child.gameObject);
        }
    }
}