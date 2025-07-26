using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CineMachineScript : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -10);

    private void Start()
    {
        target = GetComponent<CinemachineVirtualCamera>().Follow;
    }

    private void LateUpdate()
    {
        transform.position = target.position + offset;
    }
}
