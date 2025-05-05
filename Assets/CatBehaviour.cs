using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CatBehaviour : MonoBehaviour
{
    #region Enums
    public enum Personality { Confident, Timid, Playful, Lazy, Mischievous, Social }
    #endregion

    // ?????????????????????????????????????????????????????????????
    // Inspector Variables
    // ?????????????????????????????????????????????????????????????

    [Header("Personality")]
    public Personality personality;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float saveSpeed = 2f;
    public float pauseChance = 0f; //CHANGE BACK
    public float actionChance = 0.2f;
    public float decelerationDistance;
    private bool willPauseInMiddle;
    private CatSpawner spawner;

    [Header("Animation Settings")]
    public float animSpeed;
    public float delaySpeed = 2.0f;
    public float stopThreshold = 0.5f;
    public const float CROUCH_THRESHOLD = 0.5f;
    public const float WALK_THRESHOLD = 1f;
    public const float RUN_THRESHOLD = 2f;
    public List<string> shockedAnimations;
    private bool isClimbing = false;

    [Header("Climbing Settings")]
    public string climbUpTrigger = "ClimbUp";
    public string jumpUpTrigger = "JumpUp";
    public string jumpDownStartTrigger = "JumpDown";
    public string jumpDownLandTrigger = "JumpLand";
    public float climbDuration = 0.85f;
    public float jumpStartDuration = 0.6f;
    public float jumpLandDuration = 0.67f;
    private float preClimbSpeed;

    public float raycastHeight = 2f;     
    public float maxRayDistance = 5f;
    public LayerMask groundLayer;   

    [Header("Debug & State Flags")]
    public bool wasOffset;


    #region Private Components
    private Animator animator;
    private Vector3 initialPosition;
    private Vector3 totalDisplacement;
    private bool isTracking = false;
    private bool hasReachedTarget = false;
    #endregion

    #region Waypoint Transforms
    //private Vector3 startPos;
    //private Vector3 climbPos;
    //private Vector3 middlePos;
    //private Vector3 endPos;

    private Transform startPoint;
    private Transform climbPoint;
    private Transform slowDownPoint;
    private Transform middlePoint;
    private Transform endPoint;

    private Vector3 targetPosition;
    #endregion

    #region AI Controllers
    private NavMeshAgent agent;
    private CatAIController aiController;
    private bool hasReachedEnd = false;

    #region State Flags
    private bool hasPausedOnBack = false;
    private bool hasDoneAction = false;
    private bool reachedMiddle = false;
    private bool isMoving = true;
    private bool gameStopped = false;
    

    private Transform player;
    #endregion

    // ?????????????????????????????????????????????????????????????
    // Unity Methods
    // ?????????????????????????????????????????????????????????????

    #region Unity Lifecycle

    void Start()
    {

        groundLayer = LayerMask.GetMask("Ground");
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        aiController = GetComponent<CatAIController>();
        spawner = GameObject.Find("CatStartPoint").GetComponent<CatSpawner>();

        if (agent != null) agent.enabled = false;
        if (aiController != null) aiController.enabled = false;

        animator.SetBool("isWalking", true);
        transform.position = startPoint.position;

        //targetPosition = slowDownPoint.position;

        AssignPersonality();

        willPauseInMiddle = Random.value < pauseChance;

        targetPosition = climbPoint.position;
        //targetPosition = willPauseInMiddle ? slowDownPoint.position : endPoint.position;
    }

    void Update()
    {
        if (isClimbing) return;

        float targetSpeed = speed;
        float smoothTime = 0.5f;

        float currentWalkSpeed = animator.GetFloat("WalkSpeed");
        float smoothedWalkSpeed = Mathf.Lerp(currentWalkSpeed, targetSpeed, Time.deltaTime / smoothTime);

        animator.SetFloat("WalkSpeed", smoothedWalkSpeed);

        float maxSpeedMultiplier = 1.3f;
        float animationSpeed = (smoothedWalkSpeed - 0.5f) / 4.0f;
        animationSpeed = Mathf.Clamp(animationSpeed, 0.1f, maxSpeedMultiplier);

        animator.speed = 1;
        animator.SetFloat("WalkSpeed", speed);
        animator.Update(Time.deltaTime);

        UpdateAnimSpeed();


        if (!animator.GetBool("isWalking")) return;

        if (!isClimbing && animator.GetBool("isWalking"))
        {
            float swayMultiplier = Mathf.Lerp(0f, 1f, Mathf.Clamp01(speed / 3f));
            float sway = Mathf.Sin(Time.time * 3f) * 0.15f * swayMultiplier;
            transform.position += transform.right * sway * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) < stopThreshold)
        {
            if (targetPosition == climbPoint.position)
            {
                //transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);
                targetPosition = slowDownPoint.position;
                if (willPauseInMiddle && targetPosition == slowDownPoint.position && !reachedMiddle)
                {
                    reachedMiddle = true;
                    if (willPauseInMiddle)
                    {
                        PauseOnBack();
                    }
                    else
                    {
                        ResumeAfterMiddle();
                    }
                }
                else if (!willPauseInMiddle)
                {
                    targetPosition = endPoint.position;
                }
                //transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                //if ((targetPosition == slowDownPoint.position || (targetPosition == slowDownPoint.position - new Vector3(3f, 0.0f, 0.0f) && speed > 2.0f)) && !reachedMiddle)

            }
            //else if (Vector3.Distance(transform.position, endPoint.position) < 4f)
            //{
            //    StopAtEndSmooth();
            //    transform.position = endPoint.position;
            //}
        }
    }

    #endregion

    // ?????????????????????????????????????????????????????????????
    // Setup & Personality
    // ?????????????????????????????????????????????????????????????

    #region Setup

    private void OnTriggerEnter(Collider other)
    {
        if (isClimbing) return;

        if (other.CompareTag("CatClimbUp"))
        {
            Debug.Log("Climbing");
            StartCoroutine(ClimbRoutine(climbUpTrigger, other.transform.position));
        }
        else if (other.CompareTag("CatClimbDown"))
        {
            StartCoroutine(ClimbRoutine(jumpDownStartTrigger, other.transform.position));
        }
    }

    private IEnumerator ClimbRoutine(string animationTrigger, Vector3 destination)
    {

        isClimbing = true;
        animator.applyRootMotion = true;
        preClimbSpeed = saveSpeed;
        float originalHeight = transform.position.y;


        if (animationTrigger == climbUpTrigger)
        {
            animator.SetTrigger(jumpUpTrigger);
            animator.SetTrigger(climbUpTrigger);
            yield return new WaitForSeconds(1.292f);

            Debug.Log("time up");

            speed = 0f;
            animator.SetFloat("WalkSpeed", speed);
            transform.position = new Vector3(transform.position.x + 0.57f, 0.515f, transform.position.z);
            yield return new WaitForSeconds(0.2f);

            if (saveSpeed > 1.5f)
            {
                speed = Random.Range(0.5f, 1.5f);
            }
            else
            {
                speed = saveSpeed;
            }

            StartCoroutine(SmoothSpeedChange(0f, speed, 1f));
            //animator.SetFloat("WalkSpeed", speed);
            //UpdateAnimSpeed();

            animator.applyRootMotion = false;

            var tempPos = new Vector3(targetPosition.x + 0.57f, 0.515f, targetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, tempPos, speed * Time.deltaTime);

            isClimbing = false;
            yield return new WaitForSeconds(0.2f);
            animator.SetBool("isWalking", true);
        }
        else if (animationTrigger == jumpDownStartTrigger)
        {
            animator.SetTrigger(jumpDownStartTrigger);
            yield return new WaitForSeconds(jumpStartDuration);

            if (agent != null) agent.enabled = true;
            animator.SetTrigger(jumpDownLandTrigger);
            yield return new WaitForSeconds(jumpLandDuration);

            transform.position = new Vector3(transform.position.x, 0.083f, transform.position.z);
            //var tempPosDown = new Vector3(targetPosition.x, 0.2f, targetPosition.z);
            //transform.position = Vector3.MoveTowards(transform.position, tempPosDown, speed * Time.deltaTime);


            speed = 0f;
            animator.SetFloat("WalkSpeed", speed);

            StopAtEnd();
        }
    }

    void ResumeAfterMiddle()
    {
        StartCoroutine(SmoothSpeedChange(0f, preClimbSpeed, 1.2f)); //WAS ADDING IN START SPEED AS WELL AS TARGET SPEED
        ResumeMovement();
    }
    public void OnGameStopped()
    {
        if (gameStopped) return;
        gameStopped = true;

        StopAllCoroutines();
        StartCoroutine(SlowDownAndStop());
    }

    private IEnumerator SlowDownAndStop()
    {
        float startSpeed = speed;
        float duration = 1.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            speed = Mathf.Lerp(speed, 0f, elapsed/duration);
            animator.SetFloat("WalkSpeed", speed);
            UpdateAnimSpeed();
            elapsed += Time.deltaTime;
            yield return null;
        }

        speed = 0f;
        animator.SetBool("isWalking", false);
        if (shockedAnimations !=  null && shockedAnimations.Count > 0)
        {
            string shockedTrigger = shockedAnimations[Random.Range(0, shockedAnimations.Count)];
            GetComponent<Animator>().SetTrigger(shockedTrigger);
        }
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }
    public void SetTargetPositions(Transform start, Transform climb, Transform slowDown, Transform middle, Transform end)
    {
        startPoint = start;
        climbPoint = climb;
        slowDownPoint = slowDown;
        middlePoint = middle;
        endPoint = end;
        transform.position = startPoint.position;
    }

    void AssignPersonality()
    {
        personality = (Personality)Random.Range(0, System.Enum.GetValues(typeof(Personality)).Length);
        animator.SetBool("isWalking", true);

        switch (personality)
        {
            case Personality.Confident:
                speed = Random.Range(1.1f, 1.8f);
                animator.SetTrigger("Run");
                break;
            case Personality.Timid:
                speed = Random.Range(0.5f, 0.9f);
                break;
            case Personality.Playful:
                speed = Random.Range(2f, 2.8f);
                animator.SetTrigger("Jump");
                break;
            case Personality.Lazy:
                speed = Random.Range(1f, 1.5f);
                animator.SetTrigger("LieDown");
                break;
            case Personality.Mischievous:
                speed = Random.Range(2f, 3f);
                animator.SetTrigger("Crouch");
                break;
            case Personality.Social:
                speed = Random.Range(2f, 2.5f);
                animator.SetTrigger("LookAround");
                break;
        }

        if (personality == Personality.Mischievous || personality == Personality.Social)
        {
            targetPosition -= new Vector3(3f, 0f, 0f);
            wasOffset = true;
        }

        saveSpeed = speed;
    }

    #endregion

    // ?????????????????????????????????????????????????????????????
    // Animation & Speed Helpers
    // ?????????????????????????????????????????????????????????????

    #region Animation & Speed Control

    void UpdateAnimSpeed()
    {
        float animSpeed;

        if (speed < WALK_THRESHOLD)
        {
            animSpeed = speed / WALK_THRESHOLD;
            animator.SetFloat("AnimSpeed", Mathf.Clamp(animSpeed * 0.8f, 0.4f, 0.8f));
        }
        else if (speed < RUN_THRESHOLD)
        {
            animSpeed = speed - 0.35f;
            animator.SetFloat("AnimSpeed", Mathf.Clamp(animSpeed, 0.85f, 1.2f));
        }
        else
        {
            animSpeed = speed / 2.5f;
            animator.SetFloat("AnimSpeed", Mathf.Clamp(animSpeed, 1.0f, 1.3f));
        }

        animator.SetFloat("AnimSpeed", animSpeed);
    }

    IEnumerator SmoothSpeedChange(float startSpeed, float targetSpeed, float duration)
    {
        //float velocity = 0f;
        //float startSpeed = speed;
        //float smoothTime = duration;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            speed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / duration);
            animator.SetFloat("WalkSpeed", speed);
            UpdateAnimSpeed();
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        speed = targetSpeed;
        animator.SetFloat("WalkSpeed", speed);
        UpdateAnimSpeed();
        //yield return new WaitForSeconds(duration);

    }

    #endregion

    // ?????????????????????????????????????????????????????????????
    // State & Action Handlers
    // ?????????????????????????????????????????????????????????????

    #region Movement & Pause Logic

    void PauseOnBack()
    {
        hasPausedOnBack = true;
        //animator.SetBool("isWalking", false);
        //animator.SetBool("isRunning", false);
        //animator.SetBool("isCrouchMove", false);
        //animator.SetBool("isPaused", true);

        StartCoroutine(SmoothSpeedChange(speed, 0f, delaySpeed));
        StartCoroutine(ResumeAfterDelay(3f));
    }

    void DoAction()
    {
        hasDoneAction = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isDoingAction", true);

        StartCoroutine(ResumeAfterDelay(3f));
    }

    void ResumeMovement(float delay = 2.5f)
    {
        targetPosition = new Vector3 (endPoint.position.x, endPoint.position.y, endPoint.position.z);
        animator.SetBool("isWalking", true);
    }

    public void Initialise(CatSpawner spawner)
    {
        this.spawner = spawner;
    }

    void StopAtEnd()
    {
        if (hasReachedEnd) return;

        hasReachedEnd = true;

        if (spawner != null)
        {
            spawner.CatReachedDestination(gameObject);
        }
        else
        {
            Debug.LogError("Spawner is NULL in StopAtEnd! Cannot call CatReachedDestination.");
        }

        //animator.SetBool("isWalking", false);

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
        }
        
        if (aiController != null) aiController.enabled = true;

        CatAIController catAI = GetComponent<CatAIController>();
        if (catAI != null)
        {
            catAI.EnableWandering();
        }

        ////Transition to AI behaviour
        //if (agent != null && aiController != null)
        //{
        //    agent.enabled = true;
        //    aiController.enabled = true;
        //}

        //FindObjectOfType<CatSpawner>()?.CatReachedDestination();

        //this.enabled = false; //watch out it doesnt do this for other cats and just this one?
    }

    IEnumerator ResumeAfterDelay(float delay)
    {
        animator.SetBool("isWalking", true);

        if (hasPausedOnBack)
        {
            hasPausedOnBack = false;
            animator.SetBool("isPaused", false);
            yield return new WaitForSeconds(delay);
            StartCoroutine(SmoothSpeedChange(0f,saveSpeed, delaySpeed));
            ResumeMovement();
        }
        else if (hasDoneAction)
        {
            hasDoneAction = false;
            animator.SetBool("isDoingAction", false);
            yield return new WaitForSeconds(delay);
            StartCoroutine(SmoothSpeedChange(0f, saveSpeed, delaySpeed));
            ResumeMovement();
        }
    }

    IEnumerator TimidPause()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsWalking", true);
        isMoving = true;
    }

    #endregion
}

#endregion