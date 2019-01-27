using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Do_MyWork
{
    class MyEventHandlers
    {
        public string Editor { get; private set; }
        public TreeView Tree { get; private set; }

        public MyEventHandlers(string editor, TreeView tree)
        {
            this.Editor = editor;
            this.Tree = tree;
            tree.PreviewMouseRightButtonDown += TreeView_PreviewMouseRightButtonDown;
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeItem = this.Tree.SelectedItem as TreeViewItem;
            MenuItem menuItem = sender as MenuItem;
            string menuSelection = menuItem.Header.ToString();
            string tag = treeItem.Tag.ToString();

            switch (menuSelection)
            {
                case "Edit":
                    Process.Start(this.Editor, tag);
                    break;

                case "Open location":
                    Process.Start("explorer.exe", Path.GetDirectoryName(tag));
                    break;

                case "Open URL":
                    Process.Start(tag);
                    break;
            }
        }

        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.IsSelected = true;
                e.Handled = true;
            }
        }

        static T VisualUpwardSearch<T>(DependencyObject source) where T : DependencyObject
        {
            DependencyObject returnVal = source;

            while (returnVal != null && !(returnVal is T))
            {
                DependencyObject tempReturnVal = null;

                if (returnVal is Visual || returnVal is Visual3D)
                {
                    tempReturnVal = VisualTreeHelper.GetParent(returnVal);
                }

                if (tempReturnVal == null)
                {
                    returnVal = LogicalTreeHelper.GetParent(returnVal);
                }
                else
                {
                    returnVal = tempReturnVal;
                }
            }

            return returnVal as T;
        }
    }
}
