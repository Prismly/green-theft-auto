using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] Material normalTruck;
    [SerializeField] Material dangerTruck;
    [SerializeField] Material hurtTruck;
    [SerializeField] GameObject damageCollider;
    private TrashManager myManager;
    private float timeToPrime;
    private float primedRotSpeed;
    private float timeToFire;
    private float timeToDamage;
    private float timer;
    private bool primed = false;
    private bool damaging = false;
    private bool countedCloseCall = false;
    private bool countedHurt = false;
    GameObject player;
    PointSystem pointSystem;

    public void SetPlayer(GameObject playerTruck)
    {
        player = playerTruck;
    }

    public void SetTimeToPrime(float newVal)
    {
        timeToPrime = newVal;
    }

    public void SetTimeToFire(float newVal)
    {
        timeToFire = newVal;
    }

    public void SetTimeToDamage(float newVal)
    {
        timeToDamage = newVal;
    }

    public void SetPrimedRotSpeed(float newVal)
    {
        primedRotSpeed = newVal;
    }

    public void SetRotationSpeed(float newVal)
    {
        GetComponent<Rotater>().SetRotateSpeed(newVal);
    }

    public void SetMyManager(TrashManager manager)
    {
        myManager = manager;
    }

    public void SetPointSystemRef(PointSystem ps)
    {
        pointSystem = ps;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (!primed && timer >= timeToPrime)
        {
            //Done priming, start projectile arc
            primed = true;
            SetRotationSpeed(primedRotSpeed);
            timer = 0;
            myManager.LaunchProjectileAt(transform.localPosition);
        }
        else if (primed)
        {
            if (timer >= timeToFire && !damaging)
            {
                //Projectile lands, this object because damaging for a short moment
                damaging = true;
                damageCollider.SetActive(true);
                timer = 0;
            }
            else if (timer >= timeToDamage && damaging)
            {
                //Projectile is done doing damage, disappear
                player.GetComponent<MeshRenderer>().material = normalTruck;
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (damaging)
            {
                other.GetComponent<MeshRenderer>().material = hurtTruck;
                if (!countedHurt)
                {
                    pointSystem.AddPoints(PointSystem.damageValue, "Hit by trash...");
                    countedHurt = true;
                }
            }
            else
            {
                other.GetComponent<MeshRenderer>().material = dangerTruck;
                if (!countedCloseCall)
                {
                    pointSystem.AddPoints(PointSystem.dodgeValue, "Close call!");
                    countedCloseCall = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<MeshRenderer>().material = normalTruck;
        }
    }
}
