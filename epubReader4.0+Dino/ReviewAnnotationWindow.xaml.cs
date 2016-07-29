﻿using System;
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

using System.IO;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Ink;

namespace epubReader4._0_Dino
{
    /// <summary>
    /// ReviewAnnotationWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ReviewAnnotationWindow : Window
    {
        public ReviewAnnotationWindow()
        {
            InitializeComponent();
        }

        List<StrokeLine> strokeLines = new List<StrokeLine>();
        List<LearningLog> learningLogs = new List<LearningLog>();
        int counter = 0;

        //Load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inkCanvas1.EditingMode = InkCanvasEditingMode.None;
        }

        //初期処理
        public void init(string nowPagePath, List<StrokeLine> strokeLines, List<LearningLog> learningLogs)
        {
            this.strokeLines = strokeLines;
            this.learningLogs = learningLogs;

            //現在のページを表示
            BitmapImage bi = new BitmapImage();
            using (FileStream fs = new FileStream(nowPagePath, FileMode.Open, FileAccess.Read))
            {
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = fs;
                bi.EndInit();
            }
            image1.Source = bi;
        }

        //動作を進める
        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //一次元目がallなら全消去なのですべて消去する
                if (learningLogs[counter].GetStrokeId().Equals("all"))
                {
                    inkCanvas1.Strokes.Clear();
                }

                //二次元目がdrawのとき
                else if (learningLogs[counter].GetBehavior().Equals("draw"))
                {
                    //ターゲットとなるストロークのidを取ってくる
                    int x = Int16.Parse(learningLogs[counter].GetStrokeId());

                    //strokeLiesの中から該当するidのストロークを探す
                    StrokeLine sl;
                    for (int i = 0; i < strokeLines.Count; i++)
                    {
                        if (strokeLines[i].GetId() == x)
                        {
                            sl = strokeLines[x];

                            //（とりあえず）隠れたスペースに書いてなければを再描画
                            if (!sl.GetInSpace())
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
                    }
                }

                //二次元目がeraseのとき
                else if (learningLogs[counter].GetBehavior().Equals("erase"))
                {
                    //ターゲットとなるストロークのidを取ってくる
                    int x = Int16.Parse(learningLogs[counter].GetStrokeId());

                    //strokeLiesの中から該当するidのストロークを探す
                    StrokeLine sl;
                    for (int i = 0; i < strokeLines.Count; i++)
                    {
                        if (strokeLines[i].GetId() == x)
                        {
                            sl = strokeLines[x];
                            break;
                        }
                    }

                    //いったん全部消し、当該idまで、再描画する
                    inkCanvas1.Strokes.Clear();
                    drawAll(x+1);
                }
                counter++;
            }
            catch
            {
                MessageBox.Show("最後の動作です。");
            }
        }

        //動作を戻る
        private void backButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //指定したidまでで、消されていないストロークを再描画
        private void drawAll(int limId)
        {
            for (int i = 0; i <= limId; i++)
            {
                StrokeLine sl = strokeLines[i];

                //消去済みでなく、隠れたスペースに書いていないストローク以外を再描画
                if (!sl.GetInSpace() && !sl.GetEreased())
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
        } 
    }
}
