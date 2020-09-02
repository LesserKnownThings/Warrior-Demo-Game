using System.Collections.Generic;
using Dungeon.Player;
public static class Static_Info
{
    /*
     PLAYER TAGS USED TO VERIFY WHICH PLAYER CAN BE SELECTED
         */
    private static string player1_tag = "Player1", player2_tag = "Player2";

    public static List<Hero> player1_heroes = new List<Hero>();
    public static List<Hero> player2_heroes = new List<Hero>();

    public static string Player_Tag (int player_number)
    {
        switch (player_number)
        {
            case 1:
                return player1_tag;
            case 2:
                return player2_tag;
            default:
                return null;
        }

    }

    public static bool Load_Fight()
    {
        return player1_heroes.Count == 4 && player2_heroes.Count == 4;
    }
}
