using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] Material normalTruck;
    [SerializeField] Material dangerTruck;
    private float timeToPrime;
    private float primedRotSpeed;
    private float timeToFire;
    private float timer;
    private bool primed = false;

    public void SetTimeToPrime(float newVal)
    {
        timeToPrime = newVal;
    }

    public void SetTimeToFire(float newVal)
    {
        timeToFire = newVal;
    }

    public void SetPrimedRotSpeed(float newVal)
    {
        primedRotSpeed = newVal;
    }

    public void SetRotationSpeed(float newVal)
    {
        GetComponent<Rotater>().SetRotateSpeed(newVal);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (!primed && timer >= timeToPrime)
        {
            primed = true;
            SetRotationSpeed(primedRotSpeed);
            timer = 0;
        }
        else if (primed && timer >= timeToFire)
        {
            //fire routine here
            Debug.Log("yeouch");
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<MeshRenderer>().material = dangerTruck;
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
