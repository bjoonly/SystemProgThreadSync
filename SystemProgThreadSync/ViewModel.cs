using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SystemProgThreadSync
{
    class ViewModel: INotifyPropertyChanged
    {
        public ViewModel()
        {
            startCommand = new DelegateCommand(Start, () => !String.IsNullOrEmpty(DirectoryPath));
            selectFolderCommand = new DelegateCommand(SelectFolderSource);
            PropertyChanged += (sender, args) =>
            {


                if (args.PropertyName == nameof(DirectoryPath))
                {
                    startCommand.RaiseCanExecuteChanged();
                }
            };
            }
       private string directoryPath;
        public string DirectoryPath { get { return directoryPath; } set { if (value != directoryPath) { directoryPath = value;OnPropertyChanged(); } } }
        private TextCounter tC; 
        public TextCounter TC { get { return tC; } set { if (value != tC) { tC = value;OnPropertyChanged(); } } }

        private readonly ICollection<string> files = new ObservableCollection<string>();
        public IEnumerable<string> Files => files;
     
        public void Start()
        {
            try
            {
                TC = new TextCounter();
                var fileNames = Directory.GetFiles(DirectoryPath, "*.txt");
                files.Clear();
                foreach (var item in fileNames)
                {
                    files.Add(item);
                }
                foreach (var item in fileNames)
                {
                    FileStream fs = File.OpenRead(item);
                    ThreadPool.QueueUserWorkItem(TC.Calculate, fs);

                }
                
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private Command selectFolderCommand;
        public ICommand SelectFolderCommand => selectFolderCommand;
        private Command startCommand;
        public ICommand StartCommand => startCommand;
        private void SelectFolderSource()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                DirectoryPath = fbd.SelectedPath;

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
