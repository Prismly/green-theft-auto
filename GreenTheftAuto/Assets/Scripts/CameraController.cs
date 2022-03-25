using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Tooltip("")] private GameObject playerObject;
    [SerializeField] [Tooltip("")] private Vector3 offset;

    private void Update()
    {
        transform.position = playerObject.transform.position + offset;
    }
}
