using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambrus.Renderer
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Ambrus.Logic;
    using Ambrus.Model;
    using Ambrus.Repository;
    using Windows.UI.Xaml;

    public class GameControl : FrameworkElement
    {
        private readonly double fps = 60;

        private readonly int levelWidth = 800;

        private readonly int levelHeight = 800;

        private ILogic logic;

        private IGameRepository repository;

        private GameRenderer renderer;

        private DispatcherTimer mainTimer;

        private int score = 0;

        private string playerName;

        private GameState gameState = GameState.Playing;

        private DateTime stateSwitchTime = DateTime.Now;

        public GameControl(string playerName)
        {
            this.playerName = playerName;
            this.Loaded += this.GameControl_Loaded;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this.renderer != null)
            {
                this.renderer.Render(drawingContext, this.gameState);
            }
        }

        private void GameControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.logic = new Logic(this.levelWidth, this.levelHeight);
            this.logic.PlayerHit += this.OnPlayerHit;
            this.logic.ScoreChanged += this.OnScoreChanged;

            this.repository = new XmlRepository();
            IEnumerable<Entry> scores = this.repository.LoadScores();

            this.renderer = new GameRenderer(this.levelWidth, this.levelHeight, this.logic, scores, this);
            this.renderer.SetBackgroundImage(new Uri(@"pack://application:,,,/Resource/background.jpg", UriKind.Absolute));
            this.renderer.SetPanelImage(new Uri(@"pack://application:,,,/Resource/panel.png", UriKind.Absolute));
            this.renderer.SetHealthbarImage(new Uri(@"pack://application:,,,/Resource/healthbar.png", UriKind.Absolute));
            this.renderer.SetSpriteImage(SpriteType.Player, new Uri(@"pack://application:,,,/Resource/Ambrus_car.png", UriKind.Absolute));
            this.renderer.SetSpriteImage(SpriteType.Enemy, new Uri(@"pack://application:,,,/Resource/police.png", UriKind.Absolute));
            this.renderer.SetSpriteImage(SpriteType.MissilePlayer, new Uri(@"pack://application:,,,/Resource/bullet.png", UriKind.Absolute));
            this.renderer.SetSpriteImage(SpriteType.MissileEnemy, new Uri(@"pack://application:,,,/Resource/missile_enemy.png", UriKind.Absolute));

            Window window = Window.GetWindow(this);
            if (window != null)
            {
                this.mainTimer = new DispatcherTimer();
                this.mainTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / this.fps);
                this.mainTimer.Tick += this.Timer_Tick;
                this.mainTimer.Start();
            }

            this.InvalidateVisual();
        }

        private void OnPlayerHit(int currentHp, int maxHp)
        {
            this.renderer.SetPlayerHealthbar(currentHp, maxHp);

            if (currentHp == 0)
            {
                this.gameState = GameState.Scores;
                this.ReloadScores();
            }
        }

        private void ReloadScores()
        {
            this.repository.SaveScore(this.playerName, this.score);
            this.renderer.Scores = this.repository.LoadScores();
        }

        private void OnScoreChanged(int score)
        {
            this.renderer.UpdateScore(score);
            this.score = score;
        }

        private void HandleInput()
        {
            if (this.gameState == GameState.Playing)
            {
                if (Keyboard.IsKeyDown(Key.Left))
                {
                    this.logic.PlayerMove(Direction.Left);
                }

                if (Keyboard.IsKeyDown(Key.Up))
                {
                    this.logic.PlayerMove(Direction.Up);
                }

                if (Keyboard.IsKeyDown(Key.Right))
                {
                    this.logic.PlayerMove(Direction.Right);
                }

                if (Keyboard.IsKeyDown(Key.Down))
                {
                    this.logic.PlayerMove(Direction.Down);
                }

                if (Keyboard.IsKeyDown(Key.Space))
                {
                    this.logic.PlayerShoot();
                }
            }

            if (Keyboard.IsKeyDown(Key.Escape) && this.gameState != GameState.GameOver)
            {
                if (DateTime.Now - this.stateSwitchTime >= new TimeSpan(0, 0, 0, 0, 500))
                {
                    this.gameState = this.gameState == GameState.Playing ? GameState.Scores : GameState.Playing;
                    this.stateSwitchTime = DateTime.Now;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.HandleInput();

            if (this.gameState == GameState.Playing)
            {
                this.logic.Update();
            }

            this.InvalidateVisual();
        }
    }
}

