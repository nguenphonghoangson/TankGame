using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunFollowMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y; // Đảm bảo vật chỉ xoay theo trục x và z

            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position, Vector3.up);
            transform.rotation = targetRotation;
        }

        Debug.DrawRay(transform.position, hit.point);

    }

}
