using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Tooltip("")] private Transform playerTransform;
    [SerializeField] [Tooltip("")] private Transform currentTrack;
    [SerializeField] [Tooltip("")] private float followSpeed;
    [SerializeField] [Tooltip("The time it should take for the camera to rotate between two points, in seconds")] private float slerpTime;
    private Transform targetPathPoint;
    [SerializeField] [Tooltip("")] private Vector3 offsetRotation;
    private bool followingTrack = true;
    private bool turning = false;
    private int pathIndex;
    private int lookIndex;
    private Transform rotTarget = null;
    [SerializeField] GameObject debugSphereBottomText;

    private void Start()
    {
        InitializePathVars();
    }

    private void InitializePathVars()
    {
        pathIndex = 0;
        lookIndex = 1;
        targetPathPoint = currentTrack.GetChild(1);
    }

    private void Update()
    {
        float distFromTarget = Vector3.Distance(transform.position, targetPathPoint.position);

        if (distFromTarget < 0.1f)
        {
            // The camera has finished traveling along the current path segment
            if (pathIndex < currentTrack.childCount - 1)
            {
                // There is at least one more segment for the camera to follow; start following it
                pathIndex++;
                targetPathPoint = currentTrack.GetChild(pathIndex + 1);
                distFromTarget = Vector3.Distance(transform.position, targetPathPoint.position);
            }
            else
            {
                followingTrack = false;
            }
        }

        bool doSlerp = false;

        if (distFromTarget < slerpTime * followSpeed / 2 && pathIndex < currentTrack.childCount - 1 && pathIndex == lookIndex - 1)
        {
            //Debug.Log("doSlerp");
            //Debug.Log(slerpTime * followSpeed / 2);
            lookIndex++;
            rotTarget = currentTrack.GetChild(lookIndex);
            GameObject newDebugSphereBottomText = Instantiate(debugSphereBottomText);
            newDebugSphereBottomText.transform.position = rotTarget.position;
            doSlerp = true;
        }
        else if (rotTarget != null)
        {
            doSlerp = true;
        }

        if (doSlerp)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotTarget.position - transform.position, Vector3.up);
            targetRotation.eulerAngles = targetRotation.eulerAngles + offsetRotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slerpTime * Time.deltaTime);
            if (Quaternion.Angle(targetRotation, transform.rotation) < 0.1f)
            {
                // The camera has finished rotating; no need to call Slerp until next path point
                rotTarget = null;
            }
        }  

        if (followingTrack)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPathPoint.position, followSpeed * Time.deltaTime);
        }
    }

    IEnumerator LookAtPoint()
    {
        yield return null;
    }

    public Transform GetCurrentTrack()
    {
        return currentTrack;
    }

    public void SetCurrentTrack(Transform newTrack)
    {
        currentTrack = newTrack;
        InitializePathVars();
    }

    public void SetRotTarget(Transform newRotTarget)
    {
        rotTarget = newRotTarget;
    }
}
