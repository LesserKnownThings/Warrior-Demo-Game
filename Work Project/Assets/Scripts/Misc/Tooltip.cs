using UnityEngine;

namespace Dungeon.Core
{
    public class Tooltip : MonoBehaviour
    {
        private UI_Manager manager;

        private void Start()
        {
            manager = GameObject.FindWithTag("UI_Manager").GetComponent<UI_Manager>();
        }

        public void Display(string _info)
        {
            manager.Display_Info(_info);
        }

        public void Close_Display()
        {
            manager.Close_Display();
        }
     
    }
}