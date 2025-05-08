using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AlignToTerrain : MonoBehaviour
{
    public string bodyChildName = "CartoonCat"; // Name of the child object with the visible mesh
    public float raycastDistance = 2f;
    public float alignSpeed = 5f;
    public float fixedBaseOffset = 0f;

    private Transform body;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.baseOffset = fixedBaseOffset;

        // Find child automatically
        body = transform.Find(bodyChildName);
        if (body == null)
        {
            Debug.LogError($"[CatTerrainAligner] Could not find child named '{bodyChildName}' on {gameObject.name}");
        }
    }

    void Update()
    {
        // Lock base offset
        if (agent.baseOffset != fixedBaseOffset)
        {
            agent.baseOffset = fixedBaseOffset;
        }

        // Align to terrain
        if (body != null)
        {
            if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, raycastDistance))
            {
                Quaternion targetRotation = Quaternion.FromToRotation(body.up, hit.normal) * body.rotation;
                body.rotation = Quaternion.Slerp(body.rotation, targetRotation, Time.deltaTime * alignSpeed);
            }
        }
    }
}


