using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "SimpleStorage/storage", order = 1)]
public class SimpleStorage : SimpleResource
{
    public List<SimpleResouceList> resource = new List<SimpleResouceList>();
}

