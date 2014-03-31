using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TileTrial.Model;

namespace TileTrial.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                if (_welcomeTitle == value)
                {
                    return;
                }

                _welcomeTitle = value;
                RaisePropertyChanged(WelcomeTitlePropertyName);
            }
        }

        public List<UserControl> ProductItems { get; set; }
        public List<UserControl> TodoTasks { get; set; }
        public List<UserControl> DoingTasks { get; set; }
        public List<UserControl> DoneTasks { get; set; }

        public RelayCommand SelectionCommand { get; set; }

        public UserControl testcontrol { get; set; }

        private double _testHeight;
        public double testHeight 
        {
            get { return _testHeight; }
            set
            {
                if (_testHeight == value) return;

                _testHeight = value;
                RaisePropertyChanged("testHeight");
            } 
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {

            testcontrol = new TileControl();
            SelectionCommand = new RelayCommand(() => MessageBox.Show("selected!"));

            ProductItems = new List<UserControl>();
            TodoTasks = new List<UserControl>();
            DoingTasks = new List<UserControl>();
            DoneTasks = new List<UserControl>();
            ProductItems.Add(new TileControl());
            ProductItems.Add(new TileControl());
            TodoTasks.Add(new TileControl());
            TodoTasks.Add(new TileControl());
            DoingTasks.Add(new TileControl());
            DoneTasks.Add(new TileControl());
            DoneTasks.Add(new TileControl());
            DoneTasks.Add(new TileControl());

            testHeight = 50.0;

            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    WelcomeTitle = item.Title;
                });
        }


        public void Drag(Point p)
        {
            testHeight = p.Y;
        }


        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}