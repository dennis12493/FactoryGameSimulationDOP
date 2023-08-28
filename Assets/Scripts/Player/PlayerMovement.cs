using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;

    void Update()
    {
        if(Input.GetKey(KeyCode.Mouse2)) Rotate();
        
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(h, 0f, v).normalized;
        moveDirection = transform.rotation * moveDirection;
        characterController.Move(moveDirection * 5f * Time.deltaTime);
    }

    void Rotate()
    {
        transform.Rotate(Vector3.forward, Input.GetAxis("Mouse X") * 4f, Space.World);
    }
}
