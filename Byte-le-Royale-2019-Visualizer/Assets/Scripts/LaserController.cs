using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour

{

    private Vector3 emissionLocation = new Vector3(0, 0, 0);
    private Vector3 impactLocation = new Vector3(-1, -1, -1);
    private Vector3 target;
    private GameObject targetShip = null;
    private Transform laser;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Emit(Vector3 _emissionLocation, GameObject _targetShip,  Vector3 _target, bool targetDead=false)
    {
        laser = transform.Find("Laser");

        emissionLocation = _emissionLocation;
        target = _target;
        targetShip = _targetShip;

        if (!targetDead)
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(new Ray(emissionLocation, (_target - emissionLocation)));

            var shipTransform = targetShip.transform.Find("Ship").transform;

            foreach (var hit in hits)
            {
                if (hit.transform == shipTransform)
                {
                    impactLocation = hit.point;
                    break;
                }
            }

            if (impactLocation.x == -1) return;
        }
        else
        {
            impactLocation = _target;
        }

        var lineRender = laser.gameObject.GetComponent<LineRenderer>();
        lineRender.SetPositions(new Vector3[] { emissionLocation, impactLocation });

        var emissionGlow = transform.Find("EmissionGlow");
        emissionGlow.position = emissionLocation;

        var impactEffects = transform.Find("ImpactLocation");
        impactEffects.position = impactLocation;

    }


}
