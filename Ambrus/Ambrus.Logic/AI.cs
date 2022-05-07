using System.Windows;
using Ambrus.Model;

namespace Ambrus.Logic
{
   
    public class AI
    {
       
        private static readonly int MoveAmount = 10;

       
        private Enemy enemy;

      
        private int moves = 0;

        public AI(Enemy enemy)
        {
            this.enemy = enemy;
        }

       
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