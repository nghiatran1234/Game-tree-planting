using UnityEngine;

[System.Serializable]
public class Blueprint
{
    public string itemName;
    public string Req1;
    public string Req2;
    public int Req1Amount;
    public int Req2Amount;
    public int nunOfRequirements;

    public int numberOfProducedItems;

    public Blueprint(string name,int producedItem, int reqNUM, string R1, int R1num, string R2, int R2num)
    { 
        itemName = name;
        nunOfRequirements = reqNUM;
        numberOfProducedItems = producedItem;

        Req1 = R1;
        Req2 = R2;
        
        Req1Amount = R1num;
        Req2Amount = R2num;

    }
}
