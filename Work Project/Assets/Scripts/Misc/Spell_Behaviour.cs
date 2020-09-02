using System.Collections;
using UnityEngine;
using Dungeon.Player;

namespace Dungeon.Core
{
    //********************************************************************************
    //********************************************************************************
    /*USED THE SAME AS PROJECTILE, BUT SPAWNS ON THE TARGET AND HAS A TIMER TO DEATH*/
    //********************************************************************************
    //********************************************************************************
    public class Spell_Behaviour : MonoBehaviour
    {
        private Transform target; //TARGET TO ATTACK
        private Spells spell; //USED SPELL TO ATTACK THE TARGET
        private Combat_Manager combat_manager; 

        private void Start()
        {
            combat_manager = GameObject.FindWithTag("Manager").GetComponent<Combat_Manager>();
        }

        /*SETTING THE SPELL
         _spell => SPELL USED TO ATTACK
         _target => TARGET TO ATTACK
             */
        public void Set_Projectile(Transform _target, Spells _spell)
        {
            target = _target;
            spell = _spell;
        }

        private void Update()
        {
            /*MOVE THE SPELL TO THE TERGET*/
            if(target != null)
            transform.position = Vector3.MoveTowards(transform.position, target.position, 25f * Time.deltaTime);
        }

        /*DESTROY ON COLLIDE WITH THE TARGET AND GO TO NEXT PLAYER*/
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == target.tag)
            {
                target.GetComponent<Combat_Interface>().Get_Hit(spell.damage);                
                StartCoroutine("Delay_Spell",spell.death_timer);
            }
        }

        private IEnumerator Delay_Spell(float _delay)
        {
            yield return new WaitForSeconds(_delay); //DESTROY DELAY
            Combat_Manager.casting_spells = false;
            Combat_Manager.player_turn = !Combat_Manager.player_turn;
            combat_manager.Decide_Hero();
            Destroy(gameObject);
        }
    }
}