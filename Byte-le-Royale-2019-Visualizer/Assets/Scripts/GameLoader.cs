using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using static RawEntities;

public class GameLoader : MonoBehaviour
{

    private float world_height = 700;
    private float world_width = 1000;

    Manifest manifest;

    public GameObject station_model;
    public GameObject cuprite_field_model;
    public GameObject goethite_field_model;
    public GameObject gold_field_model;
    public GameObject black_market_station_model;
    public GameObject secure_station_model;
    public GameObject ship_model;
    public GameObject police_model;
    public List<GameObject> billboards;
    public GameObject illegal_salvage_model;

    List<Station> stations = new List<Station>();
    AsteroidField cuprite_field;
    AsteroidField goethite_field;
    AsteroidField gold_field;
    List<Ship> ships = new List<Ship>();

    List<GameObject> station_objects = new List<GameObject>();
    GameObject cuprite_field_game_object;
    GameObject goethite_field_game_object;
    GameObject gold_field_game_object;
    List<GameObject> ship_objects = new List<GameObject>();
    List<GameObject> illegal_salvage_game_objects = new List<GameObject>();

    private int tick = 1;
    private float intermediate = int.MaxValue;
    private float smoothing = 0.1f;
    public int maxSmoothing = 25;

    private bool pause = false;

    public Camera perspectiveCam;
    public Camera topDownCam;
    public Camera currentCamera;


