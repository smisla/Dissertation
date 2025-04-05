using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CatBehaviour : MonoBehaviour
{
    public enum Personality { Confident, Timid, Playful, Lazy, Mischievous, Social }
    public Personality personality;

    

    private Animator animator;
    private Vector3 startPos;
    private Vector3 middlePos;
    private Vector3 endPos;

    //public GameObject startPointObj;
    //public GameObject slowDownPointObj;
    //public GameObject middlePointObj;
    //public GameObject endPointObj;


    private Transform startPoint;
    private Transform slowDownPoint;
    private Transform middlePoint;
    private Transform endPoint;

    private Vector3 targetPosition;

    private bool hasPausedOnBack = false;
    private bool hasDoneAction = false;

    private bool reachedMiddle = false;

    public float speed = 2f;
    public float saveSpeed = 2f;
    public float pauseChance = 0.3f;
    public float actionChance = 0.2f;
    private bool isMoving = true;

    public float animSpeed;
    public float delaySpeed = 3.0f;
    public float stopThreshold = 0.5f;
    public bool wasOffset;
    public const float CROUCH_THRESHOLD = 0.5f;
    public const float WALK_THRESHOLD = 1f;
    public const float RUN_THRESHOLD = 2f;

    //private Vector3 targetPosition;
    //private bool hasInteracted = false;

    void Start()
    {
       
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", true);

        //speed = Random.Range(0.5f, 3.0f);

        //animator.SetFloat("WalkSpeed", speed);

        //startPoint = startPointObj.transform;
        //middlePoint = middlePointObj.transform;
        //endPoint = endPointObj.transform;

        transform.position = startPoint.position;
        targetPosition = slowDownPoint.position;
        AssignPersonality();
        //ChooseMovement();
    }

    void Update()
    {
        float targetSpeed = speed;
        float smoothTime = 0.5f;

        float currentWalkSpeed = animator.GetFloat("WalkSpeed");
        float smoothedWalkSpeed = Mathf.Lerp(currentWalkSpeed, targetSpeed, Time.deltaTime / smoothTime);

        animator.SetFloat("WalkSpeed", smoothedWalkSpeed);

        //float baseSpeed = 1.0f;
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

        //float animationSpeed = Mathf.Lerp(0.9f, 1.2f, (speed - 0.5f) / 2.5f);
        //animationSpeed = Mathf.Clamp(animationSpeed, 0.9f, 1.2f);
        //animator.speed = animationSpeed;

        if (!animator.GetBool("isWalking")) return;

        if (animator.GetBool("isWalking"))
        {
            float sway = Mathf.Sin(Time.time * 3f) * 0.15f;
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
                    //if (Vector3.Distance(targetPosition, slowDownPoint.position - new Vector3(3f, 0f, 0f)) < 0.1f)
                    //{
                    //    targetPosition = middlePoint.position + new Vector3(3f, 0f, 0f);
                    //}
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
    //void UpdateAnimSpeed()
    //{
    //    float animSpeed = 1.0f;

    //    if (speed < 0.5f)
    //    {
    //        animSpeed = 0f; // Idle
    //    }
    //    else if (speed < 1.0f)
    //    {
    //        // Crouch walk (between 0.5 and 0.9)
    //        animSpeed = Mathf.Lerp(0.3f, 0.6f, (speed - 0.5f) / 0.4f);
    //    }
    //    else if (speed < 2.0f)
    //    {
    //        // Normal walk (1.0 to 1.9)
    //        animSpeed = Mathf.Lerp(0.6f, 1.0f, (speed - 1.0f) / 0.9f);
    //    }
    //    else
    //    {
    //        // Run (2.0 to 3.0)
    //        animSpeed = Mathf.Lerp(1.0f, 1.2f, (speed - 2.0f)); // small bump up
    //    }

    //    animator.SetFloat("AnimSpeed", animSpeed);
    //}
    void UpdateAnimSpeed()
    {
        float animSpeed;

        if (speed < WALK_THRESHOLD) // 0.5 - 0.9 range (Crouch Walk)
        {
            animSpeed = speed / WALK_THRESHOLD; // Normalize crouch speed
            animator.SetFloat("AnimSpeed", Mathf.Clamp(animSpeed * 0.8f, 0.4f, 0.8f));
        }
        else if (speed < RUN_THRESHOLD) // 1.0 - 1.9 range (Walk)
        {
            animSpeed = speed - 0.35f;
            animator.SetFloat("AnimSpeed", Mathf.Clamp(animSpeed, 0.85f, 1.2f));
        }
        else // 2.0 - 3.0 range (Run)
        {
            animSpeed = speed / 2.5f; // Normalize run speed
            animator.SetFloat("AnimSpeed", Mathf.Clamp(animSpeed, 1.0f, 1.3f));
        }
        //float animSpeed = 1f;


        //if (speed < WALK_THRESHOLD) // 0.5 - 0.9 speed range (Crouch Walk)
        //{
        //    animSpeed = WALK_THRESHOLD - speed;
        //    animator.SetFloat("AnimSpeed", Mathf.Min(0.8f, 1.0f - animSpeed));
        //}
        //else if (speed < RUN_THRESHOLD) // 1.0 - 1.9 speed range (Walk)
        //{
        //    animSpeed = speed;
        //    animator.SetFloat("AnimSpeed", animSpeed - 0.35f);
        //    stopThreshold = 0.7f;
        //}
        //else // 2.0 - 3.0 speed range (Run)
        //{ 
        //    animator.SetFloat("AnimSpeed", Mathf.Min(0.7f, 1.0f - animSpeed));
        //    stopThreshold = 1.5f;
        //    delaySpeed = 4.0f;

        //}

        animator.SetFloat("AnimSpeed", animSpeed);
    }

    public void SetTargetPositions(Transform start, Transform slowDown, Transform middle, Transform end)
    {
        startPoint = start;
        slowDownPoint = slowDown;
        middlePoint = middle;
        endPoint = end;
        transform.position = startPoint.position;
    }

    void PauseOnBack()//float delay)
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
    void AssignPersonality()
    {
        personality = (Personality)Random.Range(0, System.Enum.GetValues(typeof(Personality)).Length);
        animator.SetBool("isWalking", true);

        switch (personality)
        {
            case Personality.Confident:
                speed = Random.Range(1.1f, 1.8f); // Fast walk
                animator.SetTrigger("Run");
                break;
            case Personality.Timid:
                speed = Random.Range(0.5f, 0.9f); // Slow cautious walk
                //animator.SetTrigger("CrouchMove");
                break;
            case Personality.Playful:
                speed = Random.Range(2f, 2.8f);
                animator.SetTrigger("Jump");
                break;
            case Personality.Lazy:
                speed = Random.Range(1f, 1.5f); // Very slow
                animator.SetTrigger("LieDown");
                break;
            case Personality.Mischievous:
                speed = Random.Range(2f, 3f);
                animator.SetTrigger("Crouch");
                //targetPosition -= new Vector3(3f, 0.0f, 0.0f);
                break;
            case Personality.Social:
                speed = Random.Range(2f, 2.5f);
                animator.SetTrigger("LookAround");
                //targetPosition -= new Vector3(3f, 0.0f, 0.0f);
                break;
        }
        if (personality == Personality.Mischievous || personality == Personality.Social)
        {
            targetPosition -= new Vector3(3f, 0f, 0f);
            wasOffset = true;
        }

        saveSpeed = speed;
        
    }


    void PerformAction()
    {
        switch (personality)
        {
            case Personality.Confident:
                ResumeMovement();
                break;
            case Personality.Timid:
                StartCoroutine(TimidPause());
                break;
            case Personality.Playful:
                animator.SetTrigger("Jump");
                ResumeMovement(1f);
                break;
            case Personality.Lazy:
                animator.SetTrigger("LieDown");
                ResumeMovement(2f);
                break;
            case Personality.Mischievous:
                animator.SetTrigger("Crouch");
                ResumeMovement(1.5f);
                break;
            case Personality.Social:
                animator.SetTrigger("LookAround");
                ResumeMovement(1.2f);
                break;
        }
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

    void ResumeMovement(float delay = 2.5f)
    {
        targetPosition = endPoint.position;
        animator.SetBool("isWalking", true);
        //StartCoroutine(ResumeAfterDelay(delay));
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
}
