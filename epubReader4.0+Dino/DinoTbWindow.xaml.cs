using System;
using System.Reflection;
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

namespace epubReader4._0_Dino
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DinoTbWindow : Window
    {
        private Point _position;
        private bool _trimEnable = false;
        string pageNameMain, pageDirectory1;
        System.Windows.Controls.Image[] img = new System.Windows.Controls.Image[100];
        string[] page = new string[100];

        public DinoTbWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // プライマリスクリーンサイズの取得
            var screen = System.Windows.Forms.Screen.PrimaryScreen;

            // ウィンドウサイズの設定
            this.Left = screen.Bounds.Left;
            this.Top = screen.Bounds.Top;
            this.Width = screen.Bounds.Width;
            this.Height = screen.Bounds.Height;

            // ジオメトリサイズの設定
            this.ScreenArea.Geometry1 = new RectangleGeometry(new Rect(0, 0, screen.Bounds.Width, screen.Bounds.Height));
        }

        public void subToTb(string pageNameSub, string pageDirectorySub)
        {
            pageNameMain = pageNameSub;
            pageDirectory1 = pageDirectorySub;
        }

        private void DrawingPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if (path == null)
                return;

            // 開始座標を取得
            var point = e.GetPosition(path);
            _position = point;

            // マウスキャプチャの設定
            _trimEnable = true;
            this.Cursor = Cursors.Cross;
            path.CaptureMouse();
        }

        private void DrawingPath_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var path = sender as Path;
            if (path == null)
                return;

            // 現在座標を取得
            var point = e.GetPosition(path);

            // マウスキャプチャの終了
            _trimEnable = false;
            this.Cursor = Cursors.Arrow;
            path.ReleaseMouseCapture();

            // 画面キャプチャ
            CaptureScreen(point);

            DinoSubWindow dialog = new DinoSubWindow();
            dialog.Show();
            dialog.sub(pageNameMain, pageDirectory1);

            // アプリケーションの終了
            this.Close();
        }

        private void DrawingPath_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_trimEnable)
                return;

            var path = sender as Path;
            if (path == null)
                return;

            // 現在座標を取得
            var point = e.GetPosition(path);

            // キャプチャ領域枠の描画
            DrawStroke(point);
        }

        private void DrawStroke(Point point)
        {
            // 矩形の描画
            var x = _position.X < point.X ? _position.X : point.X;
            var y = _position.Y < point.Y ? _position.Y : point.Y;
            var width = Math.Abs(point.X - _position.X);
            var height = Math.Abs(point.Y - _position.Y);
            this.ScreenArea.Geometry2 = new RectangleGeometry(new Rect(x, y, width, height));
        }

        private void CaptureScreen(Point point)
        {
            // 座標変換
            var start = PointToScreen(_position);
            var end = PointToScreen(point);

            // キャプチャエリアの取得
            var x = start.X < end.X ? (int)start.X : (int)end.X;
            var y = start.Y < end.Y ? (int)start.Y : (int)end.Y;
            var width = (int)Math.Abs(end.X - start.X);
            var height = (int)Math.Abs(end.Y - start.Y);
            if (width == 0 || height == 0)
                return;

            // スクリーンイメージの取得
            using (var bmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
            using (var graph = System.Drawing.Graphics.FromImage(bmp))
            {
                // 画面をコピーする
                graph.CopyFromScreen(new System.Drawing.Point(x, y), new System.Drawing.Point(), bmp.Size);

                // イメージの保存
                string folder = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //bmp.Save(System.IO.Path.ChangeExtension(System.IO.Path.Combine(folder, "image"), "png"), System.Drawing.Imaging.ImageFormat.Png);

                //すでに保存されている枚数に応じて番号をつけて保存
                bmp.Save(pageDirectory1 + "\\ImageTB\\" + pageNameMain.Replace(".isf", "") + ".png", System.Drawing.Imaging.ImageFormat.Png);
                //bmp.Save(System.IO.Path.Combine(folder, pageNameTB + "_" + pageNameTB + ".png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

    }
}
