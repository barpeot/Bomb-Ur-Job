using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // reference ke player
    public GameObject player;

    // smooth nya
    public float smoothCam = 0.3f;

    // kecepatannya dibikin 0 aja
    Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        float xPosition = player.transform.position.x;

        // biar smooth pakai nya smoothdamp
        Vector3 newPos = new Vector3(xPosition, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothCam);
    }
}
