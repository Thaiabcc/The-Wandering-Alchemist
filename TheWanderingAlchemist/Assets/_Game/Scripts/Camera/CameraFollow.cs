using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine; // Nhớ dòng này

public class CameraFindPlayer : MonoBehaviour
{
    private void Start()
    {
        // Tìm object Player (Dựa trên Tag hoặc Script Stats)
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Hoặc tìm theo script: FindObjectOfType<PlayerStats>().gameObject;

        if (player != null)
        {
            var vcam = GetComponent<CinemachineVirtualCamera>();
            if (vcam != null)
            {
                vcam.Follow = player.transform;
                // vcam.LookAt = player.transform; // Nếu game 3D top-down thì bỏ dòng này
            }
        }
    }
}
