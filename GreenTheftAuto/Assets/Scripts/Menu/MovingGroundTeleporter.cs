using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGroundTeleporter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MovingGround>() != null)
        {
            other.GetComponent<MovingGround>().TeleportHome();
        }
    }
}
