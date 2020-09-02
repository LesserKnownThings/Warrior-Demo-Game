using TMPro;
using UnityEngine;

namespace Dungeon.Core
{
    public class UI_Manager : MonoBehaviour
    {
        /*USED FOR THE TOOLTIP*/
        public GameObject tooltip; //THE TOOLTIP
        private TextMeshProUGUI tooltip_text; // THE INFO TEXT

        
       
        void Awake()
        {
            /*CHECK IF WE DO NOT HAVE A MANAGER, IF WE DO DESTROY IT*/
            GameObject[] objs = GameObject.FindGameObjectsWithTag("UI_Manager");

            if (objs.Length > 1)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);         
        }


        private void Update()
        {
            if (tooltip_text == null)
                tooltip_text = tooltip.GetComponentInChildren<TextMeshProUGUI>();

            /*FOLLOW THE MOUSE*/
            if (tooltip!=null && tooltip.activeSelf)
                tooltip.GetComponent<RectTransform>().position = Input.mousePosition;
        }

        /*USED TO DISPLAY OR CLOSE THE TOOLTIP WITH INFO*/
        #region TOOLTIP DISPLAY
        public void Display_Info(string _info)
        {
            if (tooltip.activeSelf)
                return;

            tooltip.SetActive(true);
            tooltip_text.text = _info;
        }

        public void Close_Display()
        {
            tooltip.SetActive(false);
            tooltip_text.text = "";
        }
        #endregion
    }
}