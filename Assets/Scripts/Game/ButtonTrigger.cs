using UnityEngine;

public class ButtonTrigger : InteractableItem
{
    [SerializeField] MeshRenderer btnRend;
    [SerializeField] Color initColor, pressedColor;



    public override void Setup(bool active)
    {
        interacted = active;
        if (btnRend)
        {
            if (btnRend.sharedMaterial)
            {
                Material m = new Material(btnRend.sharedMaterial);
                m.color = interacted ? pressedColor : initColor;
                btnRend.material = m;
            }
        }
    }
}
