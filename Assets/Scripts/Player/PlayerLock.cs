using UnityEngine;

public class PlayerLock : MonoBehaviour
{
    public Vector3 fixedPos = new Vector3(0, 1, 0);
    public bool lockRotation = true;

    private void LateUpdate()
    {
        transform.position = fixedPos;

        if (lockRotation)
        {
            transform.rotation = Quaternion.identity;
        }
    }
}
