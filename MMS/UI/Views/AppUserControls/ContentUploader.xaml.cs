using Microsoft.Win32;
using MMS.Backend;
using MMS.DataModels;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MMS.UI.Views.AppUserControls
{
    /// <summary>
    /// Interaction logic for ContentUploader.xaml
    /// </summary>
    public partial class ContentUploader : UserControl
    {
        private string PathPrefix;
        private string[] FilesToUpload;

        public NodeModel Node { get; set; }
        public string Mode { get; set; }
        public ContentUploader()
        {
            InitializeComponent();
        }

        private void UploaderLoaded(object sender, RoutedEventArgs e)
        {
            if (Mode == "Content") PathPrefix = @"files\media\content";
            else if (Mode == "Software") PathPrefix = @"files\media\software";
        }

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            var res = ofd.ShowDialog(App.Current.MainWindow);
            if (res.HasValue)
            {
                if (res.Value) FilesToUpload = ofd.FileNames;
            }
        }

        private void UploadClick(object sender, RoutedEventArgs e)
        {
            if (Mode == "Software")
            {
                var files = Directory.GetFiles(PathPrefix);
                foreach (var file in files) File.Delete(file);
            }
            foreach (string file in FilesToUpload)
            {
                string filepath = Path.GetFullPath(Path.Combine(PathPrefix, Path.GetFileName(file)));
                if (!File.Exists(filepath)) File.Copy(file, filepath);
            }
            if (Mode == "Content")
            {
                var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.AddContent);
                command?.Prepare();
                command?.Parameters.Add(JsonConvert.SerializeObject(FilesToUpload.Select(a => Path.GetFileName(a))));
                Globals.SendCommand(command, Node, true);
            }
            if (Mode == "Software")
            {
                var command = DataHub.Commands.FirstOrDefault(a => a.CommandNumber == (int)CommandNumber.Update);
                command?.Prepare();
                Globals.SendCommand(command, Node, true);
            }
        }

        private void ContentDropped(object sender, DragEventArgs e)
        {
            FilesToUpload = (string[])e.Data.GetData(DataFormats.Text);
        }

        private void UploaderDragOver(object sender, DragEventArgs e)
        {

        }
    }
}
