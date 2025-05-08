using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;

//[RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(Collider), typeof(Rigidbody))]
public class CatAIController : MonoBehaviour
{
    #region Enums
    public enum CatBehaviourState { Wander, Idle } //Walk, Run, Sleep, ClimbTree, PlayWithOtherCat, Hunt, SitAndLookAround, Drink, PawFace } // Add others when looking at animations
    #endregion

    public Animator animator;
    private NavMeshAgent agent;
    public CatBehaviourState state;
    public float decisionInterval = 0.5f;
    public float wanderRadius = 12f;
    public float forwardWanderAngle = 90f;
    public float playerVisibilityRadius = 18f;
    public float minDistanceFromPlayer = 3f;
    public List<Transform> pointsOfInterest;
    public Transform playerTransform;
    public LayerMask catLayer;

    
    private bool isWandering = false;

    private CatBehaviourState currentState;

    [Header("Animation Settings")]
    public float animSpeed;
    public float delaySpeed = 3.0f;
    public float stopThreshold = 0.5f;
    public const float CROUCH_THRESHOLD = 0.5f;
    public const float WALK_THRESHOLD = 1f;
    public const float RUN_THRESHOLD = 2f;
    public List<string> shockedAnimations;
    public float slowDownTime = 1.5f;
    public float lookAtSpeed = 5f;
    private bool gameStopped = false;
    private string sleepTrig;

    string[] sleep = new string[] { "Sleep", "Sleep2", "Sleep3" };

    private CatBehaviour catBehaviour;

    private float targetSpeed = 0f;
    private float accelerationRate = 4f;

    private Vector3 startPos;
    private float targetAnimSpeed = 0f;
    private float currentAnimSpeed = 0f;
    private float rampDistance = 2f;
    private bool rampingUp = false;

    private Transform head;
    private Transform neck;
    private Vector3 lookTarget;
    public float headTurnSpeed = 5f;

    private Transform player;

    public float fixedOffset = -0.3f;

    private bool firstTime = true;
    private float firstDecision = 0.1f;


    public void Start()
    {
        //    float velocityMag = agent.velocity.magnitude;
        //    animator.SetFloat("WalkSpeed", velocityMag);

        //    float animSpeedValue = Mathf.Clamp(velocityMag / baseSpeed, 0.5f, 2f);
        //    animator.SetFloat("AnimSpeed", animSpeedValue);")
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        catBehaviour = GetComponent<CatBehaviour>();
        //head = transform.Find("Arm_Cat/root_bone/Spine_base/spine_02/spine_03/spine_04/spine_05/neck/head");
        //neck = transform.Find("Arm_Cat/root_bone/Spine_base/spine_02/spine_03/spine_04/spine_05/neck");

        //if (head == null) Debug.Log("No Head Bone");
        //if (neck == null) Debug.Log("No Neck Bone");

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on this object");
        }
        agent.isStopped = true;
        agent.speed = 0;

        agent.baseOffset = fixedOffset;
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
        
        //float speed = Random.Range(0.5f, 2.5f);
        //agent.speed = speed;
        //animator.SetFloat("WalkSpeed", speed);


