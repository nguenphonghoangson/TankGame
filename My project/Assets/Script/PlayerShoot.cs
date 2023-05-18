using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{

    public static Action shoot;
    public static Action stopShoot;
    private void Awake()
    {
    }


    private void Update()
    {
        if (Input.GetMouseButton(0))
            shoot?.Invoke();
        if (Input.GetMouseButtonUp(0))
        {
            stopShoot?.Invoke();
        }
    }
}