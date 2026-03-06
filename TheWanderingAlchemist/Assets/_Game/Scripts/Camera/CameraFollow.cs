using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFindPlayer : MonoBehaviour
{
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            var vcam = GetComponent<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = player.transform;
            }
        }
    }
}
