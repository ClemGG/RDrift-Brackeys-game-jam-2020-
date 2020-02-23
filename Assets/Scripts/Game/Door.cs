using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] protected bool isFinalDoor;
    [SerializeField] protected Camera uiCamera;
    [SerializeField] protected GameObject ecranMeshRend, doorCollider, timerIcon, uiCanvas;
    [SerializeField] protected MeshRenderer[] neons;
    [SerializeField] protected InteractableItem[] triggers;

    [Space(20)]

    [SerializeField] protected TextMeshProUGUI remainingItemsText;
    [SerializeField] Color doorUnlockedColor, doorLockedColor;
    [SerializeField] AudioClip unlockedClip;

    public float activationDelay = 10f;
    [HideInInspector] public float _timer;

    protected bool opened = false;
    [HideInInspector] public bool shouldCount = false;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        ChangeColor(false);


        //Si on veut qu'un seul bouton désactive une porte, on peut référencer directement la fonction OpenDoor() de la porte dans l'event
        remainingItemsText.text = isFinalDoor ? "GG!" : triggers.Length > 0 ? triggers.Length.ToString() : "";



        //On n'affiche l'icône de timer que si la porte a un délai supérieur à 0 et qu'elle possède des triggers dans sa liste
        timerIcon.SetActive(!isFinalDoor && activationDelay > 0f && triggers.Length > 0);

        for (int i = 0; i < triggers.Length; i++)
        {
            triggers[i].Setup(false);
        }

        _timer = activationDelay;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (shouldCount)
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
                //Print time on screen
            }
            else
            {
                _timer = activationDelay;
                shouldCount = false;

                OnReset();

            }
        }
    }


    public void OnTriggerInteracted()
    {
        if (!opened)
        {
            //Si le délai d'activation est déjà en dessous de 0, ça veut dire que cette porte ne nécessite pas de timer
            if (activationDelay > 0f)
            {
                shouldCount = true;
                StartCoroutine(ItemManager.instance.ShowDoorTimer(this));
                //Appeler le gameManager pour afficher et cacher le popup du timer
            }

            int remainingItems = 0;
            for (int i = 0; i < triggers.Length; i++)
            {

                if (triggers[i].interacted)
                {
                    remainingItems++;
                }

            }

            if(!isFinalDoor)
                remainingItemsText.text = (triggers.Length - remainingItems).ToString();

            //Si un seul bouton n'est pas encore pressé, on continue de compter
            for (int i = 0; i < triggers.Length; i++)
            {
                if (!triggers[i].interacted)
                    return;

            }

            //Sinon, on arrête de compter et on ouvre la porte
            OpenDoor();
        }
    }


    protected void OnReset()
    {
        Start();
    }

    RenderTexture rendTex;
    void ChangeColor(bool done)
    {
        for (int i = 0; i < neons.Length; i++)
        {
            Material pulseMat = neons[i].material;
            pulseMat.SetColor("_mainColor", done ? doorUnlockedColor : doorLockedColor);
        }
        doorCollider.GetComponent<MeshRenderer>().material.SetColor("_haloColor", done ? doorUnlockedColor : doorLockedColor);

        //Pour affiche l'UI de la camUI à l'écran
        uiCanvas.SetActive(!done && !isFinalDoor);
        if (!done)
        {
            if (triggers.Length > 0)
            {
                bool isCoin = triggers[0].GetType() == typeof(PickableItem);
                uiCanvas.transform.GetChild(1).gameObject.SetActive(isCoin);
                uiCanvas.transform.GetChild(2).gameObject.SetActive(!isCoin);
            }
            else
            {
                uiCanvas.SetActive(false);
            }

            rendTex = new RenderTexture(256, 256, 4);
            rendTex.useDynamicScale = true;
            uiCamera.targetTexture = rendTex;
            ecranMeshRend.GetComponent<MeshRenderer>().material.SetTexture("_mainTexture", rendTex);
        }
        else
        {
            //On affiche un écran noir et on désactive la cam pour gagner en performance
            uiCamera.gameObject.SetActive(false);
            rendTex.Release();
        }
    }

    public void OpenDoor()
    {
        shouldCount = false;
        opened = true;
        //doorCollider.SetActive(false);
        doorCollider.GetComponent<Collider>().isTrigger = true;
        //doorCollider.GetComponent<MeshRenderer>().sharedMaterial.color = doorUnlockedColor;

        ChangeColor(true);
        AudioManager.instance.Play(unlockedClip);



        //TODO : AUDIO et VFX
        remainingItemsText.enabled = false;
        timerIcon.SetActive(false);
    }

    private void OnTriggerEnter(Collider c)
    {
        if(c.CompareTag("Entity/Player") && isFinalDoor && !ItemManager.instance.epreuveHasEnded)
        {
            ItemManager.instance.OnEpreuveEnded(true);
        }
    }
}
