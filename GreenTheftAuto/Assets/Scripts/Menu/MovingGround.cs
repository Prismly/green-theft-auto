using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    [SerializeField] [Tooltip("")] float scrollSpeed;

    private void Update()
    {
        transform.position += Vector3.back * scrollSpeed * Time.deltaTime;
    }

    public void TeleportHome()
    {
        transform.position += Vector3.forward * transform.localScale.z * 2;
    }
}
