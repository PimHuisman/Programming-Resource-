using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SimpleResouceList
{
    public string name;
    public int maxAmount;
    public int currentAmount;
    public bool full;

    public SimpleResouceList(string newName, int newMaxAmount, int newCurrentAmount, bool newFull)
    {
        name = newName;
        maxAmount = newMaxAmount;
        currentAmount = newCurrentAmount;
        full = newFull;
    }
}
