using System;
using UnityEngine;

public class Sun : MonoBehaviour
{

    public Light sun;
    public float dayLengthSeconds = 360f / 120f;


    void Awake()
    {

        if (sun == null)
            sun = GetComponent<Light>();
    }
    void Update()
    {
        transform.Rotate(Vector3.right, dayLengthSeconds * Time.deltaTime);

        var angle = Vector3.Dot(sun.transform.forward, Vector3.down);
        sun.intensity = Mathf.Clamp01(angle);

        sun.color = Color.Lerp(Color.red, Color.white, Mathf.Clamp01((angle + 0.2f) * 5f));
    }
}
