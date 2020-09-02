using System.Collections.Generic;
using UnityEngine;
using Dungeon.Player;
using TMPro;
using UnityEngine.UI;
using Dungeon.Menu;
using UnityEngine.SceneManagement;

namespace Dungeon.Core
{
    public class Combat_Manager : MonoBehaviour
    {
        [SerializeField] private GameObject player_go; //GAMEOBJECT USED FOR PLAYER

        #region HERO UI
        [SerializeField] private GameObject hero_ui; //THE UI HOLDER
        [SerializeField] private TextMeshProUGUI hero_info; //HERO DESCRIPTION HOLDER
        [SerializeField] private Image hero_icon; // HERO ICON
        [SerializeField] private GameObject player_won; //UI FOR END GAME
        [SerializeField] private GameObject tutorial; //TUTORIAL CANVAS
        private GameObject[] hero_spells; //4 ICONS FOR THE HERO SPELLS
        #endregion

        /*USED TO VERIFY WHAT PLAYER IS GOING
         * TRUE IF PLAYER 1 
         * FALSE IF PLAYER 2
         * */
        public static bool player_turn = true;
        public static bool casting_spells;

        /*
         LIST OF HEROES OF EACH PLAYER
             */
        public static List<Player_Behaviour> player1_heroes = new List<Player_Behaviour>();
        public static List<Player_Behaviour> player2_heroes = new List<Player_Behaviour>();
        

        #region DICE ROLLING
        private int player1_dice, player2_dice;
        #endregion

        private void Set_UIStart()
        {
            Transform _spellHolder = hero_ui.transform.Find("Spell_Holder");
            hero_spells = new GameObject[_spellHolder.childCount];
            for (int i = 0; i < _spellHolder.childCount; i++)
            {
                hero_spells[i] = _spellHolder.GetChild(i).gameObject;
                hero_spells[i].SetActive(false);
            }
        }


        private void Start()
        {
            Spawn_Player("Player1", 0);
            Spawn_Player("Player2", 3);
            Decide_Player();
            Set_UIStart();
            Decide_Hero();
        }

        /*
         DECIDES WHAT PLAYER GOES FIRST
             */
        #region PLAYER DECISION
        private void Decide_Player()
        {
            Dice_Roll();
            if (player1_dice > player2_dice)
                player_turn = true;
            else if (player1_dice < player2_dice)
                player_turn = false;
            else if (player1_dice == player2_dice)
                Decide_Player();
        }

        private void Dice_Roll()
        {
            player1_dice = Random.Range(0, 100);
            player2_dice = Random.Range(0, 100);
        }
        #endregion

      

        /*
         VERIFIES WHAT HERO GOES NEXT
             */
        #region HERO DECISION
        public void Decide_Hero()
        {
            /*
             VERIFY WHAT PLAYER HAS THE TURN
             */
            if (player1_heroes.Count > 0 && player2_heroes.Count > 0)
            {
                if (player_turn)
                {
                    Player(player1_heroes);
                }
                else if (!player_turn)
                {
                    Player(player2_heroes);
                }
            }
            else
            {
                player_won.SetActive(true);
                if (player1_heroes.Count <= 0)
                    player_won.GetComponentInChildren<TextMeshProUGUI>().text = "Player 2 won!";
                else if (player2_heroes.Count <= 0)
                    player_won.GetComponentInChildren<TextMeshProUGUI>().text = "Player 1 won!";
            }

        }
        #endregion

        /*QUIT GAME*/
        public void Game_Ended()
        {
            Application.Quit();
        }
        /*
         SELECTS A HERO THAT HAS NOT MOVED YET
         IF ALL HERO MOVED RESET THE MOVE AND SELECT RANDOM HERO

         _heroes ==> THE LIST OF THE PLAYER HEROES
             */
        private void Player(List<Player_Behaviour> _heroes)
        {
            Hero _hero = null;
            for (int i = 0; i < _heroes.Count; i++)
            {
                if (!_heroes[i].moved)
                {
                    _heroes[i].selected = true;
                    _heroes[i].moved = true;
                    _hero = _heroes[i].hero;
                    UI_Display(_hero, _heroes[i]);
                    return;
                }
            }
            for (int i = 0; i < _heroes.Count; i++)
            {
                _heroes[i].moved = false;
            }
            int _random = Random.Range(0, _heroes.Count);
            _heroes[_random].selected = true;
            _hero = _heroes[_random].hero;
            UI_Display(_hero,_heroes[_random]);
        }

