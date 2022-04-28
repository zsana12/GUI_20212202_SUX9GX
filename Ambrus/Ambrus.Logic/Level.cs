using System;
using System.Collections.Generic;
using System.Windows;
using Ambrus.Model;
using Ambrus.Model.Entities;
using Ambrus.Model.Enums;
using Ambrus.Model.Interfaces;

namespace Ambrus.Logic
{
    

    /// <summary>
    /// Level class stores, controls and updates game entities.
    /// </summary>
    public class Level : IVisitor
    {
        /// <summary>
        /// Max hit points of the player.
        /// </summary>
        public static readonly int PlayerMaxHP = 5;

        /// <summary>
        /// Health of the enemy.
        /// </summary>
        private const int EnemyHP = 1;

        /// <summary>
        /// Rate at which the player moves per update.
        /// </summary>
        private readonly int playerMoveSpeed = 5;

        /// <summary>
        /// Callback for the event when player is hit.
        /// </summary>
        private Action<int, int> playerHit;

        /// <summary>
        /// Callback for the event when score is changed.
        /// </summary>
        private Action<int> scoreChanged;

        /// <summary>
        /// Reference to physics.
        /// </summary>
        private Physics physics;

        /// <summary>
        /// List of entities.
        /// </summary>
        private List<GameEntity> entities = new List<GameEntity>();

        /// <summary>
        /// A list of entities to add after the update loop.
        /// </summary>
        private List<GameEntity> entitiesToAdd = new List<GameEntity>();

        /// <summary>
        /// Dictionary of AI objects for each Enemy.
        /// </summary>
        private Dictionary<GameEntity, AI> ais = new Dictionary<GameEntity, AI>();

        /// <summary>
        /// Reference to player.
        /// </summary>
        private Player player = null;

        /// <summary>
        /// How many enemies are in the level.
        /// </summary>
        private int enemiesCount = 0;

        /// <summary>
        /// Current score.
        /// </summary>
        private int score = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Level"/> class.
        /// </summary>
        /// <param name="width">Width of the Level.</param>
        /// <param name="height">Width of the Height.</param>
        /// <param name="playerHit">Delegate for when player is hit.</param>
        /// <param name="scoreChanged">Delegate for when score changes.</param>
        public Level(int width, int height, Action<int, int> playerHit, Action<int> scoreChanged)
        {
            this.Width = width;
            this.Height = height;
            this.playerHit = playerHit;
            this.scoreChanged = scoreChanged;
            this.physics = new Physics(width, height, this.entities);
        }

        /// <summary>
        /// Event that fires when all enemies are dead.
        /// </summary>
        public event Action EnemiesCleared;

        /// <summary>
        /// Gets width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Gets height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Gets entities.
        /// </summary>
        public IEnumerable<GameEntity> Entities
        {
            get { return this.entities; }
        }

        /// <summary>
        /// Add the player to the level.
        /// </summary>
        /// <param name="position">Position of the player.</param>
        /// <returns>Created player.</returns>
        public Player AddPlayer(Point position)
        {
            this.player = new Player(new Rect(position, Player.Size), PlayerMaxHP, this.PlayerHit);
            this.entitiesToAdd.Add(this.player);
            return this.player;
        }

        /// <summary>
        /// Add an enemy to the level.
        /// </summary>
        /// <param name="position">Position of the enemy.</param>
        /// <param name="strategy">Strategy used by the enemy.</param>
        /// <param name="hp">Health of the enemy with a default value.</param>
        /// <returns>Created enemy.</returns>
        public Enemy AddEnemy(Point position, Enemy.StrategyType strategy, int hp = EnemyHP)
        {
            var enemy = new Enemy(new Rect(position, Enemy.Size), hp, strategy);
            this.entitiesToAdd.Add(enemy);
            this.ais.Add(enemy, new AI(enemy));
            ++this.enemiesCount;
            return enemy;
        }

        /// <summary>
        /// Add a projectile to the level.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="dir">Direction vector.</param>
        /// <param name="team">Team.</param>
        /// <returns>Created projectile.</returns>
        public Projectile AddProjectile(Point position, Vector dir, Team team)
        {
            SpriteType sprite = team == Team.Player ? SpriteType.MissilePlayer : SpriteType.MissileEnemy;
            var projectile = new Projectile(new Rect(position, Projectile.Size), sprite, dir, team);
            this.entitiesToAdd.Add(projectile);
            return projectile;
        }

        /// <summary>
        /// Move player in a direction.
        /// </summary>
        /// <param name="direction">Direction.</param>
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

        /// <summary>
        /// Player shoots a missile.
        /// </summary>
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

        /// <summary>
        /// Update all entities and AI in the level.
        /// </summary>
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

        /// <inheritdoc/>
        public void Visit(Player player)
        {
            player.ReloadTime = Math.Max(0, player.ReloadTime - 1);
        }

        /// <inheritdoc/>
        public void Visit(Enemy enemy)
        {
            enemy.ReloadTime = Math.Max(0, enemy.ReloadTime - 1);
            this.SimulateAI(enemy);
        }

        /// <inheritdoc/>
        public void Visit(Projectile projectile)
        {
            projectile.MoveDirection = projectile.FacingDirection;
            if (projectile.Area.Top < 0 || projectile.Area.Bottom > this.Height)
            {
                projectile.Health = 0;
            }
        }

        /// <summary>
        /// Adds the entities from entitiesToAdd to entities and clears the former.
        /// </summary>
        private void AddNewEntities()
        {
            foreach (var entity in this.entitiesToAdd)
            {
                this.entities.Add(entity);
            }

            this.entitiesToAdd.Clear();
        }

        /// <summary>
        /// Removes entities who's health is zero or below.
        /// </summary>
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

        /// <summary>
        /// Callback for when player is hit, invokes the passed callback.
        /// </summary>
        /// <param name="currentHealth">Current health of the player.</param>
        private void PlayerHit(int currentHealth)
        {
            this.playerHit?.Invoke(currentHealth, PlayerMaxHP);
        }

        /// <summary>
        /// AI controls the movement of the enemy.
        /// </summary>
        /// <param name="enemy">Enemy.</param>
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
