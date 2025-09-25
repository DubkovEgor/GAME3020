using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance;

    [Header("Current Resources")]
    public int gold = 1000;
    public int stone = 500;
    public int wood = 500;

    public event Action OnResourcesChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanAfford(ResourceCost cost)
    {
        return gold >= cost.gold && stone >= cost.stone && wood >= cost.wood;
    }

    public bool SpendResources(ResourceCost cost)
    {
        if (!CanAfford(cost)) return false;

        gold -= cost.gold;
        stone -= cost.stone;
        wood -= cost.wood; 
        OnResourcesChanged?.Invoke(); 
        return true;
    }

    public void AddResources(ResourceCost cost)
    {
        gold += cost.gold;
        stone += cost.stone;
        wood += cost.wood;

        OnResourcesChanged?.Invoke();
    }
    public void Add1000Resources()
    {
        ResourceCost bonus = new ResourceCost(1000, 1000, 1000);
        AddResources(bonus);

    }

}

[System.Serializable]
public struct ResourceCost
{
    public int gold;
    public int stone;
    public int wood;

    public ResourceCost(int gold, int stone, int wood)
    {
        this.gold = gold;
        this.stone = stone;
        this.wood = wood;
    }
}
