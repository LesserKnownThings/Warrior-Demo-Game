using UnityEngine;
using Dungeon.Player;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Dungeon.Core;

namespace Dungeon.Menu
{
    public class Menu_Manager : MonoBehaviour
    {
        [SerializeField] private Transform ui_spawn; //PARENT TO SPAWN ALL HEROES
        [SerializeField] private GameObject hero_ui; //SELECTED HERO UI HOLDER

        [SerializeField] private Transform player1_spawn, player2_spawn; //HERO HOLDERS
        [SerializeField] private TextMeshProUGUI player_turn_text; //SHOWS WHO HAS THE TURN
        [SerializeField] private GameObject tooltip;

        private Hero[] heroes; //ALL HEROES
        private GameObject current_hero; //CURRENT SELECTED HERO


        private bool player_turn; //TRUE IF PLAYER1 FALSE IF PLAYER2 GOES FIRST

        #region HERO SELECTION UI
        [SerializeField] private GameObject selected_hero; //HERO UI FOR THE SELECTED HERO
        private Image icon; //HERO ICON
        private GameObject[] spells; //HERO SPELLS
        private TextMeshProUGUI info; // HERO STATS
        #endregion

        private void Start()
        {
            selected_hero.SetActive(false); // DISABLE THE HERO UI
            heroes = Resources.LoadAll("Heroes", typeof(Hero)).Cast<Hero>().ToArray();

            GameObject.FindWithTag("UI_Manager").GetComponent<UI_Manager>().tooltip = tooltip;
            tooltip.SetActive(false);

            /*INSTANTIATING ALL THE HEROES*/
            foreach (var item in heroes)
            {
                GameObject _copy = Instantiate(hero_ui);
                _copy.transform.SetParent(ui_spawn, false);
                _copy.GetComponent<Hero_UI>().hero = item;
            }
            Set_UI(); //SETTING THE UI FOR FUTURE USE
            Player_Turn(); // VERIFYING WHO IS GOING FIRST
        }

        /*
         VERIFIES WHICH PLAYER GOES FIRST
             */
        private void Player_Turn()
        {
            int player1_random = Random.Range(0, 10);
            int player2_random = Random.Range(0, 10);

            if (player1_random > player2_random)
                player_turn = true;
            else if (player2_random > player1_random)
                player_turn = false;
            else
                Player_Turn();
        }

        /*
         ADDS HEROES TO PLAYER LISTS
             */
        public void Add_Hero()
        {
            if (current_hero == null)
                return;

            /*ADD A HERO TO PLAYER 1*/
            if (player_turn)
            {
                Static_Info.player1_heroes.Add(current_hero.GetComponent<Hero_UI>().hero);
                current_hero.GetComponent<Button>().interactable = false;
                player_turn = !player_turn;
                Add_Hero_UI(player1_spawn);
            }
            /*ADD A HERO TO PLAYER 2*/
            else if (!player_turn)
            {
                Static_Info.player2_heroes.Add(current_hero.GetComponent<Hero_UI>().hero);
                current_hero.GetComponent<Button>().interactable = false;
                player_turn = !player_turn;
                Add_Hero_UI(player2_spawn);
            }

            /*IF ALL THE LISTS ARE FULL START THE FIGHT*/
            if (Static_Info.Load_Fight())
            {
                SceneManager.LoadScene(1);
            }
        }

        /*ADDING THE SELECTED HERO TO THE UI TO SEE WHAT HERO EACH PLAYER HAS*/
        private void Add_Hero_UI(Transform _parent)
        {
            for (int i = 0; i < _parent.childCount; i++)
            {
                if (!_parent.GetChild(i).gameObject.activeSelf)
                {
                    _parent.GetChild(i).gameObject.SetActive(true);
                    _parent.GetChild(i).GetComponent<Image>().sprite = current_hero.GetComponent<Hero_UI>().hero.icon;
                    break;
                }
                    
            }
        }

        /*
         SETTING UP THE PLAYER UI TO SELECT HEROES
             */
        private void Set_UI()
        {
            icon = selected_hero.transform.Find("Icon").GetComponent<Image>();

            Transform _spellHolder = selected_hero.transform.Find("Spell_Holder");
            spells = new GameObject[_spellHolder.childCount];
            for (int i = 0; i < _spellHolder.childCount; i++)
            {
                spells[i] = _spellHolder.GetChild(i).gameObject;
            }
            info = selected_hero.transform.Find("Info").GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            //DISABLE THE HERO UI IF LEFT CLICK IS PRESSED
            if (Input.GetMouseButtonDown(1))
            {
                selected_hero.SetActive(false);
                current_hero = null;
            }

            /*SHOWING THE PLAYER TURN*/
            if (player_turn)
                player_turn_text.text = "Turn: Player 1";
            else
                player_turn_text.text = "Turn: Player 2";
        }

        /*
         SELECTING A HERO FROM THE LIST
         _hero SELECTED HERO
             */
        public void Select(GameObject _hero)
        {
            Hero _copyHero = _hero.GetComponent<Hero_UI>().hero;

            selected_hero.SetActive(true);
            /*DISABLING ALL THE SPELL SLOTS*/
            for (int i = 0; i < spells.Length; i++)
            {
                spells[i].SetActive(false);
            }

            icon.sprite = _copyHero.icon; //SET THE HERO ICON
            info.text = _copyHero.Description(); //SET THE HERO DESCRIPTION

            /*ENABLING ANY SPELL SLOTS IF THE HERO HAS ANY*/
            for (int i = 0; i < _copyHero.spells.Count; i++)
            {
                spells[i].SetActive(true);
                spells[i].GetComponentInChildren<Spell_UI>().spell = _copyHero.spells[i];
            }
            current_hero = _hero; //SET THE CURRENT HERO
        }
    }
}