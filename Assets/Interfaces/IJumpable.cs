namespace Interfaces
{
    public interface IJumpable
    {
        public float JumpForce { get; set; }
        public void Jump();
    }
}