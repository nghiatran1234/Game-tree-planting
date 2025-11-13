using UnityEngine;

public class GlobalState : MonoBehaviour
{
    public static GlobalState instance { get; set; }

    public float resourceHealth ;
    public  float resourceMaxHealth;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


}

