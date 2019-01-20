using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Linq;
public class NPC : MonoBehaviour
{
    #region  all variables
    public enum MindSet { atEase, cautious, anxious }
    public enum VitalWater { moist, thirsty, dehydration }
    public enum VitalFood { stuffed, hungry, starving }
    public enum Goal { idle, exploring, searching, wandering }
    Animator anim;
    NavMeshAgent agent;
    [SerializeField] MindSet mind;
    [SerializeField] VitalWater vitalWater;
    [SerializeField] VitalFood vitalFood;
    public Goal goal;
    [SerializeField] bool isTrigger;
    [SerializeField] float viewRadius;
    [SerializeField] LayerMask maskSensfield;
    GameObject sensefield;
    Vector3 destination;
    Vector3 target;
    public Vector3 newPosition;
    public bool dead;
    [Header("Vitals")]
    [Range(.5f, .9f)]
    [SerializeField] float hunger;
    [Range(.05f, .5f)]
    [SerializeField] float starve;
    [Range(.5f, .9f)]
    [SerializeField] float thirst;
    [Range(.05f, .5f)]
    [SerializeField] float dehydration;
    Slider hungerAmount;
    int hungermax;
    Slider thirstAmount;
    int thirstmax;
    [Header("Timer")]
    [SerializeField] float mintimer;
    [SerializeField] float maxtimer;
    public float thinkTimer;
    [Header("RandomCircle")]
    [SerializeField] LayerMask mask;
    [SerializeField] float minUnitCircleRadius;
    [SerializeField] float maxUnitCircleRadius;
    float randomUnitCircleRadius;
    float newyPos;
    RaycastHit hit;
    [Header("Searching")]
    [SerializeField] float radius;
    [SerializeField] List<Transform> allHarvestItems = new List<Transform>();
    [SerializeField] List<float> dist = new List<float>();
    [Header("Explore")]
    [SerializeField] int destinationIndex;
    #endregion

    void Sensefield()// add senseField
    {
        sensefield = new GameObject();
        Transform field = Instantiate(sensefield, transform.position, transform.rotation).transform;
        field.SetParent(transform);
        field.gameObject.AddComponent(typeof(SphereCollider));
        field.gameObject.AddComponent(typeof(SensField));
        int maskNumb = Mathf.RoundToInt(Mathf.Log(maskSensfield, 2));
        field.gameObject.layer = maskNumb;
        field.GetComponent<SphereCollider>().isTrigger = isTrigger;
        field.GetComponent<SphereCollider>().radius = viewRadius;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        destination = agent.destination;
        mind = MindSet.atEase;
        Sensefield();
        Mind();
        RandomPos();
        thinkTimer = Random.Range(mintimer, maxtimer);
        goal = Goal.wandering;
        // PlayerVitals
        hungerAmount = GetComponent<PlayerVitals>().hungerSlider;
        hungermax = GetComponent<PlayerVitals>().maxHunger;
        thirstAmount = GetComponent<PlayerVitals>().thirstSlider;
        thirstmax = GetComponent<PlayerVitals>().maxThirst;
    }
    void Update()
    {
        CheckVitals();
        VitalStatCheck();
        GoalCheck();
        ThinkTimer();
        CheckDist(newPosition);
    }
    void ThinkTimer()
    {
        if (!dead)
        {
            thinkTimer -= Time.deltaTime;
            if (thinkTimer <= 0)
            {
                thinkTimer = Random.Range(mintimer, maxtimer);
                RandomPos();
            }
        }
    }
    #region RandomPos
    void RandomPos()
    {
        randomUnitCircleRadius = Random.Range(minUnitCircleRadius, maxUnitCircleRadius);
        // Pick a random point in the insideUnitCircle for X and Y and set it in a vector3
        Vector3 newPos = transform.position + new Vector3(Random.insideUnitCircle.x * randomUnitCircleRadius, transform.position.y, Random.insideUnitCircle.y * randomUnitCircleRadius);
        // check where the y pos is so you can set it to the hight of the terrain
        #region check Y position
        if (Physics.Raycast(new Vector3(newPos.x, 9999f, newPos.z), Vector3.down, out hit, Mathf.Infinity, mask))
        {
            if (hit.transform.tag == "Water")
            {
                //RandomPos();
            }
            if (hit.transform.tag == "Ground")
            {
                newyPos = hit.point.y + 0.1f;
            }
        }
        #endregion
        // Put the newPos in the setDestination   
        newPosition = new Vector3(newPos.x, newyPos, newPos.z);
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }
    #endregion
    void Mind()
    {
        switch (mind)
        {
            case MindSet.atEase:
                Debug.Log("i am at ease");
                break;
            case MindSet.cautious:
                Debug.Log("i am very cautious");
                break;
            case MindSet.anxious:
                Debug.Log("i am very anxious");
                break;
        }
    }

