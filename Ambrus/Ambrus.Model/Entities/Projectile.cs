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
    public class Projectile : GameEntity, IVisitable
    {
        private static System.Windows.Size size;

        public Projectile(Rect area, SpriteType sprite, Vector facingDir, Enums.Team team)
            : base(area, sprite, team, 1)
        {
            this.FacingDirection = facingDir;

            if (team == Enums.Team.Player)
            {
                Size = new System.Windows.Size(17, 33);
            }
            else
            {
                Size = new System.Windows.Size(13, 33);
            }

            this.MoveSpeed = 8;

            this.IsRigid = false;
            this.DieOnCollision = true;
            this.DamageCollision = 1;
        }

        public static System.Windows.Size Size
        {
            get { return new System.Windows.Size(20, 39); }
            set { size = value; }
        }

        public Vector FacingDirection { get; private set; }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
