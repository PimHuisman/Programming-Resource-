using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public bool empty;
    public SimpleResource item;
    public int currentAmount;
    void Start()
    {
        currentAmount = item.amount;
    }

    void Update()
    {
        if (currentAmount <= 0)
        {
            empty = true;
        }
    }
}
