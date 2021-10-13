using PastMediaPlayer_Project.MediaControl;
using System;
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

namespace PastMediaPlayer_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LocalInfo localInfo;

        public MainWindow()
        {
            InitializeComponent();

            localInfo = LocalInfo.GetSingle();
            localInfo.InitRoot_FolderTree(localInfo.FolderPath); // 根据路径初始化

            folderCtrl.selectedMediaFile += PlayMediaFile;
            folderCtrl.InitRootTreeItem(localInfo.rootFolderTree);
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                object data = e.Data.GetData(DataFormats.FileDrop);
                string path = ((System.Array)data).GetValue(0).ToString();
                localInfo.InitRoot_FolderTree(path);

                folderCtrl.InitRootTreeItem(localInfo.rootFolderTree);
            }
        }

        private void Window_Keydown(object sender, KeyEventArgs e)
        {
            if (mediaCtrl == null)
                return;

            //MessageBox.Show("1");
            //if (e.KeyStates == Keyboard.GetKeyStates(Key.F4) && Keyboard.Modifiers == ModifierKeys.Alt) 组合键
            if (e.KeyStates == Keyboard.GetKeyStates(Key.Right))
            {
                mediaCtrl.MainMedia_CtrlTime(5);
            }
            else if(e.KeyStates == Keyboard.GetKeyStates(Key.Left))
            {
                mediaCtrl.MainMedia_CtrlTime(-5);
            }
        }

        private void PlayMediaFile(FolderTree tree)
        {
            mediaCtrl.OpenMediaByFolderInfo(tree);
        }
    }
}