        /*DISPLAY THE UI FOR THE CURRENT SELECTED HERO
         _attacker => THE CURRENT SELECTED Player_Behaviour from the gameobject
         _hero => the current selected hero for the stats and icon
             */
        private void UI_Display(Hero _hero, Player_Behaviour _attacker)
        {
            hero_icon.sprite = _hero.icon;
            hero_info.text = _hero.Description();

            /*SET ADD THE HERO SPELLS TO THE Player_Behaviour*/
            if (_attacker.hero_spells.Count <= 0)
                foreach (var item in _hero.spells)
                {
                    _attacker.hero_spells.Add(new Hero_Spells(item));
                }

            /*DISABLING THE HERO SPELLS DISPLAY*/
            for (int i = 0; i < hero_spells.Length; i++)
            {                
                hero_spells[i].GetComponentInChildren<Spell_UI>().spell = null;
                hero_spells[i].SetActive(false);
            }            

            /*ENABLE ANY SPELL DISPLAY THAT THE HERO HAS (IF THE PLAYER HAS ANY)*/
            for (int i = 0; i < _hero.spells.Count; i++)
            {
                Spell_UI _spellUI = hero_spells[i].GetComponentInChildren<Spell_UI>();
                hero_spells[i].SetActive(true);
                _spellUI.spell = _hero.spells[i];
                _spellUI.attacker = _attacker;

                if (_attacker.hero_spells[i].current_cooldown > 0)
                    _attacker.hero_spells[i].current_cooldown--;

                _spellUI.spell_cooldown = _attacker.hero_spells[i].current_cooldown;
            }            
        }


        /*
         SPAWN PLAYER UNITS IN POSITION
         _tag ==> unit tags to verify in which zone to spawn
         _rang ==> the platform rang in the dictionary
             */
        private void Spawn_Player(string _tag, int _rang)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject _platform = Scene_Management.platform_rag[_rang][i]; //CURRENT PLATFORM TO SPAWN
                GameObject _copy = Instantiate(player_go);                    
                _copy.transform.position = _platform.transform.position + new Vector3(0,1,0); //CHANGE TRANSFORM POSITION
                // ASSIGN CURRENT PLATFORM
                _copy.GetComponent<Player_Behaviour>().current_platform = _platform.transform; 
                //THE ASSIGNED PLATFORM IS OCCUPIED (TAKEN => TRUE)
                Scene_Management.platform_rag[_rang][i].GetComponent<Platform>().taken = true;
                _copy.tag = _tag; //ASSIGN PLAYER TAG TO HERO

                /*
                 ASSIGN TO APROPRIATE LIST
                 */
                if(_copy.tag == Static_Info.Player_Tag(1))
                {
                    _copy.GetComponent<Player_Behaviour>().hero = Static_Info.player1_heroes[i];

                    _copy.GetComponent<Renderer>().material.color = _copy.GetComponent<Player_Behaviour>().hero.color;

                    player1_heroes.Add(_copy.GetComponent<Player_Behaviour>());
                }
                else if (_copy.tag == Static_Info.Player_Tag(2))
                {
                    _copy.GetComponent<Player_Behaviour>().hero = Static_Info.player2_heroes[i];

                    _copy.GetComponent<Renderer>().material.color = _copy.GetComponent<Player_Behaviour>().hero.color;

                    _copy.transform.rotation = Quaternion.Euler(_copy.transform.rotation.x,-180, _copy.transform.rotation.z);
                    player2_heroes.Add(_copy.GetComponent<Player_Behaviour>());
                }
            }
        }    

        /*CLOSE THE TUTORIAL WINDOW AT THE BEGGINING OF THE GAME*/
        public void Close_Tutorial()
        {
            tutorial.SetActive(false);
        }

    }
}