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
        /// <summary>
        /// Event that fires when the player is hit. It passes the current and max health of player.
        /// </summary>
        event Action<int, int> PlayerHit;

        /// <summary>
        /// Event that fires when the score is changed. It passes the new score.
        /// </summary>
        event Action<int> ScoreChanged;

        /// <summary>
        /// Gets game entity models.
        /// </summary>
        IEnumerable<IGameModel> Models { get; }

        /// <summary>
        /// Add an enemy wave which will come after the previous was cleared.
        /// </summary>
        void AddEnemyWave();

        /// <summary>
        /// Update all entities and AI in the level.
        /// </summary>
        void Update();

        /// <summary>
        /// Move player in a direction.
        /// </summary>
        /// <param name="direction">Direction.</param>
        void PlayerMove(Direction direction);

        /// <summary>
        /// Player shoots a missile.
        /// </summary>
        void PlayerShoot();
    }
}

