using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Ambrus.Model.Entities;

namespace Ambrus.Logic
{


   
    internal class Physics
    {
       
        private int levelWidth;

        
        private int levelHeight;

        private IEnumerable<GameEntity> entities;

        
        public Physics(int levelWidth, int levelHeight, IEnumerable<GameEntity> entities)
        {
            this.levelWidth = levelWidth;
            this.levelHeight = levelHeight;
            this.entities = entities;
        }

        
        public void Update()
        {
            this.MoveEntities();
            this.CheckCollisions();
        }

       
        private void MoveEntities()
        {
            foreach (var entity in this.entities)
            {
                this.Move(entity, entity.MoveDirection * entity.MoveSpeed);
                entity.MoveDirection = new Vector(0, 0);
            }
        }

        
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

        
        private void CollideWith(GameEntity x, GameEntity y)
        {
            if (x.DieOnCollision)
            {
                x.Health = 0;
            }

            y.TakeDamage(x.DamageCollision);
        }

        
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