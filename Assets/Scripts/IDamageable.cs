namespace Interfaces
{
    public interface IDamageable
    {
        public int Health { get; set; }
        public void Die();
        public void TakeDamage(int damage);
    }
}