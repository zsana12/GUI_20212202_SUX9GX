using System;
using System.Collections.Generic;
using System.Windows;
using Ambrus.Model;
using Ambrus.Model.Interfaces;

namespace Ambrus.Logic
{
    public class Logic : ILogic
    {
       
        private readonly int waitingPeriod = 40;

      
        private Level level;

        
        private int round = 0;

        
        private int waiting = 0;

       
        private bool beginNextRound = true;

        public Logic(int levelWidth, int levelHeight)
        {
            this.level = new Level(levelWidth, levelHeight, this.OnPlayerHit, this.OnScoreChanged);
            this.level.AddPlayer(new Point(
                (levelWidth / 4) - (Player.Size.Width / 4),
                levelHeight - 30 - Player.Size.Height));
            this.level.EnemiesCleared += this.PrepareNextRound;
        }

       
        public event Action<int, int> PlayerHit;
      
        public event Action<int> ScoreChanged;

        public IEnumerable<IGameModel> Models
        {
            get { return this.level.Entities; }
        }

      
        public void AddEnemyWave()
        {
        }

        
        public void PlayerMove(Direction direction)
        {
            this.level.PlayerMove(direction);
        }

       
        public void PlayerShoot()
        {
            this.level.PlayerShoot();
        }

        public void Update()
        {
            if (this.beginNextRound && this.waiting == 0)
            {
                this.NextRound();
            }

            this.level.Update();
            this.waiting = Math.Max(0, this.waiting - 1);
        }

        private void OnPlayerHit(int currentHp, int maxHp)
        {
            if (this.PlayerHit != null)
            {
                this.PlayerHit.Invoke(currentHp, maxHp);
            }
        }

        
        private void OnScoreChanged(int score)
        {
            if (this.ScoreChanged != null)
            {
                this.ScoreChanged.Invoke(score);
            }
        }

        
        private void PrepareNextRound()
        {
            this.waiting = this.waitingPeriod;
            this.beginNextRound = true;
        }

        private void NextRound()
        {
            if (this.round % 2 == 0)
            {
                this.level.AddEnemy(new Point((this.level.Width / 2) - (Enemy.Size.Width / 4), 100), Enemy.StrategyType.Diamond_Right);
                this.level.AddEnemy(new Point(100, 25), Enemy.StrategyType.Right_Left);
                this.level.AddEnemy(new Point(this.level.Width - Enemy.Size.Width - 100, 25), Enemy.StrategyType.Left_Right);
            }
            else
            {
                this.level.AddEnemy(new Point((this.level.Width / 2) - (Enemy.Size.Width / 4) - Enemy.Size.Width, 200), Enemy.StrategyType.Diamond_Right);
                this.level.AddEnemy(new Point((this.level.Width / 2) - (Enemy.Size.Width / 4) + Enemy.Size.Width, 200), Enemy.StrategyType.Diamond_Left);
            }

            ++this.round;
            this.beginNextRound = false;
        }
    }
}