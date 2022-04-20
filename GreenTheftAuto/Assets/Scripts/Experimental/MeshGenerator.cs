using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    [SerializeField] [Tooltip("")] int[] triangles;
    [SerializeField] Color[] newColors;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void Update()
    {
        mesh.Clear();

        Vector3[] vertices = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            vertices[i] = transform.GetChild(i).position;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = newColors;
    }

    private void OnDrawGizmos()
    {
    }
}
