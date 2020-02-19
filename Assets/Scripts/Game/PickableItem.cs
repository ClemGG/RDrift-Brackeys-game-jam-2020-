using UnityEngine;

public class PickableItem : InteractableItem
{
    [SerializeField] float rotationSpeed;

    Transform t;


    // Start is called before the first frame update
    void Start()
    {
        t = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        t.Rotate(t.up * rotationSpeed * Time.fixedDeltaTime);
    }

    public void HideItemOnPickedUp(bool hide)
    {
        
    }

    public override void Setup(bool active)
    {
        interacted = active;
        GetComponent<Collider>().enabled = !interacted;
        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = !interacted;
    }
}
