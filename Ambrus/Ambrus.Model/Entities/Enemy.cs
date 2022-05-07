using Ambrus.Model.Entities;
using Ambrus.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ambrus.Model
{
    public class Enemy : GameEntity, IVisitable
    {
        private System.Windows.Point centerFront;

        public Enemy(Rect area, int health, StrategyType strategy)
            : base(area, SpriteType.Enemy, Enums.Team.Enemy, health)
        {
            this.centerFront = new System.Windows.Point(area.X + (area.Width / 2), area.Bottom);
            this.Strategy = strategy;
            this.ReloadDuration = 35;
            this.ReloadTime = 0;
            this.MoveSpeed = 2;

            this.IsRigid = true;
            this.DieOnCollision = false;
            this.DamageCollision = 0;
        }

        public enum StrategyType
        {
            Right_Left,
            Left_Right,
            Diamond_Right,
            Diamond_Left,
        }

        public System.Windows.Point CenterFront
        {
            get { return this.centerFront; }
            private set { this.centerFront = value; }
        }

        public StrategyType Strategy { get; set; }

        public override void Move(Vector dir)
        {
            base.Move(dir);
            this.centerFront += dir;
        }

        public static System.Windows.Size Size
        {
            get { return new System.Windows.Size(100, 130); }
        }


        public int ReloadDuration { get; set; }


        public int ReloadTime { get; set; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
