using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawEntities
{
    public class Station {
        public string id;
        public int object_type;
        public string name;
        public List<int> position;

        public int sell_price;
        public int primary_buy_price;
        public int secondary_buy_price;

        public int base_sell_price;
        public int base_primary_buy_price;
        public int base_secondary_buy_price;

        Dictionary<int, string> cargo;

        public int acessibility_radius;
    }

    public class BlackMarketStation
    {
        public string id;
        public int object_type;
        public string name;
        public List<int> position;

        public int sell_price;
        public int primary_buy_price;
        public int secondary_buy_price;

        public int base_sell_price;
        public int base_primary_buy_price;
        public int base_secondary_buy_price;

        Dictionary<int, string> cargo;

        public int acessibility_radius;
    }

    public class SecureStation
    {
        public string id;
        public int object_type;
        public string name;
        public List<int> position;

        public int sell_price;
        public int primary_buy_price;
        public int secondary_buy_price;

        public int base_sell_price;
        public int base_primary_buy_price;
        public int base_secondary_buy_price;

        Dictionary<int, string> cargo;

        public int acessibility_radius;
    }

    public class AsteroidField
    {
        public string id;
        public int object_type;
        public string name;
        public List<int> position;
        public int acessibility_radius;
    }

    public class Ship
    {
        public string id;

        public bool is_npc;
        public string team_name;
        public List<int> color;

        public int max_hull;
        public int current_hull;
        public int engine_speed;

        public int weapon_damage;
        public int weapon_range;

        public int cargo_space;
        public int mining_yield;

        public int sensor_range;

        public int module_0;
        public int module_1;
        public int module_2;
        public int moduke_3;

        public int module_0_level;
        public int module_1_level;
        public int module_2_level;
        public int module_3_level;

        public Dictionary<int, int> inventory;

        public List<int> position;

        public int notoriety;
        public int legal_standing;
        public int bounty;

        public int credits;

        public GameObject gameObject;
      
    }

    public class Manifest
    {
        public string version;
        public int ticks;
        public Dictionary<string, object> results;

        public Dictionary<string, int> GetLeaderboard()
        {
            return new Dictionary<string, int>();
        }
    }

    public class AttackEvent
    {
        public int type;
        public string attacker;
        public string target;
        public int damage;
    }
}
