using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardController : MonoBehaviour
{
    private float rotationSpeed = 30;
    private float rotationDir;

    // Start is called before the first frame update
    void Start()
    {
        rotationDir = Random.Range(0, 1) > 0.5 ? -1f : 1f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationDir * rotationSpeed * Time.deltaTime, 0);
    }
}
