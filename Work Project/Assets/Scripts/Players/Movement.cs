using UnityEngine;
using System.Collections;
using Dungeon.Core;
using Dungeon.Player;

namespace Dungeon.Mover
{
    public class Movement : MonoBehaviour
    {
        private Combat_Manager manager; 

        private void Start()
        {
            manager = GameObject.FindWithTag("Manager").GetComponent<Combat_Manager>();
        }

        /*
         BASIC MOVE
         _position => POSITION WHERE THE HERO SHOULD MOVE
             */
        public void Move(Vector3 _position)
        {
            StartCoroutine("Move_Target", _position);
        }

        /*
         *Function to attack   
         * _target ==> THE TARGET THAT WILL BE ATTACKED
         * _hero => THE HERO THAT IS ATTACKING
         * */
        public void Attack(Transform _target, Transform _hero)
        {
            StartCoroutine(Attack_Target(_target,_hero));
        }

        private IEnumerator Attack_Target(Transform _target, Transform _hero)
        {
            Vector3 original_pos = transform.position;

           InGame_Hero _heroCopy = _hero.GetComponent<Player_Behaviour>().game_hero;
            /*
             MELE ATTACK
             */
            if (_heroCopy.hero.attack_type == Attack_Type.mele)
            {
                float _distance = Vector3.Distance(_target.position, transform.position);
                /*MOVE THE HERO IN FRONT OF THE TARGET*/
                while (_distance > 2f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, _target.position, 15f * Time.deltaTime);
                    _distance = Vector3.Distance(transform.position, _target.position);
                    yield return null;
                }

                //ATTACK THE PLAYER
                _target.GetComponent<Combat_Interface>().Get_Hit(_heroCopy.current_damage);

                yield return new WaitForSeconds(.5f); // DELAY FOR ATTACK ANIMATION
                _distance = Vector3.Distance(transform.position, original_pos);
                /*RETURN THE HERO TO THE INITIAL POSITON*/
                while (_distance > .5f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, original_pos, 15f * Time.deltaTime);
                    _distance = Vector3.Distance(transform.position, original_pos);
                    yield return null;
                }
                transform.position = original_pos;//MAKE SURE THE HERO IS IN INITIAL POSITION
                //END THE TURN AND GO TO NEXT HERO
                Combat_Manager.player_turn = !Combat_Manager.player_turn;
                manager.Decide_Hero();
            }
            /*
             RANGE ATTACK
             */
            else if (_heroCopy.hero.attack_type == Attack_Type.range)
            {
                GameObject _copy = Instantiate(_heroCopy.hero.projectile, _hero.position + new Vector3(0,1.25f,0),Quaternion.identity);
                _copy.GetComponent<Projectile>().Set_Projectile(_target, _hero);
            }              
        }


        private IEnumerator Move_Target(Vector3 _position)
        {
            float _distance = Vector3.Distance(transform.position, _position);
            while (_distance > .5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, _position, 15f * Time.deltaTime);
                _distance = Vector3.Distance(transform.position, _position);
                yield return null;
            }
            transform.position = _position;
            Combat_Manager.player_turn = !Combat_Manager.player_turn;
            manager.Decide_Hero();            
        }
    }
}