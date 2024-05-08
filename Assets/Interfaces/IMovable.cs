using UnityEngine;

namespace Interfaces
{
    public interface IMovable
    {
        void Move(Vector2 direction);
        public int MoveRange { get; set; }
    }
}