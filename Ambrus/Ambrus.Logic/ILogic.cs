using Ambrus.Model;
using Ambrus.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrus.Logic
{
    public interface ILogic
    {
      
        event Action<int, int> PlayerHit;

        event Action<int> ScoreChanged;

      
        IEnumerable<IGameModel> Models { get; }

        
        void AddEnemyWave();

        void Update();

        void PlayerMove(Direction direction);

       
        void PlayerShoot();
    }
}