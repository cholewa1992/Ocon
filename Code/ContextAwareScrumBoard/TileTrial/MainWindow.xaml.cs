using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TileTrial.ViewModel;

namespace TileTrial
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            var draggableControl = sender as UserControl;

            
                Point currentPosition = e.GetPosition(this.Parent as UIElement);

                var transform = draggableControl.RenderTransform as TranslateTransform;
                if (transform == null)
                {
                    transform = new TranslateTransform();
                    draggableControl.RenderTransform = transform;
                }


            transform.X = 300.0;
            transform.Y = 200.0;

        }
    }
}