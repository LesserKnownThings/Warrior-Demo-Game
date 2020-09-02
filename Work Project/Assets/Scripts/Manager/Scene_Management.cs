using System.Collections.Generic;
using UnityEngine;

namespace Dungeon.Core
{
    public class Scene_Management : MonoBehaviour
    {
        public static Dictionary<int, List<GameObject>> platform_rag = new Dictionary<int, List<GameObject>>(); //USED TO SPAWN PLATFORMS AND TRACK THE RANG FOR THEM

        [SerializeField] private GameObject tooltip;
        [SerializeField] private GameObject platform; // PLATFORM TO SPAWN FOR MOVEMENT
        [SerializeField] private Transform starting_point; //STARTING POINT FOR THE PLATFORMS


        private void Awake()
        {
            platform_rag.Clear();
            Create_Grid();
        }

        private void Start()
        {
            GameObject.FindWithTag("UI_Manager").GetComponent<UI_Manager>().tooltip = tooltip;
            tooltip.SetActive(false);
        }

        /*CREATE A 4X4 GRID OF PLATFORMS FOR MOVEMENT*/
        private void Create_Grid()
        {
            float x_axis = 0, z_axis = 0;
            int rang = 0;
            for (int i = 0; i < 4; i++)
            {
                List<GameObject> rang_platforms = new List<GameObject>();
                for (int j = 0; j < 4; j++)
                {
                    GameObject _copy = Instantiate(platform, new Vector3(starting_point.position.x + x_axis, starting_point.position.y, starting_point.position.z + z_axis), Quaternion.identity);
                    _copy.GetComponent<Platform>().rang = rang;
                    rang_platforms.Add(_copy);
                    x_axis += platform.transform.localScale.x + 1;
                }
                z_axis += platform.transform.localScale.z + 1;
                platform_rag.Add(rang, rang_platforms);
                rang++;
                x_axis = 0;
            }
        }

    }
}