using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RawEntities;

public class ShipController : MonoBehaviour
{
    public GameObject LaserBlast;
    Ship shipData;
    Vector3 original_pos;
    GameObject currentLaser;
    Transform turretHeadRotation;
    Quaternion initialTurretRotation;
    Transform turretEmissionPoint;
    GameObject attackTarget;
    bool attackingThisTurn = false;


    bool startedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        Startup();

    }

    void Startup()
    {
        if (!startedUp) {
            startedUp = true;
            currentLaser = null;
            turretHeadRotation = transform.Find("Ship/TurretHeadRotation");
            turretEmissionPoint = transform.Find("Ship/TurretHeadRotation/EmissionPoint");
            initialTurretRotation = turretHeadRotation.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateFromLog(Ship ship, float intermediate)
    {
        Startup();

        shipData = ship;

        if(intermediate == 0f)
        {
            original_pos = transform.position;
            
        }

        if (IsAlive())
        {
            //lerp location
            var raw_pos = new Vector3(ship.position[0], 0, 700 - ship.position[1]);

            var new_pos = Vector3.Lerp(original_pos, raw_pos, intermediate);

            transform.LookAt(new_pos);
            transform.position = new_pos;
        }
        else
        {
            // set position directly
            var raw_pos = new Vector3(ship.position[0], 0, 700 - ship.position[1]);
            transform.position = raw_pos;
        }

        if (!IsAlive())
        {
            transform.Find("Ship").gameObject.SetActive(false);
            transform.Find("TextContainer").gameObject.SetActive(false);
        }
        else
        {
            foreach (Transform child in transform)
            {
                transform.Find("Ship").gameObject.SetActive(true);
                transform.Find("TextContainer").gameObject.SetActive(true);
            }
        }

        if(!attackingThisTurn && currentLaser != null)
        {
            currentLaser.SetActive(false);
        }

        if(attackingThisTurn && currentLaser != null)
        {
            PointAndFire();
        }



        if(intermediate == 1)
        {
            attackingThisTurn = false;
        }

        
    }

    public void Attack(GameObject target)
    {
        Startup();


        attackTarget = target;

        if (currentLaser == null)
        {
            currentLaser = Instantiate(LaserBlast, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            currentLaser.SetActive(true);
        }

        PointAndFire();

        attackingThisTurn = true;
    }

    public void PointAndFire()
    {
        turretHeadRotation.transform.LookAt(attackTarget.transform.position);
        turretHeadRotation.localEulerAngles = new Vector3(turretHeadRotation.transform.localEulerAngles.x- 90, turretHeadRotation.transform.localEulerAngles.y+90, turretHeadRotation.transform.localEulerAngles.z);
        currentLaser.GetComponent<LaserController>().Emit(turretEmissionPoint.transform.position, attackTarget);
    }

    public bool IsAlive()
    {
        return shipData.current_hull > 0;
    }

}
