using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public SimpleStorage storage;
    public int maxStorage;
    public List<GameObject> worker = new List<GameObject>();
    public List<SimpleResouceList> storageList = new List<SimpleResouceList>();
    public float upgradeMultiplier;

    void Start()
    {
        print(storage.resource.Count);
        for (int i = 0; i < storage.resource.Count; i++)
        {
            storageList.Add(new SimpleResouceList(storage.resource[i].name, storage.resource[i].maxAmount, storage.resource[i].currentAmount, false));
        }
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Upgrade();
        }
    }
    public void Add(string newName, int newAmount)
    {
        int newInt = newAmount;
        int max = 0;
        bool full = false;
        print(storageList.Count);
        for (int i = 0; i < storageList.Count; i++)
        {
            if (storageList.Count > 0)
            {
                if (storageList[i].name == newName)
                {
                    max = storageList[i].maxAmount;

                    newInt = newInt + storageList[i].currentAmount;
                    if (newInt >= storageList[i].maxAmount)
                    {
                        full = true;
                    }
                    storageList.RemoveAt(i);
                }
            }
        }
        storageList.Add(new SimpleResouceList(newName, max, newInt,full));
    }
    // upgrade calculator
    void Upgrade()
    {
        for (int i = 0; i < storageList.Count; i++)
        {
            storageList[i].maxAmount = Mathf.RoundToInt(storageList[i].maxAmount * upgradeMultiplier);
        }
    }
}
