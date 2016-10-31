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
using System.IO;

using System.Runtime.InteropServices;
using System.Management;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// CaptureWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CaptureWindow : Window
    {
        private Point _position;
        private bool _trimEnable = false;
        string epubDirectory;
        string epubName;
        string pagePath;
        string[] epubImage = new string[1024];
        string savedFilePath;
        string subjectName;
        string unitName;
        bool needToClip;
        string savePath;

        public CaptureWindow()
        {
            InitializeComponent();
        }

        //初期処理
        public void init(string epubDirectory, string epubName, string pagePath, string subjectName, string unitName, bool need)
        {
            this.epubDirectory = epubDirectory.Replace(".epub","");
            this.epubName = epubName;
            this.pagePath = pagePath;
            this.subjectName = subjectName;
            this.unitName = unitName;
            needToClip = need;
            for (int i = 0; i < epubImage.Length; i++) 
            {
                epubImage[i] = null;
            }
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

        private void DrawingPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var path = sender as System.Windows.Shapes.Path;
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
            var path = sender as System.Windows.Shapes.Path;
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

            //画面を閉じる
            this.Close();
        }

        private void DrawingPath_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_trimEnable)
                return;

            var path = sender as System.Windows.Shapes.Path;
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

                string pageFileName = pagePath.Replace(epubDirectory + "\\OEBPS\\image\\", "");
                string captureFileDirectory;
                string[] files;

                //ファイル共有するならこっち
                if (Directory.Exists(GetUniversalName(@"\\MCDYNA20\ContentsData")))
                {
                    captureFileDirectory =
                            @"\\MCDYNA20\ContentsData\Annotation\" + ((PNGWindow)this.Owner).user.GetId() + "\\" + ((PNGWindow)this.Owner).epubFileName.Replace(".epub", "");

                    string unc_path = GetUniversalName(captureFileDirectory);

                    //自分のアノテーションファイルの置き場がなければつくる
                    if (!Directory.Exists(unc_path))
                    {
                        Directory.CreateDirectory(unc_path);
                    }

                    //保存先にページ.pngが何枚保存されているか調べる
                    files = System.IO.Directory.GetFiles(captureFileDirectory, pageFileName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);
                }

                //しないならこっち
                else
                {
                    captureFileDirectory =
                        ((PNGWindow)this.Owner).epubDirectory.Replace("epub", "Annotation\\") + ((PNGWindow)this.Owner).user.GetId() + "\\" + ((PNGWindow)this.Owner).epubFileName.Replace(".epub", "");


                    //自分のアノテーションファイルの置き場がなければつくる
                    if (!Directory.Exists(captureFileDirectory))
                    {
                        Directory.CreateDirectory(captureFileDirectory);
                    }

                    //保存先にページ.pngが何枚保存されているか調べる
                    files = System.IO.Directory.GetFiles(captureFileDirectory, pageFileName + "*" + ".png", System.IO.SearchOption.TopDirectoryOnly);
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
                savePath = captureFileDirectory + "\\" + pageFileName + "_" + k + ".png";

                //すでに保存されている枚数に応じて番号をつけて保存
                bmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Png);                
            }

            //「おくる」ボタンをクリックしていた場合は、クリップボードにコピー
            if (needToClip)
            {
                MemoryStream data = new MemoryStream(File.ReadAllBytes(savedFilePath));
                WriteableBitmap bmpim = new WriteableBitmap(BitmapFrame.Create(data));
                data.Close();
                Clipboard.SetImage(bmpim);
                bool a = File.Exists(savePath);
                //File.Delete(savedFilePath);
                
                DinoMainWindow dinoMain = new DinoMainWindow();
                dinoMain.Show();
                dinoMain.pasteClipBorad(subjectName, unitName, savedFilePath);
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