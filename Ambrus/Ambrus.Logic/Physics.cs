using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Ambrus.Model.Entities;

namespace Ambrus.Logic
{
    

    /// <summary>
    /// Updates the position of entities based on their movement direction and checks collisions.
    /// </summary>
    internal class Physics
    {
        /// <summary>
        /// Width of the level.
        /// </summary>
        private int levelWidth;

        /// <summary>
        /// Height of the level.
        /// </summary>
        private int levelHeight;

        /// <summary>
        /// Reference to the list of entities from level.
        /// </summary>
        private IEnumerable<GameEntity> entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="Physics"/> class.
        /// </summary>
        /// <param name="levelWidth">Width of the level.</param>
        /// <param name="levelHeight">Height of the level.</param>
        /// <param name="entities">Entities in the level.</param>
        public Physics(int levelWidth, int levelHeight, IEnumerable<GameEntity> entities)
        {
            this.levelWidth = levelWidth;
            this.levelHeight = levelHeight;
            this.entities = entities;
        }

        /// <summary>
        /// Updates the position of entities based on their movement direction and checks collisions.
        /// </summary>
        public void Update()
        {
            this.MoveEntities();
            this.CheckCollisions();
        }

        /// <summary>
        /// Update the position of each entity based on its move direction and reset it.
        /// </summary>
        private void MoveEntities()
        {
            foreach (var entity in this.entities)
            {
                this.Move(entity, entity.MoveDirection * entity.MoveSpeed);
                entity.MoveDirection = new Vector(0, 0);
            }
        }

        /// <summary>
        /// Check collision between rigid and non-rigid entities, i.e. between projectiles and ships.
        /// </summary>
        private void CheckCollisions()
        {
            for (int i = 0; i < this.entities.Count(); ++i)
            {
                for (int j = i + 1; j < this.entities.Count(); ++j)
                {
                    var x = this.entities.ElementAt(i);
                    var y = this.entities.ElementAt(j);
                    if (x.Area.IntersectsWith(y.Area) && (x.IsRigid ^ y.IsRigid) && x.Team != y.Team)
                    {
                        this.CollideWith(x, y);
                        this.CollideWith(y, x);
                    }
                }
            }
        }

        /// <summary>
        /// Two entities collide and x deals damage to y.
        /// </summary>
        /// <param name="x">An entity dealing damage.</param>
        /// <param name="y">An entity taking damage.</param>
        private void CollideWith(GameEntity x, GameEntity y)
        {
            if (x.DieOnCollision)
            {
                x.Health = 0;
            }

            y.TakeDamage(x.DamageCollision);
        }

        /// <summary>
        /// Move an entity and make sure it stays in the level if it's rigid.
        /// </summary>
        /// <param name="entity">An entity.</param>
        /// <param name="dir">Move direction.</param>
        private void Move(GameEntity entity, Vector dir)
        {
            if (!entity.IsRigid)
            {
                entity.Move(dir);
            }
            else
            {
                Vector d = new Vector(dir.X, dir.Y);
                var x1 = entity.Area.Left + dir.X;
                var x2 = entity.Area.Right + dir.X;
                var y1 = entity.Area.Top + dir.Y;
                var y2 = entity.Area.Bottom + dir.Y;

                if (x1 < 0)
                {
                    d.X = -entity.Area.Left;
                }
                else if (x2 >= this.levelWidth)
                {
                    d.X = this.levelWidth - entity.Area.Right - 1;
                }

                if (y1 < 0)
                {
                    d.Y = -entity.Area.Top;
                }
                else if (y2 >= this.levelHeight)
                {
                    d.Y = this.levelHeight - entity.Area.Bottom - 1;
                }

                entity.Move(d);
            }
        }
    }
}