        //StartCoroutine(MoveAwayFromPlayer());
    }

    //public void Awake()
    //{
    //    agent.updateRotation = false;
    //    agent.updateUpAxis = false;
    //}
    public void Update()
    {
        if (agent == null || animator == null || !agent.enabled) return;

        float velocityMag = agent.velocity.magnitude;

        // SPEED UP
        float animSpeed = velocityMag;
        if (rampingUp)
        {
            float distMoved = Vector3.Distance(transform.position, startPos);
            float t = Mathf.Clamp01(distMoved / rampDistance);


            animSpeed = Mathf.SmoothStep(0.1f, targetAnimSpeed, t);

            if (t >= 1f)
            {
                rampingUp = false;
            }
        }

        // Set animation parameters
        animator.SetFloat("WalkSpeed", animSpeed);
        animator.SetFloat("AnimSpeed", Mathf.Clamp((animSpeed / Mathf.Max(agent.speed, 0.01f)) * 0.9f, 0.5f, 2f));

        if (agent.baseOffset != fixedOffset)
        {
            agent.baseOffset = fixedOffset;
        }

        // TURNING 
        Vector3 desiredDirection = agent.desiredVelocity.normalized;

        if (desiredDirection.sqrMagnitude > 0.1f)
        {
            float angleToTarget = Vector3.SignedAngle(transform.forward, desiredDirection, Vector3.up);
            float normalizedTurn = Mathf.Clamp(angleToTarget / 90f, -1f, 1f); // -1 for left, 1 for right

            animator.SetFloat("TurnAngle", normalizedTurn);

            // This controls the speed of movement (0 for idle, 1 for walking, 2 for running, etc.)
            float moveSpeed = agent.velocity.magnitude;
            animator.SetFloat("WalkSpeed", moveSpeed);

            // Adjust layer weight: full turning weight when turning, otherwise 0.
            float turnWeight = Mathf.Abs(normalizedTurn) > 0.1f ? 1f : 0f;
            animator.SetLayerWeight(animator.GetLayerIndex("Turning"), turnWeight);
        }

        // STOPPING
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                agent.isStopped = true;
                agent.speed = 0;
                animator.SetFloat("WalkSpeed", 0);
                animator.SetFloat("AnimSpeed", 0.5f);
                state = CatBehaviourState.Idle;
            }
        }

        //if (agent.hasPath)
        //{
        //    lookTarget = agent.steeringTarget;
        //}

        //RotateHead(lookTarget);

        //UpdateLookTarget();

        //if (head != null)
        //{
        //    Vector3 directionToLook = lookAtTarget - head.position;
        //    directionToLook.y = 0;

        //    if (directionToLook.sqrMagnitude > 0.01f)
        //    {
        //        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
        //        head.rotation = Quaternion.Slerp(head.rotation, targetRotation, Time.deltaTime * headTurnSpeed);
        //    }
        //}

        //if (neck != null)
        //{
        //    Vector3 directionToLook = lookAtTarget - neck.position;
        //    directionToLook.y = 0;

        //    if (directionToLook.sqrMagnitude > 0.01f)
        //    {
        //        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
        //        neck.rotation = Quaternion.Slerp(neck.rotation, targetRotation, Time.deltaTime * (headTurnSpeed * 0.5f));
        //    }
        //}
    }

    //void RotateHead(Vector3 targetPos)
    //{
    //    if (agent == null || head == null) return;
    //    if (!agent.hasPath) return;

    //    // Where the cat is heading
    //    Vector3 direction = targetPos - head.position;
    //    direction.y = 0f;

    //    if (direction.magnitude < 0.01f) return;

    //    Quaternion targetRotation = Quaternion.LookRotation(direction);
    //    head.rotation = Quaternion.Slerp(head.rotation, targetRotation, Time.deltaTime * 6f);

    //    //if (direction.sqrMagnitude > 0.001f)
    //    //{
    //    //    Quaternion lookRot = Quaternion.LookRotation(lookDir);

    //    //    // Smoothly rotate head and neck
    //    //    head.rotation = Quaternion.Slerp(head.rotation, lookRot, Time.deltaTime * 5f);
    //    //    if (neck != null)
    //    //        neck.rotation = Quaternion.Slerp(neck.rotation, lookRot, Time.deltaTime * 3f);
    //    //}
    //}
    public void EnableWandering()
    {
        if (catBehaviour != null) catBehaviour.enabled = false;
        isWandering = true;
        firstTime = true;
        StartCoroutine(StartWanderingAfterAgentReady());
        //state = CatBehaviourState.Wander;
        //StartCoroutine(BehaviourLoop());
    }

    //IEnumerator MoveAwayFromPlayer()
    //{
    //    Vector3 targetPos = GetRandomPoint();
    //    agent.SetDestination(targetPos);
    //    yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);
    //    StartCoroutine(BehaviourLoop());
    //}
    IEnumerator StartWanderingAfterAgentReady()
    {
        // Wait until the NavMeshAgent is initialized and enabled
        while (agent == null || !agent.isOnNavMesh)
        {
            yield return null; // Wait for a frame
        }

        StartCoroutine(InitialWanderAndLoop());
    }

    IEnumerator InitialWanderAndLoop()
    {
        currentState = CatBehaviourState.Wander;
        firstTime = false;
        Vector3 wanderPoint = GetWanderPoint();
        if (wanderPoint != Vector3.zero)
        {
            agent.isStopped = false;
            agent.SetDestination(wanderPoint);
            float newSpeed = Random.Range(0.5f, 2.5f);
            agent.speed = newSpeed;

            rampDistance = Mathf.Lerp(0.8f, 2f, Mathf.InverseLerp(0.5f, 2.5f, newSpeed));
            startPos = transform.position;
            targetAnimSpeed = newSpeed;
            rampingUp = true;
        }
        yield return new WaitForSeconds(decisionInterval);
        StartCoroutine(BehaviourLoop());
    }

    IEnumerator BehaviourLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionInterval);

            if (agent.hasPath && agent.velocity.sqrMagnitude > 0f) continue;
            ChooseNewBehaviour();
        }
    }

    IEnumerator Idle()
    {
        float idleDuration = Random.Range(5f, 15f);
        yield return new WaitForSeconds(idleDuration);
        ChooseNewBehaviour();
    }

    void ChooseNewBehaviour()
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is NULL");
            return;
        }

        //currentState = forceWander ? CatBehaviourState.Wander : (CatBehaviourState)Random.Range(0, 3);
        currentState = (CatBehaviourState)Random.Range(0, 3);
        switch (currentState)
        {
            case CatBehaviourState.Idle:
                agent.isStopped = true;
                agent.speed = 0;
                Idle();
                break;
            case CatBehaviourState.Wander:
                Vector3 wanderPoint = GetWanderPoint();
                if (wanderPoint != Vector3.zero)
                {
                    agent.isStopped = false;
                    agent.SetDestination(wanderPoint);
                    float newSpeed = Random.Range(0.5f, 2.5f);
                    agent.speed = newSpeed;

                    rampDistance = Mathf.Lerp(0.8f, 2f, Mathf.InverseLerp(0.5f, 2.5f, newSpeed)); // swap over 0.8 and 2 if u want to revert back to high speed less distance.
                    //Track ramp up animation
                    startPos = transform.position;
                    targetAnimSpeed = newSpeed;
                    //currentAnimSpeed = 0f;
                    rampingUp = true;
                    if (!agent.hasPath && !agent.pathPending)
                    {
                        ChooseNewBehaviour();
                    }
                }
                break;
            //case CatBehaviourState.Sleep:
            //    sleepTrig = sleep[Random.Range(0, sleep.Length)];
            //    StartCoroutine(SleepRoutine(sleepTrig));
            //    break;
        }

        state = currentState;
        //currentState = (CatBehaviourState)Random.Range(0.System.Enum.GetValues(typeof(CatBehaviourState)).Length);
        //switch (currentState)
        //{
        //    case CatBehaviourState.Idle:
        //        agent.isStopped = true;
        //        animator.SetFloat("WalkSpeed", 0);
        //        break;
        //    case CatBehaviourState.Walk:
        //        MoveToRandPoint(1.2f);
        //        break;
        //    case CatBehaviourState.Run:
        //        MoveToRandPoint(2.5f);
        //        break;
        //    case CatBehaviourState.Sleep:
        //        agent.isStopped = true;
        //        animator.SetTrigger("Sleep");
        //        break;
        //    case CatBehaviourState.ClimbTree:
        //        //StartCoroutine(ClimbAction("ClimbUp"));
        //        StartCoroutine(ClimbTree());
        //        break;
        //    case CatBehaviourState.PlayWithOtherCat:
        //        TryPlayWithAnotherCat();
        //        break;
        //    case CatBehaviourState.Hunt:
        //        StartCoroutine(HuntRoutine());
        //        break;
        //    case CatBehaviourState.SitAndLookAround:
        //        agent.isStopped = true;
        //        animator.SetTrigger("Sit");
        //        break;
        //    case CatBehaviourState.Drink:
        //        MoveToSpecificPoint("DrinkPoint", "Drink");
        //        break;
        //    case CatBehaviourState.PawFace:
        //        TryPawFace();
        //        break;
        //}
    }
    void WalkSway()
    {
        float swayMultiplier = Mathf.Lerp(0f, 1f, Mathf.Clamp01(agent.speed / 3f));
        float sway = Mathf.Sin(Time.time * 3f) * 0.15f * swayMultiplier;
        transform.position += transform.right * sway * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, agent.destination, agent.speed * Time.deltaTime);
    }

    IEnumerator SleepRoutine(string sleepTrig)
    {
        agent.isStopped = true;
        agent.speed = 0;
        animator.SetTrigger(sleepTrig); 
        animator.SetBool("isSleeping", true);

        float sleepDuration = Random.Range(5f, 15f);
        yield return new WaitForSeconds(sleepDuration);

        animator.SetBool("isSleeping", false); // Exit loop and transition to sleep end
        float sleepEndDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(sleepEndDuration);

        // Resume other behavior (e.g., wander)
        ChooseNewBehaviour();
    }
    Vector3 GetWanderPoint()
    {
        Vector3 randomDir = Random.insideUnitSphere;
        randomDir.y = 0f;
        randomDir = randomDir.normalized;

        Vector3 forward = transform.forward; // Use cat's forward direction instead
        float angle = Vector3.Angle(forward, randomDir);

        if (angle > forwardWanderAngle) return Vector3.zero; // Only accept directions within the forward cone
        Vector3 target = transform.position + randomDir * Random.Range(minDistanceFromPlayer, wanderRadius);
        if (NavMesh.SamplePosition(target, out NavMeshHit navhit, 3f, NavMesh.AllAreas))
        {
            Ray r = new Ray(navhit.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 10f, LayerMask.GetMask("Default")))
            {
                agent.baseOffset = -hit.distance;
            }
            return navhit.position;
        }
        return Vector3.zero;

    }

    public void OnGameStopped()
    {
        if (gameStopped) return;
        gameStopped = true;

        StopAllCoroutines();

        if (agent != null)
        {
            StartCoroutine(SlowDownAndStop());
        }

        if (playerTransform != null)
        {
            StartCoroutine(LookAtPlayer());
        }
        else
        {
            Debug.Log($"{gameObject.name} playerTransform is: {(playerTransform == null ? "NULL" : playerTransform.name)}");
        }

        if (animator != null && shockedAnimations.Count > 0)
        {
            string shockedTrigger = shockedAnimations[Random.Range(0, shockedAnimations.Count)];
            animator.SetTrigger(shockedTrigger);

        }
    }

    IEnumerator SlowDownAndStop()
    {
        float startSpeed = agent.speed;
        float elapsed = 0f;

        while (elapsed < slowDownTime)
        {
            float t = elapsed / slowDownTime;
            agent.speed = Mathf.Lerp(startSpeed, 0f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        agent.speed = 0f;
        agent.isStopped = true;

        if (animator != null)
        {
            animator.SetFloat("WalkSpeed", 0f);
            animator.SetFloat("AnimSpeed", 0.1f);
        }
    }

    IEnumerator LookAtPlayer()
    {
        while (true)
        {
            //Debug.Log($"{gameObject.name} starting to look at player.");
            if (playerTransform == null) yield break;

            Vector3 lookDir = playerTransform.position - transform.position;
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * lookAtSpeed);

                //handle animation turn??

            }

            yield return null;
        }
    }

    //void MoveToRandPoint(float speed)
    //{
    //    Vector3 randomPoint = GetRandomPoint();
    //    MoveToPoint(randomPoint, speed);
    //    //if (pointsOfInterest.Count == 0) return;

    //    //agent.isStopped = false;
    //    //agent.speed = speed;
    //    //animator.SetFloat("Speed", speed);

    //    //Transform target = pointsOfInterest[Random.Range(0, pointsOfInterest.Count)];
    //    //agent.SetDestination(target.position);
    //}

    //void MoveToPoint(Vector3 point, float speed)
    //{
    //    agent.isStopped = false;
    //    agent.speed = speed;
    //    animator.SetFloat("WalkSpeed", speed);
    //    agent.SetDestination(point);
    //}

    //IEnumerator ClimbAction(string trigger)
    //{
    //    Transform climbTarget = FindClosestTaggedPoint("ClimbTreePoint");
    //    if (climbTarget == null) yield break;

    //    agent.SetDestination(climbTarget.position);
    //    yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);

    //    agent.isStopped = true;
    //    animator.CrossFade(trigger, 0.2f);
    //    yield return new WaitForSeconds(2.5f); // duration of animation
    //    animator.SetTrigger("Idle");
    //}

    //IEnumerator ClimbTree()
    //{
    //    agent.isStopped = true;
    //    animator.SetTrigger("Climb");
    //    yield return new WaitForSeconds(3f);
    //    animator.SetTrigger("Idle");
    //}

    //void TryPlayWithAnotherCat()
    //{
    //    CatAIController[] allCats = FindObjectsOfType<CatAIController>();
    //    foreach (var cat in allCats)
    //    {
    //        if (cat != this && Vector3.Distance(transform.position, cat.transform.position) < 3f)
    //        {
    //            agent.isStopped = true;
    //            cat.agent.isStopped = true;
    //            cat.animator.SetTrigger("Play");
    //            animator.SetTrigger("Play");
    //            return;
    //        }
    //    }
    //    MoveToRandPoint(2f);
    //}

    //IEnumerator HuntRoutine()
    //{
    //    Vector3 huntPoint = GetRandomPoint();
    //    MoveToPoint(huntPoint, 0.75f);
    //    animator.SetTrigger("Hunt");

    //    yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);
    //    animator.SetTrigger("Pounce");
    //    yield return new WaitForSeconds(1.5f); //pounce duration
    //    animator.SetTrigger("Idle");
    //}

    //void MoveToSpecificPoint(string tag, string animTrigger)
    //{
    //    Transform point = FindClosestTaggedPoint(tag);
    //    if (point == null) return;

    //    StartCoroutine(MoveThenAnimate(point.position, animTrigger));
    //}

    //IEnumerator MoveThenAnimate(Vector3 point, string animTrigger)
    //{
    //    MoveToPoint(point, 1f);
    //    yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance < 0.5f);

    //    agent.isStopped = true;
    //    animator.SetTrigger(animTrigger);
    //}

    //void TryPawFace()
    //{
    //    float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
    //    if (distanceToPlayer < 2.5f)
    //    {
    //        agent.isStopped = true;
    //        transform.LookAt(playerTransform);
    //        animator.SetTrigger("PawFace");

    //        //maybe make another empty as a specific point for cat to move to before pawing face?
    //    }
    //}

    //Utility

    //Vector3 GetRandomPoint()
    //{
    //    float radius = 8f;
    //    float angleRange = 160f;

    //    for (int i = 0; i < 30; i++)
    //    {
    //        float angle = Random.Range(-angleRange / 2f, angleRange / 2f);
    //        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.Up);
    //        Vector3 dir = rot * transform.forward;
    //        Vector3 randomPos = transform.position + dir * Random.Range(3f, radius);
    //    }
    //    //Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
    //    //randomDirection += playerTransform.position;

    //    if(NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
    //    {
    //        return hit.position;
    //    }
    //    return transform.position;
    //}

    //Transform FindClosestTaggedPoint(string tag)
    //{
    //    GameObject[] taggedPoints = GameObject.FindGameObjectsWithTag(tag);
    //    Transform closest = null;
    //    float minDist = float.MaxValue;

    //    foreach (var point in taggedPoints)
    //    {
    //        float dist = Vector3.Distance(transform.position, point.transform.position);
    //        if (dist < minDist)
    //        {
    //            minDist = dist;
    //            closest = point.transform;
    //        }
    //    }
    //    return closest;
    //}
}


