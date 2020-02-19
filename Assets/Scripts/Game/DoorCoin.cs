using UnityEngine;

public class DoorCoin : Door
{
    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < items.Length; i++)
        {
            items[i].Setup(false);
        }
    }

    protected override void OnReset()
    {
        Start();
    }

    protected override void OnItemPickedUp()
    {
        if (!opened)
        {
            //Si le délai d'activation est déjà en dessous de 0, ça veut dire que cette porte ne nécessite pas de timer
            if(activationDelay > 0f)
            {
                shouldCount = true;
                StartCoroutine(ItemManager.instance.ShowDoorTimer(this));
                //Appeler le gameManager pour afficher et cacher le popup du timer
            }

            int remainingItems = 0;
            for (int i = 0; i < items.Length; i++)
            {

                if (items[i].interacted)
                {
                    remainingItems = items.Length - i + 1;
                }

            }

            remainingItemsText.text = remainingItems.ToString();

            //Si un seul bouton n'est pas encore pressé, on continue de compter
            for (int i = 0; i < items.Length; i++)
            {
                if (!items[i].interacted)
                    return;

            }

            //Sinon, on arrête de compter et on ouvre la porte
            OpenDoor();
        }
    }

}
