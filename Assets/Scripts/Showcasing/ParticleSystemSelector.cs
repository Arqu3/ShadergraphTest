using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemSelector : MonoBehaviour
{
    #region Public variables

    [Header("Particle system players")]
    [SerializeField]
    ParticleSystemPlayer[] players;

    #endregion

    Vector3 targetPosition;
    int selectedIndex = 0;

    private void Awake()
    {
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

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetPosition.x, transform.position.y, transform.position.z), 20f * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Space)) players[selectedIndex].Play();
    }
}
