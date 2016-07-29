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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.Management;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DinoMainWindow : Window
    {
        StylusPointCollection points = new StylusPointCollection();
        StylusPointCollection points2 = new StylusPointCollection();
        DrawingAttributes i = new DrawingAttributes();
        int style = 0;//手書きと直線と選択で切り替える
        Stroke stroke1;
        //ColorDialog cd = new ColorDialog();
        // System.Windows.Media.Color color = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
        System.Windows.Point t;
        StrokeCollection temp = new StrokeCollection();
        bool drag = false;
        int s;
        int a;
        int textColor = 0;
        bool imageClickOK = false; //画像移動OKにする

        public DinoSubWindow subWindowPointer;

        //教科名とページ名
        string pageNameMain = null;
        string subjectNameMain = null;
        string unitNameMain = null;
        private int GRID_SIZE = 40;

        public DinoMainWindow()
        {
            InitializeComponent();
            inkCanvas1.EraserShape = new EllipseStylusShape(slider1.Value, slider1.Value);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BuildView();

            i.Width = 10;
            i.Height = 10;
            i.Color = Colors.Black;

            inkCanvas1.DefaultDrawingAttributes = i;

        }

        private void imageCanvas_Loaded(object sender, EventArgs e)
        {
            this.AllowDrop = true;
        }
        int x = 1;
        System.Windows.Controls.Image[] img = new System.Windows.Controls.Image[100];
        string pageDirectory1 = "";
        string today = "";
        string[] page = new string[100];
        string pageName1 = "";
        double imageX = 0;
        double imageY = 0;
        string type = null;
        string[] txtLine = new string[10];
        string epubFileName1;
        int epubCurrentPageLeft1;

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BuildView();
        }

        //罫線の幅と高さを決める
        private void gridSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GRID_SIZE = (int)gridSize.Value;
            BuildView();
        }

        private void BuildView()
        {
            lineCanvas.Children.Clear();

            Style lineStyle = this.FindResource("GridLineStyle") as Style;

            for (int k = 0; k < this.ActualWidth; k += GRID_SIZE)
            {

                Line line = new Line()
                {
                    X1 = k,
                    Y1 = 0,
                    X2 = k,
                    Y2 = this.ActualHeight,
                    Style = lineStyle
                };
                lineCanvas.Children.Add(line);
            }
            for (int k = 0; k < this.ActualHeight; k += GRID_SIZE)
            {
                Line line = new Line()
                {
                    X1 = 0,
                    Y1 = k,
                    X2 = this.ActualWidth,
                    Y2 = k,
                    Style = lineStyle
                };
                lineCanvas.Children.Add(line);
            }
        }

        // フォルダを作成
        private void button3_Click(object sender, RoutedEventArgs e)
        {

            string dum1 = DateTime.Today.ToString();
            string[] dum2 = new string[2];
            dum2 = dum1.Split(' ');
            string[] dum3 = new string[3];
            dum3 = dum2[0].Split('/');
            today = dum3[0] + dum3[1] + dum3[2];

            FileSystem.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Note\\" + today);
        }

        private void drawStyle_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            style = 0;
            //drawStyle.Content = "手書き";
            i.IsHighlighter = false;
            i.Color = Colors.Black;
            textColor = 0;
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void drawStyle3_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            style = 1;
            //drawStyle.Content = "直線";
            i.IsHighlighter = false;
            inkCanvas1.EditingMode = InkCanvasEditingMode.None;
        }

        private void drawStyle1_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            style = 2;
            //drawStyle.Content = "選択";
            inkCanvas1.EditingMode = InkCanvasEditingMode.Select;
        }

        private void drawStyle2_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            style = 3;
            //drawStyle.Content = "消しゴム";
            inkCanvas1.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;

        }

        //太さ変更　スライダー
        private void slider1_ValueChanged(object sender, RoutedEventArgs e)
        {
            if (inkCanvas1 == null)
                return;

            //ペンの色の幅と高さを変化
            i.Width = slider1.Value;
            i.Height = slider1.Value;

            inkCanvas1.DefaultDrawingAttributes = i;

            inkCanvas1.EraserShape = new EllipseStylusShape(slider1.Value, slider1.Value);

        }

        // 蛍光黄色に変更
        private void lineYellow_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            i.Color = Colors.Yellow;
            i.IsHighlighter = true;
            inkCanvas1.DefaultDrawingAttributes = i;
            style = 1;

            inkCanvas1.EditingMode = InkCanvasEditingMode.None;
        }

        //蛍光ピンクに変更
        private void linePink_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            i.Color = Colors.Pink;
            i.IsHighlighter = true;
            inkCanvas1.DefaultDrawingAttributes = i;
            style = 1;

            inkCanvas1.EditingMode = InkCanvasEditingMode.None;
        }

        //しゅうーーーりょーーーーーう
        private void close_Click(object sender, RoutedEventArgs e)
        {
            System.IO.FileStream fs = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Create);
            inkCanvas1.Strokes.Save(fs);
            // System.IO.File.Copy(@"C:\Users\a111414\Documents\お絵かき\"+ x + ".isf");

            CaptureScreen();

            fs.Close();

            this.Close();
        }

        //かく
        private void inkCanvas1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.MessageBox.Show(imageClickOK + "");
            if (imageClickOK)
            {
                drag = false;
                var path = sender as System.Windows.Shapes.Path;
                var point = e.GetPosition(path);
                imageX = point.X;
                imageY = point.Y;
            }
            else
            {
                drag = true;
                if (i.IsHighlighter == false && drag == true)
                {
                    if (textColor == 0)
                    {
                        i.Color = Colors.Black;
                        inkCanvas1.DefaultDrawingAttributes = i;
                    }
                    else if (textColor == 1)
                    {
                        i.Color = Colors.Red;
                        inkCanvas1.DefaultDrawingAttributes = i;
                    }
                    else if (textColor == 2)
                    {
                        i.Color = Colors.Blue;
                        inkCanvas1.DefaultDrawingAttributes = i;
                    }
                }
                if (style == 1 && drag == true)
                {
                    temp = new StrokeCollection(inkCanvas1.Strokes);
                    points = new StylusPointCollection();
                    System.Windows.Point p = e.GetPosition(inkCanvas1);
                    t = p;
                    points.Add(new StylusPoint(p.X, p.Y));
                    s = 0;
                }
            }
        }

        private void inkCanvas1_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (imageClickOK)
            {

            }
            else
            {
                if (style == 1 && drag == true)
                {

                    points2 = new StylusPointCollection();
                    System.Windows.Point p = e.GetPosition(inkCanvas1);
                    points2.Add(new StylusPoint(t.X, t.Y));
                    points2.Add(new StylusPoint(p.X, p.Y));
                    stroke1 = new Stroke(points2, inkCanvas1.DefaultDrawingAttributes);
                    if (s != 0)
                    {
                        inkCanvas1.Strokes.Remove(inkCanvas1.Strokes.Last());
                    }
                    inkCanvas1.Strokes.Add(stroke1.Clone());
                    s++;
                    //inkCanvas1.Strokes.Remove(inkCanvas1.Strokes.Last());
                }
            }
        }

        private void inkCanvas1_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (imageClickOK)
            {
                inkCanvas1.EditingMode = InkCanvasEditingMode.None;

                var path = sender as System.Windows.Shapes.Path;
                var point = e.GetPosition(path);
                double imageX2 = point.X;
                double imageY2 = point.Y;

                double dx = imageX2 - imageX;
                double dy = imageY2 - imageY;
                double left = img[x].Margin.Left + dx;
                double top = img[x].Margin.Top + dy;
                //double right = imageCanvas.Width - (left + img[a].Width);
                //double bottom = imageCanvas.Height - (top + img[a].Height);
                img[x].Margin = new Thickness(left, top, 0, 0);
            }
            else
            {
                if (style == 1 && drag == true)
                {

                    System.Windows.Point p = e.GetPosition(inkCanvas1);
                    points.Add(new StylusPoint(p.X, p.Y));
                    stroke1 = new Stroke(points, inkCanvas1.DefaultDrawingAttributes);
                    inkCanvas1.Strokes = temp;
                    inkCanvas1.Strokes.Add(stroke1.Clone());

                    //inkCanvas1.Strokes.
                    points = null;
                    stroke1 = null;
                }
                drag = false;
            }

        }

        // いろいろなものを開いちゃい♡
        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;

            Microsoft.Win32.OpenFileDialog dlgOpen = new Microsoft.Win32.OpenFileDialog();

            dlgOpen.Filter = "png(*.png)|*.png|" + "bitmap(*.bmp)|*.bmp|" + "jpeg(*.jpeg)|*.jpeg";

            if ((bool)dlgOpen.ShowDialog())
            {
                string extention = System.IO.Path.GetExtension(dlgOpen.FileName);
                if (extention == ".isf")
                {
                    using (System.IO.FileStream fs =
                        new System.IO.FileStream(dlgOpen.FileName, System.IO.FileMode.Open))
                    {
                        inkCanvas1.Strokes = new System.Windows.Ink.StrokeCollection(fs);
                    }
                }
                else
                {
                    /*ImageBrush imageblush = new ImageBrush();
                    imageblush.ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri(dlgOpen.FileName));
                    inkCanvas1.Background = imageblush;*/

                    //画像を開くnew!

                    BitmapImage bi = new BitmapImage();
                    using (FileStream fs = new FileStream(dlgOpen.FileName, FileMode.Open, FileAccess.Read))
                    {
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = fs;
                        bi.EndInit();
                    }
                    //Imageコントロールにセットする
                    a++;
                    img[x] = new System.Windows.Controls.Image();
                    imageCanvas.Children.Add(img[x]);
                    img[x].Height = 800;
                    img[x].Width = 800;
                    img[x].Source = bi;
                    img[x].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);
                    //画像のページの位置を格納する
                    page[x] = pageName1;

                    if (dlgOpen.FileName.Contains(".png"))
                    {
                        type = ".png";
                    }
                    else if (dlgOpen.FileName.Contains(".bmp"))
                    {
                        type = ".bmp";
                    }
                    else if (dlgOpen.FileName.Contains(".jpeg"))
                    {
                        type = ".jpeg";
                    }
                    if (File.Exists(pageDirectory1 + "\\image\\" + x + type))
                    {
                        File.Delete(pageDirectory1 + "\\image\\" + x + type);

                    }
                    System.IO.File.Copy(dlgOpen.FileName, pageDirectory1 + "\\image\\" + x + type);

                    // txtファイルを生成
                    StreamWriter sw = new StreamWriter(pageDirectory1 + "\\image\\" + x + ".txt");
                    {
                        txtLine[0] = img[x].Width.ToString();
                        txtLine[1] = img[x].Height.ToString();
                        txtLine[2] = img[x].Margin.Left.ToString();
                        txtLine[3] = img[x].Margin.Top.ToString();

                        sw.WriteLine(txtLine[0]);
                        sw.WriteLine(txtLine[1]);
                        sw.WriteLine(txtLine[2]);
                        sw.WriteLine(txtLine[3]);
                    }
                    sw.Close();
                }
            }
        }
        Rect rectBounds;
        
        // 画像を保存
        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;

            CaptureScreen();

            Microsoft.Win32.SaveFileDialog dlgSave = new Microsoft.Win32.SaveFileDialog();
            dlgSave.Filter = "bitmap(*.bmp)|*.bmp|" + "jpeg(*.jpeg)|*.jpeg|" + "png(*.png)|*.png";
            dlgSave.AddExtension = true;

            if ((bool)dlgSave.ShowDialog())
            {
                string extension = System.IO.Path.GetExtension(dlgSave.FileName).ToUpper();
                rectBounds = new Rect(0, 0, 0, 0);


                //Rect rectBounds = inkCanvas1.Strokes.GetBounds();

                DrawingVisual dv = new DrawingVisual();
                DrawingContext dc = dv.RenderOpen();
                dc.PushTransform(new TranslateTransform(-rectBounds.X, -rectBounds.Y));

                dc.DrawRectangle(inkCanvas1.Background, null, rectBounds);

                inkCanvas1.Strokes.Draw(dc);
                dc.Close();

                RenderTargetBitmap rtb = new RenderTargetBitmap((int)rectBounds.Width, (int)rectBounds.Height, 96, 96, PixelFormats.Default);
                rtb.Render(dv);

                BitmapEncoder enc = null;
                //if (extension != ".ISF")
                //{
                switch (extension)
                {
                    case ".BMP":
                        enc = new BmpBitmapEncoder();
                        break;
                    case ".JPEG":
                        enc = new JpegBitmapEncoder();
                        break;
                    case ".PNG":
                        enc = new PngBitmapEncoder();
                        break;
                }
                //System.Windows.Forms.MessageBox.Show(inkCanvas1.Width.ToString());
                if (enc != null)
                {
                    enc.Frames.Add(BitmapFrame.Create(rtb));
                    System.IO.Stream stream = System.IO.File.Create(dlgSave.FileName);
                    enc.Save(stream);
                    stream.Close();
                }
                //}
                //else
                //{
                //    inkCanvas1.Strokes.Clear();
                //    using(System.IO.FileStream(dlgOpen))
                //}
            }
        }

        //ストロークを保存
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;

            Microsoft.Win32.SaveFileDialog dlgSave = new Microsoft.Win32.SaveFileDialog();
            dlgSave.Filter = "InkSerializedFormat(*.isf)|*.isf";
            dlgSave.AddExtension = true;

            if ((bool)dlgSave.ShowDialog())
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(dlgSave.FileName, System.IO.FileMode.Create))
                {
                    inkCanvas1.Strokes.Save(fs);
                }

            }
        }

        //次のページ
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            CaptureScreen();

            System.IO.FileStream fs = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Create);
            inkCanvas1.Strokes.Save(fs);
            fs.Close();
            // System.IO.File.Copy(@"C:\Users\a111414\Documents\お絵かき\"+ x + ".isf");

            x = x + 1;
            pageNameMain = x + ".isf";
            pageName1 = pageDirectory1 + "\\" + x + ".isf";


            inkCanvas1.Strokes.Clear();
            if (File.Exists(pageDirectory1 + "\\image\\" + (x - 1) + ".png"))
            {
                img[x - 1].Source = null;
            }

            if (System.IO.File.Exists(pageDirectory1 + "\\" + x + ".isf"))
            {
                System.IO.FileStream fs2 = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Open);
                inkCanvas1.Strokes = new StrokeCollection(fs2);
                fs2.Close();
                if (File.Exists(pageDirectory1 + "\\image\\" + x + ".png"))
                {
                    /*if ((pageDirectory1 + "\\image\\" + x + type).Contains(".png"))
                    {
                        type = ".png";
                    }
                    else if ((pageDirectory1 + "\\image\\" + x + type).Contains(".bmp"))
                    {
                        type = ".bmp";
                    }
                    else if ((pageDirectory1 + "\\image\\" + x + type).Contains(".jpeg"))
                    {
                        type = ".jpeg";
                    }*/
                    BitmapImage bi = new BitmapImage();
                    //System.Windows.MessageBox.Show(x.ToString());
                    int b = x;
                    using (FileStream fs3 = new FileStream(pageDirectory1 + "\\image\\" + x + ".png", FileMode.Open, FileAccess.Read))
                    {
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = fs3;
                        bi.EndInit();
                    }

                    StreamReader txt = new StreamReader(pageDirectory1 + "\\image\\" + x + ".txt");
                    string txtAll = txt.ReadToEnd();
                    txt.Close();

                    string[] txtLine = new string[10];
                    string[] newtxtLine = new string[10];

                    txt = new StreamReader(pageDirectory1 + "\\image\\" + x + ".txt");
                    {
                        txtLine[0] = txt.ReadLine();
                        txtLine[1] = txt.ReadLine();
                        txtLine[2] = txt.ReadLine();
                        txtLine[3] = txt.ReadLine();

                    }
                    txt.Close();

                    img[b] = new System.Windows.Controls.Image();
                    imageCanvas.Children.Add(img[b]);
                    img[b].Width = int.Parse(txtLine[0]);
                    img[b].Height = int.Parse(txtLine[1]);
                    img[b].Margin = new Thickness(int.Parse(txtLine[2]), int.Parse(txtLine[3]), 0, 0);
                    img[b].Source = bi;
                    img[b].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);
                }
            }
        }

        //前のページ
        private void inkCanvas1_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            imageClickOK = false;
            drag = false;
            /* if (drag == true) {
                 drag = false;
                 if (style % 4 == 1) { inkCanvas1.Strokes = temp; }//dragまえにもどしちゃう
             }*/
        }

        //関係なさそう？
        private void invisibleButton_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            SolidColorBrush solidColorBrush = new SolidColorBrush();
            solidColorBrush.Color = System.Windows.Media.Color.FromArgb(0, 0, 0, 0);

            window1.Background = solidColorBrush;
            inkCanvas1.Background = solidColorBrush;

        }

        //前のページ
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            if (x > 1)
            {
                System.IO.FileStream fs = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Create);
                inkCanvas1.Strokes.Save(fs);
                // System.IO.File.Copy(@"C:\Users\a111414\Documents\お絵かき\"+ x + ".isf");
                fs.Close();

                CaptureScreen();

                x = x - 1;
                pageNameMain = x + ".isf";
                System.IO.FileStream fs1 = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Open);
                inkCanvas1.Strokes = new StrokeCollection(fs1);

                if (File.Exists(pageDirectory1 + "\\image\\" + (x + 1) + ".png"))
                {
                    img[x + 1].Source = null;
                }

                pageName1 = pageDirectory1 + "\\" + x + ".isf";
                if (File.Exists(pageDirectory1 + "\\image\\" + x + ".png"))
                {
                    /*if ((pageDirectory1 + "\\image\\" + x + type).Contains(".png"))
                    {
                        type = ".png";
                    }
                    else if ((pageDirectory1 + "\\image\\" + x + type).Contains(".bmp"))
                    {
                        type = ".bmp";
                    }
                    else if ((pageDirectory1 + "\\image\\" + x + type).Contains(".jpeg"))
                    {
                        type = ".jpeg";
                    }*/
                    BitmapImage bi = new BitmapImage();
                    //System.Windows.MessageBox.Show(x.ToString());
                    int b = x;
                    using (FileStream fs3 = new FileStream(pageDirectory1 + "\\image\\" + x + ".png", FileMode.Open, FileAccess.Read))
                    {
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = fs3;
                        bi.EndInit();
                    }

                    StreamReader txt = new StreamReader(pageDirectory1 + "\\image\\" + x + ".txt");
                    string txtAll = txt.ReadToEnd();
                    txt.Close();

                    string[] txtLine = new string[10];
                    string[] newtxtLine = new string[10];

                    txt = new StreamReader(pageDirectory1 + "\\image\\" + x + ".txt");
                    {
                        txtLine[0] = txt.ReadLine();
                        txtLine[1] = txt.ReadLine();
                        txtLine[2] = txt.ReadLine();
                        txtLine[3] = txt.ReadLine();

                    }
                    txt.Close();

                    img[b] = new System.Windows.Controls.Image();
                    imageCanvas.Children.Add(img[b]);
                    img[b].Width = int.Parse(txtLine[0]);
                    img[b].Height = int.Parse(txtLine[1]);
                    img[b].Margin = new Thickness(int.Parse(txtLine[2]), int.Parse(txtLine[3]), 0, 0);
                    img[b].Source = bi;
                    img[b].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);
                }
                fs1.Close();
            }
        }

        //pageWindowからくるとき
        public void isfShow(string subjectNamePW, string pageName, string pageDirectory, string unitNamePage, string epubFileName, int epubCurrentPageLeft)
        {
            epubFileName1 = epubFileName;
            epubCurrentPageLeft1 = epubCurrentPageLeft;

            imageClickOK = false;
            subjectNameMain = subjectNamePW;
            //System.Windows.MessageBox.Show(pageName);
            pageNameMain = pageName;
            x = int.Parse(pageName.Replace(".isf", ""));
            //System.Windows.MessageBox.Show(x.ToString());
            pageDirectory1 = pageDirectory;
            unitNameMain = unitNamePage;
            pageName1 = pageDirectory + "\\" + pageName;
            //System.Windows.MessageBox.Show(subjectName+ pageName+ pageDirectory);
            if (System.IO.File.Exists(pageDirectory1 + "\\" + x + ".isf"))
            {
                System.IO.FileStream fs2 = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Open);
                inkCanvas1.Strokes = new StrokeCollection(fs2);
                //if (a > 0)
                //{
                //for (int b = 1; b != null; b++)
                //{

                if (File.Exists(pageDirectory1 + "\\image\\" + x + ".png"))
                {
                    /*if ((pageDirectory1 + "\\image\\" + x + type).Contains(".png"))
                    {
                        type = ".png";
                    }
                    else if ((pageDirectory1 + "\\image\\" + x + type).Contains(".bmp"))
                    {
                        type = ".bmp";
                    }
                    else if ((pageDirectory1 + "\\image\\" + x + type).Contains(".jpeg"))
                    {
                        type = ".jpeg";
                    }*/
                    BitmapImage bi = new BitmapImage();
                    //System.Windows.MessageBox.Show(x.ToString());
                    int b = x;
                    using (FileStream fs3 = new FileStream(pageDirectory1 + "\\image\\" + x + ".png", FileMode.Open, FileAccess.Read))
                    {
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = fs3;
                        bi.EndInit();
                    }

                    StreamReader txt = new StreamReader(pageDirectory1 + "\\image\\" + x + ".txt");
                    string txtAll = txt.ReadToEnd();
                    txt.Close();

                    string[] txtLine = new string[10];
                    string[] newtxtLine = new string[10];

                    //System.Windows.MessageBox.Show(x.ToString());
                    txt = new StreamReader(pageDirectory1 + "\\image\\" + x + ".txt");
                    {
                        txtLine[0] = txt.ReadLine();
                        txtLine[1] = txt.ReadLine();
                        txtLine[2] = txt.ReadLine();
                        txtLine[3] = txt.ReadLine();

                    }
                    txt.Close();

                    img[b] = new System.Windows.Controls.Image();
                    imageCanvas.Children.Add(img[b]);
                    img[b].Width = int.Parse(txtLine[0]);
                    img[b].Height = int.Parse(txtLine[1]);
                    img[b].Margin = new Thickness(int.Parse(txtLine[2]), int.Parse(txtLine[3]), 0, 0);
                    img[b].Source = bi;
                    img[b].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);
                }
                /*else
                {
                    img[b].Visibility = System.Windows.Visibility.Hidden;
                }*/
                //}
                //}
                fs2.Close();

                System.IO.FileStream fs = new System.IO.FileStream(pageName1, System.IO.FileMode.Open);

                //System.Windows.MessageBox.Show(x.ToString());
                inkCanvas1.Strokes = new StrokeCollection(fs);
                fs.Close();
            }
        }

        //subWindowからくるとき
        public void subShow(string pageNameSub, string pageDirectorySub)
        {
            imageClickOK = false;
            //System.Windows.MessageBox.Show(pageNameSub); OK!!
            x = int.Parse(pageNameSub.Replace(".isf", ""));
            pageDirectory1 = pageDirectorySub;
            pageNameMain = pageNameSub;
            System.IO.FileStream fs = new System.IO.FileStream(pageDirectorySub + "\\" + pageNameSub, System.IO.FileMode.Open);
            inkCanvas1.Strokes = new StrokeCollection(fs);
            fs.Close();
        }

        //subWindow の送るを押されたとき
        public void tbShow(string pageNameSub, string pageDirectorySub)
        {
            imageClickOK = false;
            x = int.Parse(pageNameSub.Replace(".isf", ""));
            pageDirectory1 = pageDirectorySub;
            pageNameMain = pageNameSub;
            System.IO.FileStream fs = new System.IO.FileStream(pageDirectory1 + "\\" + pageNameMain, System.IO.FileMode.Open);
            inkCanvas1.Strokes = new StrokeCollection(fs);
            fs.Close();

            System.IO.FileStream fsimg = new System.IO.FileStream(pageDirectory1 + "\\imageTB\\" + x + ".png", System.IO.FileMode.Open);


            BitmapImage bi = new BitmapImage();

            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = fsimg;
            bi.EndInit();
            fsimg.Close();

            //Imageコントロールにセットする
            a++;
            img[x] = new System.Windows.Controls.Image();
            imageCanvas.Children.Add(img[x]);
            img[x].Height = 800;
            img[x].Width = 800;
            img[x].Source = bi;
            img[x].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);
            //画像のページの位置を格納する
            page[a] = pageName1;

            fs.Close();
        }

        //黒色に変える
        private void black_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;

            i.Color = Colors.Black;
            //System.Windows.MessageBox.Show(i.Color.ToString());
            inkCanvas1.DefaultDrawingAttributes = i;
            textColor = 0;

            if (i.IsHighlighter == true)
            {
                i.IsHighlighter = false;
                style = 0;
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
            else if (style == 3 || style == 2)
            {
                i.IsHighlighter = false;
                style = 0;
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        //赤色に変える
        private void red_Click(object sender, RoutedEventArgs e)
        {

            imageClickOK = false;
            i.Color = Colors.Red;
            inkCanvas1.DefaultDrawingAttributes = i;
            textColor = 1;

            if (i.IsHighlighter == true)
            {
                i.IsHighlighter = false;
                style = 0;
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
            else if (style == 3 || style == 2)
            {
                i.IsHighlighter = false;
                style = 0;
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        //青色に変える
        private void blue_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;

            i.Color = Colors.Blue;
            inkCanvas1.DefaultDrawingAttributes = i;
            textColor = 2;

            if (i.IsHighlighter == true)
            {
                i.IsHighlighter = false;
                style = 0;
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
            else if (style == 3 || style == 2)
            {
                i.IsHighlighter = false;
                style = 0;
                inkCanvas1.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        //全部消す
        private void allClear_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;

            inkCanvas1.Strokes.Clear();
            if (File.Exists(pageDirectory1 + "\\image\\" + x + ".png"))
            {
                //System.Windows.MessageBox.Show(x.ToString());
                File.Delete(pageDirectory1 + "\\image\\" + x + ".png");
                File.Delete(pageDirectory1 + "\\image\\" + x + ".txt");
                img[x].Source = null;
            }
        }

        //移動(はりつける)
        private void petapetaButton_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            CaptureScreen();

            System.IO.FileStream fs = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Create);
            inkCanvas1.Strokes.Save(fs);
            fs.Close();

            MainWindow dialog = new MainWindow();
            dialog.Show();
            dialog.ePubClick(epubFileName1, epubCurrentPageLeft1, x + ".isf");

            //DinoSubWindow dialog = new DinoSubWindow();
            //dialog.Show();
            //System.Windows.MessageBox.Show(pageNameMain, pageDirectory1); OK!!
            //dialog.sub(pageNameMain, pageDirectory1);

            this.Close();
        }

        //デジタル教科書の画像をはりつける
        public void txBook(string pageNameSub, string pageDirectorySub)
        {
            imageClickOK = false;
            pageDirectory1 = pageDirectorySub;
            pageNameMain = pageNameSub;
        }

        //隠すボタン
        private void hideButton_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;

            //MainWindowの非表示
            this.Hide();
        }

        //うごかすボタン
        private void moveButton_Click(object sender, RoutedEventArgs e)
        {
            //var img1 = img[a].TransformToVisual(null).Transform(new System.Windows.Point (0,0));
            imageClickOK = true;
            drag = false;
        }

        //スクショで保存するよ～
        private void CaptureScreen()
        {
            imageClickOK = false;

            System.Windows.Point browserLeftTop = new System.Windows.Point();
            System.Drawing.Size screenSize = new System.Drawing.Size();

            browserLeftTop = inkCanvas1.PointToScreen(new System.Windows.Point(0.0, 0.0));
            screenSize.Height = (int)inkCanvas1.RenderSize.Height;
            screenSize.Width = (int)inkCanvas1.RenderSize.Width;

            System.Drawing.Rectangle rc = new System.Drawing.Rectangle();
            rc.Location = new System.Drawing.Point((int)browserLeftTop.X, (int)browserLeftTop.Y);
            rc.Size = screenSize;

            Bitmap bmpChallenge;
            bool fsCheck = File.Exists(pageDirectory1 + "\\" + x + ".png");
            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size);
                }
                //bmp.Dispose();
                bmpChallenge = new Bitmap(bmp);
                //System.Windows.MessageBox.Show(File.Exists(pageDirectory1 + "\\" + x + ".png").ToString());
                //bmp.Save(pageDirectory1 + "\\" + x + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);   
            }
            if (File.Exists(pageDirectory1 + "\\" + x + ".png"))
            {

                //System.IO.File.Delete(pageDirectory1 + "\\" + x + ".png");
                using (bmpChallenge)
                {
                    bmpChallenge.Save(pageDirectory1 + "\\" + x + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            else
            {
                using (bmpChallenge)
                {
                    bmpChallenge.Save(pageDirectory1 + "\\" + x + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }
            }

            imageSave();
        }

        private void imageSave()
        {
            if (File.Exists(pageDirectory1 + "\\image\\" + x + ".png"))
            {
                File.Delete(pageDirectory1 + "\\image\\" + x + ".txt");
                StreamWriter sw = new StreamWriter(pageDirectory1 + "\\image\\" + x + ".txt");
                {
                    sw.WriteLine(img[x].Width.ToString());
                    sw.WriteLine(img[x].Height.ToString());
                    sw.WriteLine(img[x].Margin.Left.ToString());
                    sw.WriteLine(img[x].Margin.Top.ToString());
                }
                sw.Close();
            }
        }

        //クリップボードにある画像を、新規ページに張り付ける
        public void pasteClipBorad(string subjectName, string unitName, string savedFilePath)
        {
            int pageNum = 1;
            string notePath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Note";

            //".isf"を含むファイルを、大文字小文字を区別して探す
            System.Collections.ObjectModel.ReadOnlyCollection<string> files =
                Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                    notePath + "\\" + subjectName + "\\" + unitName,
                    "",
                    false,
                    Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories,
                    new string[] { "*.isf" });

            foreach (string f in files)
            {
                pageNum++;
            }

            isfShow(subjectName, pageNum.ToString(), notePath + "\\" + subjectName + "\\" + unitName, unitNameMain, epubFileName1, epubCurrentPageLeft1);

            string pagePath = notePath + "\\" + subjectName + "\\" + unitName;
            File.Move(savedFilePath, pagePath + "\\image\\" + pageNum + ".png");

            System.IO.FileStream fsimg = new System.IO.FileStream(pagePath + "\\image\\" + pageNum + ".png", System.IO.FileMode.Open);

            //画像の貼り付け
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.CacheOption = BitmapCacheOption.OnLoad;
            bi.StreamSource = fsimg;
            bi.EndInit();
            fsimg.Close();

            //Imageコントロールにセットする
            img[pageNum] = new System.Windows.Controls.Image();
            imageCanvas.Children.Add(img[pageNum]);
            img[pageNum].Height = 800;
            img[pageNum].Width = 800;
            img[pageNum].Source = bi;
            img[pageNum].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);

            //画像のページの位置を格納する
            page[pageNum] = pagePath + pageNum + ".isf";

            fsimg.Close();

            // txtファイルを生成
            StreamWriter sw = new StreamWriter(pageDirectory1 + "\\image\\" + x + ".txt");
            {
                txtLine[0] = img[x].Width.ToString();
                txtLine[1] = img[x].Height.ToString();
                txtLine[2] = img[x].Margin.Left.ToString();
                txtLine[3] = img[x].Margin.Top.ToString();

                sw.WriteLine(txtLine[0]);
                sw.WriteLine(txtLine[1]);
                sw.WriteLine(txtLine[2]);
                //sw.WriteLine(txtLine[3]);
            }
            sw.Close();


        }

        // 電子黒板に送る
        private void send_Click(object sender, RoutedEventArgs e)
        {
            CaptureScreen();

            using(var seacher = new ManagementObjectSearcher("SELECT * FROM win32_share"))
            {
                var list = seacher.Get().Cast<ManagementObject>().Select(y => new {Name = y["Name"], Path = y["Path"]});
                foreach (var element in list)
                {
                    Console.WriteLine("共有名：" + element.Name + "パス：" + element.Path);
                }
            }

            Console.WriteLine(DateTime.Now);
            DateTime date = DateTime.Now;
            string machine = Environment.MachineName;

            //System.Windows.MessageBox.Show(date.ToString("yyyyMMddhhmmss"));
            //string sendDirectory = @"Z:\fromStudent";

            string sendDirectory = @"\\JOHO-EBOARD\receive";
            //string sendDirectory = @"\\MCDYNA13\receive";
            //System.Windows.MessageBox.Show(File.Exists(sendDirectory + "\\004.PNG").ToString());

            //string sD = (System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\notesend\\インタラクション\\fromStudent");

            //共有フォルダに関する処理
            string unc_path;
            unc_path = GetUniversalName(sendDirectory);

            //System.Windows.MessageBox.Show(unc_path + "\\" + date.ToString("yyyyMMddmmss") + ".png");
            //try
            //{
                if(!Directory.Exists(unc_path + "\\" + date.ToString("yyyy") + "_" + date.ToString("MM") + "_" + date.ToString("dd") + "_" + subjectNameMain))
                {
                    System.Windows.MessageBox.Show(unc_path + "\\" + date.ToString("yyyy") + "_" + date.ToString("MM") + "_" + date.ToString("dd") + "_" + subjectNameMain);
                    Directory.CreateDirectory(unc_path + "\\" + date.ToString("yyyy") + "_" + date.ToString("MM") + "_" + date.ToString("dd") + "_" + subjectNameMain);
                }
                File.Copy(pageDirectory1 + "\\" + x + ".png", unc_path + "\\" + date.ToString("yyyy") + "_" + date.ToString("MM") + "_" + date.ToString("dd") + "_" + subjectNameMain + "\\" + machine + "_" + date.ToString("yyyyMMddmmss") + ".png");
                //Bitmap bmp1 = new Bitmap(unc_path + "\\" + machine + "_" + date.ToString("yyyyMMddmmss") + ".png");
                //bmp1.Save(unc_path + "\\" + machine + "_" + date.ToString("yyyyMMddmmss") + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                System.Windows.MessageBox.Show("そうしんできました！");
            //}
            //catch
            //{
                //System.Windows.MessageBox.Show("そうしんできませんでした。");
            //}
        }

        //電子黒板からもらう
        private void fromTeacher_Click(object sender, RoutedEventArgs e)
        {
            string teacherDirectory = @"\\JOHO-EBOARD\send";
            //string teacherDirectory = @"\\MCDYNA13\send";

            //共有フォルダに関する処理
            string unc_path;
            unc_path = GetUniversalName(teacherDirectory);
            DateTime latestTime = new DateTime(2000, 1, 1, 0, 0, 0);
            DateTime compTime;
            int i = 0;
            int j = 0;
            //System.Windows.MessageBox.Show(unc_path);

            //bmpファイルを、大文字小文字を区別して探す
            System.Collections.ObjectModel.ReadOnlyCollection<string> bmpFiles =
                Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                unc_path,
                "",
                false,
                Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories,
                new string[] { "*.bmp" });

            foreach (string f in bmpFiles)
            {
                compTime = System.IO.File.GetCreationTime(f);
                if(compTime > latestTime)
                {
                    latestTime = compTime;
                    j = i;
                }
                i++;
            }
            string lastFile = bmpFiles[j];
            string receiveFilePath = lastFile.Replace(".bmp", ".png");
            Bitmap bmp2 = new Bitmap(lastFile);
            bmp2.Save(receiveFilePath, System.Drawing.Imaging.ImageFormat.Png);

            //System.Windows.MessageBox.Show(receiveFilePath);

            if (File.Exists(lastFile))
            {
                if (!File.Exists(pageDirectory1 + "\\image\\" + x + ".png"))
                {
                    BitmapImage bi = new BitmapImage();
                    //System.Windows.MessageBox.Show(x.ToString());
                    int b = x;

                    using (FileStream fs3 = new FileStream(receiveFilePath, FileMode.Open, FileAccess.Read))
                    {
                        bi.BeginInit();
                        bi.CacheOption = BitmapCacheOption.OnLoad;
                        bi.StreamSource = fs3;
                        bi.EndInit();
                    }

                    img[b] = new System.Windows.Controls.Image();
                    imageCanvas.Children.Add(img[b]);
                    img[b].Width = 800;
                    img[b].Height = 800;
                    img[b].Margin = new Thickness(0, 0, 0, 0);
                    img[b].Source = bi;
                    img[b].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);

                    System.IO.File.Copy(receiveFilePath, pageDirectory1 + "\\image\\" + x + ".png");

                    // txtファイルを生成
                    StreamWriter sw = new StreamWriter(pageDirectory1 + "\\image\\" + x + ".txt");
                    {
                        txtLine[0] = img[x].Width.ToString();
                        txtLine[1] = img[x].Height.ToString();
                        txtLine[2] = img[x].Margin.Left.ToString();
                        txtLine[3] = img[x].Margin.Top.ToString();
                        sw.WriteLine(txtLine[0]);
                        sw.WriteLine(txtLine[1]);
                        sw.WriteLine(txtLine[2]);
                        sw.WriteLine(txtLine[3]);
                    }
                    sw.Close();
                }

                else {
                    //新規ページに電子黒板からの画像を貼り付ける場合

                    //現在のページを保存
                    CaptureScreen();
                    System.IO.FileStream fs = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Create);
                    inkCanvas1.Strokes.Save(fs);
                    fs.Close();
                    img[x].Source = null;

                    // txtファイルを生成
                    File.Delete(pageDirectory1 + "\\image\\" + x + ".txt");
                    StreamWriter sw = new StreamWriter(pageDirectory1 + "\\image\\" + x + ".txt");
                    {
                        txtLine[0] = img[x].Width.ToString();
                        txtLine[1] = img[x].Height.ToString();
                        txtLine[2] = img[x].Margin.Left.ToString();
                        txtLine[3] = img[x].Margin.Top.ToString();
                        sw.WriteLine(txtLine[0]);
                        sw.WriteLine(txtLine[1]);
                        sw.WriteLine(txtLine[2]);
                        sw.WriteLine(txtLine[3]);
                    }
                    sw.Close();

                    //isfファイルを、大文字小文字を区別して探す
                    System.Collections.ObjectModel.ReadOnlyCollection<string> isfFiles =
                        Microsoft.VisualBasic.FileIO.FileSystem.FindInFiles(
                        pageDirectory1,
                        "",
                        false,
                        Microsoft.VisualBasic.FileIO.SearchOption.SearchAllSubDirectories,
                        new string[] { "*.isf" });

                    int number = 0;
                    foreach (string f in isfFiles)
                    {
                        number++;
                    }
                    number++;
                    x = number;

                    isfShow(subjectNameMain, x + ".isf", pageDirectory1, unitNameMain, epubFileName1, epubCurrentPageLeft1);

                    File.Copy(receiveFilePath, pageDirectory1 + "\\image\\" + x + ".png");
                    System.IO.FileStream fsimg = new System.IO.FileStream(pageDirectory1 + "\\image\\" + x + ".png", System.IO.FileMode.Open);

                    //画像の貼り付け
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = fsimg;
                    bi.EndInit();
                    fsimg.Close();

                    //Imageコントロールにセットする
                    img[x] = new System.Windows.Controls.Image();
                    imageCanvas.Children.Add(img[x]);
                    img[x].Height = 800;
                    img[x].Width = 800;
                    img[x].Source = bi;
                    img[x].MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.inkCanvas1_MouseDown);

                    //画像のページの位置を格納する
                    page[x] = pageDirectory1 + "\\" + x + ".isf";
                    //System.Windows.MessageBox.Show(page[x]);

                    fsimg.Close();

                    // txtファイルを生成
                    StreamWriter newSw = new StreamWriter(pageDirectory1 + "\\image\\" + x + ".txt");
                    {
                        txtLine[0] = img[x].Width.ToString();
                        txtLine[1] = img[x].Height.ToString();
                        txtLine[2] = img[x].Margin.Left.ToString();
                        txtLine[3] = img[x].Margin.Top.ToString();

                        newSw.WriteLine(txtLine[0]);
                        newSw.WriteLine(txtLine[1]);
                        newSw.WriteLine(txtLine[2]);
                        //sw.WriteLine(txtLine[3]);
                    }
                    newSw.Close();

                }
            } 
        }


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
       static string GetUniversalName(string path_src)
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
           switch(apiRetVal)
           {
               case ERROR_MORE_DATA :
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

               case ERROR_BAD_DEVICE   : //すでにUNC名(\\servername\test)
               case ERROR_NOT_CONNECTED: //ローカル・ドライブのパス(C:\test)
                   //MessageBox.Show(path_src +"\nErrorCode:" + apiRetVal.ToString());
                   break;
               default:
                   //MessageBox.Show(path_src + "\nErrorCode:" + apiRetVal.ToString());
                   break;

           }

           return unc_path_dest;
       }

        private void toPageWin_Click(object sender, RoutedEventArgs e)
        {
            imageClickOK = false;
            CaptureScreen();

            System.IO.FileStream fs = new System.IO.FileStream(pageDirectory1 + "\\" + x + ".isf", System.IO.FileMode.Create);
            inkCanvas1.Strokes.Save(fs);
            fs.Close();

            DinoPageWindow dialog = new DinoPageWindow();
            dialog.Show();
            //System.Windows.MessageBox.Show(pageNameMain, pageDirectory1); OK!!
            dialog.pageShow(subjectNameMain,unitNameMain, epubFileName1, epubCurrentPageLeft1);

            this.Close();


        }
    }
}
