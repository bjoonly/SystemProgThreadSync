using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SystemProgThreadSync
{
    class TextCounter:INotifyPropertyChanged
    {
        int countWords=0;
        int countLines=0;
        int countPunctuationMarks=0;
        public int CountWords { get { return countWords; } set { countWords = value; OnPropertyChanged(); } }
        public int CountLines { get { return countLines; } set { countLines = value; OnPropertyChanged(); } }
        public int CountPunctuationMarks { get { return countPunctuationMarks; } set { countPunctuationMarks = value;OnPropertyChanged(); } }
        public void Calculate(object obj)
        {
            lock (this)
            {
                FileStream file = (FileStream)obj;
                StreamReader streamReader = new StreamReader(file);
                string line;
                string[] punct = new string[] { ",",".", ";", ":", "-", "—", "!", "?", "\"", "'", "«", "»", "(", ")", "{", "}", "[", "]", "<", ">", "/" };
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();

                        int index = line.IndexOf("...");
                     while (index != -1)
                    {
                        line = line.Remove(index, 3);
                        index = line.IndexOf("...", index);
                        CountPunctuationMarks++;
                    }
                    foreach (var item in line)
                    {
                        if (punct.Contains(item.ToString()))
                            CountPunctuationMarks++;
                    }
                     
                CountLines++;
                CountWords += (line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Count();
                    Thread.Sleep(500);
            }
        }
    }
      
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
