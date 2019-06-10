using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "SimpleItem/Items", order = 1)]
public class SimpleResource : ScriptableObject
{
    public string name;
    public int amount;
}
