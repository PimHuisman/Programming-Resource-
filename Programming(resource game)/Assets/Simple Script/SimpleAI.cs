using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;

public class SimpleAI : MonoBehaviour
{
    public enum Goal { collect, upgrade }
    public Goal goal;
    public int amountFull;
    public Transform home;
    public Vector3 newPosition;
    [Header("Distance")]
    [SerializeField] float radius;
    public List<Transform> target = new List<Transform>();
    [SerializeField] List<float> dist = new List<float>();
    [SerializeField] int destinationIndex;
    [Header("Inventory")]
    public int multiplier;
    public int maxAmountInventory;
    public int currentAmountInventory;
    public string subtractname;
    public List<SimpleResouceList> inventory = new List<SimpleResouceList>();
    [Header("Move")]
    public float walksSpeed;
    public float runSpeed;
    NavMeshAgent agent;
    void Start()
    {
        goal = Goal.collect;
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = walksSpeed;
        Searching();
        CalculateDistance();
    }
    void Searching()
    {
        //search for all the harvestable objects
        if (goal == Goal.collect)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider nearbyObject in colliders)
            {
                Transform harvestItem = nearbyObject.gameObject.transform;
                if (harvestItem.transform.tag == "Harvest")
                {
                    if (harvestItem.GetComponent<Container>().item.name != subtractname)
                    {
                        if (!harvestItem.GetComponent<Container>().empty)
                        {
                            target.Add(harvestItem);
                        }
                    }
                }
            }
        }
    }

    void CalculateDistance()
    {
        //Gets the one with shortest distace
        if (target.Count > 0)
        {
            for (int i = 0; i < target.Count; i++)
            {
                dist.Add(Vector3.Distance(transform.position, target[i].transform.position));
            }
        }
        else
        {
            agent.isStopped = true;
        }
        var index = dist.IndexOf(dist.Min());
        destinationIndex = index;

        newPosition = target[destinationIndex].position;
    }


    void Update()
    {
        Check();
    }
    void Check()
    {
        if (currentAmountInventory == maxAmountInventory)
        {
            newPosition = home.position;
            agent.speed = walksSpeed;
        }
        else
        {
            agent.speed = runSpeed;
        }

        if (goal == Goal.upgrade)
        {
            agent.SetDestination(home.position);
        }
        else
        {
            agent.SetDestination(newPosition);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it is harvestable
        if (other.transform.tag == "Harvest")
        {
            // check what kind of resource it is
            inventory.Add(new SimpleResouceList(other.GetComponent<Container>().item.name, 0, maxAmountInventory, false));
            currentAmountInventory = currentAmountInventory + maxAmountInventory;
            // add it to your inventory
            other.GetComponent<Container>().currentAmount -= currentAmountInventory;
        }
        // Check if you are home
        if (other.transform.tag == "Home")
        {
            // add it to the right repository
            other.GetComponent<Storage>().Add(inventory[0].name, inventory[0].currentAmount);
            // and clear all lists
            currentAmountInventory = 0;
            inventory.Clear();
            target.Clear();
            dist.Clear();
            CheckStorage(other.transform);
            if (goal == Goal.upgrade)
            {
                other.GetComponent<Storage>().Upgrade();
                maxAmountInventory = maxAmountInventory * multiplier;
                goal = Goal.collect;
            }
            amountFull = 0;
            agent.isStopped = true;
            Invoke("Delay", 2);
        }
    }
    void Delay()
    {
        Searching();
        CalculateDistance();
        agent.isStopped = false;
    }
    void CheckStorage(Transform resource)
    {
        Storage fullStorage = resource.GetComponent<Storage>();
        for (int i = 0; i < fullStorage.storageList.Count; i++)
        {
            if (fullStorage.storageList[i].full)
            {
                subtractname = (fullStorage.storageList[i].name);
                amountFull++;
            }
            if (amountFull == fullStorage.storageList.Count)
            {
                goal = Goal.upgrade;
            }
        }
    }
}
