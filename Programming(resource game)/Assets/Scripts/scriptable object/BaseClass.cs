using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToUseItem { harvestItems = 0, resource = 1, tool = 2,  };
public class BaseClass : ScriptableObject
{
    public string name;
    public ToUseItem toUseItem;
    public int cost;

}
