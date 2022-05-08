using Ambrus.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Ambrus.Renderer;

namespace Ambrus.Wpf
{

    public partial class MainWindow : Window
    {        
        private GameControl game = null;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.mainGrid.Children.Clear();
            this.game = new GameControl(this.textName.Text);
            this.game.Width = 800;
            this.game.Height = 800;
            this.mainGrid.Children.Add(this.game);
            this.SizeToContent = SizeToContent.WidthAndHeight;

            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += Dt_Tick;
            dt.Start();

        }

        private int increment = 0;
        private void Dt_Tick(object sender, EventArgs e)
        {
            increment++;

        }
    }
}
