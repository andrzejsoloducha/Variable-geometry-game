using UnityEngine;

namespace Interfaces
{
    public interface IMovable
    {
        void TryMove();
        public float MoveSpeed { get; set; }
    }
}