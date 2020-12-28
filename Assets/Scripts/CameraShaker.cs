using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
    public CinemachineVirtualCamera _vcam;
    public CinemachineBasicMultiChannelPerlin _vcamNoise;

    public float _shakeDuration = 0.5f;
    public float _shakeAmplitude = 2f;
    public float _shakeFrequency = 1.2f;

    private float _shakeElapsedTime = 0f;

    void Start()
    {
        if (_vcam != null)
        {
            _vcamNoise = _vcam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_shakeElapsedTime != 0)
        {
            if (_shakeElapsedTime > 0)
            {
                _vcamNoise.m_AmplitudeGain = _shakeAmplitude;
                _vcamNoise.m_FrequencyGain = _shakeFrequency;

                _shakeElapsedTime -= Time.deltaTime;
            }
            else
            {
                _vcamNoise.m_AmplitudeGain = 0;
                _shakeDuration = 0;
            }
        }
    }

    public void ShakeCamera()
    {
        _shakeElapsedTime = _shakeDuration;
    }
}
