using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public List<GameObject> worker = new List<GameObject>();
    public SimpleStorage storage;
    public List<SimpleResouceList> storageList = new List<SimpleResouceList>();
    public float upgradeMultiplier;

    void Start()
    {
        //add the list of resources
        for (int i = 0; i < storage.resource.Count; i++)
        {
            storageList.Add(new SimpleResouceList(storage.resource[i].name, storage.resource[i].maxAmount, storage.resource[i].currentAmount, false));
        }
    }
    // when you worker takes the resources he activates the function
    public void Add(string newName, int newAmount)
    {
        int newInt = newAmount;
        int max = 0;
        bool full = false;
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
                        storageList[i].currentAmount = storageList[i].maxAmount;
                    }
                    storageList.RemoveAt(i);
                }
            }
        }
        storageList.Add(new SimpleResouceList(newName, max, newInt, full));
    }
    // upgrade calculator
    public void Upgrade()
    {
        for (int i = 0; i < storageList.Count; i++)
        {
            storageList[i].maxAmount = Mathf.RoundToInt(storageList[i].maxAmount * upgradeMultiplier);
            storageList[i].currentAmount = 0;
            storageList[i].full = false;
        }
    }
}
