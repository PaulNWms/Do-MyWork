using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Do_MyWork
{
    internal enum TreeNodeType { Files, Dirs }

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

                case "Open Folder":
                    Process.Start("explorer.exe", Path.GetDirectoryName(tag));
                    break;

                case "Open URL":
                    Process.Start(tag);
                    break;
            }
        }

        public void FilesItem_Expanded(object sender, RoutedEventArgs e)
        {
            FileSystemItem_Expanded(TreeNodeType.Files, sender, e);
        }

        public void DirsItem_Expanded(object sender, RoutedEventArgs e)
        {
            FileSystemItem_Expanded(TreeNodeType.Dirs, sender, e);
        }

        public void FileSystemItem_Expanded(TreeNodeType treeNodeType, object sender, RoutedEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            string pathFilter = treeViewItem.Tag.ToString();
            string path = null;
            string filter = null;

            if (TryGetPathFilter(pathFilter, out path, out filter))
            {
                string[] children = null;

                switch (treeNodeType)
                {
                    case TreeNodeType.Dirs:
                        children = Directory.GetDirectories(path, filter);
                        break;

                    case TreeNodeType.Files:
                    default:
                        children = Directory.GetFiles(path, filter);
                        break;
                }

                RemovePlaceholder(treeViewItem.Items);

                foreach (string child in children)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = child;
                    treeViewItem.Items.Add(item);
                }
            }
        }
        private bool TryGetPathFilter(string pathFilter, out string path, out string filter)
        {
            int idx = pathFilter.LastIndexOf('\\');

            if (idx >= 0)
            {
                path = pathFilter.Substring(0, idx);
                filter = pathFilter.Substring(idx + 1);
                return true;
            }
            else
            {
                path = null;
                filter = null;
                return false;
            }
        }

        private void RemovePlaceholder(ItemCollection items)
        {
            int idx = -1;

            for (int i = 0; i < items.Count; i++)
            {
                TreeViewItem item = items[i] as TreeViewItem;

                if (item.Header.ToString() == ".")
                {
                    idx = i;
                }
            }

            if (idx >= 0)
            {
                items.RemoveAt(idx);
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
