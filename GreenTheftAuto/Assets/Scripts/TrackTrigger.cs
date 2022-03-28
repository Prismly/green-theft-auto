using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTrigger : MonoBehaviour
{
    [SerializeField] [Tooltip("The instance of the camera script that manages the camera's movement")] private CameraController mainCam;
    private Transform connectedTrack;

    private void Start()
    {
        connectedTrack = transform.parent.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (mainCam.GetCurrentTrack() != connectedTrack)
            {
                mainCam.SetCurrentTrack(connectedTrack);
                UpdatePlayerHomeAngle(other);
                mainCam.SetRotTarget(connectedTrack.GetChild(1));
            }
            else
            {
                UpdatePlayerHomeAngle(other);
                mainCam.SetRotTarget(connectedTrack.GetChild(transform.parent.GetSiblingIndex() + 1));
            }
        }
    }

    private void UpdatePlayerHomeAngle(Collider other)
    {
        int sibIndex = transform.parent.GetSiblingIndex();
        if (sibIndex < connectedTrack.childCount - 1)
        {
            // There is at least one path segment remaining from this point. Change the player's max and min angles for the upcoming segment.
            Vector3 from = new Vector3(transform.parent.position.x, 0, transform.parent.position.z);
            Vector3 to = new Vector3(connectedTrack.GetChild(sibIndex + 1).position.x, 0, connectedTrack.GetChild(sibIndex + 1).position.z);
            float newHomeAngle = Quaternion.LookRotation(to - from, Vector3.up).eulerAngles.y;
            other.GetComponent<PlayerTruck>().SetHomeAngle(newHomeAngle);
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 colliderSizeHalved = GetComponent<BoxCollider>().size / 2;
        Vector3 xDim = transform.rotation * Vector3.right * colliderSizeHalved.x;
        Vector3 yDim = transform.rotation * Vector3.up * colliderSizeHalved.y;
        Vector3 zDim = transform.rotation * Vector3.forward * colliderSizeHalved.z;
        Gizmos.color = transform.parent.parent.GetComponent<Track>().GetTrackColor();

        // Left-hand (with no rotation) face
        Gizmos.DrawLine(transform.position - xDim + yDim + zDim, transform.position - xDim - yDim + zDim);
        Gizmos.DrawLine(transform.position - xDim - yDim + zDim, transform.position - xDim - yDim - zDim);
        Gizmos.DrawLine(transform.position - xDim - yDim - zDim, transform.position - xDim + yDim - zDim);
        Gizmos.DrawLine(transform.position - xDim + yDim - zDim, transform.position - xDim + yDim + zDim);

        // Right-hand (with no rotation) face
        Gizmos.DrawLine(transform.position + xDim + yDim + zDim, transform.position + xDim - yDim + zDim);
        Gizmos.DrawLine(transform.position + xDim - yDim + zDim, transform.position + xDim - yDim - zDim);
        Gizmos.DrawLine(transform.position + xDim - yDim - zDim, transform.position + xDim + yDim - zDim);
        Gizmos.DrawLine(transform.position + xDim + yDim - zDim, transform.position + xDim + yDim + zDim);

        // Connecting lines
        Gizmos.DrawLine(transform.position + xDim + yDim + zDim, transform.position - xDim + yDim + zDim);
        Gizmos.DrawLine(transform.position + xDim + yDim - zDim, transform.position - xDim + yDim - zDim);
        Gizmos.DrawLine(transform.position + xDim - yDim - zDim, transform.position - xDim - yDim - zDim);
        Gizmos.DrawLine(transform.position + xDim - yDim + zDim, transform.position - xDim - yDim + zDim);
    }
}
