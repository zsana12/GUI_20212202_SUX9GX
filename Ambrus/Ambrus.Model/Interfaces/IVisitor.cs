using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrus.Model.Interfaces
{
    public interface IVisitor
    {
        void Visit(Player player);

        void Visit(Enemy enemy);

        void Visit(Projectile projectile);
    }
}
