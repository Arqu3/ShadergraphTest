using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParticleSystemSelector : MonoBehaviour
{
    Vector3 targetPosition;
    int selectedIndex = 0;
    ParticleSystemPlayer[] players;

    private void Awake()
    {
        players = FindObjectsOfType<ParticleSystemPlayer>().OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToArray();
        targetPosition = players[selectedIndex].transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            players[selectedIndex].enabled = false;
            int delta = Input.GetKeyDown(KeyCode.A) ? -1 : 1;

            selectedIndex += delta;
            if (selectedIndex < 0) selectedIndex = players.Length - 1;
            else if (selectedIndex >= players.Length) selectedIndex = 0;

            players[selectedIndex].enabled = true;
            targetPosition = players[selectedIndex].transform.position;
        }

        Vector3 target = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, target, 10f * Mathf.Max(2f, Vector3.Distance(transform.position, target)) * Time.deltaTime);

        transform.position += transform.forward * Input.mouseScrollDelta.y * 20f * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space)) players[selectedIndex].Play();
        if (Input.GetKeyDown(KeyCode.S)) players[selectedIndex].Stop();
    }
}
