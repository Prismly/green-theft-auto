using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float gravity;

    public void SetGravity(float newGravity)
    {
        gravity = newGravity;
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity += Vector3.up * gravity * Time.deltaTime;
        if (transform.position.y < -50)
        {
            SelfDestruct();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            SelfDestruct();
        }
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
