using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour
{
    public float XMovingSpeed = 5f;

    //Move container.
    void Update()
    {

        float newXPosition = transform.position.x + XMovingSpeed * Time.deltaTime;
        transform.position = new Vector3( newXPosition, 0f, 0f );

    }
}
