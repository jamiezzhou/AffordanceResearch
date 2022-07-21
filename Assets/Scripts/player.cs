using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{

    private Vector3 _direction = Vector3.zero;
    public int speed = 1;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _direction = Vector3.forward * 3;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _direction = Vector3.back * 3;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 2, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -2, 0);
        }
        else{
            _direction = Vector3.zero;
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
