namespace Do_MyWork
{
    public enum TreeNodeType { File, FileParent, ChildFile, DirParent, ChildDir, Url }

    class TreeNode
    {
        public TreeNodeType Type { get; private set; }
        public string Path { get; private set; }
        public string Filter { get; private set; }

        public TreeNode(TreeNodeType treeNodeType, string path, string filter)
        {
            this.Type = treeNodeType;
            this.Path = path;
            this.Filter = filter;
        }
    }
}
