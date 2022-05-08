using Ambrus.Logic;
using Ambrus.Model;
using Ambrus.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Ambrus.Renderer
{
    internal class GameRenderer
    {
        //font stuff
        private readonly double fontSize = 15;

        private readonly double fontSizeScores = 30;

        private FormattedText scoreText;

        private FormattedText timerText;

        private Typeface font = new Typeface("Courier New");

        //bitmapimages
        public BitmapImage background = null;

        private readonly Dictionary<SpriteType, BitmapImage> spriteImages = new Dictionary<SpriteType, BitmapImage>();

        private BitmapImage panel = null;

        private BitmapImage healthbar = null;

        private BitmapImage clock = null;
        
        private int increment;
        //background moving
        private readonly int backgroundMoveSpeed = 1;

        private int backgroundY = 0;


        private ILogic logic;

        //healtbar stuff
        private Rect hbLocation;
        private Int32Rect hbCrop;
        private Point hbPanelOffset = new Point(1, 1);


        //player screen information
        private int width;
        private int height;

        private FrameworkElement control;

        public IEnumerable<Entry> Scores { get; set; }

        public GameRenderer(int width, int height, ILogic logic, IEnumerable<Entry> scores, FrameworkElement control)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += Dt_Tick;
            dt.Start();



            this.width = width;
            this.height = height;
            this.logic = logic;
            this.Scores = scores;
            this.control = control;
            this.scoreText = this.CreateFormattedText("Score: 0", this.font, this.fontSize);
            this.timerText = this.CreateFormattedText("Time: 90", this.font, this.fontSize);
        }

        public int MaxTime = 90;

        private void Dt_Tick(object sender, EventArgs e)
        {
            increment++;

            MaxTime--;

            UpdateTimer(MaxTime);
             
        }
        public void SetBackgroundImage(Uri imagePath)
        {
            this.background = new BitmapImage(imagePath);
        }

        public void SetPanelImage(Uri imagePath)
        {
            this.panel = new BitmapImage(imagePath);
        }

        public void SetSpriteImage(SpriteType sprite, Uri imagePath)
        {
            this.spriteImages[sprite] = new BitmapImage(imagePath);
        }

        public void SetHealthbarImage(Uri imagePath)
        {
            this.healthbar = new BitmapImage(imagePath);
            this.hbCrop = new Int32Rect(0, 0, this.healthbar.PixelWidth, this.healthbar.PixelHeight);
            this.hbLocation = new Rect(this.hbPanelOffset.X, this.hbPanelOffset.Y, this.healthbar.PixelWidth, this.healthbar.PixelHeight);
        }

        public void SetClockImage(Uri imagePath)
        {
            this.clock = new BitmapImage(imagePath);
        }

        public void Render(DrawingContext dc, GameState gameState)
        {
            if (gameState == GameState.Playing)
            {
                this.DrawLevel(dc);

                if (MaxTime <= 0)
                {

                    this.DrawHighScores(dc);

                }
            }
            else
            {
                System.Environment.Exit(0);
            }
        }

        public void SetPlayerHealthbar(int currentHp, int maxHp)
        {
            double r = (double)Math.Max(currentHp, 0) / maxHp;
            int width = (int)(this.healthbar.PixelWidth * r);
            this.hbCrop = new Int32Rect(0, 0, width, this.healthbar.PixelHeight);
            this.hbLocation = new Rect(this.hbPanelOffset.X, this.hbPanelOffset.Y, width, this.healthbar.PixelHeight);
        }

        public void UpdateScore(int score)
        {
            this.scoreText = this.CreateFormattedText(string.Format("Score: {0}", score), this.font, this.fontSize);
        }

        public void UpdateTimer(int time)
        {


                this.timerText = this.CreateFormattedText(string.Format("timer: {0}", time), this.font, this.fontSize);

        }


        private void DrawLevel(DrawingContext dc)
        {
            // Clear
            dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, this.width, this.height));
            this.DrawBackground(dc, true);
            this.DrawPanel(dc);

            // Draw entities
            foreach (var model in this.logic.Models)
            {
                var image = this.spriteImages[model.Sprite];
                dc.DrawImage(image, model.Area);
            }



            this.DrawScore(dc);
            this.DrawTimer(dc);
        }

        private void DrawHighScores(DrawingContext dc)
        {
            dc.DrawRectangle(Brushes.Black, null, new Rect(0, 0, this.width, this.height));
            this.DrawBackground(dc, false);

            int i = 0;
            foreach (var score in this.Scores)
            {
                FormattedText text = this.CreateFormattedText(
                    string.Format("{0}: {1}", score.Name, score.Score), this.font, this.fontSizeScores);

                double x = (this.width / 2) - (text.Width / 2);
                dc.DrawText(text, new Point(x, 50 + (2 * i * text.Height)));

                ++i;
            }
        }

        private void DrawScore(DrawingContext dc)
        {
            double x = (this.width / 2) - (this.scoreText.Width / 2);
            dc.DrawText(this.scoreText, new Point(x, 20));
        }

        private void DrawTimer(DrawingContext dc)
        {
            double x = (this.width / 3) - (this.timerText.Width / 3);
            dc.DrawText(this.timerText, new Point(x, 20));
        }

        private void DrawBackground(DrawingContext dc, bool scroll)
        {
            if (this.background != null)
            {
                if (scroll)
                {
                    this.backgroundY = (this.backgroundY + this.backgroundMoveSpeed) % this.background.PixelHeight;
                }

                if (this.backgroundY == 0)
                {
                    dc.DrawImage(this.background, new Rect(0, 0, this.width, this.height));
                }
                else
                {
                    Int32Rect crop = new Int32Rect(0, this.height - this.backgroundY, this.width, this.backgroundY);
                    CroppedBitmap cropped = new CroppedBitmap(this.background, crop);
                    Rect area = new Rect(0, 0, this.width, this.backgroundY);
                    dc.DrawImage(cropped, area);

                    crop = new Int32Rect(0, 0, this.width, this.height - this.backgroundY);
                    cropped = new CroppedBitmap(this.background, crop);
                    area = new Rect(0, this.backgroundY, this.width, this.height - this.backgroundY);
                    dc.DrawImage(cropped, area);
                }
            }
        }

        private void DrawPanel(DrawingContext dc)
        {
            if (this.panel != null)
            {
                dc.DrawImage(this.panel, new Rect(0, 0, this.panel.PixelWidth, this.panel.PixelHeight));
            }

            this.DrawHealthbar(dc);
        }

        private void DrawClock(DrawingContext dc)
        {
            if (this.panel != null)
            {
                dc.DrawImage(this.panel, new Rect(0, 0, this.panel.PixelWidth, this.panel.PixelHeight));
            }

            this.DrawHealthbar(dc);
        }

            private void DrawHealthbar(DrawingContext dc)
        {
            if (this.healthbar != null && this.hbCrop.Width > 0)
            {
                CroppedBitmap cropped = new CroppedBitmap(this.healthbar, this.hbCrop);
                dc.DrawImage(cropped, this.hbLocation);
            }
        }

        private FormattedText CreateFormattedText(string text, Typeface font, double fontSize)
        {
            return new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                font,
                fontSize,
                Brushes.White,
                VisualTreeHelper.GetDpi(this.control).PixelsPerDip);
        }


    }
}
