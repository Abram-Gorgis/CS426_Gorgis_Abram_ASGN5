﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject player = collision.gameObject;
        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        rigidbody.AddForce(Vector3.up * 2000);
    }
}
