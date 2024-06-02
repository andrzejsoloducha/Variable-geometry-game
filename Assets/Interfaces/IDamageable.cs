namespace Interfaces
{
    public interface IDamageable
    {
        public int Health { get; set; }
        public void OnPlayerDeath(Player player);
        public void TakeDamage(int damage);
    }
}