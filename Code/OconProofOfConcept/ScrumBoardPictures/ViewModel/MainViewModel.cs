using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ocon;
using Ocon.OconCommunication;


namespace ScrumBoardPictures.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {


        private const string OverviewSituationString = "Overview";
        private const string StandupSituationString = "Standup";
        private const string CloseupSituationString = "Closeup";


        private readonly Dictionary<string, string> _viewMap = new Dictionary<string, string>();
       

        private string _imageUri;

        public string ImageUri
        {
            get { return _imageUri; }
            set
            {
                if (_imageUri == value)
                    return;

                _imageUri = value;
                RaisePropertyChanged("ImageUri");
            }
        }

        private string _statusText = "status";

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (_statusText == value)
                    return;

                _statusText = value;
                RaisePropertyChanged("StatusText");
            }
        }

        private bool _overrule;
        public bool Overrule
        {
            get { return _overrule; }
            set
            {
                if (_overrule == value)
                    return;

                _overrule = value;
                RaisePropertyChanged("Overrule");
            }
        }

        public RelayCommand OverviewCommand { get; set; }
        public RelayCommand StandupCommand { get; set; }
        public RelayCommand CloseupCommand { get; set; }


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            _viewMap.Add(OverviewSituationString, "pack://application:,,,/ScrumBoardPictures;component/BoardOverview.png");
            _viewMap.Add(StandupSituationString, "pack://application:,,,/ScrumBoardPictures;component/BoardStandup.png");
            _viewMap.Add(CloseupSituationString, "pack://application:,,,/ScrumBoardPictures;component/BoardCloseup.png");

            ImageUri = _viewMap["Overview"];


            //Setup commands
            OverviewCommand = new RelayCommand(() =>
            {
                ImageUri = _viewMap[OverviewSituationString];
                Overrule = true;
            });
            StandupCommand = new RelayCommand(() =>
            {
                ImageUri = _viewMap[StandupSituationString];
                Overrule = true;
            });
            CloseupCommand = new RelayCommand(() =>
            {
                ImageUri = _viewMap[CloseupSituationString];
                Overrule = true;
            });


            //Choose a logging instance if any
            var log = Console.Out;

            //Instantiate a network helper. Here passing the logging target
            //alternatively instantiate as new TcpHelper(); if no logging is needed
            var tcpCom = new OconTcpCom(log);

            //Instantiate the client with communication, log, and params of situation names strings
            var oconClient = new OconClient(tcpCom, log, StandupSituationString, CloseupSituationString);

            //Subscribe a delegate to be run when a situation change event is fired
            oconClient.SituationStateChangedEvent += (sender, args) => UpdatePicture(args.SituationName, args.State);

        }

        private void UpdatePicture(string name, bool state)
        {

            if (Overrule)
            {
                Console.WriteLine("Sensor overruled");
                return;
            }

            if (_viewMap.ContainsKey(name))
            {
                ImageUri = state ? _viewMap[name] : _viewMap["Overview"];
                StatusText += "\n update view request: " + name + " - state: " + state;
            }
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}