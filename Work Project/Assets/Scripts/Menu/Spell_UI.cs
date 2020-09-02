using UnityEngine.UI;
using UnityEngine;
using Dungeon.Player;
using UnityEngine.EventSystems;
using Dungeon.Core;
using TMPro;

namespace Dungeon.Menu
{
    public class Spell_UI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [HideInInspector] public Spells spell; //SPELLS STATS NOT TO BE MODIFIED
        [HideInInspector] public Player_Behaviour attacker; //THE ONE WHO IS CASTING THE SPELL
        [HideInInspector] public float spell_cooldown = 0; //THE SPELL COOLDOWN

        [SerializeField] private GameObject cooldown_image;//THE COOLDOWN IMAGE ON THE SPELL
        [SerializeField] private TextMeshProUGUI cooldown_text; //THE COOLDOWN TEXT
       

        private Image icon;//THE ICON FOR THE SPELL

        private bool chosen_spell; //CHECK IF WE CHOSE A SPELL

        #region USED TO RAYCAST
        private Ray ray;
        private RaycastHit hit;
        private Camera main_cam;
        #endregion

        /*
         IF WE CLICK ON THIS SPELL WE WILL USE IT
             */
        public void OnPointerClick(PointerEventData eventData)
        {
            /*DO NOT CONTINUE IF WE ARE ALREADY CASTING A SPELL*/
            if (Combat_Manager.casting_spells)
                return;

            /*DO NOT CONTINUE IF THE SPELL IS ON A COOLDOWN*/
            if (spell_cooldown > 0)
                return;
            chosen_spell = true;
        }

        /*WHEN WE HOVER THE SPELL WE GET THE INFORMATION ABOUT IT*/
        #region USED FOR THE TOOLTIP 
        public void OnPointerEnter(PointerEventData eventData)
        {
            GetComponent<Tooltip>().Display(spell.Display_Info());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GetComponent<Tooltip>().Close_Display();
        }
        #endregion



        private void Start()
        {
            icon = GetComponent<Image>();
           
            main_cam = Camera.main;
        }

      
        /*VERIFY IF WE NEED TO SHOW THE COOLDOWN IMAGE OR NO*/
        private void Verify_Cooldown()
        {
            if (spell_cooldown == 0)
                cooldown_image.SetActive(false);
            else
                cooldown_image.SetActive(true);

            cooldown_text.text = spell_cooldown.ToString("0");
        }


        private void Update()
        {
            Verify_Cooldown();


            if (spell != null)
                icon.sprite = spell.icon;

            
            //IF WE CHOSE THIS SPELL CAST IT
            if (chosen_spell)
            {
                //USING RIGHT CLICK TO CANCEL SPELL
                if (Input.GetMouseButtonDown(1))
                {
                    chosen_spell = false;
                }

                ray = main_cam.ScreenPointToRay(Input.mousePosition);

                //USING LEFT CLICK TO CAST SPELL
                if (Input.GetMouseButtonDown(0))
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        /*
                         IF WE ARE PLAYER 1 AND WE ARE HITTING A HERO FROM PLAYER 2
                         */
                        if (attacker.tag == Static_Info.Player_Tag(1))
                        {
                            /* ALLY TARGET SPELLS */
                            if (spell.spell_target == Spell_Target.ally)
                            {
                                if (hit.transform.tag == Static_Info.Player_Tag(1))
                                {
                                    Spell_Cast(hit.transform);
                                }
                            }
                            /* ENEMY TARGET SPELL */
                            else if (spell.spell_target == Spell_Target.enemy)
                            {
                                if (hit.transform.tag == Static_Info.Player_Tag(2))
                                {
                                    Spell_Cast(hit.transform);
                                }
                            }
                        }
                        /*
                         IF WE ARE PLAYER 2 AND WE ARE HITTING A HERO FROM PLAYER 1
                         */
                        else if (attacker.tag == Static_Info.Player_Tag(2))
                        {
                            /* ALLY TARGET SPELLS */
                            if (spell.spell_target == Spell_Target.ally)
                            {
                                if (hit.transform.tag == Static_Info.Player_Tag(2))
                                {
                                    Spell_Cast(hit.transform);
                                }
                            }
                            /* ENEMY TARGET SPELL */
                            else if (spell.spell_target == Spell_Target.enemy)
                            {
                                if (hit.transform.tag == Static_Info.Player_Tag(1))
                                {
                                    Spell_Cast(hit.transform);
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
         FUNCTION TO CAST THE SPELL
         _target THE TARGET THAT THE SPELL WILL BE CAST ON
             */
        private void Spell_Cast(Transform _target)
        {
            Combat_Manager.casting_spells = true; //WE ARE CASTING
            attacker.selected = false; //DESELECTED CURRENT HERO
            chosen_spell = false;// DESELECT CURRENT SPELL
            /*SPAWN CURRENT SPELL*/
            GameObject _copy = Instantiate(spell.spell_go);
            _copy.transform.position = _target.position + spell.spawn_position;
            _copy.GetComponent<Spell_Behaviour>().Set_Projectile(_target,spell);
            attacker.Spell_Cooldown(this); //SEND THE COOLDOWN TIMER
        }
    }
}