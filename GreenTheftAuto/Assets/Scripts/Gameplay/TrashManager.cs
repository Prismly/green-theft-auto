using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashManager : MonoBehaviour
{
    [SerializeField] private GameObject playerGreen;
    [SerializeField] private GameObject playerOrange;
    [SerializeField] private GameObject playerRed;
    [SerializeField] private float trashPerMinute;
    private float secondsPerTrash;
    private float timer;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] GameObject launchPoints;
    private float areaSizeX;
    private float areaSizeZ;
    [SerializeField] private float unprimedRotSpeed;
    [SerializeField] private float timeToPrime;
    [SerializeField] private float primedRotSpeed;
    [SerializeField] private float timeToFire;
    [SerializeField] private float timeToDamage;
    [SerializeField] private float projectileGrav;
    [SerializeField] private int groundFindAttempts;
    [SerializeField] GameObject pointSystem;
    [SerializeField] GameObject audioManager;

    // Start is called before the first frame update
    private void Start()
    {
        secondsPerTrash = 60 / trashPerMinute;
        areaSizeX = GetComponent<BoxCollider>().size.x;
        areaSizeZ = GetComponent<BoxCollider>().size.z;
    }

    public void AddDifficulty()
    {
        trashPerMinute += 2;
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
        int attempts = 0;
        bool found = false;
        float randX = 0;
        float randZ = 0;
        
        while (attempts < groundFindAttempts && !found)
        {
            randX = Random.Range(0f, areaSizeX);
            randX -= areaSizeX / 2;
            randZ = Random.Range(0f, areaSizeZ);
            randZ -= areaSizeZ / 2;
            int layerMask = LayerMask.GetMask("Ground");

            //Debug.Log(transform.TransformPoint(new Vector3(randX, 15, randZ)));
            if (Physics.Raycast(transform.TransformPoint(new Vector3(randX, 15, randZ)), Vector3.down, 30, layerMask))
            {
                found = true;
            }
            attempts++;
        }

        GameObject newTarget = Instantiate(targetPrefab);
        newTarget.transform.parent = transform;
        newTarget.transform.localPosition = new Vector3(randX, newTarget.transform.position.y, randZ);
        newTarget.GetComponent<Target>().SetTimeToPrime(timeToPrime);
        newTarget.GetComponent<Target>().SetTimeToFire(timeToFire);
        newTarget.GetComponent<Target>().SetTimeToDamage(timeToDamage);
        newTarget.GetComponent<Target>().SetPrimedRotSpeed(primedRotSpeed);
        newTarget.GetComponent<Target>().SetRotationSpeed(unprimedRotSpeed);
        newTarget.GetComponent<Target>().SetMyManager(this);
        newTarget.GetComponent<Target>().SetPlayers(playerGreen, playerOrange, playerRed);
        newTarget.GetComponent<Target>().SetPointSystemRef(pointSystem.GetComponent<PointSystem>());
        newTarget.GetComponent<Target>().SetAudioManager(audioManager);
    }

    public void LaunchProjectileAt(Vector3 localTarget)
    {
        //First, select a random launch point to start the arc from
        int randChild = Random.Range(0, launchPoints.transform.childCount);
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.parent = transform;

        Vector3 localStart = launchPoints.transform.GetChild(randChild).transform.localPosition;
        Vector3 flatStart = new Vector3(localStart.x, 0, localStart.z);
        Vector3 flatEnd = new Vector3(localTarget.x, 0, localTarget.z);

        float horiDist = Vector3.Distance(flatStart, flatEnd);
        float vertDist = Vector3.Distance(new Vector3(0, localStart.y, 0), new Vector3(0, localTarget.y, 0));

        newProjectile.transform.localPosition = localStart;
        float initVelX = horiDist / timeToFire;
        float initVelY = (-vertDist - (0.5f * projectileGrav * timeToFire * timeToFire)) / timeToFire;

        //Debug.Log("t: " + timeToFire);
        //Debug.Log("hd: " + horiDist);
        //Debug.Log("vd: " + vertDist);
        //Debug.Log("velX: " + initVelX);
        //Debug.Log("velY: " + initVelY);

        newProjectile.GetComponent<Projectile>().SetGravity(projectileGrav);
        newProjectile.GetComponent<Rigidbody>().velocity = ((flatEnd - flatStart) / horiDist) * initVelX;
        newProjectile.GetComponent<Rigidbody>().velocity += Vector3.up * initVelY;
        Transform containerTransform = transform.parent;
        Quaternion rotation = Quaternion.Euler(containerTransform.eulerAngles.x, containerTransform.eulerAngles.y, containerTransform.eulerAngles.z);
        newProjectile.GetComponent<Rigidbody>().velocity = rotation * newProjectile.GetComponent<Rigidbody>().velocity;
        
    }
}
