using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    [System.Serializable]
    private struct TrackEdge
    {
        [SerializeField] PathPoint point;
        //[SerializeField] PathColor color;
    }

    [SerializeField] [Tooltip("")] TrackEdge[] nextPoints;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            int sibIndex = transform.GetSiblingIndex();
            if (sibIndex < transform.parent.childCount - 1)
            {
                // There is at least one path segment remaining from this point. Change the player's max and min angles for the upcoming segment.
                Vector3 from = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 to;
                if (sibIndex >= transform.parent.childCount)
                {
                    to = new Vector3(transform.parent.GetChild(0).position.x, 0, transform.parent.GetChild(0).position.z);
                }
                else
                {
                    to = new Vector3(transform.parent.GetChild(sibIndex + 1).position.x, 0, transform.parent.GetChild(sibIndex + 1).position.z);
                }
                
                float newHomeAngle = Quaternion.LookRotation(to - from, Vector3.up).eulerAngles.y;
                other.GetComponent<PlayerTruck>().SetHomeAngle(newHomeAngle);
            }
        }
    }

    private void OnDrawGizmos()
    {
        
    }
}
