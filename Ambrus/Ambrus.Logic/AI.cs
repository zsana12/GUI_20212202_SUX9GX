using System.Windows;
using Ambrus.Model;

namespace Ambrus.Logic
{
    /// <summary>
    /// AI controls an instance of enemy whenver level is updated.
    /// </summary>
    public class AI
    {
        /// <summary>
        /// A constant determining the amount of moves made to one side.
        /// </summary>
        private static readonly int MoveAmount = 20;

        /// <summary>
        /// Reference to the enemy object which is controlled by AI.
        /// </summary>
        private Enemy enemy;

        /// <summary>
        /// Counter for how many moves have been made.
        /// </summary>
        private int moves = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="AI"/> class.
        /// </summary>
        /// <param name="enemy">The enemy that will be controlled by AI.</param>
        public AI(Enemy enemy)
        {
            this.enemy = enemy;
        }

        /// <summary>
        /// Update the movement direction of the enemy.
        /// </summary>
        public void Update()
        {
            if (this.enemy.Strategy == Enemy.StrategyType.Diamond_Left ||
                this.enemy.Strategy == Enemy.StrategyType.Diamond_Right)
            {
                if (this.moves < MoveAmount)
                {
                    this.enemy.MoveDirection =
                        this.enemy.Strategy == Enemy.StrategyType.Diamond_Left ?
                        new Vector(-1, 1) :
                        new Vector(1, 1);
                }
                else if (this.moves < 2 * MoveAmount)
                {
                    this.enemy.MoveDirection =
                        this.enemy.Strategy == Enemy.StrategyType.Diamond_Left ?
                        new Vector(1, 1) :
                        new Vector(-1, 1);
                }
                else if (this.moves < 3 * MoveAmount)
                {
                    this.enemy.MoveDirection =
                        this.enemy.Strategy == Enemy.StrategyType.Diamond_Left ?
                        new Vector(1, -1) :
                        new Vector(-1, -1);
                }
                else
                {
                    this.enemy.MoveDirection =
                        this.enemy.Strategy == Enemy.StrategyType.Diamond_Left ?
                        new Vector(-1, -1) :
                        new Vector(1, -1);
                }
            }
            else
            {
                if (this.moves < 2 * MoveAmount)
                {
                    this.enemy.MoveDirection =
                        this.enemy.Strategy == Enemy.StrategyType.Left_Right ?
                        new Vector(-1, 0) :
                        new Vector(1, 0);
                }
                else
                {
                    this.enemy.MoveDirection =
                        this.enemy.Strategy == Enemy.StrategyType.Left_Right ?
                        new Vector(1, 0) :
                        new Vector(-1, 0);
                }
            }

            this.moves = (this.moves + 1) % (4 * MoveAmount);
        }
    }
}
