﻿using System.Configuration;
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
                string path = null;
                string filter = null;

                if (xmlNode.Attributes["file"] != null)
                {
                    if (TryGetPathFilter(xmlNode.Attributes["file"].Value, out path, out filter))
                    {
                        item.Tag = new TreeNode(TreeNodeType.File, path, filter);
                        AddFileMenu(item);
                    }
                }

                if (xmlNode.Attributes["url"] != null)
                {
                    item.Tag = new TreeNode(TreeNodeType.Url, path, null);
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
                        AddPlaceholder(item);
                    }
                }

                if (xmlNode.Attributes["dirs"] != null)
                {
                    if (TryGetPathFilter(xmlNode.Attributes["dirs"].Value, out path, out filter))
                    {
                        item.Tag = new TreeNode(TreeNodeType.DirParent, path, filter);
                        item.Expanded += this.MyEventHandlers.DirsItem_Expanded;
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
                    item.Tag = child;

                    switch (treeNodeType)
                    {
                        case TreeNodeType.DirParent:
                            AddDirectoryMenu(item);
                            break;

                        case TreeNodeType.FileParent:
                        default:
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
            item.ContextMenu = new ContextMenu();
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
