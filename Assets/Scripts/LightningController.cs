using System.Collections;
using UnityEngine;

public class LightningController : MonoBehaviour
{
    public Light lightningLight;     // Đèn ánh sáng sét
    public float minDelay = 3f;      // Thời gian chờ tối thiểu giữa các tia sét
    public float maxDelay = 5f;     // Thời gian chờ tối đa
    public float flashIntensity = 5f; // Độ sáng khi lóe
    public float flashDuration = 0.1f; // Thời gian lóe sáng

    public AudioClip thunderSound; // Âm thanh sấm chớp
    public AudioSource audioSource;


    void Start()
    {
        StartCoroutine(FlashLightning());
    }
    
        IEnumerator FlashLightning()
    {
        while (true)
        {
            var waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);
            StartCoroutine(Makelightning());
        }
    }
    IEnumerator Makelightning()
    {
        var originalIntensity = lightningLight.intensity;
        lightningLight.intensity = Random.Range(2f, 8f);

        audioSource.clip = thunderSound;
        audioSource.Play();
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        lightningLight.intensity = originalIntensity;
    }
    
}
