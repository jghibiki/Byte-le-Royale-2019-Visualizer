using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RawEntities;

public class ShipController : MonoBehaviour
{
    public GameObject LaserBlast;
    public Ship shipData;
    Vector3 original_pos;
    GameObject currentLaser;
    Transform turretHeadRotation;
    Quaternion initialTurretRotation;
    Transform turretEmissionPoint;
    GameObject attackTarget;
    public GameObject explosion;
    bool attackingThisTurn = false;
    private Vector3 targetOverridePosition = Vector3.one * -1;

    GameObject shipGameObj;
    GameObject team_name;

    bool wasDead = false;


    bool startedUp = false;
    bool updatedName = false;
    bool print = false;
    bool skipLerp = false;


    public bool police = false;

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

            shipGameObj = transform.Find("Ship").gameObject;
            team_name = transform.Find("TextContainer/Text").gameObject;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateFromLog(Ship ship)
    {
        Startup();

        shipData = ship;

        original_pos = transform.position;
        skipLerp = false;
        attackingThisTurn = false;
    }

    public void UpdateSubTick(float intermediate) { 

        if (!updatedName)
        {
            gameObject.name = shipData.team_name;
            team_name.GetComponent<TextController>().RefreshTeamName();
        }

        if(!IsAlive() && !wasDead)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        if (!IsAlive())
        {
            shipGameObj.SetActive(false);
            team_name.SetActive(false);
            wasDead = true;
        }
        else if (wasDead && IsAlive())
        {
            shipGameObj.SetActive(true);
            team_name.SetActive(true);
            wasDead = false;
            skipLerp = true;
            transform.position = new Vector3(500, 0, 700 - 350);

        }

        if (IsAlive() && !skipLerp)
        {
            //lerp location
            var raw_pos = new Vector3(shipData.position[0], 0, 700 - shipData.position[1]);

            var new_pos = Vector3.Lerp(original_pos, raw_pos, intermediate);

            transform.LookAt(new_pos);
            transform.position = new_pos;
        }
        else if(wasDead)
        {
            // set position directly
            var raw_pos = new Vector3(shipData.position[0], 0, 700 - shipData.position[1]);
            transform.position = raw_pos;
        }




        if(!attackingThisTurn && currentLaser != null)
        {
            currentLaser.SetActive(false);
        }

        if(attackingThisTurn && currentLaser != null)
        {
            PointAndFire();
        }


        
    }

    public void Attack(GameObject target, int[] target_position)
    {
        Startup();


        attackTarget = target;
        if (target_position[0] == -1 && target_position[1] == -1)
        {
            targetOverridePosition = Vector3.one * -1f;
        }
        else
        {
            targetOverridePosition = new Vector3(target_position[0], 0, 700 - target_position[1]);
        }

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
        if (targetOverridePosition != new Vector3(-1, -1, -1))
        {
            turretHeadRotation.transform.LookAt(targetOverridePosition);
            turretHeadRotation.localEulerAngles = new Vector3(turretHeadRotation.transform.localEulerAngles.x - 90, turretHeadRotation.transform.localEulerAngles.y + 90, turretHeadRotation.transform.localEulerAngles.z);

            currentLaser.GetComponent<LaserController>().Emit(turretEmissionPoint.transform.position, attackTarget, targetOverridePosition, true);
        }
        else
        {
            //target is alive
            turretHeadRotation.transform.LookAt(attackTarget.transform.position);
            turretHeadRotation.localEulerAngles = new Vector3(turretHeadRotation.transform.localEulerAngles.x - 90, turretHeadRotation.transform.localEulerAngles.y + 90, turretHeadRotation.transform.localEulerAngles.z);

            currentLaser.GetComponent<LaserController>().Emit(turretEmissionPoint.transform.position, attackTarget, attackTarget.transform.position);
        }
    }

    public bool IsAlive()
    {
        return shipData.current_hull > 0 && shipData.respawn_counter < 0;
    }

}
