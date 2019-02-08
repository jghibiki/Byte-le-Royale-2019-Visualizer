using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RawEntities;

public class IllegalSalvageController : MonoBehaviour
{

    public IllegalSalvage illegalSalvageData;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool FindSelfInUniverse(List<IllegalSalvage> illegalSalvage)
    {
        foreach(IllegalSalvage ils in illegalSalvage)
        {
            if(ils.id == illegalSalvageData.id)
            {
                return true;
            }
        }
        return false;
    }
}
