using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RawEntities;

public class PoliceSquadController : MonoBehaviour
{
    public GameObject LaserBlast;
    public Ship shipData;
    public GameObject explosion;
    Vector3 original_pos;

    bool lasersActive = false;
    GameObject attackTarget;
    bool attackingThisTurn = false;

    GameObject squad;
    List<GameObject> shipGameObjs = new List<GameObject>();
    GameObject team_name;

    bool wasDead = false;


    bool startedUp = false;
    bool updatedName = false;

    bool skipLerp = false;


    public bool police = false;

    // Start is called before the first frame update
    void Start()
    {
        Startup();

    }

    void Startup()
    {
        if (!startedUp)
        {
            startedUp = true;

            squad = transform.Find("Squad").gameObject;
            shipGameObjs.Add(transform.Find("Squad/1").gameObject);
            shipGameObjs.Add(transform.Find("Squad/2").gameObject);
            shipGameObjs.Add(transform.Find("Squad/3").gameObject);
            shipGameObjs.Add(transform.Find("Squad/4").gameObject);
            shipGameObjs.Add(transform.Find("Squad/5").gameObject);

            team_name = transform.Find("TextContainer/Text").gameObject;
        }
    }

    public void UpdateFromLog(Ship ship)
    {
        Startup();

        shipData = ship;

        original_pos = transform.position;
        skipLerp = false;
        attackingThisTurn = false;
    }

    public void UpdateSubTick(float intermediate)
    {

        if (!updatedName)
        {
            gameObject.name = shipData.team_name;
            team_name.GetComponent<TextController>().RefreshTeamName();
        }

        if (!IsAlive() && !wasDead)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        if (!IsAlive())
        {
            squad.SetActive(false);
            team_name.SetActive(false);
            wasDead = true;
        }
        else if (wasDead && IsAlive())
        {
            squad.SetActive(true);
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
        else if (wasDead)
        {
            // set position directly
            var raw_pos = new Vector3(shipData.position[0], 0, 700 - shipData.position[1]);
            transform.position = raw_pos;
        }




        if (!attackingThisTurn && lasersActive)
        {
            foreach(var ship in shipGameObjs)
            {
                ship.GetComponent<PoliceController>().currentLaser.SetActive(false);
            }
            lasersActive = false;
        }

        if (attackingThisTurn && lasersActive)
        {
            foreach (var ship in shipGameObjs)
            {
                ship.GetComponent<PoliceController>().PointAndFire();
            }

        }

    }

    public void Attack(GameObject target, int[] target_position)
    {
        foreach(var shipObj in shipGameObjs)
        {
            shipObj.GetComponent<PoliceController>().Attack(target, target_position);
        }
        attackingThisTurn = true;
        lasersActive = true;
    }

    public bool IsAlive()
    {
        return shipData.current_hull > 0 && shipData.respawn_counter < 0;
    }
}
