using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RawEntities;

public class ShipController : MonoBehaviour
{

    Vector3 original_pos;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateFromLog(Ship ship, float intermediate)
    {

        if(intermediate == 0f)
        {
            original_pos = transform.position;
        }

        var raw_pos = new Vector3(ship.position[0], 0, 700-ship.position[1]);

        var new_pos = Vector3.Lerp(original_pos, raw_pos, intermediate);

        transform.LookAt(new_pos);
        transform.position = new_pos;
        
    }
}
