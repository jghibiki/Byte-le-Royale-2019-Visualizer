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

    private int tick = 1;
    private int intermediate = int.MaxValue;
    private int smoothing = 5;



    // Start is called before the first frame update
    void Start()
    {
        LoadGameData();
        LoadManifest();
    }

    // Update is called once per frame
    void Update()
    {

        if (intermediate > smoothing){

            Debug.Log($"Tick: {tick}");
            if (tick < manifest.ticks)
            {
                var file_no = tick.ToString().PadLeft(5, '0');
                string json = File.ReadAllText($@"../game_log/{file_no}.json");
                var json_parser = JObject.Parse(json);
                var events = json_parser["events"];


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
                        ship_game_object.name = ship.team_name;
                        ship.gameObject = ship_game_object;
                        ship_objects.Add(ship_game_object);

                    }
                    else
                    {
                        existing_ship
                            .gameObject
                            .GetComponent<ShipController>()
                            .UpdateFromLog(ship, 0f);

                        existing_ship.position = ship.position;
                        intermediate = 1;
                    }
                }



            }
            tick += 1;
        }
        else
        {

            foreach (Ship ship in ships)
            {
                Debug.Log(ship.team_name);
                ship
                    .gameObject
                    .GetComponent<ShipController>()
                    .UpdateFromLog(ship, intermediate / (float)smoothing);
            }
            intermediate += 1;

        }

        if (Input.GetKeyUp("up"))
        {
            smoothing -= 3;
            smoothing = Mathf.Clamp(smoothing, 1, 15);
        }
        if (Input.GetKeyUp("down"))
        {
            smoothing += 3;
            smoothing = Mathf.Clamp(smoothing, 1, 15);
        }

    }

    IEnumerable<JToken>  GetObjectsOfType(JToken json_parser, int object_type)
    {
        return json_parser["universe"].SelectTokens($"$[?(@.object_type =={object_type})]");  
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
        manifest = json_parser.ToObject<Manifest>();
    }

    void LoadGameData()
    {

        string json = File.ReadAllText(@"../game_data.json");
        var json_parser = JObject.Parse(json);

        foreach (Station station in LoadType<Station>(json_parser, 1))
        {
            var station_pos = new Vector3((float)station.position[0], 0f, world_height - (float)station.position[1]);
            var station_game_object = Instantiate(station_model, station_pos, Quaternion.identity);
            station_game_object.name = station.name;
            station_objects.Add(station_game_object);
        }

        //load black market stations
        foreach (Station station in LoadType<Station>(json_parser, 2))
        {
            var station_pos = new Vector3((float)station.position[0], 0f, world_height - (float)station.position[1]);
            var station_game_object = Instantiate(station_model, station_pos, Quaternion.identity);
            station_game_object.name = station.name;
            station_objects.Add(station_game_object);
        }

        // load secure station
        foreach (Station station in LoadType<Station>(json_parser, 3))
        {
            var station_pos = new Vector3((float)station.position[0], 0f, world_height - (float)station.position[1]);
            var station_game_object = Instantiate(station_model, station_pos, Quaternion.identity);
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
            ship_game_object.name = ship.team_name;
            ship.gameObject = ship_game_object;
            ship_objects.Add(ship_game_object);
        }
    }

}
