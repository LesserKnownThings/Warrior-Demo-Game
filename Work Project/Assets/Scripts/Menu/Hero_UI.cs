using UnityEngine.UI;
using UnityEngine;
using Dungeon.Player;
using UnityEngine.EventSystems;

namespace Dungeon.Menu
{
    public class Hero_UI : MonoBehaviour, IPointerClickHandler
    {
        [HideInInspector] public Hero hero;

        public void OnPointerClick(PointerEventData eventData)
        {
            GameObject.FindWithTag("Manager").GetComponent<Menu_Manager>().Select(gameObject);
        }

        private void Start()
        {
            transform.Find("Icon").GetComponent<Image>().sprite = hero.icon;
        }
    }
}