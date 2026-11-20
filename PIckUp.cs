using UnityEngine;

public class Pickup : Interactable
{
    public string itemName = "Objeto";

    public override void Interact(GameObject who)
    {
        var interactor = who.GetComponent<PlayerInteractor>();
        if (interactor != null) interactor.AddItem(itemName);

        base.Interact(who);      // por si le conectas otro evento
        Destroy(gameObject);
    }


}
