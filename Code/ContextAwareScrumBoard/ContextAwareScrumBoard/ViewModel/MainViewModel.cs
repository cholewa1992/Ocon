using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using ContextAwareScrumBoard.Model;

namespace ContextAwareScrumBoard.ViewModel
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

        public List<ProductItem> ProductItems { get; set; }

    #region colorsetup


    #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {

            

            var taskItems = new List<TaskItem>()
            {
                new TaskItem()
                {
                    Name = "TaskItem1",
                    BackgroundColor = Brushes.Brown
                },
                new TaskItem()
                {
                    Name = "TaskItem2",
                    BackgroundColor = Brushes.Brown
                },
            };

            var column = new Column()
            {
                Name = "Column",
                TaskItems = taskItems
            };


            ProductItems = new List<ProductItem>();

            var productItem = new ProductItem()
            {
                Name = "ProductItem",
                BackgroundColor = Brushes.BlueViolet
            };

            ProductItems.Add(productItem);
            ProductItems.Add(productItem);
            ProductItems.Add(productItem);

            productItem.Columns = new List<Column>();

            productItem.Columns.Add(column);
            productItem.Columns.Add(column);
            productItem.Columns.Add(column);


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

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}