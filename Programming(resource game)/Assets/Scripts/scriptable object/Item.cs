using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingItem", menuName = "BaseStat/Items/Item", order = 1)]
public class Item : BaseClass
{
    public ResourceList[] resource;
    public ItemList[] item;
}

