/* This interactable type can have multiple users at once. Imagine a couple of AI agents and the player searching a big chest together.
 * 
*/

using GenericFunctions;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public float interactDistance = 2;
    public AudioClip[] interactionEndSounds;
    public AudioClip[] interactionLoopSounds;

    public AudioClip[] interactionStartSounds;

    public string interactionText = "Interact";

    protected Animation m_animComponent;

    public bool
        mustFace = true; // Important for sitting or working at benches. Not needed for i.e. picking up items or examining the notice board

    public AudioSource audioSource { get; private set; }
    public Transform interactionPoint { get; protected set; }

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1;
            audioSource.loop = true;
        }

        m_animComponent = GetComponent<Animation>();

        //Collider collider = GetComponent<Collider>();
        //if(collider != null)
        interactionPoint = transform.GetChild(0);

        var pos = interactionPoint.position;

        RaycastHit hit;
        if (Physics.Raycast(interactionPoint.position + Vector3.up * 10, -Vector3.up, out hit, 10,
            1 << LayerMask.NameToLayer("Ground"))) pos = hit.point;

        interactionPoint.position = pos;
    }

    public void Player_Interact(ActorInput agent)
    {
        if (IsInRange(agent))
            if (IsFacingThis(agent))
                Interact(agent);
    }

    public void AI_Interact(ActorInput agent)
    {
        if (IsInRange(agent))
            if (IsFacingThis(agent))
                Interact(agent);
    }

    public abstract void Interact(ActorInput ctrl);

    protected bool IsInRange(ActorInput agent)
    {
        if (Vector3.Distance(interactionPoint.position, agent.transform.position) <= 1)
        {
            //Debug.Log("In range of " + name);
            return true;
        }

        //Debug.Log("Moving towards " + name);
        //agent.SetDestination(interactionPoint.position, interactDistance);
        return false;
    }

    private bool IsFacingThis(ActorInput agent)
    {
        if (mustFace)
            if (Get.IsInFOV(agent.transform, transform.position, 5) == false)
            {
                HelperFunctions.RotateTo(agent.transform, transform.position, 300/*, "Interact"*/);
                return false;
            }

        return true;
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if(PlayerInput.controlScheme == PlayerControlScheme.THIRDPERSON && other.gameObject.layer == LayerMask.NameToLayer("Agents"))
    //    {
    //        if(other.GetComponent<AgentMonoController>().isPlayer)
    //        {
    //            GameEventSystem.OnPlayerNearInteractable?.Invoke(this);
    //        }
    //    }
    //}
    //void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Agents"))
    //    {
    //        if(other.GetComponent<AgentMonoController>().isPlayer)
    //        {
    //            GameEventSystem.OnPlayerLeftInteractable?.Invoke();
    //        }
    //    }
    //}

    //void OnMouseEnter() {

    //    if(this is ItemContainer || this is PickupItem) {

    //        WorldInteractionManager.current.SetOrderType(InteractionType.PickUp);
    //    }
    //}

    //void OnMouseExit() {

    //    WorldInteractionManager.current.SetOrderType(InteractionType.None);
    //}

    // I.e. close a chest if workers == 0
    public virtual void OnInteractEnd()
    {
        // I.e. Power off!
    }
}