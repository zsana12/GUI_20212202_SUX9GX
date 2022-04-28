using System;
using System.Collections.Generic;
using System.Windows;
using Ambrus.Model;
using Ambrus.Model.Interfaces;

namespace Ambrus.Logic
{


    /// <summary>
    /// Logic implements the logic interface. It creates the level, adds the play and sets up the waves of enemies.
    /// </summary>
    public class Logic : ILogic
    {
        /// <summary>
        /// Determines how many updates should pass before the new wave of enemy arrives.
        /// </summary>
        private readonly int waitingPeriod = 40;

        /// <summary>
        /// The reference to level object.
        /// </summary>
        private Level level;

        /// <summary>
        /// Which round is playing, determines how many enemies and in what position they spawn.
        /// </summary>
        private int round = 0;

        /// <summary>
        /// Waiting counter. When it is zero the new wave spawns.
        /// </summary>
        private int waiting = 0;

        /// <summary>
        /// Determines if the a new round begins on next update, happens when waiting is zero.
        /// </summary>
        private bool beginNextRound = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logic"/> class.
        /// </summary>
        /// <param name="levelWidth">Width of the level.</param>
        /// <param name="levelHeight">Height of the level.</param>
        public Logic(int levelWidth, int levelHeight)
        {
            this.level = new Level(levelWidth, levelHeight, this.OnPlayerHit, this.OnScoreChanged);
            this.level.AddPlayer(new Point(
                (levelWidth / 2) - (Player.Size.Width / 2),
                levelHeight - 25 - Player.Size.Height));
            this.level.EnemiesCleared += this.PrepareNextRound;
        }

        /// <inheritdoc/>
        public event Action<int, int> PlayerHit;

        /// <inheritdoc/>
        public event Action<int> ScoreChanged;

        /// <inheritdoc/>
        public IEnumerable<IGameModel> Models
        {
            get { return this.level.Entities; }
        }

        /// <inheritdoc/>
        public void AddEnemyWave()
        {
        }

        /// <inheritdoc/>
        public void PlayerMove(Direction direction)
        {
            this.level.PlayerMove(direction);
        }

        /// <inheritdoc/>
        public void PlayerShoot()
        {
            this.level.PlayerShoot();
        }

        /// <inheritdoc/>
        public void Update()
        {
            if (this.beginNextRound && this.waiting == 0)
            {
                this.NextRound();
            }

            this.level.Update();
            this.waiting = Math.Max(0, this.waiting - 1);
        }

        /// <summary>
        /// Invokes player hit event when this callback is called from level.
        /// </summary>
        /// <param name="currentHp">Current health of player.</param>
        /// <param name="maxHp">Max health of player.</param>
        private void OnPlayerHit(int currentHp, int maxHp)
        {
            if (this.PlayerHit != null)
            {
                this.PlayerHit.Invoke(currentHp, maxHp);
            }
        }

        /// <summary>
        /// Invokes score changed event when this callback is called from level.
        /// </summary>
        /// <param name="score">Current score.</param>
        private void OnScoreChanged(int score)
        {
            if (this.ScoreChanged != null)
            {
                this.ScoreChanged.Invoke(score);
            }
        }

        /// <summary>
        /// Sets variables indicating that a new round starts on next update.
        /// </summary>
        private void PrepareNextRound()
        {
            this.waiting = this.waitingPeriod;
            this.beginNextRound = true;
        }

        /// <summary>
        /// Adds enemies depending on which kind of round comes next.
        /// </summary>
        private void NextRound()
        {
            if (this.round % 2 == 0)
            {
                this.level.AddEnemy(new Point((this.level.Width / 2) - (Enemy.Size.Width / 2), 100), Enemy.StrategyType.Diamond_Right);
                this.level.AddEnemy(new Point(100, 25), Enemy.StrategyType.Right_Left);
                this.level.AddEnemy(new Point(this.level.Width - Enemy.Size.Width - 100, 25), Enemy.StrategyType.Left_Right);
            }
            else
            {
                this.level.AddEnemy(new Point((this.level.Width / 2) - (Enemy.Size.Width / 2) - Enemy.Size.Width, 200), Enemy.StrategyType.Diamond_Right);
                this.level.AddEnemy(new Point((this.level.Width / 2) - (Enemy.Size.Width / 2) + Enemy.Size.Width, 200), Enemy.StrategyType.Diamond_Left);
            }

            ++this.round;
            this.beginNextRound = false;
        }
    }
}