using Ambrus.Logic;
using Ambrus.Model;
using Ambrus.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Ambrus.Renderer
{
    public class GameRenderer
    {
        //font stuff
        private readonly double fontSize = 15;

        private readonly double fontSizeScores = 30;

        private FormattedText scoreText;

        private Typeface font = new Typeface("Arial");

        //bitmapimages
        public BitmapImage background = null;

        private readonly Dictionary<SpriteType, BitmapImage> spriteImages;

        private BitmapImage panel = null;

        private BitmapImage healthbar = null;

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
            this.width = width;
            this.height = height;
            this.logic = logic;
            this.Scores = scores;
            this.control = control;
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
    }
}
