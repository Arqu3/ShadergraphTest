using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class GenericUnityEvent<T0, T1, T2> : UnityEvent<T0, T1, T2> { };

public abstract class CameraShakeLayer : MonoBehaviour
{
    [Header("Curves")]
    [SerializeField]
    protected bool useX = true;
    [SerializeField]
    protected AnimationCurve xCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1.0f, 0f));

    [SerializeField]
    protected bool useY = true;
    [SerializeField]
    protected AnimationCurve yCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1.0f, 0f));

    [SerializeField]
    protected bool useZ = true;
    [SerializeField]
    protected AnimationCurve zCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1.0f, 0f));

    public enum ResetMode
    {
        None = 0,
        Lerp = 1
    }

    [Header("Reset state")]
    [SerializeField]
    ResetMode resetMode = ResetMode.None;

    public float Duration
    {
        get
        {
            return Mathf.Max(useX ? xCurve.keys[xCurve.length - 1].time : 0f, useY ? yCurve.keys[yCurve.length - 1].time : 0, useZ ? zCurve.keys[zCurve.length - 1].time : 0);
        }
    }

    protected Vector3 localStartpos;
    protected Quaternion localStartRot;

    protected readonly UnityEvent onStart = new UnityEvent();
    protected readonly GenericUnityEvent<float, float, float> onLoop = new GenericUnityEvent<float, float, float>();
    protected readonly UnityEvent onReset = new UnityEvent();
    protected readonly UnityEvent onStopLoop = new UnityEvent();
    protected readonly UnityEvent onStartLoop = new UnityEvent();

    private bool playing = false;
    private bool playingLoop = false;

    protected virtual void Awake()
    {
        localStartpos = transform.localPosition;
        localStartRot = transform.localRotation;
    }

    public virtual Coroutine Play(float duration, bool resetWhenDone = true)
    {
        return StartCoroutine(_Play(duration, resetWhenDone));
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) Play();
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (playingLoop) StopLoop();
            else PlayLoop();
        }
    }

#endif

    public virtual Coroutine Play(bool resetWhenDone = true)
    {
        return Play(Duration, resetWhenDone);
    }

    protected IEnumerator _Play(float duration, bool resetWhenDone = true)
    {
        if (playing) yield break;

        playing = true;

        float timer = 0.0f;
        onStart.Invoke();

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float percentage = timer / duration;
            onLoop.Invoke(
                useX ? xCurve.Evaluate(xCurve.keys[xCurve.length - 1].time * percentage) : 0f,
                useY ? yCurve.Evaluate(yCurve.keys[yCurve.length - 1].time * percentage) : 0f,
                useZ ? zCurve.Evaluate(zCurve.keys[zCurve.length - 1].time * percentage) : 0f);

            yield return null;
        }

        if (resetWhenDone) ResetPositionAndRotation();
        playing = false;
    }

    public Coroutine PlayLoop()
    {
        return StartCoroutine(_PlayLoop());
    }

    IEnumerator _PlayLoop()
    {
        if (playingLoop) yield break;

        onStartLoop.Invoke();
        playingLoop = true;

        while (true)
        {
            float duration = Duration;
            Play(duration, false);
            yield return new WaitForSeconds(duration);
        }
    }

    public void StopLoop()
    {
        StopAllCoroutines();
        ResetPositionAndRotation();
        playing = false;
        playingLoop = false;
        onStopLoop.Invoke();
    }

    protected virtual void ResetPositionAndRotation()
    {
        switch (resetMode)
        {
            case ResetMode.None:
                transform.localPosition = localStartpos;
                transform.localRotation = localStartRot;
                break;
            case ResetMode.Lerp:
                StartCoroutine(_ResetSequence());
                break;
            default:
                break;
        }

        onReset.Invoke();
    }

    IEnumerator _ResetSequence()
    {
        float resetTime = 0.1f;
        float resetTimer = 0.0f;

        Vector3 resetStartpos = transform.localPosition;
        Quaternion resetStartrot = transform.localRotation;

        while (resetTimer < resetTime)
        {
            resetTimer += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(resetStartpos, localStartpos, resetTimer / resetTime);
            transform.localRotation = Quaternion.Lerp(resetStartrot, localStartRot, resetTimer / resetTime);

            yield return null;
        }

        transform.localPosition = localStartpos;
        transform.localRotation = localStartRot;
    }

    public bool IsPlaying
    {
        get
        {
            return playing;
        }
    }
}
