namespace Dungeon.Player
{
    public class InGame_Hero
    {
        public float current_health;
        public float current_damage;
        public float current_armor;

        public Hero hero;
        public InGame_Hero(float _health, float _damage, float _armor, Hero _hero)
        {
            current_health = _health;
            current_armor = _armor;
            current_damage = _damage;
            hero = _hero;
        }
    }
}