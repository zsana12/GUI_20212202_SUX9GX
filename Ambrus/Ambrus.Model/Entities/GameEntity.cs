using Ambrus.Model.Enums;
using Ambrus.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ambrus.Model.Entities
{
    public abstract class GameEntity : FrameworkElement, IGameModel, IVisitable
    {
        private Rect area;

        public GameEntity(Rect area, SpriteType sprite, Team team, int health)

        {
            this.area = area;
            this.Sprite = sprite;
            this.Team = team;
            this.Health = health;
        }

        public Rect Area
        {
            get { return this.area; }
            private set { this.area = value; }
        }
        public int Health { get; set; }

        public SpriteType Sprite { get; set; }

        public abstract void Accept(IVisitor visitor);

        public Team Team { get; set; }

        public Vector MoveDirection { get; set; }

        public int MoveSpeed { get; set; }

        public int DamageCollision { get; set; }

        public bool DieOnCollision { get; set; }

        public bool IsRigid { get; set; }

        Rect IGameModel.Area => throw new NotImplementedException();

        public virtual void Move(Vector dir)
        {
            this.area.Offset(dir);
        }

        public virtual void TakeDamage(int amount)
        {
            this.Health -= amount;
        }

    }
}
