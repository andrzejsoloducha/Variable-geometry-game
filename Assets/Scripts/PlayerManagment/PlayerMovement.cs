using Interfaces;
using UnityEngine;
using Weapons;

namespace PlayerManagment
{
    public class PlayerMovement
    {
        private Player player;
        private IShootable shootable;
        private IDrillable drillable;
        private IBuildable buildable;
        private GameManager gameManager;


        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                TryJump();
            }

            float horizontalInput = Input.GetAxis("Horizontal");
            Vector2 direction = new Vector2(horizontalInput, 0f);
            direction.Normalize();
            TryMove(direction);

        }
        public PlayerMovement(Player player, IShootable shootable, IDrillable drillable, IBuildable buildable)
        {
            this.player = player;
            this.shootable = shootable;
            this.drillable = drillable;
            this.buildable = buildable;
        }

        private void TryMove(Vector2 direction)
        {
            player.Move(direction);
        }

        private void TryJump()
        {
            if (IsGrounded())
            {
                player.Jump();
            }
        }
    
        // ReSharper disable Unity.PerformanceAnalysis
        private bool IsGrounded()
        {
            var playerLayer = LayerMask.NameToLayer("Player");
            Vector2 position = player.transform.position;
            var size = player.GetComponent<BoxCollider2D>().size;
            var offset = new Vector2(0, -size.y / 2f);
            var radius = 0.1f;

            var groundLayer = LayerMask.GetMask("Ground");
            bool grounded = Physics2D.OverlapCircle(position + offset, radius, groundLayer);
            if (!grounded)
            {
                var hit = Physics2D.Raycast(position + offset, Vector2.down, size.y / 2f + 0.1f, playerLayer);
                if (hit.collider != null)
                {
                    Debug.Log("Hit objects: " + hit.collider.gameObject.name);
                    grounded = true;
                }
            }

            return grounded;
        }

        public void TryShoot(Bullet bullet)
        {
            shootable.Shoot(bullet);
            CallForNextTurn();
        }

        public void TryDrill(Terrain terrain)
        {
            drillable.Drill(terrain);
            CallForNextTurn();
        }

        public void TryBuild(Terrain terrain)
        {
            buildable.Build(terrain);
            CallForNextTurn();
        }

        private void CallForNextTurn()
        {
            gameManager.NextPlayerTurnProcedure();
        }
    }
}