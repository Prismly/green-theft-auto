using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinSpawner : MonoBehaviour
{
    [SerializeField] GameObject binPrefab;
    GameObject myBin;

    private void Update()
    {
        if (myBin == null)
        {
            SpawnBin();
        }
    }

    private void SpawnBin()
    {
        myBin = Instantiate(binPrefab);
        myBin.transform.parent = transform;

        float areaSizeX = transform.localScale.x;
        float areaSizeZ = transform.localScale.z;

        float randX = Random.Range(-(areaSizeX / 2) + 1, (areaSizeX / 2) - 1) / areaSizeX;
        float randZ = Random.Range(-(areaSizeZ / 2) + 1, (areaSizeZ / 2) - 1) / areaSizeZ;

        myBin.transform.localPosition = new Vector3(randX, 1, randZ);
    }
}
