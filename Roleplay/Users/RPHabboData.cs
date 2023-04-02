namespace Plus.Roleplay.Users;
public class RPHabboData
{
    public RPHabboData(int id, int level, int health, int maxHealth, int stamina, int maxStamina, int aggression)
    {
        Id = id;
        Level = level;
        Health = health;
        MaxHealth = maxHealth;
        Stamina = stamina;
        MaxStamina = maxStamina;
        Aggression = aggression;
    }

    public int Id { get; set; }
    public int Level { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Stamina { get; set; }
    public int MaxStamina { get; set; }
    public int Aggression { get; set; }
}