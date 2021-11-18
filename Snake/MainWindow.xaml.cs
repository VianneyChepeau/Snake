using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Point> historySnakePoints = new List<Point>();
        Point bonusPoint;
        Point currentHeadPosition = new Point(400, 200);
        Timer timer;
        int length = 10;
        int headSize = 8;
        int score = 0;

        enum MOVINGDIRECTION
        {
            Up=0,
            Down=1,
            Left=2,
            Right=3
        }
        MOVINGDIRECTION previousDirection;
        MOVINGDIRECTION actualDirection;

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            actualDirection = MOVINGDIRECTION.Right;
            CreateBonus(0);
            InitTimer();
        }

        private void CreateBonus(int index)
        {
            Random random = new Random();   
            bonusPoint = new Point(random.Next(1, 765), random.Next(1, 415));

            Ellipse newEllipse = new Ellipse();
            newEllipse.Fill = Brushes.Red;
            newEllipse.Width = headSize;
            newEllipse.Height = headSize;

            Canvas.SetTop(newEllipse, bonusPoint.Y);
            Canvas.SetLeft(newEllipse, bonusPoint.X);
            MainCanvas.Children.Insert(index, newEllipse); 
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (previousDirection != MOVINGDIRECTION.Up)
                        actualDirection = MOVINGDIRECTION.Down;
                    break;
                case Key.Up:
                    if (previousDirection != MOVINGDIRECTION.Down)
                        actualDirection = MOVINGDIRECTION.Up;
                    break;
                case Key.Left:
                    if (previousDirection != MOVINGDIRECTION.Right)
                        actualDirection = MOVINGDIRECTION.Left;
                    break;
                case Key.Right:
                    if (previousDirection != MOVINGDIRECTION.Left)
                        actualDirection = MOVINGDIRECTION.Right;
                    break;
            }
            previousDirection = actualDirection;
        }

        private void InitTimer()
        {
            timer = new Timer();
            timer.Interval = 16;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { 
                switch (actualDirection)
                {
                    case MOVINGDIRECTION.Down:
                        currentHeadPosition.Y += 1;
                        CreateSnake(currentHeadPosition);
                        break;
                    case MOVINGDIRECTION.Up:
                        currentHeadPosition.Y -= 1;
                        CreateSnake(currentHeadPosition);
                        break;
                    case MOVINGDIRECTION.Left:
                        currentHeadPosition.X -= 1;
                        CreateSnake(currentHeadPosition);
                        break;
                    case MOVINGDIRECTION.Right:
                        currentHeadPosition.X += 1;
                        CreateSnake(currentHeadPosition);
                        break;
                }

                if ((currentHeadPosition.X < 1) || (currentHeadPosition.X > (765)) ||
                    (currentHeadPosition.Y < 1) || (currentHeadPosition.Y > (415)))
                    GameOver();

                if ((Math.Abs(bonusPoint.X - currentHeadPosition.X) < headSize) &&
                    (Math.Abs(bonusPoint.Y - currentHeadPosition.Y) < headSize))
                {
                    length += 10;
                    score += 10;

                    MainCanvas.Children.RemoveAt(0);
                    CreateBonus(0);
                }

                for (int q = 0; q < (historySnakePoints.Count - headSize * 2); q++)
                {
                    Point point = new Point(historySnakePoints[q].X, historySnakePoints[q].Y);
                    if ((Math.Abs(point.X - currentHeadPosition.X) < (headSize)) &&
                         (Math.Abs(point.Y - currentHeadPosition.Y) < (headSize)))
                    {
                        GameOver();
                        break;
                    }
                }
            });
        }

        private void CreateSnake(Point currentposition)
        {
            try
            {
                this.Dispatcher.Invoke(() => { 
                    Ellipse newEllipse = new Ellipse();
                    newEllipse.Fill = new SolidColorBrush(Colors.DarkSlateBlue);
                    newEllipse.Width = headSize;
                    newEllipse.Height = headSize;

                    Canvas.SetTop(newEllipse, currentposition.Y);
                    Canvas.SetLeft(newEllipse, currentposition.X);

                    int count = MainCanvas.Children.Count;
                    MainCanvas.Children.Add(newEllipse);
                    historySnakePoints.Add(currentposition);

                    if (count > length)
                    {
                        MainCanvas.Children.RemoveAt(count - length);
                        historySnakePoints.RemoveAt(count - length);
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GameOver()
        {
            this.Dispatcher.Invoke(() => { 
                timer.Enabled = false;
                if (MessageBox.Show("Vous avez perdu ! Voulez-vous recommencer ?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    currentHeadPosition = new Point(400, 200);
                    length = 10;
                    headSize = 8;
                    historySnakePoints = new List<Point>();
                    actualDirection = MOVINGDIRECTION.Right;
                    MainCanvas.Children.RemoveRange(0, MainCanvas.Children.Count-1);
                    timer.Enabled = true;
                }
                else
                    this.Close();
            });
        }
    }
}
