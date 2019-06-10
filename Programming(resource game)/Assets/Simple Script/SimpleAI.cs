using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;

public class SimpleAI : MonoBehaviour
{
    Vector3 distance;
    public Transform home;
    public Vector3 newPosition;
    [Header("Distance")]
    [SerializeField] float radius;
    public List<Transform> target = new List<Transform>();
    [SerializeField] List<float> dist = new List<float>();
    public bool searching = true;
    [SerializeField] int destinationIndex;

    public int maxAmountInventory;
    public int currentAmountInventory;
    public string subtractname;
    public List<SimpleResouceList> inventory = new List<SimpleResouceList>();
    NavMeshAgent agent;
    void Start()
    {
        newPosition = home.position;
        agent = gameObject.GetComponent<NavMeshAgent>();
        Searching();
        CalculateDistance();
    }
    void Searching()
    {
        if (searching)
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
        agent.SetDestination(newPosition);
    }

    void CalculateDistance()
    {
        //Gets the shortest distace
        for (int i = 0; i < target.Count; i++)
        {
            dist.Add(Vector3.Distance(transform.position, target[i].transform.position));
        }
        var index = dist.IndexOf(dist.Min());
        destinationIndex = index;

        newPosition = target[destinationIndex].position;
    }


    void Update()
    {
        agent.SetDestination(newPosition);
        Check();
    }
    void Check()
    {
        if (currentAmountInventory == maxAmountInventory)
        {
            newPosition = home.position;
            agent.speed = 2;
        }
        else
        {
            agent.speed = 3.5f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if it is harvestable
        if (other.transform.tag == "Harvest")
        {
            print("");
            // check what kind of resource it is
            inventory.Add(new SimpleResouceList(other.GetComponent<Container>().item.name, 0, maxAmountInventory, false));
            currentAmountInventory = currentAmountInventory + maxAmountInventory;
            //currentAmountInventory = maxAmountInventory + currentAmountInventory;
            // add it to your inventory
            // subtract the max inventory amount
            other.GetComponent<Container>().currentAmount -= currentAmountInventory;
        }
        // Check if you are home
        if (other.transform.tag == "Home")
        {
            // add it to the right repository
            other.GetComponent<Storage>().Add(inventory[0].name, inventory[0].currentAmount);
            // subtract it from your inventory
            currentAmountInventory = 0;
            inventory.Clear();
            target.Clear();
            dist.Clear();
            CheckStorage(other.transform);
            Searching();
            CalculateDistance();

        }
    }

    void CheckStorage(Transform resource)
    {
        Storage fullStorage = resource.GetComponent<Storage>();
        for (int i = 0; i < fullStorage.storageList.Count; i++)
        {
            if (fullStorage.storageList[i].full)
            {
                subtractname = fullStorage.storageList[i].name;
            }
        }
    }
}
