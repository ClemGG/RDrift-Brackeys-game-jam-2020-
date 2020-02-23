using UnityEngine;

public class ButtonTrigger : InteractableItem
{
    [SerializeField] MeshRenderer btnRend;
    [SerializeField] Material initMat, pressedMat;



    public override void Setup(bool active)
    {
        interacted = active;
        if (btnRend)
        {
            btnRend.sharedMaterial = interacted ? pressedMat : initMat;
        }
    }
}
