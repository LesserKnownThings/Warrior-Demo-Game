using UnityEngine;

namespace Dungeon.Core
{
    public class Platform : MonoBehaviour
    {
        [HideInInspector] public int rang = 0; //CHECK WHAT RANG THE PLATFORM HAS FOR MOVEMENT
        [HideInInspector] public bool taken; //CHECK IF THE PLATFORM IS OOCUPIED
    }
}