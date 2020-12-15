using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatformBehaviour : MonoBehaviour
{
    [SerializeField]
    float zAxisRate = 5.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 0f, zAxisRate) * Time.deltaTime);
    }
}
