using UnityEngine;
using Dungeon.Core;
using System.Collections.Generic;
namespace Dungeon.Player {
    [CreateAssetMenu(fileName = "Hero", menuName = "Hero")]
    public class Hero : ScriptableObject
    {
        public string hero_name;
        public Color color;

        [Header("Projectile if ranged")]
        public GameObject projectile = null;
        [Space(3)]
        [Header("Stats")]
        public float damage;
        public float health;
        public float armor;
        [Space(5)]
        [Header("Misc")]
        public Sprite icon;
        public Hero_Class hero_class;
        public Attack_Type attack_type;

        public List<Spells> spells = new List<Spells>();

        public string Description()
        {
            return "Name:   <color=black>" + hero_name + "</color>\nDamage:   <color=black>" + damage + "</color>\nArmor:   <color=black>" + armor + "</color>\nHealth:   <color=black>" + health + "</color>\nClass:   <color=black>" + hero_class + "</color>\nAttack:   <color=black>" + attack_type + "</color>";
        }
    }
}