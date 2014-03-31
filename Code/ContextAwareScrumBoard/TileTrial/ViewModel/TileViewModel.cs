using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace TileTrial.ViewModel
{
    public class TileViewModel
    {
        public string Title { get; set; }

        public int Height { get; set; }
        public int Width { get; set; }

        public RelayCommand SelectionCommand { get; set; }



        public TileViewModel()
        {

            SelectionCommand = new RelayCommand(() => MessageBox.Show("selected"));

            Title = "I'm a tile!";
            Height = 100;
            Width = 100;
        }
    }
}