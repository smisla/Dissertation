using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
    public float pauseChance = 0.3f;
    public float actionChance = 0.2f;

    [Header("Animation Settings")]
    public float animSpeed;
    public float delaySpeed = 3.0f;
    public float stopThreshold = 0.5f;
    public const float CROUCH_THRESHOLD = 0.5f;
    public const float WALK_THRESHOLD = 1f;
    public const float RUN_THRESHOLD = 2f;

    [Header("Debug & State Flags")]
    public bool wasOffset;

    #region Private Components
    private Animator animator;
    #endregion

    #region Waypoint Transforms
    private Vector3 startPos;
    private Vector3 middlePos;
    private Vector3 endPos;

    private Transform startPoint;
    private Transform slowDownPoint;
    private Transform middlePoint;
    private Transform endPoint;

    private Vector3 targetPosition;
    #endregion

    #region State Flags
    private bool hasPausedOnBack = false;
    private bool hasDoneAction = false;
    private bool reachedMiddle = false;
    private bool isMoving = true;
    #endregion

    // ?????????????????????????????????????????????????????????????
    // Unity Methods
    // ?????????????????????????????????????????????????????????????

    #region Unity Lifecycle

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", true);

        transform.position = startPoint.position;
        targetPosition = slowDownPoint.position;

        AssignPersonality();
    }

    void Update()
    {
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

        Debug.Log("cat: " + this.gameObject.name);
        Debug.Log("speed " + speed);
        Debug.Log("animSpeed " + animSpeed);

        if (!animator.GetBool("isWalking")) return;

        if (animator.GetBool("isWalking"))
        {
            float swayMultiplier = Mathf.Lerp(0f, 1f, Mathf.Clamp01(speed / 3f));
            float sway = Mathf.Sin(Time.time * 3f) * 0.15f * swayMultiplier;
            transform.position += transform.right * sway * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) < stopThreshold)
        {
            if ((targetPosition == slowDownPoint.position || (targetPosition == slowDownPoint.position - new Vector3(3f, 0.0f, 0.0f) && speed > 2.0f)) && !reachedMiddle)
            {
                reachedMiddle = true;

                if (Random.value < pauseChance)
                {
                    targetPosition = wasOffset ? middlePoint.position + new Vector3(4.5f, 0f, 0f) : middlePoint.position;
                    PauseOnBack();
                }
                else
                {
                    ResumeMovement();
                }
            }
            else if (targetPosition == endPoint.position)
            {
                StopAtEnd();
            }
        }
    }

    #endregion

    // ?????????????????????????????????????????????????????????????
    // Setup & Personality
    // ?????????????????????????????????????????????????????????????

    #region Setup

    public void SetTargetPositions(Transform start, Transform slowDown, Transform middle, Transform end)
    {
        startPoint = start;
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

    IEnumerator SmoothSpeedChange(float targetSpeed, float duration)
    {
        float startSpeed = speed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            speed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        speed = targetSpeed;
        animator.SetFloat("WalkSpeed", speed);
        UpdateAnimSpeed();
    }

    #endregion

    // ?????????????????????????????????????????????????????????????
    // State & Action Handlers
    // ?????????????????????????????????????????????????????????????

    #region Movement & Pause Logic

    void PauseOnBack()
    {
        hasPausedOnBack = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isCrouchMove", false);
        animator.SetBool("isPaused", true);

        StartCoroutine(SmoothSpeedChange(0f, delaySpeed));
        StartCoroutine(ResumeAfterDelay(5f));
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
        targetPosition = endPoint.position;
        animator.SetBool("isWalking", true);
    }

    void StopAtEnd()
    {
        animator.SetBool("isWalking", false);
    }

    IEnumerator ResumeAfterDelay(float delay)
    {
        animator.SetBool("isWalking", true);

        if (hasPausedOnBack)
        {
            hasPausedOnBack = false;
            animator.SetBool("isPaused", false);
            yield return new WaitForSeconds(delay);
            StartCoroutine(SmoothSpeedChange(saveSpeed, delaySpeed));
            ResumeMovement();
        }
        else if (hasDoneAction)
        {
            hasDoneAction = false;
            animator.SetBool("isDoingAction", false);
            yield return new WaitForSeconds(delay);
            StartCoroutine(SmoothSpeedChange(saveSpeed, delaySpeed));
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

