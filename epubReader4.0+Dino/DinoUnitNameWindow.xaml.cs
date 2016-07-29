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

namespace epubReader4._0_Dino
{
    /// <summary>
    /// unitNameWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DinoUnitNameWindow : Window
    {
        public DinoUnitNameWindow()
        {
            InitializeComponent();
        }

        string subjectName;
        string epubFileName1;
        int epubCurrentPageLeft1;

        public void unitNameIn(string subjectName2, string epubFileName, int epubCurrentPageLeft)
        {
            subjectName = subjectName2;
            epubFileName1 = epubFileName;
            epubCurrentPageLeft1 = epubCurrentPageLeft;
        }

        private void enterButton_Click(object sender, RoutedEventArgs e)
        {
            string unitName = nameBox.Text;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Note\\" + subjectName + "\\" + unitName);
            di.Create();
            System.IO.DirectoryInfo imageDI = new System.IO.DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Note\\" + subjectName + "\\" + unitName + "\\image");
            imageDI.Create();
            System.IO.DirectoryInfo imageTBDI = new System.IO.DirectoryInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Note\\" + subjectName + "\\" + unitName + "\\imageTB");
            imageTBDI.Create();

            DinoPageWindow mainDialog = new DinoPageWindow();
            mainDialog.Show();
            this.Owner = mainDialog;
            mainDialog.pageShow(subjectName, unitName, epubFileName1, epubCurrentPageLeft1);

            this.Close();
        }
    }
}
