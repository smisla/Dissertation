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

    public GameObject startPointObj;
    public GameObject middlePointObj;
    public GameObject endPointObj;


    private Transform startPoint;
    private Transform middlePoint;
    private Transform endPoint;

    private Vector3 targetPosition;

    private bool hasPausedOnBack = false;
    private bool hasDoneAction = false;

    private bool reachedMiddle = false;

    public float speed = 2f;
    public float pauseChance = 0.3f;
    public float actionChance = 0.2f;
    private bool isMoving = true;

    //private Vector3 targetPosition;
    //private bool hasInteracted = false;

    void Start()
    {
       
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", true);

        speed = Random.Range(0.5f, 3.0f);

        animator.SetFloat("WalkSpeed", speed);

        AssignPersonality();

        //startPoint = startPointObj.transform;
        //middlePoint = middlePointObj.transform;
        //endPoint = endPointObj.transform;

        transform.position = startPoint.position;
        targetPosition = middlePoint.position;
       // AssignPersonality();
        //ChooseMovement();
    }

    void Update()
    {
        float targetSpeed = speed;
        float smoothTime = 0.5f;

        float currentWalkSpeed = animator.GetFloat("WalkSpeed");
        float smoothedWalkSpeed = Mathf.Lerp(currentWalkSpeed, targetSpeed, Time.deltaTime / smoothTime);

        animator.SetFloat("WalkSpeed", smoothedWalkSpeed);

        float baseSpeed = 1.0f;
        float maxSpeedMultiplier = 1.3f;
        float animationSpeed = baseSpeed + ((smoothedWalkSpeed - 0.5f) / 4.0f);
        animationSpeed = Mathf.Clamp(animationSpeed, 0.85f, maxSpeedMultiplier);

        animator.speed = animationSpeed;
        //animator.SetFloat("WalkSpeed", speed);
        //animator.Update(Time.deltaTime);

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

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (targetPosition == middlePoint.position && !reachedMiddle)
            {
                reachedMiddle = true;

                if (Random.value < pauseChance)
                {
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
        if (!hasPausedOnBack && Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            
        }
    }

    public void SetTargetPositions(Transform start, Transform middle, Transform end)
    {
        startPoint = start;
        middlePoint = middle;
        endPoint = end;
        transform.position = startPoint.position;
    }

    void PauseOnBack()
    {
        hasPausedOnBack = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isCrouchMove", false);

        animator.SetBool("isPaused", true);

        StartCoroutine(ResumeAfterDelay(2f));
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
                speed = Random.Range(1.1f, 2.0f); // Fast walk
                animator.SetTrigger("Run");
                break;
            case Personality.Timid:
                speed = Random.Range(0.5f, 0.9f); // Slow cautious walk
                //animator.SetTrigger("CrouchMove");
                break;
            case Personality.Playful:
                speed = Random.Range(1.8f, 2.8f);
                animator.SetTrigger("Jump");
                break;
            case Personality.Lazy:
                speed = Random.Range(1f, 1.5f); // Very slow
                animator.SetTrigger("LieDown");
                break;
            case Personality.Mischievous:
                speed = Random.Range(2f, 3f);
                animator.SetTrigger("Crouch");
                break;
            case Personality.Social:
                speed = Random.Range(1.5f, 2.5f);
                animator.SetTrigger("LookAround");
                break;
        }
    }


    void MoveCat()
    {
        if (!reachedMiddle)
        {
            transform.position = Vector3.MoveTowards(transform.position, middlePos, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, middlePos) < 0.1f)
            {
                reachedMiddle = true;
                PerformAction();
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, endPos) < 0.1f)
            {
                Destroy(gameObject);
                // Cat goes into play mode

            }
        }
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

    void ResumeMovement(float delay = 0.5f)
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
            ResumeMovement();
        }

        else if (hasDoneAction)
        {
            hasDoneAction = false;
            animator.SetBool("isDoingAction", false);
            yield return new WaitForSeconds(delay);
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
