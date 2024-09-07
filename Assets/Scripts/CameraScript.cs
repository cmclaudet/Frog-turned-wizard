using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float startingXPos;

    // Start is called before the first frame update
    void Start()
    {
        startingXPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(startingXPos, transform.position.y, -4);
    }
}