    void VitalStatCheck()
    {
        if (vitalWater != VitalWater.moist || vitalFood != VitalFood.stuffed)
        {
            switch (vitalWater)
            {
                case VitalWater.thirsty:
                    goal = Goal.searching;
                    break;
                case VitalWater.dehydration:
                    goal = Goal.searching;
                    break;
            }
            switch (vitalFood)
            {
                case VitalFood.hungry:
                    goal = Goal.searching;
                    break;
                case VitalFood.starving:
                    goal = Goal.searching;
                    break;
            }
        }
        else
        {
            goal = Goal.wandering;
        }
    }
    void GoalCheck()
    {
        switch (goal)
        {
            case Goal.exploring:
                Exploring();
                break;
            case Goal.searching:
                Searching();
                break;
            case Goal.wandering:
                Wandering();
                break;
            case Goal.idle:
                break;
        }
    }
    void CheckVitals()
    {
        #region HungerCheck
        // check your hunger stats
        if (hungerAmount.value >= hungermax * hunger)
        {
            vitalFood = VitalFood.stuffed;
        }
        if (vitalFood != VitalFood.starving)
        {
            if (hungerAmount.value <= hungermax * hunger)
            {
                vitalFood = VitalFood.hungry;
                //Debug.Log("I have to find food");
            }
        }
        if (hungerAmount.value <= hungermax * starve)
        {
            vitalFood = VitalFood.starving;
            //Debug.Log("I am starving");
        }
        #endregion
        #region ThirstCheck
        // check your thirst stats
        if (thirstAmount.value >= thirstmax * thirst)
        {
            vitalWater = VitalWater.moist;
        }
        if (vitalWater != VitalWater.dehydration)
        {
            if (thirstAmount.value <= thirstmax * thirst)
            {
                vitalWater = VitalWater.thirsty;
                //Debug.Log("I have to find water");
            }
        }
        if (thirstAmount.value <= thirstmax * dehydration)
        {
            vitalWater = VitalWater.dehydration;
            //Debug.Log("my mouth is so dry");
        }
        #endregion
    }
    void Exploring()
    {
        agent.SetDestination(allHarvestItems[destinationIndex].position);
    }
    #region Searching
    void Searching()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            Transform harvestItem = nearbyObject.gameObject.transform;
            if (colliders.Length != 0)
            {
                if (harvestItem.transform.tag == "Harvest")
                {
                    if (!allHarvestItems.Contains(harvestItem))
                    {
                        AddOption(harvestItem);
                    }
                }
            }
            else
            {
                print("I have to find water and food");
            }
        }

    }
    void AddOption(Transform item)
    {
        if (item.GetComponent<ConsumablesHolder>())
        {
            if (item.GetComponent<ConsumablesHolder>().consumables)
            {
                allHarvestItems.Add(item);
            }
        }
        if (item.GetComponent<AnimalsHolder>())
        {
            if (item.GetComponent<AnimalsHolder>().animals)
            {
                allHarvestItems.Add(item);
            }
        }
        dist.Clear();
        CheckDistanceGoal();
    }
    void CheckDistanceGoal()
    {
        //Gets the shortest distace
        for (int i = 0; i < allHarvestItems.Count; i++)
        {
            dist.Add(Vector3.Distance(transform.position, allHarvestItems[i].transform.position));
        }
        var index = dist.IndexOf(dist.Min());
        destinationIndex = index;

        goal = Goal.exploring;
        newPosition = allHarvestItems[destinationIndex].position;
    }
    void Wandering()
    {
        agent.SetDestination(newPosition);
    }
    #endregion

    void CheckDist(Vector3 pos)
    {
        if (Vector3.Distance(transform.position, pos) < 0.2f)
        {
            anim.SetBool("Wandering", false);
            anim.SetFloat("IdleSmooth", Mathf.Lerp(anim.GetFloat("IdleSmooth"), 0, Time.deltaTime * 2));
            //Debug.Log("i am on my position");
        }
        else
        {
            //Debug.Log("Distance " + ":" + Vector3.Distance(transform.position, newPosition));
            anim.SetBool("Wandering", true);
            anim.SetFloat("IdleSmooth", Mathf.Lerp(anim.GetFloat("IdleSmooth"), 0.5f, Time.deltaTime * 2));
        }
    }
}
