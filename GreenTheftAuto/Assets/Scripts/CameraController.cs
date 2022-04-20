using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Tooltip("The player character object")] private Transform playerTransform;
    [SerializeField] [Tooltip("The track that the camera is currently following")] private Transform currentTrack;
    [SerializeField] private GameObject objectToMove;
    private Transform originalTrackDebug;
    [SerializeField] [Tooltip("The speed at which the camera moves along its current track, in units / second")] private float followSpeed;
    [SerializeField] [Tooltip("The time it should take for the camera to rotate between two points, in seconds")] private float slerpTime;
    private Transform targetPathPoint;
    [SerializeField] [Tooltip("Determines how much extra rotation is given to the camera, in addition to what is applied to make it look at the next track point")] private Vector3 offsetRotation;
    private bool followingTrack = true;
    private bool turning = false;
    private int pathIndex;
    private int lookIndex;
    private Transform rotTarget = null;
    [SerializeField] GameObject debugSphereBottomText;

    private void Start()
    {
        originalTrackDebug = currentTrack;
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            DebugReset();
        }

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
            objectToMove.transform.rotation = Quaternion.Slerp(objectToMove.transform.rotation, targetRotation, slerpTime * Time.deltaTime);
            if (Quaternion.Angle(targetRotation, transform.rotation) < 0.1f)
            {
                // The camera has finished rotating; no need to call Slerp until next path point
                rotTarget = null;
            }
        }  

        if (followingTrack)
        {
            objectToMove.transform.position = Vector3.MoveTowards(transform.position, targetPathPoint.position, followSpeed * Time.deltaTime);
        }
    }

    //IEnumerator LookAtPoint()
    //{
    //    yield return null;
    //}

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

    private void DebugReset()
    {
        playerTransform.position = new Vector3(0, 0, -3);
        playerTransform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = new Vector3(0, 3, -10);
        transform.rotation = Quaternion.Euler(10, 0, 0);
        currentTrack = originalTrackDebug;
        InitializePathVars();
        playerTransform.GetComponent<PlayerTruck>().SetHomeAngle(0);
        playerTransform.GetComponent<PlayerTruck>().SetFacingAngle(0);
        rotTarget = null;
    }
}
