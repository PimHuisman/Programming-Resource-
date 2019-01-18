using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    #region  all variables
    public enum MindSet { atEase, cautious, anxious }
    public enum VitalWater { thirsty, dehydration }
    public enum VitalFood { hungry, starving }
    public enum Goal { idle, exploring, searching, wandering }
    Animator anim;
    NavMeshAgent agent;
    [SerializeField] MindSet mind;
    VitalWater vitalWater;
    VitalFood vitalFood;
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
        GoalCheck();
        ThinkTimer();
        CheckDist();
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
        switch (vitalWater)
        {
            case VitalWater.thirsty:

                break;

            case VitalWater.dehydration:

                break;
        }
        switch (vitalFood)
        {
            case VitalFood.hungry:

                break;

            case VitalFood.starving:

                break;
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
        if (vitalFood != VitalFood.starving)
        {
            if (hungerAmount.value <= hungermax * hunger)
            {
                Debug.Log("I have to find food");
                vitalFood = VitalFood.hungry;
            }
        }

        if (hungerAmount.value <= hungermax * starve)
        {
            Debug.Log("I am starving");
            vitalFood = VitalFood.starving;
        }
        #endregion
        #region ThirstCheck
        // check your thirst stats
        if (vitalWater != VitalWater.dehydration)
        {
            if (thirstAmount.value <= thirstmax * thirst)
            {
                Debug.Log("I have to find water");
                vitalWater = VitalWater.thirsty;
            }
        }

        if (thirstAmount.value <= thirstmax * dehydration)
        {
            Debug.Log("my mouth is so dry");
            vitalWater = VitalWater.dehydration;
        }
        #endregion
    }
    void Exploring()
    {

    }
    void Searching()
    {

    }
    void Wandering()
    {
        agent.SetDestination(newPosition);
    }

    void CheckDist()
    {
        if (Vector3.Distance(transform.position, newPosition) < 0.2f)
        {
            anim.SetBool("Wandering", false);
            anim.SetFloat("IdleSmooth", Mathf.Lerp(anim.GetFloat("IdleSmooth"),0,Time.deltaTime * 2));
            //Debug.Log("i am on my position");
        }
        else
        {
            Debug.Log("Distance " + ":" + Vector3.Distance(transform.position, newPosition));
            anim.SetBool("Wandering", true);
            anim.SetFloat("IdleSmooth", Mathf.Lerp(anim.GetFloat("IdleSmooth"),0.5f,Time.deltaTime * 2));
        }
    }
}
