using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationController : MonoBehaviour
{

    public float rotationSpeed;
    private float rotationDir;
    public Vector3 rotationAxis;

    // Start is called before the first frame update
    void Start()
    {
        rotationDir = Random.Range(0, 1) > 0.5 ? -1f : 1f;

        rotationAxis = rotationAxis.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        var rotation = rotationDir * rotationSpeed * Time.deltaTime;
        transform.Rotate(rotationAxis.x * rotation, rotationAxis.y * rotation, rotationAxis.z*rotation);
    }
}
