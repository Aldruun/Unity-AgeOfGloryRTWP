using UnityEngine.Events;

/*
 * Used for interactables that only require you to stand close and eventually face it
 */

public class SimpleInteractable : Interactable
{
    public UnityEvent onInteractCallback;

    public override void Interact(Actor ctrl)
    {
        onInteractCallback?.Invoke();
    }
}