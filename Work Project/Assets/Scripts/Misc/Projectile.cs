using UnityEngine;
using Dungeon.Player;

namespace Dungeon.Core
{
    public class Projectile : MonoBehaviour
    {
        private Transform target; //TARGET TO ATTACK
        private Transform attacker; // WHO IS ATTACKING
        private Combat_Manager manager; 

        private void Start()
        {
            manager = GameObject.FindWithTag("Manager").GetComponent<Combat_Manager>();
        }

        /*
         SETTING THE PROJECTILE WHEN WE INSTANTIATE IT IN THE MOVER SCRIPT
                 _target => THE TARGET TO ATTACK
                 _attacker => THE HERO THAT IS SHOOTING
             */
        public void Set_Projectile(Transform _target, Transform _attacker)
        {
            target = _target;
            attacker = _attacker;
        }

        private void Update()
        {
            if (target == null)
                Destroy(gameObject);

            if (target != null)
            {
                transform.LookAt(target.position);
                transform.position = Vector3.MoveTowards(transform.position, target.position + new Vector3(0, .5f, 0), 30f * Time.deltaTime);
            }
        }

        /*
         WHEN ON IMPACT WITH THE TARGET DESTROY THE CURRENT PROJECTILE AND INFLICT DAMAGE
             */
        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == target.tag)
            {
                target.GetComponent<Combat_Interface>().Get_Hit(attacker.GetComponent<Player_Behaviour>().game_hero.current_damage); //INFLICT THE DAMAGE

                /*END THE TURN AND GO TO NEXT HERO THEN DESTROY PROJECTILE*/
                Combat_Manager.player_turn = !Combat_Manager.player_turn;
                manager.Decide_Hero();
                Destroy(gameObject);
            }
        }
    }
}