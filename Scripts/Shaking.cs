using System.Collections;
using UnityEngine;
using Cinemachine;

public class Shaking : MonoBehaviour
{
    public static Shaking Instance { get; private set; }

    private CinemachineVirtualCamera cineVirtualCam;

    private void Awake()
    {
        Instance = this;

        cineVirtualCam = GetComponent<CinemachineVirtualCamera>();
    }


    private float shakeTimer = 1.0f;
    private float startingIntensity;
    private float shakeTimerTotal;

    public void ShakeCamera(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin cinePerlinNoise =
            cineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinePerlinNoise.m_AmplitudeGain = intensity;

        shakeTimerTotal = duration;
        shakeTimer = duration;

        startingIntensity = intensity;
    }


    private void Update()
    {
        if (shakeTimer > 0.0f)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0.0f)
            {
                CinemachineBasicMultiChannelPerlin cinePerlinNoise =
                    cineVirtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinePerlinNoise.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0.0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }



    /* Old version, used for simple camera (not cinemachine)
    public bool start = false;
    public AnimationCurve curve;

    // Update is called once per frame
    private void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shake());
        }
    }



    private float elapsedTime = 0f;
    private float duration;

    

    // Shake camera effect.
    IEnumerator Shake()
    {
        elapsedTime = 0f;

        duration = Random.Range(0.25f, 0.45f);


        if (cineVirtualCam == null)
        {
            cineVirtualCam = GetComponent<CinemachineVirtualCamera>();
        }

        if (cineVirtualCam != null)
        {
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                float strength = curve.Evaluate(elapsedTime / duration);

                cineVirtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().
                        m_TrackedObjectOffset += (Random.value > 0.5f ? -1.0f : 1.0f) * (Random.Range(0.9f, 1.9f) * strength * Random.insideUnitSphere);

                cineVirtualCam.m_Lens.Dutch += Random.Range(-0.75f, 0.75f);

                yield return null;
            }

            cineVirtualCam.m_Lens.Dutch = 0;

            cineVirtualCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = Vector3.zero;
        }
    }

    */


}