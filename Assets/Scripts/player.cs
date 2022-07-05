using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    private Vector3 _direction = Vector3.forward;
    public int speed = 1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _direction = Vector3.forward;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _direction = Vector3.back;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            _direction = Vector3.right;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _direction = Vector3.left;
        }

    }

    private void FixedUpdate()
    {
        transform.Translate(_direction * speed * Time.deltaTime);
        //float x = Mathf.Round(transform.position.x) + _direction.x;
        //float z = Mathf.Round(transform.position.z) + _direction.z;
        //transform.position = new Vector3(x, 0.0f, z);
    }
}
