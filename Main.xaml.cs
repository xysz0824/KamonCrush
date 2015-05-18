using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KamonCrush
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : Window
    {
        DispatcherTimer updater;
        Game game;
        Sprite help;
        int ItemTypeCount = 8;
        int ItemSize = 50;
        int ItemCountH = 10;
        int ItemCountV = 10;

        public DispatcherTimer Updater { get { return updater; } }
        public Main()
        {
            InitializeComponent();
            InitializeGame();
            InitializeTimer();
        }

        void InitializeTimer()
        {
            updater = new DispatcherTimer();
            updater.Tick += UpdateGame;
            updater.Interval = new TimeSpan(0, 0, 0, 0, 16);
            updater.Start();
        }
        void InitializeGame()
        {
            int mapLeft = Math.Abs((int)Panel.Width - ItemCountH * ItemSize) / 2;
            int mapTop = Math.Abs((int)Panel.Height - ItemCountV * ItemSize) / 2;
            game = new Game(Panel, ItemTypeCount, ItemSize, mapLeft, mapTop, ItemCountH, ItemCountV);
            game.CreateRandomItems();
            game.HitEvent += (s, args) =>
                {
                    if (args.Hit >= 2)
                    {
                        HitImage.RenderTransformOrigin = new Point(0.5f, 0.5f);
                        HitImage.RenderTransform = new ScaleTransform();
                        var hitSprite = new Sprite(HitImage);
                        hitSprite.Left.Value = (float)(Panel.Width - HitImage.Width) / 2;
                        hitSprite.Top.Value = (float)(Panel.Height - HitImage.Height) / 2;
                        hitSprite.ScaleX.Value = 3;
                        hitSprite.ScaleY.Value = 3;

                        HitXImage.RenderTransformOrigin = new Point(0.5f, 0.5f);
                        HitXImage.RenderTransform = new ScaleTransform();
                        var hitXSprite = new Sprite(HitXImage);
                        var hitNum = args.Hit <= 9 ? args.Hit.ToString() : "max";
                        HitXImage.Source = new BitmapImage(new Uri(@"Images/x" + hitNum + ".png", UriKind.Relative));
                        hitXSprite.Left.Value = (float)(Panel.Width - HitXImage.Width) / 2;
                        hitXSprite.Top.Value = (float)(Panel.Height - HitXImage.Height) / 2;
                        //连破显示动画
                        new LinearAnimation(hitSprite.ScaleX, 1.2f, 8).Start();
                        new LinearAnimation(hitSprite.ScaleY, 1.2f, 8).Start();
                        new LinearAnimation(hitSprite.Opacity, 1, 8).Start();
                        var timer1 = new TimerAnimation(20);
                        timer1.Finished += () =>
                            {
                                new SmoothAnimation(hitSprite.Left, hitSprite.Left.Value - 90, 10).Start();
                                new SmoothAnimation(hitXSprite.Left, hitXSprite.Left.Value + 160, 10).Start();
                                new LinearAnimation(hitXSprite.Opacity, 1, 5).Start();
                                var timer2 = new TimerAnimation(30);
                                timer2.Finished += () =>
                                    {
                                        new LinearAnimation(hitSprite.Opacity, 0, 10).Start();
                                        new LinearAnimation(hitXSprite.Opacity, 0, 10).Start();
                                        var time3 = new TimerAnimation(10);
                                        time3.Finished += () =>
                                            {
                                                //防止控件遮挡图案点击
                                                hitSprite.Left.Value = 9999;
                                                hitSprite.Top.Value = 9999;
                                                hitXSprite.Left.Value = 9999;
                                                hitXSprite.Top.Value = 9999;
                                            };
                                        time3.Start();
                                    };
                                timer2.Start();
                            };
                        timer1.Start();
                    }
                    else
                        game.IsGameOver();
                };
            game.GameOverEvent += (s, args) =>
                {
                    GameOverImage.RenderTransformOrigin = new Point(0.5f, 0.5f);
                    GameOverImage.RenderTransform = new ScaleTransform();
                    var gameOverSprite = new Sprite(GameOverImage);
                    gameOverSprite.Left.Value = (float)(Panel.Width - GameOverImage.Width) / 2;
                    gameOverSprite.Top.Value = (float)(Panel.Height - GameOverImage.Height) / 2;
                    gameOverSprite.ScaleX.Value = 3;
                    gameOverSprite.ScaleY.Value = 3;
                    //游戏失败动画
                    new LinearAnimation(gameOverSprite.ScaleX, 1.2f, 8).Start();
                    new LinearAnimation(gameOverSprite.ScaleY, 1.2f, 8).Start();
                    new LinearAnimation(gameOverSprite.Opacity, 1, 8).Start();
                    var timer1 = new TimerAnimation(40);
                    timer1.Finished += () =>
                    {
                        ResetImage.RenderTransform = new ScaleTransform();
                        var resetSprite = new Sprite(ResetImage);
                        resetSprite.Left.Value = 328;
                        resetSprite.Top.Value = 420;
                        resetSprite.Opacity.Value = 1;
                    };
                    timer1.Start();
                };
            //淡入图案
            foreach (var item in game.Items)
                new LinearAnimation(item.Opacity, 1, 30).Start();
            var animation = new TimerAnimation(30);
            animation.Finished += () =>
                {
                    game.Start();
                };
            animation.Start();
        }
        void UpdateGame(object sender, EventArgs args)
        {
            AnimationManager.Update();
        }

        private void Hint_MouseEnter(object sender, MouseEventArgs e)
        {
            HintImage.Source = new BitmapImage(new Uri(@"Images\hintbuttonon.png", UriKind.Relative));
        }

        private void Hint_MouseLeave(object sender, MouseEventArgs e)
        {
            HintImage.Source = new BitmapImage(new Uri(@"Images\hintbutton.png", UriKind.Relative));
        }

        private void Help_MouseEnter(object sender, MouseEventArgs e)
        {
            Help.Source = new BitmapImage(new Uri(@"Images\helpbuttonon.png", UriKind.Relative));
        }

        private void Help_MouseLeave(object sender, MouseEventArgs e)
        {
            Help.Source = new BitmapImage(new Uri(@"Images\helpbutton.png", UriKind.Relative));
        }

        private void Help_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (help == null)
            {
                HelpImage.RenderTransformOrigin = new Point(0.5f, 0.5f);
                HelpImage.RenderTransform = new ScaleTransform();
                help = new Sprite(HelpImage);
                new LinearAnimation(help.Opacity, 1, 10).Start();
            }
        }

        private void HelpImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (help != null && help.Opacity.Value >= 1)
            {
                var animation = new LinearAnimation(help.Opacity, 0, 10);
                animation.Finished += () =>
                {
                    help.Left.Value = 9999;
                    help.Top.Value = 9999;
                    help = null;
                };
                animation.Start();
            }   
        }

        private void Hint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            game.GetHint();
        }

        private void ResetImage_MouseEnter(object sender, MouseEventArgs e)
        {
            ResetImage.Source = new BitmapImage(new Uri(@"Images\resetbuttonon.png", UriKind.Relative));
        }

        private void ResetImage_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetImage.Source = new BitmapImage(new Uri(@"Images\resetbutton.png", UriKind.Relative));
        }

        private void ResetImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            game.ClearItems();
            InitializeGame();
            GameOverImage.Opacity = 0;
            GameOverImage.SetValue(Canvas.TopProperty, 9999.0d);
            GameOverImage.SetValue(Canvas.LeftProperty, 9999.0d);

            ResetImage.Opacity = 0;
            ResetImage.SetValue(Canvas.TopProperty, 9999.0d);
            ResetImage.SetValue(Canvas.LeftProperty, 9999.0d);
        }
    }
}