    // Start is called before the first frame update
    void Start()
    {
        currentCamera = topDownCam;
        LoadGameData();
        LoadManifest();
        intermediate = smoothing+1; // force update first turn
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKey(KeyCode.LeftControl)  && Input.GetMouseButtonDown(0))
        {
            //ray cast to hit ship
            Ray ray =currentCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, -ray.direction, Color.blue);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 2000f))
            {
                GameObject ship = null;
                foreach(var ship_obj in ship_objects)
                {
                    if(ship_obj.transform == hit.transform.parent)
                    {
                        ship = ship_obj;
                        break;
                    }
                }
                if(ship != null)
                {
                    // set ship camera to active camera
                    var shipCam = ship.transform.Find("TrailingCamera");

                    currentCamera.gameObject.tag = "Untagged";
                    currentCamera.enabled = false;

                    shipCam.gameObject.tag = "MyCurrentCamera";
                    currentCamera = shipCam.gameObject.GetComponent<Camera>();
                    currentCamera.enabled = true;
                    perspectiveCam.GetComponent<OrthoCameraController>().enabled = false;
                }
            }
        }

        if (Input.GetKeyUp("[+]"))
        {

            currentCamera.gameObject.tag = "Untagged";
            currentCamera.enabled = false;

            perspectiveCam.gameObject.tag = "MyCurrentCamera";
            perspectiveCam.enabled = true;
            currentCamera = perspectiveCam;
            perspectiveCam.GetComponent<OrthoCameraController>().enabled = true;
        }

        if (Input.GetKeyUp("[-]"))
        {

            currentCamera.gameObject.tag = "Untagged";
            currentCamera.enabled = false;

            currentCamera = topDownCam;
            currentCamera.enabled = true;
            currentCamera.gameObject.tag = "MyCurrentCamera";
            perspectiveCam.GetComponent<OrthoCameraController>().enabled = false;
        }

        if (Input.GetKeyUp("space"))
        {
            pause = !pause;
        }

        // Do things that can be done while paused



        if (pause) return; // things that can't be done while paused.


        if (intermediate > smoothing){

            Debug.Log($"Tick: {tick}/{manifest.ticks}");
            if (tick < manifest.ticks)
            {
                var file_no = tick.ToString().PadLeft(5, '0');
                string json = File.ReadAllText($@"../game_log/{file_no}.json");
                var json_parser = JObject.Parse(json);


                // update objects 
                foreach (JToken item in GetObjectsOfType(json_parser["turn_result"], 0))
                {
                    Ship ship = item.ToObject<Ship>();
                    var existing_ship = ships.FirstOrDefault(s => s.id.Equals(ship.id));
                    if (existing_ship == null)
                    {
                        // add ship
                        ships.Add(ship);
                        var ship_pos = new Vector3((float)ship.position[0], 0f, world_height - (float)ship.position[1]);
                        var ship_game_object = Instantiate(ship_model, ship_pos, Quaternion.identity);
                        ship.gameObject = ship_game_object;
                        ship_objects.Add(ship_game_object);
                        ship_game_object.GetComponent<ShipController>().UpdateFromLog(ship);
                    }
                    else
                    {
                        existing_ship.team_name = ship.team_name;
                        existing_ship
                            .gameObject
                            .GetComponent<ShipController>()
                            .UpdateFromLog(ship);
                    }
                }

                // add police
                foreach (JToken item in GetObjectsOfType(json_parser["turn_result"], 8))
                {
                    Ship ship = item.ToObject<Ship>();
                    var existing_ship = ships.FirstOrDefault(s => s.id.Equals(ship.id));
                    if (existing_ship == null)
                    {
                        // add ship
                        ships.Add(ship);
                        var ship_pos = new Vector3((float)ship.position[0], 0f, world_height - (float)ship.position[1]);
                        var ship_game_object = Instantiate(police_model, ship_pos, Quaternion.identity);
                        ship_game_object.name = ship.team_name;
                        ship.gameObject = ship_game_object;
                        ship_objects.Add(ship_game_object);
                        ship_game_object.GetComponent<PoliceSquadController>().UpdateFromLog(ship);
                    }
                    else
                    {
                        existing_ship
                            .gameObject
                            .GetComponent<PoliceSquadController>()
                            .UpdateFromLog(ship);
                    }
                }

                //add enforcers
                foreach (JToken item in GetObjectsOfType(json_parser["turn_result"], 9))
                {
                    Ship ship = item.ToObject<Ship>();
                    var existing_ship = ships.FirstOrDefault(s => s.id.Equals(ship.id));
                    if (existing_ship == null)
                    {
                        // add ship
                        ships.Add(ship);
                        var ship_pos = new Vector3((float)ship.position[0], 0f, world_height - (float)ship.position[1]);
                        var ship_game_object = Instantiate(police_model, ship_pos, Quaternion.identity);
                        ship_game_object.name = ship.team_name;
                        ship.gameObject = ship_game_object;
                        ship_objects.Add(ship_game_object);
                        ship_game_object.GetComponent<PoliceSquadController>().UpdateFromLog(ship);
                    }
                    else
                    {
                        existing_ship
                            .gameObject
                            .GetComponent<PoliceSquadController>()
                            .UpdateFromLog(ship);
                    }
                }

                // Get illegal salvage

                var rawIllegalSalvageObjects = GetObjectsOfType(json_parser["turn_result"], 10);
                var illegalSalvageObjectsInUni = new List<IllegalSalvage>();


                foreach(JToken salvage in rawIllegalSalvageObjects){
                    var salvageObject = salvage.ToObject<IllegalSalvage>();
                    illegalSalvageObjectsInUni.Add(salvageObject);
                }

                // handle illegal salave create events
                foreach(JToken item in GetEventOfType(json_parser["turn_result"], 16))
                {
                    var e = item.ToObject<IllegalSalvageSpawnedEvent>();
                    IllegalSalvage salvage = null;
                    foreach(IllegalSalvage salvageObject in illegalSalvageObjectsInUni)
                    {
                        if(e.id == salvageObject.id)
                        {
                            salvage = salvageObject;
                            break;
                        }
                    }
                    if (salvage == null) continue; //eh skip it something got weird
                    var new_illegal_salvage_game_object = Instantiate(illegal_salvage_model, new Vector3(e.position[0], 0f, 700 - e.position[1]), Quaternion.identity);
                    illegal_salvage_game_objects.Add(new_illegal_salvage_game_object);
                    new_illegal_salvage_game_object.GetComponent<IllegalSalvageController>().illegalSalvageData = salvage;
                }

                // verify each illegal salvage still exists in universe or destroy
                var toRemove = new List<GameObject>();
                foreach(GameObject illegal_salvage in illegal_salvage_game_objects)
                {
                    if (!illegal_salvage.GetComponent<IllegalSalvageController>().FindSelfInUniverse(illegalSalvageObjectsInUni)) {
                        toRemove.Add(illegal_salvage);
                    }
                }
                foreach(var obj in toRemove)
                {
                    illegal_salvage_game_objects.Remove(obj);
                    Destroy(obj);
                }


                // Handle attack events

                foreach (JToken item in GetEventOfType(json_parser["turn_result"], 5))
                {
                    var e = item.ToObject<AttackEvent>();
                    GameObject attacker = null;
                    Ship attackerShip = null;
                    GameObject target = null;
                    Ship targetShip = null;

                    foreach (var ship in ships)
                    {
                        if (ship.id == e.attacker)
                        {
                            attacker = ship.gameObject;
                            attackerShip = ship;
                        }
                        if (ship.id == e.target)
                        {
                            target = ship.gameObject;
                            targetShip = ship;
                        }

                        if (attacker != null && target != null) break;
                    }

                    if(attackerShip.object_type == 8 || attackerShip.object_type == 9)
                    {
                        if (!attacker.GetComponent<PoliceSquadController>().IsAlive())
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!attacker.GetComponent<ShipController>().IsAlive())
                        {
                            continue;
                        }
                    }

                    

                    if (attackerShip.object_type == 8 || attackerShip.object_type == 9)
                    {
                        var pos = !target.GetComponent<ShipController>().IsAlive() ? e.target_position : new int[] { -1, -1};
                        attacker.GetComponent<PoliceSquadController>().Attack(target, pos);
                    }
                    else
                    {
                        int[] pos;
                        if(targetShip.object_type == 8 || targetShip.object_type == 9)
                        {
                            pos = !target.GetComponent<PoliceSquadController>().IsAlive() ? e.target_position : new int[] { -1, -1 };
                        }
                        else
                        {
                            pos = !target.GetComponent<ShipController>().IsAlive() ? e.target_position : new int[] { -1, -1 };
                        }
                        attacker.GetComponent<ShipController>().Attack(target, pos);
                    }


                }
            }

            else {
                Debug.Log("Game Over.");
                return;
            }

            intermediate = 0;
            tick += 1;
        }
        else
        {

            foreach (Ship ship in ships)
            {

                //handle police and enforcer special case
                if(ship.object_type == 8 || ship.object_type == 9)
                {
                    ship
                    .gameObject
                    .GetComponent<PoliceSquadController>()
                    .UpdateSubTick(intermediate / smoothing);
                }
                else
                {

                    ship
                    .gameObject
                    .GetComponent<ShipController>()
                    .UpdateSubTick(intermediate / smoothing);
                }

            }
            intermediate += Time.deltaTime;

        }

        if (Input.GetKey("up"))
        {
            smoothing -= 0.01f;
            smoothing = Mathf.Clamp(smoothing, 0.05f, 1.5f);
            Debug.Log($"{smoothing} {intermediate}");
        }
        if (Input.GetKey("down"))
        {
            smoothing += 0.01f;
            smoothing = Mathf.Clamp(smoothing, 0.05f, 1.5f);
            if(smoothing == 0.01f)

            Debug.Log($"{smoothing} {intermediate}");
        }
    

    }

    IEnumerable<JToken>  GetObjectsOfType(JToken json_parser, int object_type)
    {

        return json_parser["universe"].SelectTokens($"$[?(@.object_type =={object_type})]");
    }

    IEnumerable<JToken> GetEventOfType(JToken json_parser, int event_type)
    {
        return json_parser["events"].SelectTokens($"$[?(@.type == {event_type})]");
    }


    List<T> LoadType<T>(JObject json_parser, int object_type)
    {
        List<T> objects = new List<T>();

        var json_objects = GetObjectsOfType(json_parser, object_type);

        foreach (JToken item in json_objects)
        {
            var deserialized_object = item.ToObject<T>();
            objects.Add(deserialized_object);
        }
        return objects;
    }

    void LoadManifest()
    {
        // Parse manifest
        string json = File.ReadAllText(@"../game_log/manifest.json");
        var json_parser = JObject.Parse(json);
        this.manifest = json_parser.ToObject<Manifest>();
    }

    void LoadGameData()
    {

        string json = File.ReadAllText(@"../game_data.json");
        var json_parser = JObject.Parse(json);

        foreach (Station station in LoadType<Station>(json_parser, 1))
        {
            var station_pos = new Vector3((float)station.position[0], 0f, world_height - (float)station.position[1]);
            var tilt_x = Random.Range(-80, -100);
            var tilt_y = Random.Range(0, 360);
            var station_game_object = Instantiate(station_model, station_pos, Quaternion.Euler(new Vector3(tilt_x, tilt_y, 0f)));
            station_game_object.name = station.name;
            station_objects.Add(station_game_object);

            // load billboard
            var raw_station_name = station_game_object.name.Split(' ')[0];
            foreach (GameObject obj in billboards)
            {
                var raw_name = obj.name.Split(' ')[0];
                if (raw_name == raw_station_name)
                {
                    var offset = station_game_object.transform.position + new Vector3(8, 10, 0);
                    var new_billboard = Instantiate(obj, offset, Quaternion.identity);

                    var offset2 = station_game_object.transform.position + new Vector3(-8, 10, 0);
                    var new_billboard_2 = Instantiate(obj, offset2, Quaternion.identity);
                }
            }
        }

        //load black market stations
        foreach (Station station in LoadType<Station>(json_parser, 2))
        {
            var station_pos = new Vector3((float)station.position[0], 0f, world_height - (float)station.position[1]);
            var station_game_object = Instantiate(black_market_station_model, station_pos, Quaternion.identity);
            station_game_object.name = station.name;
            station_objects.Add(station_game_object);

        }

        // load secure station
        foreach (Station station in LoadType<Station>(json_parser, 3))
        {
            var station_pos = new Vector3((float)station.position[0], 15f, world_height - (float)station.position[1]);
            var station_game_object = Instantiate(secure_station_model, station_pos, Quaternion.Euler(new Vector3(0f, 100f, 0f)));
            station_game_object.name = station.name;
            station_objects.Add(station_game_object);
        }


        // Load asteroid fields
        cuprite_field = LoadType<AsteroidField>(json_parser, 5)[0];
        var cuprite_field_pos = new Vector3((float)cuprite_field.position[0], 0f, world_height - (float)cuprite_field.position[1]);
        var cuprite_field_game_object = Instantiate(cuprite_field_model, cuprite_field_pos, Quaternion.identity);
        cuprite_field_game_object.name = cuprite_field.name;


        goethite_field = LoadType<AsteroidField>(json_parser, 4)[0];
        var goethite_field_pos = new Vector3((float)goethite_field.position[0], 0f, world_height - (float)goethite_field.position[1]);
        var goethite_field_game_object = Instantiate(goethite_field_model, goethite_field_pos, Quaternion.identity);
        goethite_field_game_object.name = goethite_field.name;

        gold_field = LoadType<AsteroidField>(json_parser, 6)[0];
        var gold_field_pos = new Vector3((float)gold_field.position[0], 0f, world_height - (float)gold_field.position[1]);
        var gold_field_game_object = Instantiate(gold_field_model, gold_field_pos, Quaternion.identity);
        gold_field_game_object.name = gold_field.name;

        // Load ships
        foreach (Ship ship in LoadType<Ship>(json_parser, 0))
        {
            ships.Add(ship);
            var ship_pos = new Vector3((float)ship.position[0], 0f, world_height - (float)ship.position[1]);
            var ship_game_object = Instantiate(ship_model, ship_pos, Quaternion.identity);
            //ship_game_object.name = ship.team_name;
            ship.gameObject = ship_game_object;
            ship_objects.Add(ship_game_object);
            ship_game_object.GetComponent<ShipController>().UpdateFromLog(ship);
        }
    }


}
