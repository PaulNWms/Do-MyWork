using System;
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
        public TreeBuilder TreeBuilder { get; set; }

        public MyEventHandlers(string editor, TreeView tree)
        {
            this.Editor = editor;
            this.Tree = tree;
            tree.PreviewMouseRightButtonDown += TreeView_PreviewMouseRightButtonDown;
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.Tree.SelectedItem as TreeViewItem;
            TreeNode node = item.Tag as TreeNode;
            MenuItem menuItem = sender as MenuItem;
            string menuSelection = menuItem.Header.ToString();

            switch (menuSelection)
            {
                case "Edit":
                    Process.Start(this.Editor, node.Path);
                    break;

                case "Open Folder":
                    Process.Start("explorer.exe", GetFolder(item));
                    break;

                case "Open CMD":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "cmd.exe";
                        psi.Arguments = "/K";
                        psi.WorkingDirectory = GetFolder(item);
                        Process.Start(psi);
                    }
                    break;

                case "Open PowerShell":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "powershell.exe";
                        psi.Arguments = "-NoExit";
                        psi.WorkingDirectory = GetFolder(item);
                        Process.Start(psi);
                    }
                    break;

                case "Open URL":
                    Process.Start(node.Path);
                    break;

                case "Run Executable":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.CreateNoWindow = false;
                        psi.UseShellExecute = false;
                        psi.WindowStyle = ProcessWindowStyle.Hidden;
                        psi.FileName = Path.GetFileName(node.Path);
                        psi.WorkingDirectory = Path.GetDirectoryName(node.Path);

                        try
                        {
                            Process.Start(psi);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    break;

                case "Run Batch":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "cmd.exe";
                        psi.Arguments = string.Format(@"/K ""{0}""", Path.GetFileName(node.Path));
                        psi.WorkingDirectory = GetFolder(item);
                        Process.Start(psi);
                    }
                    break;

                case "Run Script":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "powershell.exe";
                        psi.Arguments = string.Format(@"-NoExit -Command "".\{0}""", Path.GetFileName(node.Path));
                        psi.WorkingDirectory = GetFolder(item);
                        Process.Start(psi);
                    }
                    break;
            }
        }

        private string GetFolder(TreeViewItem item)
        {
            TreeNode node = item.Tag as TreeNode;

            switch (node.Type)
            {
                case TreeNodeType.FileParent:
                case TreeNodeType.Dir:
                case TreeNodeType.DirParent:
                case TreeNodeType.ChildDir:
                case TreeNodeType.Url:
                    return node.Path;

                case TreeNodeType.File:
                case TreeNodeType.ChildFile:
                default:
                    return Path.GetDirectoryName(node.Path);
            }

        }

        public void FilesItem_Expanded(object sender, RoutedEventArgs e)
        {
            FileSystemItem_Expanded(TreeNodeType.FileParent, sender, e);
        }

        public void DirsItem_Expanded(object sender, RoutedEventArgs e)
        {
            FileSystemItem_Expanded(TreeNodeType.DirParent, sender, e);
        }

        public void FileSystemItem_Expanded(TreeNodeType treeNodeType, object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            TreeNode node = item.Tag as TreeNode;
            string[] children = null;

            switch (treeNodeType)
            {
                case TreeNodeType.DirParent:
                    children = Directory.GetDirectories(node.Path, node.Filter);
                    break;

                case TreeNodeType.FileParent:
                default:
                    children = Directory.GetFiles(node.Path, node.Filter);
                    break;
            }

            item.Items.Clear();
            this.TreeBuilder.AddChildNodes(treeNodeType, item.Items, children);
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
