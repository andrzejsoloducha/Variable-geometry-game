using UnityEngine;

namespace Interfaces
{
    public interface IMovable
    {
        void Move(float direction);
        public int MoveSpeed { get; set; }
    }
}