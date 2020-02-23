using UnityEngine;

public class PickableItem : InteractableItem
{
    Transform t;


    // Start is called before the first frame update
    void Start()
    {
        t = transform;
        OnInteractedEvent.AddListener(delegate { Setup(true); });
    }



    public override void Setup(bool active)
    {
        interacted = active;
        GetComponent<Collider>().enabled = !interacted;
        transform.GetChild(0).gameObject.SetActive(!interacted);
    }
}
