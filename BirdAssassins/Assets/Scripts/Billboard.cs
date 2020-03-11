using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{

    Quaternion original;

    private void Start()
    {
        original = transform.rotation;
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        transform.rotation = Camera.main.transform.rotation * original;
    }
}

