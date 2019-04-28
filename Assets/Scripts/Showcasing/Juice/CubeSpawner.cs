using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField]
    GameObject prefab;

    [Header("Spawn variables")]
    [SerializeField]
    AnimationCurve movementCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
    [SerializeField]
    AnimationCurve scaleCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

    GameObject prev;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Create();
    }

    void Create()
    {
        if (prev) Destroy(prev);

        var instance = Instantiate(prefab);
        StartCoroutine(_SpawnSequence(instance, 0.5f, 0.5f));
        prev = instance;
    }

    IEnumerator _SpawnSequence(GameObject gObj, float spawnTime, float initialDelay)
    {
        float timer = 0.0f;

        float movementCurveLength = movementCurve[movementCurve.keys.Length - 1].time;

        Vector3 destination = gObj.transform.position;
        Vector3 start = gObj.transform.position - Vector3.up * 3f;

        gObj.transform.position = start;

        yield return new WaitForSeconds(initialDelay);

        while (timer < spawnTime)
        {
            timer += Time.deltaTime;

            float percentage = movementCurveLength * (timer / spawnTime);
            gObj.transform.position = Vector3.LerpUnclamped(start, destination, movementCurve.Evaluate(percentage));

            yield return null;
        }
    }
}
