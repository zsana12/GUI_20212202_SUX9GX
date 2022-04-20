using Ambrus.Model.Entities;
using Ambrus.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Ambrus.Model
{
    public class Player : GameEntity, IVisitable
    {
        public static readonly Size Size = new Size(113, 116);

        private Action<int> playerHit;

        private Point centerFront;
        
        public Player(Rect area, int health, Action<int> playerHit)
            : base(area, SpriteType.Player, Enums.Team.Player, health)
        {
            this.centerFront = new Point((area.X + (area.Width / 2)), area.Y);
            this.playerHit = playerHit;
            this.ReloadDuration = 10;
            this.ReloadTime = 0;
            this.MoveSpeed = 2;

            this.IsRigid = true;
            this.DieOnCollision = false;
            this.DamageCollision = 0;
        }

        public int ReloadDuration { get; set; }

        public int ReloadTime { get; set; }

        public Point CenterFront
        {
            get { return this.centerFront; }
            private set { this.centerFront = value; }
        }

        public override void Move(Vector dir)
        {
            base.Move(dir);
            this.centerFront += dir;
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
            this.playerHit.Invoke(this.Health);
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}

