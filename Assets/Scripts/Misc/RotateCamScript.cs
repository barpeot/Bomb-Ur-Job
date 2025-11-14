using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamScript : MonoBehaviour
{
    public float startYRotation;
    public float endYRotation;

    Quaternion startRotation;
    Quaternion endRotation;
    public float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        startRotation = Quaternion.Euler(transform.rotation.x, startYRotation, transform.rotation.z);
        endRotation = Quaternion.Euler(transform.rotation.x, endYRotation, transform.rotation.z);
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1.0f);
        transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);
    }
}
