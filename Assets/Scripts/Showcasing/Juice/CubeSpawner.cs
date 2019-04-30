using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CubeSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    ParticleSystem attachedSystem;
    [SerializeField]
    ParticleSystem smokeSystem;
    [SerializeField]
    ParticleSystem explosionSystem;

    [Header("Spawn variables")]
    [SerializeField]
    float initialDelay = 0.5f;
    [SerializeField]
    float spawnTime = 0.5f;
    [SerializeField]
    AnimationCurve movementCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
    [SerializeField]
    AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

    [Header("Hologram preview")]
    [SerializeField]
    Material ghostMaterial;

    [Header("Camera")]
    [SerializeField]
    List<CameraShakeLayer> shakeLayers = new List<CameraShakeLayer>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) CreateIterative(3);
    }

    void CreateIterative(int iterations)
    {
        for(int i = 0; i < iterations; ++i)
        {
            StartCoroutine(_CreateDelayed(i, prefab.transform.position + Vector3.left * 3f + Vector3.right * i * 3f));
        }
    }

    IEnumerator _CreateDelayed(float delay, Vector3 destination, bool skipSequence = false)
    {
        yield return new WaitForSeconds(delay);

        Create(destination, skipSequence);
    }

    void Create(bool skipSequence = false)
    {
        Create(prefab.transform.position, skipSequence);
    }

    void Create(Vector3 destination, bool skipSequence = false)
    {
        var instance = Instantiate(prefab);
        instance.transform.position = destination;
        if (skipSequence) return;
        StartCoroutine(_SpawnSequence(instance, destination, spawnTime, initialDelay));
    }

    IEnumerator _SpawnSequence(GameObject gObj, Vector3 destination, float spawnTime, float initialDelay)
    {
        Vector3 start = gObj.transform.position - Vector3.up * 3f;

        gObj.transform.position = start;

        var preview = Instantiate(gObj);
        preview.transform.position = destination;
        preview.transform.localScale *= 1.01f;
        var material = new Material(ghostMaterial);
        preview.GetComponentInChildren<Renderer>().material = material;

        string colorName = "Color_11EBE5FF";
        string clipName = "Vector1_5AB8A4D";

        Color toColor = material.GetColor(colorName);
        Color fromColor = new Color(0, 0, 0, 0);

        float toClip = material.GetFloat(clipName);
        float fromClip = 0.5f;

        float initialTimer = 0.0f;
        float offsetMulti = 1.5f;

        while (initialTimer < initialDelay)
        {
            initialTimer += Time.deltaTime;

            material.SetColor(colorName, Color.Lerp(fromColor, toColor, initialTimer * offsetMulti / initialDelay));
            material.SetFloat(clipName, Mathf.Lerp(fromClip, toClip, initialTimer * offsetMulti / initialDelay));

            yield return null;
        }

        float timer = 0.0f;
        float movementCurveLength = movementCurve[movementCurve.keys.Length - 1].time;

        var attach = Instantiate(attachedSystem);
        attach.transform.SetParent(gObj.transform.GetChild(0));
        attach.transform.localPosition = Vector3.zero;
        attach.Stop();
        var main = attach.main;
        main.duration = spawnTime * 1.2f;
        attach.Play();

        StartCoroutine(_SpawnSystemDelayed(smokeSystem, destination, smokeSystem.transform.rotation, spawnTime * 0.8f));
        StartCoroutine(_SpawnSystemDelayed(explosionSystem, destination + Vector3.up, explosionSystem.transform.rotation, spawnTime * 0.8f));
        StartCoroutine(_InvokeFunctionDelayed(() => shakeLayers.ForEach(x => x.Play()), spawnTime * 0.8f));

        while (timer < spawnTime)
        {
            timer += Time.deltaTime;

            float percentage = movementCurveLength * (timer / spawnTime);
            gObj.transform.position = Vector3.LerpUnclamped(start, destination, movementCurve.Evaluate(percentage));

            yield return null;
        }

        float exitTime = 0.5f;
        float exitTimer = 0.0f;

        Vector3 fromScale = gObj.transform.localScale;
        Vector3 toScale = gObj.transform.localScale * 0.3f;

        while (exitTimer < exitTime)
        {
            exitTimer += Time.deltaTime;

            gObj.transform.localScale = Vector3.LerpUnclamped(fromScale, toScale, scaleCurve.Evaluate(exitTimer / exitTime));

            material.SetColor(colorName, Color.Lerp(toColor, fromColor, exitTimer / exitTime));
            material.SetFloat(clipName, Mathf.Lerp(toClip, fromClip, exitTimer / exitTime));

            yield return null;
        }
        if (preview) Destroy(preview);
        if (material) Destroy(material);
    }

    IEnumerator _SpawnSystemDelayed(ParticleSystem system, Vector3 position, Quaternion rotation, float delay)
    {
        yield return new WaitForSeconds(delay);

        Instantiate(system, position, rotation);
    }

    IEnumerator _InvokeFunctionDelayed(System.Action func, float delay)
    {
        yield return new WaitForSeconds(delay);

        func();
    }
}
