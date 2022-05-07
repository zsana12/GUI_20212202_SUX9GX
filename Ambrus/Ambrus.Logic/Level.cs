using System;
using System.Collections.Generic;
using System.Windows;
using Ambrus.Model;
using Ambrus.Model.Entities;
using Ambrus.Model.Enums;
using Ambrus.Model.Interfaces;

namespace Ambrus.Logic
{
    public class Level : IVisitor
    {
      
        public static readonly int PlayerMaxHP = 7;

       
        private const int EnemyHP = 2;

       
        private readonly int playerMoveSpeed = 4;

        private Action<int, int> playerHit;

      
        private Action<int> scoreChanged;

        private Physics physics;

        private List<GameEntity> entities = new List<GameEntity>();

        private List<GameEntity> entitiesToAdd = new List<GameEntity>();

        
        private Dictionary<GameEntity, AI> ais = new Dictionary<GameEntity, AI>();

        
        private Player player = null;

        
        private int enemiesCount = 0;

        
        private int score = 0;

       
        public Level(int width, int height, Action<int, int> playerHit, Action<int> scoreChanged)
        {
            this.Width = width;
            this.Height = height;
            this.playerHit = playerHit;
            this.scoreChanged = scoreChanged;
            this.physics = new Physics(width, height, this.entities);
        }

        
        public event Action EnemiesCleared;

        
        public int Width { get; private set; }

        public int Height { get; private set; }

        public IEnumerable<GameEntity> Entities
        {
            get { return this.entities; }
        }

        public Player AddPlayer(Point position)
        {
            this.player = new Player(new Rect(position, Player.Size), PlayerMaxHP, this.PlayerHit);
            this.entitiesToAdd.Add(this.player);
            return this.player;
        }

        
        public Enemy AddEnemy(Point position, Enemy.StrategyType strategy, int hp = EnemyHP)
        {
            var enemy = new Enemy(new Rect(position, Enemy.Size), hp, strategy);
            this.entitiesToAdd.Add(enemy);
            this.ais.Add(enemy, new AI(enemy));
            ++this.enemiesCount;
            return enemy;
        }

        
        public Projectile AddProjectile(Point position, Vector dir, Team team)
        {
            SpriteType sprite = team == Team.Player ? SpriteType.MissilePlayer : SpriteType.MissileEnemy;
            var projectile = new Projectile(new Rect(position, Projectile.Size), sprite, dir, team);
            this.entitiesToAdd.Add(projectile);
            return projectile;
        }

        public void PlayerMove(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    this.player.MoveDirection += new Vector(0, -this.playerMoveSpeed);
                    break;
                case Direction.Right:
                    this.player.MoveDirection += new Vector(this.playerMoveSpeed, 0);
                    break;
                case Direction.Down:
                    this.player.MoveDirection += new Vector(0, this.playerMoveSpeed);
                    break;
                case Direction.Left:
                    this.player.MoveDirection += new Vector(-this.playerMoveSpeed, 0);
                    break;
                default:
                    break;
            }
        }

      
        public void PlayerShoot()
        {
            if (this.player.ReloadTime == 0)
            {
                this.player.ReloadTime = this.player.ReloadDuration;
                this.AddProjectile(
                    new Point(this.player.CenterFront.X - (Projectile.Size.Width / 2), this.player.CenterFront.Y),
                    new Vector(0, -1),
                    Team.Player);
            }
        }

        
        public void Update()
        {
            bool containsEnemies = this.enemiesCount > 0;

            this.AddNewEntities();

            foreach (var entity in this.entities)
            {
                entity.Accept(this);
            }

            this.physics.Update();

            this.RemoveDeadEntities();

            if (containsEnemies && this.enemiesCount == 0)
            {
                if (this.EnemiesCleared != null)
                {
                    this.EnemiesCleared?.Invoke();
                }
            }
        }

        
        public void Visit(Player player)
        {
            player.ReloadTime = Math.Max(0, player.ReloadTime - 1);
        }

        
        public void Visit(Enemy enemy)
        {
            enemy.ReloadTime = Math.Max(0, enemy.ReloadTime - 1);
            this.SimulateAI(enemy);
        }

        
        public void Visit(Projectile projectile)
        {
            projectile.MoveDirection = projectile.FacingDirection;
            if (projectile.Area.Top < 0 || projectile.Area.Bottom > this.Height)
            {
                projectile.Health = 0;
            }
        }

       
        private void AddNewEntities()
        {
            foreach (var entity in this.entitiesToAdd)
            {
                this.entities.Add(entity);
            }

            this.entitiesToAdd.Clear();
        }

        private void RemoveDeadEntities()
        {
            for (int i = this.entities.Count - 1; i >= 0; --i)
            {
                if (this.entities[i].Health <= 0)
                {
                    if (this.ais.Remove(this.entities[i]))
                    {
                        --this.enemiesCount;
                        ++this.score;
                        this.scoreChanged(this.score);
                    }

                    this.entities.RemoveAt(i);
                }
            }
        }

      
        private void PlayerHit(int currentHealth)
        {
            this.playerHit?.Invoke(currentHealth, PlayerMaxHP);
        }

        private void SimulateAI(Enemy enemy)
        {
            if (enemy.ReloadTime == 0)
            {
                enemy.ReloadTime = enemy.ReloadDuration;
                this.AddProjectile(
                    new Point(enemy.CenterFront.X - (Projectile.Size.Width / 2), enemy.CenterFront.Y),
                    new Vector(0, 1),
                    Team.Enemy);
            }

            this.ais[enemy].Update();
        }
    }
}