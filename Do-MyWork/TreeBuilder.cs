using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Text.RegularExpressions;

namespace Do_MyWork
{
    class TreeBuilder
    {
        public TreeView Tree { get; private set; }
        public MyEventHandlers MyEventHandlers { get; private set; }
        private Regex exePattern = new Regex(@"\.exe$", RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex cmdPattern = new Regex(@"\.bat$|\.cmd$", RegexOptions.Singleline | RegexOptions.Compiled);
        private Regex ps1Pattern = new Regex(@"\.ps1$", RegexOptions.Singleline | RegexOptions.Compiled);

        public TreeBuilder(MyEventHandlers myEventHandlers, TreeView tree)
        {
            this.MyEventHandlers = myEventHandlers;
            this.Tree = tree;
        }

        public void BuildTree()
        {
            try
            {
                XmlNode treeContents = (XmlNode)ConfigurationManager.GetSection("TreeContents");
                this.Tree.Items.Clear();
                this.Tree.BeginInit();
                AddChildNodes(treeContents, this.Tree.Items);
                this.Tree.EndInit();
            }
            catch (ConfigurationErrorsException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddChildNodes(XmlNode parentXmlNode, ItemCollection items)
        {
            foreach (XmlNode xmlNode in parentXmlNode.ChildNodes)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = xmlNode.Attributes["name"].Value;
                string path = null;
                string filter = null;

                if (xmlNode.Attributes["file"] != null)
                {
                    item.Tag = new TreeNode(TreeNodeType.File, xmlNode.Attributes["file"].Value, null);
                    AddFileMenu(item);
                }

                if (xmlNode.Attributes["url"] != null)
                {
                    item.Tag = new TreeNode(TreeNodeType.Url, xmlNode.Attributes["url"].Value, null);
                    item.ContextMenu = new ContextMenu();
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Open URL";
                    menuItem.Click += this.MyEventHandlers.MenuItem_Click;
                    item.ContextMenu.Items.Add(menuItem);
                }

                if (xmlNode.Attributes["files"] != null)
                {
                    if (TryGetPathFilter(xmlNode.Attributes["files"].Value, out path, out filter))
                    {
                        item.Tag = new TreeNode(TreeNodeType.FileParent, path, filter);
                        item.Expanded += this.MyEventHandlers.FilesItem_Expanded;
                        AddDirectoryMenu(item);
                        AddPlaceholder(item);
                    }
                }

                if (xmlNode.Attributes["dirs"] != null)
                {
                    if (TryGetPathFilter(xmlNode.Attributes["dirs"].Value, out path, out filter))
                    {
                        item.Tag = new TreeNode(TreeNodeType.DirParent, path, filter);
                        item.Expanded += this.MyEventHandlers.DirsItem_Expanded;
                        AddDirectoryMenu(item);
                        AddPlaceholder(item);
                    }
                }

                items.Add(item);
                AddChildNodes(xmlNode, item.Items);
            }
        }

        public void AddChildNodes(TreeNodeType treeNodeType, ItemCollection items, string[] children)
        {
            if (children.Length == 0)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = ".";
                items.Add(item);
            }
            else
            {
                foreach (string child in children)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Header = child;

                    switch (treeNodeType)
                    {
                        case TreeNodeType.DirParent:
                            item.Tag = new TreeNode(TreeNodeType.Dir, child, null);
                            AddDirectoryMenu(item);
                            break;

                        case TreeNodeType.FileParent:
                        default:
                            item.Tag = new TreeNode(TreeNodeType.File, child, null);
                            AddFileMenu(item);
                            break;
                    }

                    items.Add(item);
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

        private void AddPlaceholder(TreeViewItem item)
        {
            TreeViewItem placeholder = new TreeViewItem();
            placeholder.Header = ".";
            item.Items.Add(placeholder);
        }

        private void AddFileMenu(TreeViewItem item)
        {
            item.ContextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Edit";
            menuItem.Click += this.MyEventHandlers.MenuItem_Click;
            item.ContextMenu.Items.Add(menuItem);
            menuItem = new MenuItem();
            menuItem.Header = "Open Folder";
            menuItem.Click += this.MyEventHandlers.MenuItem_Click;
            item.ContextMenu.Items.Add(menuItem);
            menuItem = new MenuItem();
            menuItem.Header = "Open CMD";
            menuItem.Click += this.MyEventHandlers.MenuItem_Click;
            item.ContextMenu.Items.Add(menuItem);
            menuItem = new MenuItem();
            menuItem.Header = "Open PowerShell";
            menuItem.Click += this.MyEventHandlers.MenuItem_Click;
            item.ContextMenu.Items.Add(menuItem);
            TreeNode node = item.Tag as TreeNode;

            if (exePattern.IsMatch(node.Path))
            {
                item.ContextMenu.Items.Add(new Separator());
                menuItem = new MenuItem();
                menuItem.Header = "Run Executable";
                menuItem.Click += this.MyEventHandlers.MenuItem_Click;
                item.ContextMenu.Items.Add(menuItem);
            }
            else if (cmdPattern.IsMatch(node.Path))
            {
                item.ContextMenu.Items.Add(new Separator());
                menuItem = new MenuItem();
                menuItem.Header = "Run Batch";
                menuItem.Click += this.MyEventHandlers.MenuItem_Click;
                item.ContextMenu.Items.Add(menuItem);
            }
            else if (ps1Pattern.IsMatch(node.Path))
            {
                item.ContextMenu.Items.Add(new Separator());
                menuItem = new MenuItem();
                menuItem.Header = "Run Script";
                menuItem.Click += this.MyEventHandlers.MenuItem_Click;
                item.ContextMenu.Items.Add(menuItem);
            }
        }

        private void AddDirectoryMenu(TreeViewItem item)
        {
            item.ContextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();
            menuItem.Header = "Open Folder";
            menuItem.Click += this.MyEventHandlers.MenuItem_Click;
            item.ContextMenu.Items.Add(menuItem);
            menuItem = new MenuItem();
            menuItem.Header = "Open CMD";
            menuItem.Click += this.MyEventHandlers.MenuItem_Click;
            item.ContextMenu.Items.Add(menuItem);
            menuItem = new MenuItem();
            menuItem.Header = "Open PowerShell";
            menuItem.Click += this.MyEventHandlers.MenuItem_Click;
            item.ContextMenu.Items.Add(menuItem);
        }
    }
}
