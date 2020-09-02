namespace Dungeon.Player
{
    public class Hero_Spells
    {
        public float current_cooldown = 0;
        public Spells spell;

        public Hero_Spells(Spells _spell)
        {
            spell = _spell;
        }

        public void Set_Cooldown()
        {
            current_cooldown = spell.cooldown;
        }
    }
}