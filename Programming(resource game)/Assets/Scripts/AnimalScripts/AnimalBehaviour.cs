using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AnimalBehaviour : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] float mintimer;
    [SerializeField] float maxtimer;
    public float thinkTimer;
    public bool dead;
    [Header("RandomCircle")]
    [SerializeField] float minUnitCircleRadius;
    [SerializeField] float maxUnitCircleRadius;
    [SerializeField] LayerMask mask;
    public Vector3 newPosition;
    float randomUnitCircleRadius;
    NavMeshAgent agent;
    float newyPos;
    RaycastHit hit;
    void Start()
    {
        thinkTimer = Random.Range(mintimer, maxtimer);
        agent = GetComponent<NavMeshAgent>();
        RandomPos();
    }
    void Update()
    {
        Walking();
        ThinkTimer();
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }

    void Walking()
    {
        agent.SetDestination(newPosition);
    }
}
