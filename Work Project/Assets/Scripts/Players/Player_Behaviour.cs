using UnityEngine;
using Dungeon.Mover;
using Dungeon.Core;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Dungeon.Menu;

namespace Dungeon.Player
{
    public class Player_Behaviour : MonoBehaviour, Combat_Interface
    {
        private Movement mover; //GET THE SCRIPT FOR THE MOVING

        [HideInInspector] public Hero hero; //HERO STATS CANNOT BE MODIFIED
        [HideInInspector] public InGame_Hero game_hero; //HERO STATS IN GAME

        /* THE LIST OF SPELLS THE HERO CAN US*/
        [HideInInspector] public List<Hero_Spells> hero_spells = new List<Hero_Spells>();

        [HideInInspector] public Transform current_platform; // PLATFORM ON WHICH HERO IS PLACED

        [HideInInspector] public bool moved; //VERIFY IF THIS UNIT MOVED

        [HideInInspector] public bool selected; //CHECK IF THIS HERO IS SELECTED

        [SerializeField] private GameObject indice; //INDICE THAT SHOWS IF THE HERO IS SELECTED

        [SerializeField] private TextMeshProUGUI health_text; // THE HEALTH TEXT FOR THE HEALTHBAR
        [SerializeField] private Image health_image; //HEALTH BAR

        /*REGION USED FOR RAYCAST ELEMETS*/
        #region RAYCAST ELEMENTS
        private Camera main_cam;
        private RaycastHit hit;
        private Ray ray;
        #endregion

        private void Start()
        {
            mover = GetComponent<Movement>();

            main_cam = Camera.main;

            game_hero = new InGame_Hero(hero.health,hero.damage,hero.armor, hero);
        }

        /*DISPLAY HEALTH UI*/
        private void Health_UI()
        {
            health_image.fillAmount = game_hero.current_health / hero.health;
            health_text.text = game_hero.current_health.ToString("0") + " / " + hero.health;
        }

        private void Update()
        {
            Health_UI();

            if (selected)
                indice.SetActive(true);
            else
                indice.SetActive(false);


            if (!selected) //IF THIS UNIT IS NOT SELECTED SKIP TURN
                return;

            if (Combat_Manager.casting_spells)
                return;

            ray = main_cam.ScreenPointToRay(Input.mousePosition);

            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.tag == "Platform") //CHECK IF WE ARE MOVING
                    {
                        //GET PLATFORM THAT WE RAYCAST
                        Platform _platform = hit.transform.GetComponent<Platform>();
                        //COMPARE IF THE PLATFORM WE WANT TO MOVE IS IN RANGE OF 1 RANG
                        int _result = Mathf.Abs(_platform.rang - current_platform.GetComponent<Platform>().rang);

                        /*
                         * VERIFY 
                         * 1: IF WE ARE NOT CLICKING CURRENT PLATFORM
                         * 2: IF PLATFORM WE ARE CLICKING IS NOT TAKEN
                         * 3: IF PLATFORM IS IN RANGE OF 1 RANG
                         */
                        if (hit.transform != current_platform && !_platform.taken && _result <= 1)
                        {
                            current_platform.GetComponent<Platform>().taken = false;
                            _platform.taken = true;
                            current_platform = hit.transform;
                            Move_To(hit.transform.position + new Vector3(0,1,0)); //SEND MOVE CALL TO MOVEMENT SCRIPT
                        }
                    }
                    else //IF WE ARE NOT MOVING WE ATTACK
                    {
                        if (tag == Static_Info.Player_Tag(1))
                        {
                            if (hit.transform.tag == Static_Info.Player_Tag(2))
                                Attack(hit.transform);

                        }
                        else if (tag == Static_Info.Player_Tag(2))
                        {
                            if (hit.transform.tag == Static_Info.Player_Tag(1))
                                Attack(hit.transform);
                        }

                    }
                }
            }
        }

        /*
         FUNCTION CALLED WHEN WE MOVE TO TARGET
         _position POSITION WHERE WE MOVE VECTOR 3
             */
        private void Move_To(Vector3 _position)
        {
            selected = false;
            mover.Move(_position); 
        }

        /*
         FUNCTION GETS CALLED WHEN WE ATTACK
         _target TARGET THAT WE ATTACK
             */
        private void Attack(Transform _target)
        {
            Platform _platform = current_platform.GetComponent<Platform>();
            Platform _platform2 = _target.GetComponent<Player_Behaviour>().current_platform.GetComponent<Platform>();
            int _result = Mathf.Abs(_platform.rang - _platform2.rang);
            if (hero.attack_type == Attack_Type.mele)
            {
                if (_result <= 1)
                {
                    selected = false;
                    mover.Attack(_target, transform);
                }
            }else if (hero.attack_type == Attack_Type.range)
            {
                if(_result <= 2)
                {
                    selected = false;
                    mover.Attack(_target, transform);
                }
            }
        }

        /*
         * FUNCTION TO GET HIT BY ATTACK
         * _damage AMOUNT OF DAMAGE BY ATTACKER
         EACH 1 ARMOR DECREASES 5% DAMAGE
         EACH 1 ARMOR UNDER 0 INCREASES DAMAGE 5%
             */
        public void Get_Hit(float _damage)
        {
            if (hero.armor == 0)
                game_hero.current_health -= _damage;
            else if (hero.armor > 0)
                game_hero.current_health -= _damage / ((game_hero.current_armor * 5 / 100) + 1);
            else if (hero.armor < 0)
                game_hero.current_health -= _damage * ((game_hero.current_armor * 5 / 100) + 1);

            /* IF WE HEAL THE TARGET DO NOT GO OVER LIMIT*/
            if (game_hero.current_health > hero.health)
                game_hero.current_health = hero.health;

            //CHECK IF THE HERO IS DEAD
            if (game_hero.current_health <= 0)
            {
                if (tag == Static_Info.Player_Tag(1))
                {
                    Combat_Manager.player1_heroes.Remove(this);
                }
                else if (tag == Static_Info.Player_Tag(2))
                {
                    Combat_Manager.player2_heroes.Remove(this);
                }
                current_platform.GetComponent<Platform>().taken = false;
                Destroy(gameObject);
            }
        }
        

        /*SET THE COOLDOWN FOR THE USED SPELL
         _spell => THE SPELL THAT WE USED*/
        public void Spell_Cooldown(Spell_UI _spell)
        {
            int _index = hero_spells.FindIndex(x=> {return x.spell == _spell.spell; });
            hero_spells[_index].Set_Cooldown();
        }
       
    }
}