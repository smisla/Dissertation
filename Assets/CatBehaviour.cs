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

        startPoint = startPointObj.transform;
        middlePoint = middlePointObj.transform;
        endPoint = endPointObj.transform;

        transform.position = startPoint.position;
        targetPosition = middlePoint.position;
       // AssignPersonality();
        //ChooseMovement();
    }

    void Update()
    {
        if (animator.GetBool("isWalking"))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            if (!reachedMiddle)
            {
                reachedMiddle = true;
                if (!hasPausedOnBack && Random.value < pauseChance)
                {
                    PauseOnBack();
                }
                else if (!hasDoneAction && Random.value < actionChance)
                {
                    DoAction();
                }
                else
                {
                    ResumeMovement();
                }
                targetPosition = endPoint.position;
                return;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        //if (animator.GetBool("isWalking"))
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        //}
        //if (isMoving)
        //{
        //    MoveCat();
        //}
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
    //void ChooseMovement()
    //{
    //    switch (personality)
    //    {
    //        case Personality.Confident:
    //            animator.SetTrigger("Run");
    //            targetPosition = GetRandomTarget();
    //            break;
    //        case Personality.Timid:
    //            StartCoroutine(TimidBehaviour());
    //            break;
    //        case Personality.Playful:
    //            StartCoroutine(PlayfulBehaviour());
    //            break;
    //        case Personality.Lazy:
    //            StartCoroutine(LazyBehaviour());
    //            break;
    //        case Personality.Mischievous:
    //            StartCoroutine(MischievousBehaviour());
    //            break;
    //        case Personality.Social:
    //            StartCoroutine(SocialBehaviour());
    //            break;
    //    }
    //}
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
        StartCoroutine(ResumeAfterDelay(delay));
    }

    IEnumerator ResumeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isPausedOnBack", false);
        animator.SetBool("isDoingPersonalityAction", false);
        animator.SetBool("isWalking", true);
    }

    IEnumerator TimidPause()
    {
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsWalking", true);
        isMoving = true;
    }

    //IEnumerator TimidBehaviour()
    //{
    //    animator.SetTrigger("Walk");
    //    yield return new WaitforSeconds(Random.Range(1f, 3f));
    //    animator.SetTrigger("Idle");
    //}

    //IEnumerator PlayfulBehaviour()
    //{
    //    animator.SetTrigger("Jump");
    //    yield return new WaitforSeconds(1f);
    //    animator.SetTrigger("Run");
    //}

    //IEnumerator LazyBehaviour()
    //{
    //    animator.SetTrigger("LieDown");
    //    yield return new WaitforSeconds(Random.Range(3f, 5f));
    //    animator.SetTrigger("Walk");
    //}

    //IEnumerator MischievousBehaviour()
    //{
    //    animator.SetTrigger("Crouch");
    //    yield return new WaitforSeconds(1.5f);
    //    animator.SetTrigger("Run");
    //}

    //IEnumerator SocialBehaviour()
    //{
    //    animator.SetTrigger("Walk");
    //    yield return new WaitforSeconds(2f);
    //    animator.SetTrigger("Interact");
    //}

    //private Vector3 GetRandomTarget()
    //{
    //    return new Vector3(transform.position.x + GetRandomTarget().Range(1f, 5f), transform.position.y, transform.position.z);
    //}
}
