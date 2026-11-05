// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;


// [RequireComponent(typeof(Light))]
// [RequireComponent(typeof(TrailRenderer))]

// public class ElectricScript : MonoBehaviour
// {
//     [Header("Light Flicker Settings")]
//     public float baseIntensity = 15f;
//     public float flickerRange = 6f;

//     [Header("Position Offset (Spark Jitter)")]
//     public float jitterAmount = 0.05f;

//     private Light electricLight;
//     private TrailRenderer trail;
//     private Vector3 initialLocalPos;

//     void Start()
//     {
//         electricLight = GetComponent<Light>();
//         trail = GetComponent<TrailRenderer>();
//         initialLocalPos = transform.localPosition;
//     }

//     void Update()
//     {
//         // 1. Rapid Light Intensity Flicker
//         electricLight.intensity = baseIntensity + Random.Range(-flickerRange, flickerRange);

//         // 2. Micro Position Jitter (Simulates unstable arcing off the central line)
//         Vector3 randomJitter = new Vector3(
//             Random.Range(-jitterAmount, jitterAmount),
//             Random.Range(-jitterAmount, jitterAmount),
//             Random.Range(-jitterAmount, jitterAmount)
//         );
//         transform.localPosition = initialLocalPos + randomJitter;
//     }
// }
