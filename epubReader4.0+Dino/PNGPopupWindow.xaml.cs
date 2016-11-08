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

using System.Windows.Ink;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using System.Runtime.InteropServices;
using System.Management;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// PNGPopupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PNGPopupWindow : Window
    {
        public PNGPopupWindow()
        {
            InitializeComponent();
        }

        //ファイル操作に関する変数
        string popupPath;
        string thawPath;
        string annotationDirectory;
        string popupFileName;

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
        public void init(string popupPath, string thawPath, string popupFileName, User user)
        {
            this.popupPath = popupPath;
            this.thawPath = thawPath;
            this.popupFileName = popupFileName;
            this.user = user;

            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(popupPath);
            bmp.EndInit();
            image1.Source = bmp;

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

            //前に描画した情報があれば読み込む
            try
            {
                RoadAnnotateRecord();
                drawAll();
            }
            catch
            {
                //MessageBox.Show("再読み込み失敗");
            }
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

        //ストローク情報の保存
        private void SaveAnnotateRecord()
        {
            //動作ログの保存と、ストローク情報の保存
            if (!Directory.Exists(thawPath + "\\Strokes"))
            {
                Directory.CreateDirectory(thawPath + "\\Strokes");
            }
            if (!Directory.Exists(thawPath + "\\LearningLog"))
            {
                Directory.CreateDirectory(thawPath + "\\LearningLog");
            }

            //XmlSerializerオブジェクトを作成
            //オブジェクトの型を指定する
            System.Xml.Serialization.XmlSerializer strokeSerializer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<StrokeLine>));
            System.Xml.Serialization.XmlSerializer logSerializer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<LearningLog>));

            //MessageBox.Show(thawPath + "\\strokes\\" + pageContent[currentPageNum].Replace(thawPath + "\\OEBPS\\image\\" , ""));

            //書き込むファイルを開く
            StreamWriter ssw = new StreamWriter(
                thawPath + "\\Strokes\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml", false, new System.Text.UTF8Encoding(false));
            StreamWriter lsw = new StreamWriter(
                thawPath + "\\LearningLog\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml", false, new System.Text.UTF8Encoding(false));

            //シリアル化しxmlファイルに保存
            strokeSerializer.Serialize(ssw, strokeLines);
            logSerializer.Serialize(lsw, learningLogs);

            //ファイルを閉じる
            ssw.Close();
            lsw.Close();
        }

        //ストローク情報の読み込み
        private void RoadAnnotateRecord()
        {
            if (!Directory.Exists(thawPath + "\\Strokes"))
            {
                Directory.CreateDirectory(thawPath + "\\Strokes");
            }
            if (!Directory.Exists(thawPath + "\\LearningLog"))
            {
                Directory.CreateDirectory(thawPath + "\\LearningLog");
            }

            //保存元のファイル名
            string strokeXmlPath = thawPath + "\\Strokes\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml";
            string logXmlPath = thawPath + "\\LearningLog\\" + popupPath.Replace(thawPath + "\\PopupImage\\", "") + ".xml";

            //XmlSerializerオブジェクトを作成
            System.Xml.Serialization.XmlSerializer strokeSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<StrokeLine>));
            System.Xml.Serialization.XmlSerializer logSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<LearningLog>));


            //読み込むファイルを開く
            System.IO.StreamReader ssr = new System.IO.StreamReader(strokeXmlPath, new System.Text.UTF8Encoding(false));
            System.IO.StreamReader lsr = new System.IO.StreamReader(logXmlPath, new System.Text.UTF8Encoding(false));

            //XMLファイルから読み込み、逆シリアル化する
            strokeLines = (List<StrokeLine>)strokeSerializer.Deserialize(ssr);
            learningLogs = (List<LearningLog>)logSerializer.Deserialize(lsr);

            //ファイルを閉じる
            ssr.Close();
            lsr.Close();
        }

        //ストローク再生画面の表示
        private void reviewButton_Click(object sender, RoutedEventArgs e)
        {
            PNGPopupReviewWindow pprw = new PNGPopupReviewWindow();
            pprw.Owner = this;
            pprw.Show();
            pprw.init(popupPath, strokeLines, learningLogs);
        }

        //キャプチャボタンを押下した時の処理
        private void captureButton_Click(object sender, RoutedEventArgs e)
        {
            ImageCaptureAll();
            MessageBox.Show("保存しました！");
        }

        //キャプチャ一覧の表示
        private void showCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            PNGSelectAnnotationWindow pslaw = new PNGSelectAnnotationWindow();
            pslaw.Owner = this;
            pslaw.Show();
            pslaw.init(popupFileName + ".png", ((PNGWindow)this.Owner).epubDirectory, ((PNGWindow)this.Owner).epubFileName, ((PNGWindow)this.Owner).user);

        }

        //閉じるボタン
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            //ストローク情報の保存
            SaveAnnotateRecord();

            //新たにキャプチャが必要なら保存する
            if (needToCapturenow)
            {
                ImageCaptureAll();
            }

            this.Close();
        }

        //ポップアップ紙面全体をキャプチャ
        public void ImageCaptureAll()
        {
            string[] files;

            //ファイル共有するならこっち
            if (Directory.Exists(GetUniversalName(@"\\MCDYNA20\ContentsData")))
            {
                annotationDirectory =
                        @"\\MCDYNA20\ContentsData\Annotation\" + user.GetId() + "\\" + ((PNGWindow)this.Owner).epubFileName.Replace(".epub", "");

                string unc_path = GetUniversalName(annotationDirectory);

                //自分のアノテーションファイルの置き場がなければつくる
                if (!Directory.Exists(unc_path))
                {
                    Directory.CreateDirectory(unc_path);
                }

                //保存先にページ.pngが何枚保存されているか調べる
                files = System.IO.Directory.GetFiles(annotationDirectory,  popupFileName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);
            }

            //しないならこっち
            else
            {
                annotationDirectory =
                    ((PNGWindow)this.Owner).epubDirectory.Replace("epub", "Annotation\\") + user.GetId() + "\\" + ((PNGWindow)this.Owner).epubFileName.Replace(".epub", "");


                //自分のアノテーションファイルの置き場がなければつくる
                if (!Directory.Exists(annotationDirectory))
                {
                    Directory.CreateDirectory(annotationDirectory);
                }

                //保存先にページ.pngが何枚保存されているか調べる
                files = System.IO.Directory.GetFiles(annotationDirectory, popupFileName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);
            }
            string k = null;


            int i = 0;
            foreach (string f in files)
            {
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

            //imageを保存
            string savePath = annotationDirectory + "\\" + popupFileName + ".png_" + k + ".png";
            CaptureScreen(savePath);

            //保存必要性をリセット
            needToCapturenow = false;
        }

        //スクリーンショットの処理
        public void CaptureScreen(string saveFileName)
        {
            System.Windows.Point imageLeftTop = new System.Windows.Point();
            System.Drawing.Size imageSize = new System.Drawing.Size();

            imageLeftTop = inkCanvas1.PointToScreen(new System.Windows.Point(0.0, 0.0));
            imageSize.Height = (int)inkCanvas1.RenderSize.Height;
            imageSize.Width = (int)inkCanvas1.RenderSize.Width;


            System.Drawing.Rectangle rc = new System.Drawing.Rectangle();
            rc.Location = new System.Drawing.Point((int)imageLeftTop.X, (int)imageLeftTop.Y);
            rc.Size = imageSize;

            ImageBrush ib = new ImageBrush();

            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size);

                    ib.ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        bmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }

                //スクリーンショットの保存
                bmp.Save(saveFileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        //以下、ファイル共有関係のコード（おれもよくわかんない笑）
        /* 
        * WNetGetUniversalNameをインポートする
        */
        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        static extern int
            WNetGetUniversalName(
            string lpLocalPath,                                 // ネットワーク資源のパス 
            [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,      // 情報のレベル
            IntPtr lpBuffer,                                    // 名前バッファ
            [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize  // バッファのサイズ
        );


        /*
         * dwInfoLevelに指定するパラメータ
         *  lpBuffer パラメータが指すバッファで受け取る構造体の種類を次のいずれかで指定
         */
        const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
        const int REMOTE_NAME_INFO_LEVEL = 0x00000002; //こちらは、テストしていない


        /*
         * lpBufferで受け取る構造体
         */
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct UNIVERSAL_NAME_INFO
        {
            public string lpUniversalName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct _REMOTE_NAME_INFO  //こちらは、テストしていない
        {
            string lpUniversalName;
            string lpConnectionName;
            string lpRemainingPath;
        }

        /* エラーコード一覧
        * WNetGetUniversalName固有のエラーコード
        *   http://msdn.microsoft.com/ja-jp/library/cc447067.aspx
        * System Error Codes (0-499)
        *   http://msdn.microsoft.com/en-us/library/windows/desktop/ms681382(v=vs.85).aspx
        */
        const int NO_ERROR = 0;
        const int ERROR_NOT_SUPPORTED = 50;
        const int ERROR_MORE_DATA = 234;
        const int ERROR_BAD_DEVICE = 1200;
        const int ERROR_CONNECTION_UNAVAIL = 1201;
        const int ERROR_NO_NET_OR_BAD_PATH = 1203;
        const int ERROR_EXTENDED_ERROR = 1208;
        const int ERROR_NO_NETWORK = 1222;
        const int ERROR_NOT_CONNECTED = 2250;

        /*
        * UNC変換ロジック本体
        */
        public static string GetUniversalName(string path_src)
        {
            string unc_path_dest = path_src; //解決できないエラーが発生した場合は、入力されたパスをそのまま戻す
            int size = 1;

            /*
             * 前処理
             *   意図的に、ERROR_MORE_DATAを発生させて、必要なバッファ・サイズ(size)を取得する。
             */
            //1バイトならば、確実にERROR_MORE_DATAが発生するだろうという期待。
            IntPtr lp_dummy = Marshal.AllocCoTaskMem(size);

            //サイズ取得をトライ
            int apiRetVal = WNetGetUniversalName(path_src, UNIVERSAL_NAME_INFO_LEVEL, lp_dummy, ref size);

            //ダミーを解放
            Marshal.FreeCoTaskMem(lp_dummy);
            /*
                        * UNC変換処理
                        */
            switch (apiRetVal)
            {
                case ERROR_MORE_DATA:
                    //受け取ったバッファ・サイズ(size)で再度メモリ確保
                    IntPtr lpBufUniversalNameInfo = Marshal.AllocCoTaskMem(size);

                    //UNCパスへの変換を実施する。
                    apiRetVal = WNetGetUniversalName(path_src, UNIVERSAL_NAME_INFO_LEVEL, lpBufUniversalNameInfo, ref size);

                    //UNIVERSAL_NAME_INFOを取り出す。
                    UNIVERSAL_NAME_INFO a = (UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(lpBufUniversalNameInfo, typeof(UNIVERSAL_NAME_INFO));

                    //バッファを解放する
                    Marshal.FreeCoTaskMem(lpBufUniversalNameInfo);

                    if (apiRetVal == NO_ERROR)
                    {
                        //UNCに変換したパスを返す
                        unc_path_dest = a.lpUniversalName;
                    }
                    else
                    {
                        //MessageBox.Show(path_src +"ErrorCode:" + apiRetVal.ToString());
                    }
                    break;

                case ERROR_BAD_DEVICE: //すでにUNC名(\\servername\test)
                case ERROR_NOT_CONNECTED: //ローカル・ドライブのパス(C:\test)
                    //MessageBox.Show(path_src +"\nErrorCode:" + apiRetVal.ToString());
                    break;
                default:
                    //MessageBox.Show(path_src + "\nErrorCode:" + apiRetVal.ToString());
                    break;

            }

            return unc_path_dest;
        }
    }
}