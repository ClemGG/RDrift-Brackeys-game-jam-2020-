using TMPro;
using UnityEngine;

public abstract class Door : MonoBehaviour
{
    [SerializeField] protected bool isFinalDoor;
    [SerializeField] protected GameObject doorCollider, timerIcon;
    [SerializeField] protected InteractableItem[] items;
    [SerializeField] protected TextMeshProUGUI remainingItemsText;
    [SerializeField] Color doorUnlockedColor;

    public float activationDelay = 10f;
    [HideInInspector] public float _timer;

    protected bool opened = false;
    [HideInInspector] public bool shouldCount = false;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Si on veut qu'un seul bouton désactive une porte, on peut référencer directement la fonction OpenDoor() de la porte dans l'event
        remainingItemsText.text = items.Length > 0 ? items.Length.ToString() : "";



        //Si le délai d'activation est déjà en dessous de 0, ça veut dire que cette porte ne nécessite pas de timer
        //On cacher alors l'icône de timer au dessus de la porte pour indiquer au joueur que celle-ci ne nécessite pas de temps
        timerIcon.SetActive(activationDelay < 0f && items.Length > 0);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (shouldCount)
        {
            if (_timer < activationDelay)
            {
                _timer += Time.deltaTime;
                //Print time on screen
            }
            else
            {
                _timer = 0f;
                shouldCount = false;

                OnReset();

            }
        }
    }


    protected virtual void OnItemPickedUp()
    {
        
    }


    protected abstract void OnReset();


    public void OpenDoor()
    {
        shouldCount = false;
        opened = true;
        doorCollider.SetActive(false);
        //doorCollider.GetComponent<Collider>().isTrigger = true;
        //doorCollider.GetComponent<MeshRenderer>().sharedMaterial.color = doorUnlockedColor;

        //TODO : AUDIO et VFX
        remainingItemsText.enabled = false;
    }

    private void OnTriggerEnter(Collider c)
    {
        if(c.CompareTag("Entity/Player") && isFinalDoor && !ItemManager.instance.epreuveHasEnded)
        {
            ItemManager.instance.OnEpreuveEnded(true);
        }
    }
}
