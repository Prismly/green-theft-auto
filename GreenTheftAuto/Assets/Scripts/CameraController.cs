using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Tooltip("")] private Transform playerTransform;
    [SerializeField] [Tooltip("")] private Transform pathTransform;
    [SerializeField] [Tooltip("")] private float followSpeed;
    [SerializeField] [Tooltip("The time it should take for the camera to rotate between two points, in seconds")] private float slerpTime;
    private Transform targetPathPoint;
    [SerializeField] [Tooltip("")] private Vector3 offset;
    private bool followingPath = true;
    private bool turning = false;
    private int pathIndex;
    private Transform rotTarget = null;
    [SerializeField] GameObject debugSphereBottomText;

    private void OnDrawGizmos()
    {
        Vector3 startOfSeg;
        Vector3 endOfSeg;
        Color[] rainbow = { Color.red, new Color(1, 0.5f, 0), Color.yellow, Color.green, Color.blue, new Color(0.5f, 0, 1), Color.magenta }; 
        for (int i = 0; i < pathTransform.childCount - 1; i++)
        {
            startOfSeg = pathTransform.GetChild(i).position;
            endOfSeg = pathTransform.GetChild(i + 1).position;
            Gizmos.color = rainbow[i % rainbow.Length];
            Gizmos.DrawLine(startOfSeg, endOfSeg);
        }
    }

    private void Start()
    {
        pathIndex = 0;
        targetPathPoint = pathTransform.GetChild(1);
    }

    private void Update()
    {
        float distFromTarget = Vector3.Distance(transform.position, targetPathPoint.position);

        if (distFromTarget < 0.1f)
        {
            // The camera has finished traveling along the current path segment
            if (pathIndex < pathTransform.childCount - 1)
            {
                // There is at least one more segment for the camera to follow; start following it
                pathIndex++;
                targetPathPoint = pathTransform.GetChild(pathIndex);
            }
            else
            {
                followingPath = false;
            }
        }

        bool doSlerp = false;

        if (distFromTarget < slerpTime * followSpeed / 2 && pathIndex < pathTransform.childCount - 1)
        {
            Debug.Log("doSlerp");
            Debug.Log(slerpTime * followSpeed / 2);
            rotTarget = pathTransform.GetChild(pathIndex + 1);
            GameObject newDebugSphereBottomText = Instantiate(debugSphereBottomText);
            newDebugSphereBottomText.transform.position = rotTarget.position;
            doSlerp = true;
            turning = true;
        }
        else if (rotTarget != null)
        {
            doSlerp = true;
        }

        if (doSlerp)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotTarget.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slerpTime * Time.deltaTime);
            if (Quaternion.Angle(targetRotation, transform.rotation) < 0.1f)
            {
                // The camera has finished rotating; no need to call Slerp until next path point
                rotTarget = null;
            }
        }  

        if (followingPath)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPathPoint.position, followSpeed * Time.deltaTime);
        }
    }
}
