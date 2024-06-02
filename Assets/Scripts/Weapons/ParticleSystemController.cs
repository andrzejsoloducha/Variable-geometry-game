using System;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    private new ParticleSystem particleSystem;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (particleSystem.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
