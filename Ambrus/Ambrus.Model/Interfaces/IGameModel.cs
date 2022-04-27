using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ambrus.Model.Interfaces
{

    public interface IGameModel
    {
        Rect Area { get; }

        int Health { get; }

        SpriteType Sprite { get; }
    }
}
