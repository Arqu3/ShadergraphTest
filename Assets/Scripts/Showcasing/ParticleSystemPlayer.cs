using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public abstract class ParticleSystemPlayer : MonoBehaviour
{
    protected ParticleSystem pSystem;

    protected virtual void Awake()
    {
        pSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        pSystem.Play();
        OnPlay();
    }

    protected abstract void OnPlay();

    private void OnEnable()
    {
        OnSelected();
    }

    private void OnDisable()
    {
        OnDeselected();
    }

    protected virtual void OnSelected()
    {

    }

    protected virtual void OnDeselected()
    {
        pSystem.Stop();
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        enabled = false;
    }

#endif
}
