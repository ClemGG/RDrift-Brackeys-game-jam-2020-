using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public abstract class InteractableItem : MonoBehaviour
{
    [SerializeField] protected UnityEvent OnInteractedEvent;
    public bool interacted = false;

    public abstract void Setup(bool active);

#if UNITY_EDITOR
    private void Reset()
    {
        Start();
    }
#endif


    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    protected void OnTriggerEnter(Collider c)
    {

        if (c.CompareTag("Entity/Player") && !interacted)
        {
            interacted = true;
            //on ajoute une pièce au game manager ou on augmente sa limite de temps
            //On se sert aussi de cet event pour désactiver les pièces / activer les boutons
            OnInteractedEvent?.Invoke();

            //TODO : Audio et VFX
        }
    }
}
