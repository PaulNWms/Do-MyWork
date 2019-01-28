using System;
using System.Configuration;
using System.IO;
using System.Windows;
namespace Do_MyWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadAppSettings();

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = ".";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "Do-MyWork.exe.config";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Action action = ReloadAppSettings;
            Dispatcher.BeginInvoke(action);
        }

        private void LoadAppSettings()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string editor = appSettings.Get("Editor");
            var eventHandlers = new MyEventHandlers(editor, this.tree);
            TreeBuilder treeBuilder = new TreeBuilder(eventHandlers, this.tree);
            eventHandlers.TreeBuilder = treeBuilder;
            treeBuilder.BuildTree();
            this.tree.Focus();
        }

        private void ReloadAppSettings()
        {
            ConfigurationManager.RefreshSection("appSettings");
            ConfigurationManager.RefreshSection("TreeContents");
            LoadAppSettings();
        }
    }
}
