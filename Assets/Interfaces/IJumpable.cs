namespace Interfaces
{
    public interface IJumpable
    {
        public float JumpForce { get; }
        public void TryJump();
    }
}