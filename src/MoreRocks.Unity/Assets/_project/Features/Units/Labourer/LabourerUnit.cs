using Assets._project.Features.Buildings;
using Assets._project.Features.Units;
using Assets._project.Features.Units.Labourer;
using Assets.Features.Resources;
using System.Collections;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public class LabourerUnit : BaseUnit
{
    [SerializeField] Animator animator;
    [SerializeField] LabourerStates currentState;
    [SerializeField] int gatherAmount = 1;
    [SerializeField] float gatherDistance = 0.1f;
    [SerializeField] int gatherCycleSeconds = 1;
    [SerializeField] int maxGatherAmount = 5;
    [SerializeField] int depositRetryDelaySeconds = 1;
    
    private const string IDLE_ANIMATION_KEY = "idle";
    private const string WALKING_ANIMATION_KEY = "walking";
    private const string GATHERING_ANIMATION_KEY = "gathering";

    private ResourceNode targetNode;
    private int heldResourceAmount;
    private ResourceType heldResourceType;

    public LabourerStates CurrentState => currentState;
    
    public int HeldResourceAmount => heldResourceAmount;

    public ResourceType HeldResourceType => HeldResourceType;

    #region Unity
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        Idle();
    }

    private void Update()
    {

        // Watch our current state for the appropriate exit condition, and then issue new orders.
        // It is important that the methods called handle changing the state, to ensure that new 
        // orders can short-circuit the update loop
        // Yes, I do regret not doing this as a FSM, that's on the books for v2
        switch (currentState)
        {
            case LabourerStates.Moving:
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    Debug.Log("Reached destination");
                    Idle();
                }
                break;
            case LabourerStates.MovingToGather:
                if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= (gatherDistance / 2))
                {
                    Debug.Log("Beginning to gather", this);
                    GatherFromNode(targetNode);
                }
                break;
            case LabourerStates.Returning:
                if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance + gatherDistance)
                {
                    Idle();
                }
                break;
            case LabourerStates.Gathering:
                if (heldResourceAmount >= maxGatherAmount)
                {
                    ReturnToNearestHub();
                }
                else if (targetNode == null)
                {
                    // the node we were gathering from has been destroyed, issue command to find a new one
                    Gather();
                }
                break;
            case LabourerStates.Depositing:
                if (heldResourceAmount <= 0)
                {
                    Debug.Log("Returning to gathering");
                    Gather();
                }
                break;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject hitObject = other.gameObject;
        if (currentState == LabourerStates.MovingToGather && other.gameObject == targetNode.gameObject)
        {
            // We've entered our target node's trigger; begin gathering
            //Debug.Log("Beginning to gather", this);
            //GatherFromNode(targetNode);
        }
        else if (heldResourceAmount > 0 && hitObject.TryGetComponent(out ResourceHub hub))
        {
            // We've entered our deposit hub's trigger; make a deposit
            Deposit(hub);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(GatherTick(null));
        StopCoroutine(DepositTick(null));
    }
    #endregion
    
    public override void Interact(GameObject target)
    {   
        if (target.TryGetComponent(out ResourceNode node))
        {
            MoveToGatherFromNode(node);
            return;
        }
        else if (target.TryGetComponent(out ResourceHub hub))
        {
            if (heldResourceAmount > 0)
            {

            }
        }
        else
        {
            base.Interact(target);
        }
        
    }


    public override void Idle()
    {
        Debug.Log("Going idle", this);
        navMeshAgent.isStopped = true;
        AnimateIdle();
        currentState = LabourerStates.Idle;
    }

    public override void Move(Vector3 location)
    {
        var coordinates = $"({location.x}, {location.y}, {location.z})";
        Debug.Log($"Moving agent to {coordinates}");
        
        //navMeshAgent.SetDestination(location);
        navMeshAgent.destination = location;
        if (navMeshAgent.SetDestination(location))
        {
            navMeshAgent.isStopped = false;
            currentState = LabourerStates.Moving;
            AnimateWalking();
        }
    }

    public void MoveToGatherFromNode(ResourceNode node)
    {
        var destination = GetClosestPoint(node.gameObject);
        var coordinates = $"({destination.x}, {destination.y}, {destination.z})";
        Debug.Log($"Moving to node at {coordinates}");
        targetNode = node;
        currentState = LabourerStates.MovingToGather;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(destination);
        AnimateWalking();
    }

    public void GatherFromNode(ResourceNode node)
    {
        navMeshAgent.isStopped = true;
        currentState = LabourerStates.Gathering;
        AnimateGathering();
        StartCoroutine(GatherTick(targetNode));
    }

    public void Gather() // TODO: rename this, it's confusing
    {
        if (targetNode == null)
        {
            if (heldResourceType == null)
            {
                // We can't act
                Idle();
                return;
            }
            else
            {
                var targetNode = FindNearestResourceNodeOfType(heldResourceType);
                if (targetNode == null)
                {
                    Idle();
                }
                else
                {
                    this.targetNode = targetNode;
                    
                }
            }
        }

        MoveToGatherFromNode(targetNode);
    }

    public ResourceHub FindNearestResourceHub()
    {
        var hubs = GameObject.FindObjectsOfType<ResourceHub>();
        ResourceHub closestHub = null;
        float closestDistance = Mathf.Infinity;
        foreach (var hub in hubs)
        {
            Vector3 direction = hub.transform.position - this.transform.position;
            float distance = direction.sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHub = hub;
            }
        }

        return closestHub;
    }

    public ResourceNode FindNearestResourceNode()
    {
        var nodes = FindObjectsOfType<ResourceNode>();
        ResourceNode closestNode = null;
        float closestDistance = Mathf.Infinity;
        foreach (var node in nodes)
        {
            Vector3 direction = node.transform.position - this.transform.position;
            float distance = direction.sqrMagnitude;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }
    
    public ResourceNode FindNearestResourceNodeOfType(ResourceType resourceType)
    { 
        var nodes = FindObjectsOfType<ResourceNode>();
        ResourceNode closestNode = null;
        float closestDistance = Mathf.Infinity;
        foreach (var node in nodes)
        {
            if (node.ResourceType == resourceType)
            {
                Vector3 direction = node.transform.position - this.transform.position;
                float distance = direction.sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNode = node;
                }
            }
        }

        return closestNode;
    }

    public void ReturnToNearestHub()
    {
        var hub = FindNearestResourceHub();
        if (hub.TryGetComponent(out Collider collider))
        {
            var destination = collider.ClosestPoint(this.transform.position);
            Move(destination);
        }
        else
        {
            Debug.LogWarning("Closest resource hub does not have a collider attached, unable to move to it");
        }
    }

    IEnumerator DepositTick(ResourceHub resourceHub)
    {
        while (currentState == LabourerStates.Depositing && heldResourceAmount > 0)
        {
            if (resourceHub.TryMakeDeposit(this, heldResourceAmount, heldResourceType))
            {
                heldResourceAmount = 0;
            }
            else
            {
                yield return new WaitForSeconds(depositRetryDelaySeconds);
            }
        }
    }

    IEnumerator GatherTick(ResourceNode node)
    {
        var distance = Vector3.Distance(this.transform.position, node.transform.position);
        while (currentState == LabourerStates.Gathering && heldResourceAmount < maxGatherAmount)
        {
            // wait first, so a player can't cheat by starting & stopping the gathering
            yield return new WaitForSeconds(gatherCycleSeconds);
            distance = Vector3.Distance(this.transform.position, node.transform.position);
            if (targetNode == null)
            {
                Debug.LogWarning("Target node has been destroyed");
            }
            else if (distance <= gatherDistance)
            {
                var gatherResult = node.TryGatherResource(gatherAmount);
                if (gatherResult.Success)
                {
                    if (heldResourceType == null)
                    {
                        heldResourceType = node.ResourceType;
                    }

                    if (heldResourceType == node.ResourceType)
                    {
                        heldResourceAmount = Mathf.Min(heldResourceAmount + gatherResult.ReceivedAmount, maxGatherAmount);
                    }
                    else
                    {
                        Debug.LogWarning($"Labourer was holding an unexpected resource type ({heldResourceType.DisplayName}); resetting held resource amount with new type {node.ResourceType.DisplayName}", this);
                        heldResourceType = node.ResourceType;
                        heldResourceAmount = gatherResult.ReceivedAmount;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Labourer is too far away from the node", this);
            }


            //else
            //{
            //    Debug.LogWarning("Gathering resources from the target node failed");
            //    Idle();

            //    if (targetNode == null)
            //    {
            //        var newTarget = FindNearestResourceNodeOfType(heldResourceType);
            //        if (newTarget != null)
            //        {
            //            MoveToGatherFromNode(newTarget);
            //        }
            //        // TODO: find the next closest
            //    }
            //}
        }
    }

    private void Deposit(ResourceHub hub)
    {
        AnimateIdle();
        currentState = LabourerStates.Depositing;
        // because a deposit can fail (e.g. the player is full on resources), in the absence
        // of other commands, we will create a retry loop
        StartCoroutine(DepositTick(hub));
    }
    

    #region animations
    public void AnimateIdle() => Animate(IDLE_ANIMATION_KEY);

    public void AnimateWalking() => Animate(WALKING_ANIMATION_KEY);

    public void AnimateGathering() => Animate(GATHERING_ANIMATION_KEY);

    private void Animate(string animationName)
    {
        DisableOtherAnimations(animator, animationName);
        animator.SetBool(animationName, true);
    }

    private void DisableOtherAnimations(Animator animator, string animation)
    {
        foreach (var parameter in animator.parameters)
        {
            if (parameter.name != animation)
            {
                animator.SetBool(parameter.name, false);
            }
        }
    }
    #endregion
}
