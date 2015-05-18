using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace KamonCrush
{
    enum GameStatus
    {
        Paused,
        SelectedNone,
        SelectedOne,
        SelectedTwo,
        GameOver
    }
    class Game : IInputListener, INotifyPropertyChanged
    {
        public event Action<object, HitEventArgs> HitEvent;
        public event Action<object, EventArgs> GameOverEvent;
       
        Canvas panel;
        int typeCount;
        int itemSize;
        int left, top;
        int width, height;
        GameStatus status;
        GameItem[,] items;
        GameItem selectedItem;
        int maxZIndex;
        int maxScore;
        int score;
        int hint;
        bool gotHint;
        
        public GameItem[,] Items { get { return items; } }
        public int Left { get { return left; } }
        public int Top { get { return top; } }
        public int MaxScore 
        { 
            get 
            {
                if (maxScore < score)
                    maxScore = score;
                return maxScore; 
            } 
        }
        public int Score { get { return score; } }
        public int Hint { get { return hint; } }
        public Game(Canvas panel, int typeCount, int itemSize, int left, int top, int width, int height)
        {
            this.panel = panel;
            panel.DataContext = this;
            this.typeCount = typeCount;
            this.itemSize = itemSize;
            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
            items = new GameItem[width, height];
            hint = 3;
        }
        public void Start()
        {
            status = GameStatus.SelectedNone;
        }
        public List<GameItem> CreateRandomItems(int left, int top, int width, int height)
        {
            List<GameItem> newItems = new List<GameItem>();
            Random rand = new Random();
            for (int x = left; x < left + width; ++x)
                for (int y = top; y < top + height; ++y)
                {
                    //随机图案
                    var randType = rand.Next(typeCount);
                    if (x > 0 && items[x - 1, y].TypeId.Value == randType)
                    {
                        //防止横向出现连续图案
                        while (true)
                        {
                            randType = rand.Next(typeCount);
                            if (items[x - 1, y].TypeId.Value != randType)
                                break;
                        }
                    }
                    if (y > 0 && items[x, y - 1].TypeId.Value == randType)
                    {
                        //防止纵向出现连续图案
                        while (true)
                        {
                            randType = rand.Next(typeCount);
                            if (items[x, y - 1].TypeId.Value != randType)
                                break;
                        }
                    }
                    items[x, y] = CreateItem(randType, x * itemSize, y * itemSize);
                    newItems.Add(items[x, y]);
                }
            return newItems;
        }
        public void CreateRandomItems()
        {
            CreateRandomItems(0, 0, width, height);
        }
        GameItem CreateItem(int typeId, int itemLeft, int itemTop)
        {
            var item = new GameItem(panel, this);
            panel.Children.Add(item.Shadow);
            panel.Children.Add(item.Pattern);
            item.TypeId.Value = typeId;
            item.Left.Value = Left + itemLeft;
            item.Top.Value = Top + itemTop;
            item.Pattern.Width = itemSize;
            item.Pattern.Height = itemSize;
            item.Pattern.Stretch = Stretch.Uniform;
            item.Pattern.Source = new BitmapImage(new Uri(@"Images\" + (typeId + 1).ToString() + ".png", UriKind.Relative));
            item.Shadow.Width = item.Pattern.Width;
            item.Shadow.Height = item.Pattern.Height;
            item.Shadow.Source = new BitmapImage(new Uri(@"Images\shadow.png", UriKind.Relative));
            return item;
        }
        void DeleteItem(GameItem item)
        {
            panel.Children.Remove(item.Shadow);
            panel.Children.Remove(item.Pattern);
        }
        public void ClearItems()
        {
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    DeleteItem(items[x, y]);
        }
        bool IndexOfItem(GameItem item, out int xIndex, out int yIndex)
        {
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (items[x, y] == item)
                    {
                        xIndex = x;
                        yIndex = y;
                        return true;
                    }
            xIndex = -1;
            yIndex = -1;
            return false;
        }
        bool TrySwapItem(GameItem itemA, GameItem itemB)
        {
            bool succeed = false;
            int x1, y1, x2, y2;
            IndexOfItem(itemA, out x1, out y1);
            IndexOfItem(itemB, out x2, out y2);
            var temp = items[x1, y1];
            items[x1, y1] = items[x2, y2];
            items[x2, y2] = temp;
            var resultA = FindSuccessive(itemA);
            var resultB = FindSuccessive(itemB);
            if (resultA.Count >= 3 || resultB.Count >= 3)
                succeed = true;

            temp = items[x1, y1];
            items[x1, y1] = items[x2, y2];
            items[x2, y2] = temp;
            return succeed;
        }
        void SwapItem(GameItem itemA, GameItem itemB)
        {
            new LinearAnimation(itemA.ScaleX, 1, 5).Start();
            new LinearAnimation(itemA.ScaleY, 1, 5).Start();
            new LinearAnimation(itemB.ScaleX, 1, 5).Start();
            new LinearAnimation(itemB.ScaleY, 1, 5).Start();
            new LinearAnimation(itemA.Left, itemB.Left.Value, 15).Start();
            new LinearAnimation(itemA.Top, itemB.Top.Value, 15).Start();
            new LinearAnimation(itemB.Left, itemA.Left.Value, 15).Start();
            new LinearAnimation(itemB.Top, itemA.Top.Value, 15).Start();
            var timer = new TimerAnimation(20);
            timer.Finished += () =>
                {
                    int x1, y1, x2, y2;
                    IndexOfItem(itemA, out x1, out y1);
                    IndexOfItem(itemB, out x2, out y2);
                    var temp = items[x1, y1];
                    items[x1, y1] = items[x2, y2];
                    items[x2, y2] = temp;
                    var resultA = FindSuccessive(itemA);
                    var resultB = FindSuccessive(itemB);
                    if (resultA.Count >= 3 && resultB.Count >= 3)
                    {
                        resultA.AddRange(resultB);
                        CrushItems(resultA, 1);
                    }
                    else if (resultA.Count >= 3)
                        CrushItems(resultA, 1);
                    else if (resultB.Count >= 3)
                        CrushItems(resultB, 1);
                    else
                    {
                        //交换后未出现连续三个及以上相邻的图案块则撤销交换
                        new SmoothAnimation(itemA.Left, itemB.Left.Value, 5).Start();
                        new SmoothAnimation(itemA.Top, itemB.Top.Value, 5).Start();
                        new SmoothAnimation(itemB.Left, itemA.Left.Value, 5).Start();
                        new SmoothAnimation(itemB.Top, itemA.Top.Value, 5).Start();
                        temp = items[x1, y1];
                        items[x1, y1] = items[x2, y2];
                        items[x2, y2] = temp;
                        status = GameStatus.SelectedNone;
                    }
                };
            timer.Start();
        }
        void CrushItems(List<GameItem> items, int hit)
        {
            if (items != null && items.Count >= 3)
            {
                //计算分数
                score += CalculateScore(hit, items.Count);
                UpdateComponent();
                //消除图案
                foreach (var item in items)
                {
                    int x, y;
                    IndexOfItem(item, out x, out y);
                    var animation = new SmoothAnimation(item.Opacity, 0, 5);
                    animation.Finished += () =>
                        {
                            DeleteItem(item);
                        };
                    new SmoothAnimation(item.ScaleX, 0, 5).Start();
                    new SmoothAnimation(item.ScaleY, 0, 5).Start();
                }
                //申请一个表来存储每列没被消除的item
                List<GameItem>[] record = new List<GameItem>[width];
                for (int x = 0; x < record.Length; ++x)
                {
                    record[x] = new List<GameItem>();
                    for (int y = height - 1; y >= 0; --y)
                        if (!items.Contains(this.items[x, y]))
                            record[x].Add(this.items[x, y]);
                }
                //设置下落加速度
                float aspeed = 0.5f;
                //记录下最长的下落距离
                int maxDistance = int.MinValue;
                for (int x = 0; x < record.Length; ++x)
                {
                    //消除块上方的块下落
                    int ny = height - 1;
                    for (int y = 0; y < record[x].Count; ++y)
                    {
                        var top = this.items[x, ny].Top.Value;
                        this.items[x, ny] = record[x][y];
                        new QuadraticAnimation(this.items[x, ny].Top, Top + ny * itemSize, aspeed).Start();
                        ny--;
                    }
                    //生成新的块
                    var distance = height - record[x].Count;
                    if (distance > maxDistance)
                        maxDistance = distance;
                    var newItems = CreateRandomItems(x, 0, 1, distance);
                    foreach (var item in newItems)
                    {
                        new LinearAnimation(item.Opacity, 1, 30).Start();
                        item.Top.Value -= distance * itemSize;
                        new QuadraticAnimation(item.Top, item.Top.Value + distance * itemSize, aspeed).Start();
                    }
                }
                //计算完毕时间
                int length = (maxDistance + 2) * itemSize;
                int duration = (int)Math.Sqrt(length * 2 / aspeed);
                //继续检测消除
                var timer = new TimerAnimation(duration);
                timer.Finished += () =>
                {
                    var result = FindSuccessive();
                    if (result.Count >= 3)
                        CrushItems(result, hit + 1);
                    else
                    {
                        status = GameStatus.SelectedNone;
                        gotHint = false;
                        //是否消除失败
                        IsGameOver();
                        //连破显示
                        if (status != GameStatus.GameOver && HitEvent != null)
                        {
                            var args = new HitEventArgs(hit);
                            HitEvent(this, args);
                        }
                    }
                };
                timer.Start();
            }
        }
        List<GameItem> FindSuccessive()
        {
            List<GameItem> closed = new List<GameItem>();
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    //检查item是否已经是某一个连续图案块中的一个
                    if (!closed.Contains(items[x, y]))
                    {
                        var result = FindSuccessive(items[x, y]);
                        if (result.Count >= 3)
                            closed.AddRange(result);
                    }
                }

            return closed;
        }
        List<GameItem> FindSuccessive(GameItem item)
        {
            //找出item周围相连的相同图案
            List<GameItem> closedHorizon = new List<GameItem>();
            List<GameItem> closedVertical = new List<GameItem>();
            closedHorizon.Add(item);
            closedVertical.Add(item);
            int x, y;
            if (IndexOfItem(item, out x, out y))
            {
                //扩展
                CheckAccessive(closedHorizon, item, x - 1, y, true);
                CheckAccessive(closedHorizon, item, x + 1, y, true);
                CheckAccessive(closedVertical, item, x, y - 1, false);
                CheckAccessive(closedVertical, item, x, y + 1, false);
            }
            return closedHorizon.Count >= 3 ? closedHorizon : closedVertical;
        }
        void CheckAccessive(List<GameItem> closed, GameItem last, int x, int y, bool left)
        {
            //对于超出范围的立即终止
            if (x < 0 || y < 0 || x >= width || y >= height)
                return;
            //对于已检测过的不再检测
            foreach (var item in closed)
                if (item == items[x, y])
                    return;
            //当图案不同时终止
            if (last.TypeId.Value != items[x, y].TypeId.Value)
                return;
            closed.Add(items[x, y]);
            if (left)
            {
                CheckAccessive(closed, items[x, y], x - 1, y, left);
                CheckAccessive(closed, items[x, y], x + 1, y, left);
            }
            else
            {
                CheckAccessive(closed, items[x, y], x, y - 1, left);
                CheckAccessive(closed, items[x, y], x, y + 1, left);
            }
        }
        GameItem[] FindSwapItem()
        {
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    if (x > 0 && TrySwapItem(items[x - 1, y], items[x, y]))
                        return new GameItem[] { items[x - 1, y], items[x, y] };
                    if (y > 0 && TrySwapItem(items[x, y - 1], items[x, y]))
                        return new GameItem[] { items[x, y - 1], items[x, y] };
                    if (x < width - 1 && TrySwapItem(items[x + 1, y], items[x, y]))
                        return new GameItem[] { items[x + 1, y], items[x, y] };
                    if (y < height - 1 && TrySwapItem(items[x, y + 1], items[x, y]))
                        return new GameItem[] { items[x, y + 1], items[x, y] };
                }
            return null;
        }
        public void GetHint()
        {
            if (status != GameStatus.GameOver && status != GameStatus.Paused && status != GameStatus.SelectedTwo 
                && hint > 0 && !gotHint)
            {
                if (selectedItem != null)
                    ItemClicked(selectedItem, true);
                var canSwapItems = FindSwapItem();
                ItemClicked(canSwapItems.First(), false);
                gotHint = true;
                hint--;
            }
        }
        public void IsGameOver()
        {
            var canSwapItems = FindSwapItem();
            //if (canSwapItems != null)
                //return;
            if (GameOverEvent != null)
                GameOverEvent(this, new EventArgs());
            status = GameStatus.GameOver;
        }
        static int CalculateScore(int hit, int count)
        {
            return count * 100 * hit;
        }
        void UpdateComponent()
        {
            panel.DataContext = null;
            panel.DataContext = this;
        }

        #region IInputListener 成员
        public bool ItemClicked(GameItem sender, bool preState)
        {
            if (status == GameStatus.Paused || status == GameStatus.SelectedTwo || status == GameStatus.GameOver)
                return preState;
            if (!preState)
            {
                if (selectedItem != null)
                {
                    //保证选取的两个块相邻
                    int x1, y1, x2, y2;
                    IndexOfItem(sender, out x1, out y1);
                    IndexOfItem(selectedItem, out x2, out y2);
                    if (Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)) > 1.0)
                        return preState;
                }
                new SmoothAnimation(sender.ScaleX, 1.5f, 5).Start();
                new SmoothAnimation(sender.ScaleY, 1.5f, 5).Start();
                sender.ZIndex.Value = ++maxZIndex;
                if (status == GameStatus.SelectedOne)
                {
                    status = GameStatus.SelectedTwo;
                    SwapItem(selectedItem, sender);
                    selectedItem = null;
                    return preState;
                }
                status = GameStatus.SelectedOne;
                selectedItem = sender;
                preState = true;
            }
            else
            {
                new SmoothAnimation(sender.ScaleX, 1f, 5).Start();
                new SmoothAnimation(sender.ScaleY, 1f, 5).Start();
                status = GameStatus.SelectedNone;
                selectedItem = null;
                preState = false;
            }
            return preState;
        }
        #endregion

        #region INotifyPropertyChanged 成员

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
