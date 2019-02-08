using UnityEngine;

public class PoliceController : MonoBehaviour
{

    public GameObject LaserBlast;
    public GameObject currentLaser;
    Transform turretHeadRotation;
    Quaternion initialTurretRotation;
    Transform turretEmissionPoint;
    GameObject attackTarget;
    bool attackingThisTurn = false;
    private Vector3 targetOverridePosition = Vector3.one * -1f;

    private bool startedUp = false;

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
            currentLaser = null;
            turretHeadRotation = transform.Find("TurretHandle");
            turretEmissionPoint = transform.Find("TurretHandle/EmissionPoint");
            initialTurretRotation = turretHeadRotation.rotation;

        }
    }

    public void Attack(GameObject target, int[] target_position)
    {
        Startup();


        attackTarget = target;
        if(target_position[0] == -1 && target_position[1] == -1)
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

    }

    public void PointAndFire()
    {
        if(targetOverridePosition != new Vector3(-1, -1, -1))
        {
            turretHeadRotation.transform.LookAt(targetOverridePosition);
            turretHeadRotation.localEulerAngles = new Vector3(turretHeadRotation.transform.localEulerAngles.x - 90, turretHeadRotation.transform.localEulerAngles.y, turretHeadRotation.transform.localEulerAngles.z);

            currentLaser.GetComponent<LaserController>().Emit(turretEmissionPoint.transform.position, attackTarget, targetOverridePosition, true);
        }
        else
        {
            //target is alive
            turretHeadRotation.transform.LookAt(attackTarget.transform.position);
            turretHeadRotation.localEulerAngles = new Vector3(turretHeadRotation.transform.localEulerAngles.x - 90, turretHeadRotation.transform.localEulerAngles.y, turretHeadRotation.transform.localEulerAngles.z);

            currentLaser.GetComponent<LaserController>().Emit(turretEmissionPoint.transform.position, attackTarget, attackTarget.transform.position);
        }
       
    }
}
