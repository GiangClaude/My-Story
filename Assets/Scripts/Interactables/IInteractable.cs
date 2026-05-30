using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);

    bool CanInteract();

    void OnTargetedState(bool isTargeted);

    GameObject GetGameObject();
}
