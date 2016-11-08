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

using WebKit;
using WebKit.JSCore;
using System.Drawing;
using System.Windows.Interop;

using System.Windows.Ink;
using System.IO;
using System.Drawing.Imaging;

using System.Runtime.InteropServices;
using System.Management;


namespace epubReader4._0_Dino
{
    /// <summary>
    /// AddinWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddinWindow : Window
    {
        public AddinWindow()
        {
            InitializeComponent();
        }

        //追加教材を閲覧するWindow
        //WebKitのインスタンス
        public WebKitBrowser webBrowser1;
        string filePath;
        string fileName;

        //アノテーションに関する変数
        System.Windows.Point startP = new System.Windows.Point();
        List<StrokeLine> strokeLines = new List<StrokeLine>();
        List<System.Windows.Point> points;
        System.Windows.Media.Color color = new System.Windows.Media.Color();
        DrawingAttributes inkDA = new DrawingAttributes();
        bool isFreeLine = true;
        bool dragging = false;
        int strokeId = 0;
        int counter = 0;

        //新たにキャプチャが必要かどうかを示すflag
        bool needToCapturenow = false;

        //動作のログ
        List<LearningLog> learningLogs = new List<LearningLog>();

        //ユーザ情報を表すオブジェクト
        User user = new User();


        //初期処理
        public void init(string addinType, string filePath, string fileName, User user)
        {
            this.filePath = filePath;
            this.fileName = fileName;
            this.user = user;

            //if(addinType.Equals("administrator"))
            //{
            ////WebKitのインスタンスを作成する
            //webBrowser1 = new WebKitBrowser();

            ////WebKitのインスタンスをWindowsFormsHostに割り当てる
            //windowsFormsHost1.Child = webBrowser1;

            ////imageは使わないので非表示に
            //image1.Visibility = System.Windows.Visibility.Hidden;

            ////読み込ませる
            //webBrowser1.Url = new Uri(filePath);
            //}
            //else
            //{
            ////WindowsFormsHostは使わないので非表示に
            //windowsFormsHost1.Visibility = System.Windows.Visibility.Hidden;

            //image1に表示
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(filePath);
            bmp.EndInit();
            image1.Source = bmp;
            
            //}

            //色の初期値として黒を指定
            color = System.Windows.Media.Color.FromArgb(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
            inkDA.Color = color;

            //太さの初期値は5
            inkDA.Width = 5;
            inkDA.Height = 5;

            inkCanvas1.DefaultDrawingAttributes = inkDA;
            inkCanvas1.AllowDrop = true;

            //inkCanvas1にマウスイベントを設定
            inkCanvas1.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(inkCanvas1_MouseDown), true);
            inkCanvas1.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(inkCanvas1_MouseMove), true);
            inkCanvas1.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(inkCanvas1_MouseUp), true);
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;

        }

        //描く処理(マウスダウン)
        private void inkCanvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement el = sender as UIElement;
            Console.WriteLine("まうすがおされたよ");

            //自由線
            if (isFreeLine)
            {
                dragging = true;
                points = new List<System.Windows.Point>();
                points.Add(e.GetPosition(el));
            }

            //直線
            else
            {
                dragging = true;
                startP = e.GetPosition(el);
            }
        }

        //描く処理(マウスムーブ)
        private void inkCanvas1_MouseMove(object sender, MouseEventArgs e)
        {
            UIElement el = sender as UIElement;
            Console.WriteLine("dragging = " + dragging + " in mousemove");

            //かくモード
            if (dragging && isFreeLine)
            {
                points.Add(e.GetPosition(el));
                Console.WriteLine(e.GetPosition(el));
                counter++;
            }

            //直線モード
            else if (!isFreeLine && dragging)
            {
                inkCanvas1.Strokes.Clear();
                drawAll();

                //点の情報を集め、始点と現在の点をむすぶ
                StylusPointCollection spc = new StylusPointCollection();
                spc.Add(new StylusPoint(startP.X, startP.Y));
                spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
                Stroke stroke = new Stroke(spc, inkDA);
                inkCanvas1.Strokes.Add(stroke);

                counter++;
            }
        }

        //描く処理(マウスアップ)
        private void inkCanvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            UIElement el = sender as UIElement;
            Console.WriteLine("まうすがはなれたよ");

            if (isFreeLine && dragging && counter > 3)
            {
                points.Add(e.GetPosition(el));

                //配列strokeLinesに追加
                StrokeLine strokeLine = new StrokeLine();
                strokeLine.SetPoints(points);
                strokeLine.SetColor(color);
                strokeLine.SetWidth((int)slider1.Value);
                strokeLine.SetDownNow(false);
                strokeLine.SetInSpace(false);
                strokeLines.Add(strokeLine);

                //動作ログに記録
                LearningLog log = new LearningLog();
                log.SetStrokeId(strokeId.ToString());
                log.SetBehavior("draw");
                learningLogs.Add(log);

                dragging = false;
                strokeId++;

                Console.WriteLine(counter.ToString());
                counter = 0;
            }

            else if (!isFreeLine && dragging && counter > 3)
            {
                inkCanvas1.Strokes.Clear();
                drawAll();

                //点の情報を集め、始点と現在の点をむすぶ
                StylusPointCollection spc = new StylusPointCollection();
                spc.Add(new StylusPoint(startP.X, startP.Y));
                spc.Add(new StylusPoint(e.GetPosition(el).X, e.GetPosition(el).Y));
                Stroke stroke = new Stroke(spc, inkDA);
                inkCanvas1.Strokes.Add(stroke);

                //pointsに始点と現在の点を格納
                points = new List<System.Windows.Point>();
                points.Add(startP);
                points.Add(e.GetPosition(el));

                //配列strokeLinesについか
                StrokeLine strokeLine = new StrokeLine();
                strokeLine.SetPoints(points);
                strokeLine.SetColor(color);
                strokeLine.SetWidth((int)slider1.Value);
                strokeLine.SetDownNow(false);
                strokeLine.SetInSpace(false);

                strokeLines.Add(strokeLine);

                dragging = false;
                counter = 0;
            }

            //１操作終わったので、新たにキャプチャが必要
            needToCapturenow = true;
        }

        //色変更
        private void colorChange(byte a, byte r, byte g, byte b)
        {
            color = System.Windows.Media.Color.FromArgb(a, r, g, b);
            inkDA.Color = color;
            inkCanvas1.DefaultDrawingAttributes = inkDA;
        }

        //黒 
        private void blackButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(0));
        }

        //赤
        private void redButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
        }

        //青
        private void blueButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
        }

        //緑
        private void greenButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
        }

        //黄色
        private void yellowButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(0));
        }

        //白
        private void whiteButton_Click(object sender, RoutedEventArgs e)
        {
            colorChange(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
        }

        //直線・曲線切り替え
        private void strokeModeChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (isFreeLine) //直線モードにする
            {
                strokeModeChangeButton.Content = "自由線";
                isFreeLine = false;

                inkCanvas1.EditingMode = InkCanvasEditingMode.None;
            }
            else //自由線モードにする
            {
                strokeModeChangeButton.Content = "直線";
                isFreeLine = true;

                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        //太さ変更
        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sl = sender as Slider;
            inkDA.Width = sl.Value;
            inkDA.Height = sl.Value;
            inkCanvas1.DefaultDrawingAttributes = inkDA;
        }

        //すべての線を再描画
        private void drawAll()
        {
            for (int i = 0; i < strokeLines.Count; i++)
            {
                StrokeLine sl = strokeLines[i];

                //消去済みでないストローク以外を再描画
                if (!sl.GetEreased())
                {
                    //線の色、幅を取得
                    DrawingAttributes DA = new DrawingAttributes();
                    DA.Color = sl.GetColor();
                    DA.Width = sl.GetWidth();
                    DA.Height = sl.GetWidth();
                    inkCanvas1.DefaultDrawingAttributes = DA;

                    //点の情報を集める
                    StylusPointCollection spc = new StylusPointCollection();
                    for (int j = 0; j < sl.GetPoints().Count; j++)
                    {
                        spc.Add(new StylusPoint(sl.GetPoints()[j].X, sl.GetPoints()[j].Y));
                    }
                    Stroke stroke = new Stroke(spc, DA);
                    inkCanvas1.Strokes.Add(stroke);
                }

            }

            //線のスタイルを戻す
            inkCanvas1.DefaultDrawingAttributes = inkDA;
        }

        //ひとつ戻る
        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //後ろからさかのぼって消えていない線を探す
                int i;
                for (i = strokeLines.Count - 1; i >= 0; i--)
                {
                    if (!strokeLines[i].GetEreased())
                    {
                        strokeLines[i].SetEreased(true);
                        strokeLines[i].SetEreasedTime(learningLogs.Count + 1);
                        inkCanvas1.Strokes.Clear();
                        drawAll();
                        break;
                    }
                }

                //動作ログに記録
                LearningLog log = new LearningLog();
                log.SetStrokeId(strokeLines[i].GetId().ToString());
                log.SetBehavior("erase");
                learningLogs.Add(log);

                //１操作終わったので、新たにキャプチャが必要
                needToCapturenow = true;
            }
            catch
            {
                MessageBox.Show("ERROR! 一つ戻るの処理過程でエラーが起きました。");
            }
        }

        //すべての線を消去
        private void clearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < strokeLines.Count(); i++)
                {
                    //ストローク一つひとつに、erase = trueをセット
                    strokeLines[i].SetEreased(true);

                    //本処理で初めてその線が消える場合のみ、erasedTimeをセット
                    if (strokeLines[i].GetEreasedTime() == -1)
                    {
                        strokeLines[i].SetEreasedTime(learningLogs.Count + 1);
                    }
                }

                //動作ログに記録。全消去の時はidの欄をallとする
                LearningLog log = new LearningLog();
                log.SetStrokeId("all");
                log.SetBehavior("erase");
                learningLogs.Add(log);

                //キャンバスをクリア
                inkCanvas1.Strokes.Clear();
            }
            catch
            {

            }
        }

        //とじる
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (needToCapturenow)
            {
                addinCapture();
            }

            this.Close();
        }

        //現在表示しているアドインに対する書き込み一覧を表示する（とりあえず、filesを渡すだけ。。。）
        private void showCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            //ファイル置き場のパス
            string searchDirectory = filePath + "\\..\\Annotation\\" + user.GetId();

            //保存先にページ名.pngが何枚保存されているか調べる
            string[] files = System.IO.Directory.GetFiles(searchDirectory, "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);

            PNGShowAnnotationWindow psaw = new PNGShowAnnotationWindow();
            psaw.Owner = this;
            psaw.Show();
            psaw.init(searchDirectory + "\\" + fileName + "_000.png", files, 1);
        }

        //現在の状態をキャプチャする（アノテーションの保存先は、とりあえずここ。。。）
        private void captureButton_Click(object sender, RoutedEventArgs e)
        {
            if(needToCapturenow)
            {
                addinCapture();
            }
        }

        //スクショの処理（アノテーションの保存先は、とりあえずここ。。。）
        private void addinCapture()
        {
            //ファイル置き場がなければ今作る
            string searchDirectory = filePath + "\\..\\Annotation\\" + user.GetId();
            if(!Directory.Exists(searchDirectory))
            {
                Directory.CreateDirectory(searchDirectory);
            }

            //まず、保存先にページ名.pngが何枚保存されているか調べる
            string[] files = System.IO.Directory.GetFiles(searchDirectory, "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);

            int i = 0;
            string k = "";
            foreach (string f in files)
            {
                //MessageBox.Show(f);
                i++;
            }
            if (i < 100)
            {
                k = "0" + i;
                if (i < 10)
                {
                    k = "0" + k;
                }
            }

            //ここからキャプチャの処理
            System.Windows.Point browserLeftTop = new System.Windows.Point();
            System.Drawing.Size canvasSize = new System.Drawing.Size();

            //キャプチャ範囲の指定
            browserLeftTop = inkCanvas1.PointToScreen(new System.Windows.Point(0.0, 0.0));
            canvasSize.Height = (int)inkCanvas1.RenderSize.Height;
            canvasSize.Width = (int)inkCanvas1.RenderSize.Width;


            System.Drawing.Rectangle rc = new System.Drawing.Rectangle();
            rc.Location = new System.Drawing.Point((int)browserLeftTop.X, (int)browserLeftTop.Y);
            rc.Size = canvasSize;

            ImageBrush ib = new ImageBrush();

            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size);

                    ib.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(
                        bmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }

                //スクリーンショットの保存
                bmp.Save(searchDirectory + "\\" + fileName + "_" + k + ".png", System.Drawing.Imaging.ImageFormat.Png);
                
                //MessageBox.Show("スクショ完了");
                needToCapturenow = false;
            }
        }
    }
}