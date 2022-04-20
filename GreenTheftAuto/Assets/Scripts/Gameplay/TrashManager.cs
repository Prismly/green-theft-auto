using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    [SerializeField] private float trashPerMinute;
    private float secondsPerTrash;
    private float timer;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] GameObject launchPoints;
    private float areaSizeX;
    private float areaSizeZ;
    [SerializeField] private float unprimedRotSpeed;
    [SerializeField] private float timeToPrime;
    [SerializeField] private float primedRotSpeed;
    [SerializeField] private float timeToFire;

    // Start is called before the first frame update
    private void Start()
    {
        secondsPerTrash = 60 / trashPerMinute;
        areaSizeX = GetComponent<BoxCollider>().size.x;
        areaSizeZ = GetComponent<BoxCollider>().size.z;
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= secondsPerTrash)
        {
            timer = 0;
            SpawnTarget();
        }
    }

    private void SpawnTarget()
    {
        float randX = Random.Range(0f, areaSizeX);
        randX -= areaSizeX / 2;
        float randZ = Random.Range(0f, areaSizeZ);
        randZ -= areaSizeZ / 2;

        GameObject newTarget = Instantiate(targetPrefab);
        newTarget.transform.parent = transform;
        newTarget.transform.localPosition = new Vector3(randX, newTarget.transform.position.y, randZ);
        newTarget.GetComponent<Target>().SetTimeToPrime(timeToPrime);
        newTarget.GetComponent<Target>().SetTimeToFire(timeToFire);
        newTarget.GetComponent<Target>().SetPrimedRotSpeed(primedRotSpeed);
        newTarget.GetComponent<Target>().SetRotationSpeed(unprimedRotSpeed);
    }
}
