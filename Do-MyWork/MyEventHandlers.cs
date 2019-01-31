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
            tree.MouseDoubleClick += Tree_MouseDoubleClick;
        }

        public void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = this.Tree.SelectedItem as TreeViewItem;
            TreeNode node = item.Tag as TreeNode;
            MenuItem menuItem = sender as MenuItem;
            string menuSelection = menuItem.Header.ToString();

            switch (menuSelection)
            {
                case "Copy To Clipboard":
                    Clipboard.SetText(node.Path);
                    break;

                case "Edit":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = this.Editor;
                        psi.Arguments = Path.GetFileName(node.Path);
                        psi.WorkingDirectory = Path.GetDirectoryName(node.Path);
                        Process.Start(psi);
                    }
                    break;

                case "Open Folder":
                    Process.Start("explorer.exe", GetFolder(node));
                    break;

                case "Open CMD":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "cmd.exe";
                        psi.Arguments = "/K";
                        psi.WorkingDirectory = GetFolder(node);
                        Process.Start(psi);
                    }
                    break;

                case "Open PowerShell":
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = "powershell.exe";
                        psi.Arguments = "-NoExit";
                        psi.WorkingDirectory = GetFolder(node);
                        Process.Start(psi);
                    }
                    break;

                case "Open URL":
                    Process.Start(node.Path);
                    break;

                case "Run Executable":
                case "Run Batch":
                case "Run Script":
                    RunFile(node);
                    break;
            }
        }

        public void Tree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = this.Tree.SelectedItem as TreeViewItem;
            TreeNode node = item.Tag as TreeNode;

            if (node != null)
            {
                switch (node.Type)
                {
                    case TreeNodeType.File:
                    case TreeNodeType.ChildFile:
                        RunFile(node);
                        break;

                    case TreeNodeType.Url:
                        Process.Start(node.Path);
                        break;

                    case TreeNodeType.Dir:
                    case TreeNodeType.ChildDir:
                    case TreeNodeType.FileParentDir:
                    case TreeNodeType.DirParentDir:
                    default:
                        {
                            string folder = GetFolder(node);

                            if (Directory.Exists(folder))
                            {
                                Process.Start("explorer.exe", folder);
                            }
                            else
                            {
                                MessageBox.Show("Path does not exist: " + folder, "Got configuration problems?", MessageBoxButton.OK, MessageBoxImage.Hand);
                            }
                        }
                        break;
                }
            }
        }

        private void RunFile(TreeNode node)
        {
            if (this.TreeBuilder.exePattern.IsMatch(node.Path))
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
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (this.TreeBuilder.cmdPattern.IsMatch(node.Path))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.Arguments = string.Format(@"/K ""{0}""", Path.GetFileName(node.Path));
                psi.WorkingDirectory = GetFolder(node);
                Process.Start(psi);
            }
            else if (this.TreeBuilder.ps1Pattern.IsMatch(node.Path))
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "powershell.exe";
                psi.Arguments = string.Format(@"-NoExit -Command "".\{0}""", Path.GetFileName(node.Path));
                psi.WorkingDirectory = GetFolder(node);
                Process.Start(psi);
            }
            else
            {
                Process.Start(this.Editor, node.Path);
            }
        }

        private string GetFolder(TreeNode node)
        {
            switch (node.Type)
            {
                case TreeNodeType.FileParentDir:
                case TreeNodeType.Dir:
                case TreeNodeType.DirParentDir:
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
            FileSystemItem_Expanded(TreeNodeType.FileParentDir, sender, e);
        }

        public void DirsItem_Expanded(object sender, RoutedEventArgs e)
        {
            FileSystemItem_Expanded(TreeNodeType.DirParentDir, sender, e);
        }

        public void FileSystemItem_Expanded(TreeNodeType treeNodeType, object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = sender as TreeViewItem;
                TreeNode node = item.Tag as TreeNode;
                string[] children = null;

                switch (treeNodeType)
                {
                    case TreeNodeType.DirParentDir:
                        children = Directory.GetDirectories(node.Path, node.Filter);
                        break;

                    case TreeNodeType.FileParentDir:
                    default:
                        children = Directory.GetFiles(node.Path, node.Filter);
                        break;
                }

                item.Items.Clear();
                this.TreeBuilder.AddChildNodes(treeNodeType, item.Items, children);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No!", MessageBoxButton.OK, MessageBoxImage.Hand);
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
