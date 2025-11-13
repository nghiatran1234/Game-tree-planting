using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public float currentHealth;
    public float currentCalories;
    public float currentHydration;
    public float currentWetness;

    public float playerPosX, playerPosY, playerPosZ;
    public float playerRotX, playerRotY, playerRotZ, playerRotW;
    public float cameraRotX, cameraRotY, cameraRotZ, cameraRotW;
    
    public List<string> inventoryItems;
    public List<string> quickSlotItems;
    
    public int dayCounter;
    public string currentSeason; 
    public string currentWeather;

    public List<WorldItemData> placedItems;
}


[System.Serializable]
public class WorldItemData
{
    public string itemName; 
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    
    public List<string> items; 

    public WorldItemData(string name, Vector3 pos, Quaternion rot)
    {
        itemName = name;
        
        posX = pos.x;
        posY = pos.y;
        posZ = pos.z;

        rotX = rot.x;
        rotY = rot.y;
        rotZ = rot.z;
        rotW = rot.w;
        
        items = new List<string>();
    }
}