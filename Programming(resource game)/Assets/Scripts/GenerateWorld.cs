using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateWorld : MonoBehaviour
{
    #region All Variables
    [Header("All Amount")]
    [SerializeField] int maxAmountTrees;
    [SerializeField] int maxAmountSmallRocks;
    [SerializeField] int maxAmountBigRocks;
    [SerializeField] int maxAmountBush;
    [SerializeField] int minTrees;
    [SerializeField] int minBigRocks;
    [SerializeField] int minSmallRocks;
    [SerializeField] int minBush;

    [Header("All Objecten")]
    [SerializeField] Transform tree;
    [SerializeField] Transform smallrocks;
    [SerializeField] Transform bigrocks;
    [SerializeField] Transform bush;

    [SerializeField] float offset;
    [SerializeField] LayerMask mask;
    Transform newrock;
    [Header("RockSize")]
    [SerializeField] float minsize;
    [SerializeField] float maxsize;
    [Header("All Lists")]
    public List<Transform> allTrees = new List<Transform>();
    public List<Transform> allSmallRocks = new List<Transform>();
    public List<Transform> allBigRocks = new List<Transform>();
    public List<Transform> allBush = new List<Transform>();
    [Header("Timer")]
    [SerializeField] float mintimer;
    [SerializeField] float maxtimer;
    public float thinkTimer;
    bool addTree;
    bool addbRock;
    bool addsRock;
    bool addBush;
    RaycastHit hit;
    float newsize;
    float newyPos;
    float newzPos;
    float newxPos;
    float xSize;
    float zSize;
    Vector3 newWorldpos;
    #endregion

    void Start()
    {
        xSize = GetComponent<TerrainCollider>().terrainData.size.x / 2;
        zSize = GetComponent<TerrainCollider>().terrainData.size.z / 2;
        newWorldpos = Vector3.zero;
        Generate();
    }
    void Generate()
    {
        #region SpawnTrees
        for (int i = 0; i < maxAmountTrees; i++)
        {
            newxPos = Random.Range(-xSize, xSize);
            newzPos = Random.Range(-zSize, zSize);
            if (Physics.Raycast(new Vector3(newxPos, 9999f, newzPos), Vector3.down, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.tag == "Ground")
                {
                    newyPos = hit.point.y;
                    newWorldpos = new Vector3(newxPos, newyPos, newzPos);
                    Transform newTree = Instantiate(tree, newWorldpos, Quaternion.identity);
                    allTrees.Add(newTree);
                }
            }
        }
        #endregion
        #region Bush
        for (int i = 0; i < maxAmountBush; i++)
        {
            newxPos = Random.Range(-xSize, xSize);
            newzPos = Random.Range(-zSize, zSize);
            if (Physics.Raycast(new Vector3(newxPos, 9999f, newzPos), Vector3.down, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.tag == "Ground")
                {
                    newyPos = hit.point.y;
                    newWorldpos = new Vector3(newxPos, newyPos, newzPos);
                    //Transform angleBush = new Transform(newWorldpos, Vector3.Angle(hit.normal));
                    Transform newBush = Instantiate(bush, newWorldpos, Quaternion.identity);
                    allBush.Add(newBush);
                }
            }
        }
        #region SpawnBigRocks
        for (int i = 0; i < maxAmountBush; i++)
        {
            newxPos = Random.Range(-xSize, xSize);
            newzPos = Random.Range(-zSize, zSize);
            if (Physics.Raycast(new Vector3(newxPos, 9999f, newzPos), Vector3.down, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.tag == "Ground")
                {
                    newyPos = hit.point.y;
                    newWorldpos = new Vector3(newxPos, newyPos + offset, newzPos);
                    Transform newBigRock = Instantiate(bush, newWorldpos, Quaternion.identity);
                    allBigRocks.Add(newBigRock);
                }
            }
        }
        #endregion
        #region SpawnSmallRocks
        for (int i = 0; i < maxAmountSmallRocks; i++)
        {
            newxPos = Random.Range(-xSize, xSize);
            newzPos = Random.Range(-zSize, zSize);
            var randomrRot = Random.Range(0, 360);
            newsize = Random.Range(minsize, maxsize);
            if (Physics.Raycast(new Vector3(newxPos, 9999f, newzPos), Vector3.down, out hit, Mathf.Infinity, mask))
            {
                if (hit.transform.tag == "Ground")
                {
                    newyPos = hit.point.y;
                    newWorldpos = new Vector3(newxPos, newyPos + offset, newzPos);
                    newrock = Instantiate(smallrocks, newWorldpos, Quaternion.Euler(randomrRot, randomrRot, randomrRot));
                    newrock.localScale = new Vector3(newsize, newsize, newsize);
                    allSmallRocks.Add(newrock);
                }
            }
        }
        #endregion
    }
    #endregion

    void Update()
    {
        CheckAmount();
        ThinkTimer();
    }

    void CheckAmount()
    {
        if (allTrees.Count < minTrees)
        {
            addTree = true;
        }
    }

    void ThinkTimer()
    {
        if (!addTree)
        {
            thinkTimer -= Time.deltaTime;
            if (thinkTimer <= 0)
            {
                thinkTimer = Random.Range(mintimer, maxtimer);
                //RandomPos();
            }
        }
    }

    void AddItem()
    {

    }
}
