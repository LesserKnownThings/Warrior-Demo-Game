using UnityEngine;

namespace Dungeon.Player
{
    [CreateAssetMenu(fileName ="Spell", menuName = "Spell")]
    public class Spells : ScriptableObject
    {
        public string description;
        [Space(5)]
        [Header("Stats")]
        public float damage;
        public float damage_modifier;
        public float armor_modifier;
        public float cooldown;
        public float death_timer;
        [Space(5)]
        public GameObject spell_go;
        public Sprite icon;        
        public Spell_Target spell_target;
        public Vector3 spawn_position;
        
        public string Display_Info()
        {
            return description + "\nDamage: <color=black>" + damage + "</color>\nCooldown: <color=black>" + cooldown + "</color>";
        }
    }
}