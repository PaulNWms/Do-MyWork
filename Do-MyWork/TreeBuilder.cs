using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace Do_MyWork
{
    class TreeBuilder
    {
        public TreeView Tree { get; private set; }
        public MyEventHandlers MyEventHandlers { get; private set; }

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

                if (xmlNode.Attributes["file"] != null)
                {
                    item.Tag = xmlNode.Attributes["file"].Value;
                    item.ContextMenu = new ContextMenu();
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Edit";
                    menuItem.Click += this.MyEventHandlers.MenuItem_Click;
                    item.ContextMenu.Items.Add(menuItem);
                    menuItem = new MenuItem();
                    menuItem.Header = "Open Folder";
                    menuItem.Click += this.MyEventHandlers.MenuItem_Click;
                    item.ContextMenu.Items.Add(menuItem);
                }

                if (xmlNode.Attributes["url"] != null)
                {
                    item.Tag = xmlNode.Attributes["url"].Value;
                    item.ContextMenu = new ContextMenu();
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = "Open URL";
                    menuItem.Click += this.MyEventHandlers.MenuItem_Click;
                    item.ContextMenu.Items.Add(menuItem);
                }

                if (xmlNode.Attributes["files"] != null)
                {
                    item.Tag = xmlNode.Attributes["files"].Value;
                    item.Expanded += this.MyEventHandlers.FilesItem_Expanded;
                    TreeViewItem placeholder = new TreeViewItem();
                    placeholder.Header = ".";
                    item.Items.Add(placeholder);
                }

                if (xmlNode.Attributes["dirs"] != null)
                {
                    item.Tag = xmlNode.Attributes["dirs"].Value;
                    item.Expanded += this.MyEventHandlers.DirsItem_Expanded;
                    TreeViewItem placeholder = new TreeViewItem();
                    placeholder.Header = ".";
                    item.Items.Add(placeholder);
                }

                items.Add(item);
                AddChildNodes(xmlNode, item.Items);
            }
        }
    }
}
