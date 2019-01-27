using System.Configuration;
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
            var appSettings = ConfigurationManager.AppSettings;
            string editor = appSettings.Get("Editor");
            var eventHandlers = new MyEventHandlers(editor, tree);
            TreeBuilder treeBuilder = new TreeBuilder(eventHandlers, this.tree);
            treeBuilder.BuildTree();
            tree.Focus();
        }
    }
}
