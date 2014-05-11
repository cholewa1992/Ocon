using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Cache;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ContextawareFramework;
using ContextawareFramework.NetworkHelper;
using GalaSoft.MvvmLight;
using ScrumBoardPictures.Model;


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


        private const string OverviewSituationName = "Overview";
        private const string StandupSituationName = "Standup";
        private const string CloseupSituationName = "Closeup";



        private readonly IContextService _contextService;

        private Dictionary<string, string> _viewMap = new Dictionary<string, string>();
       

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


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IContextService contextService)
        {
            var writer = new StringWriter();

            Task.Run(() =>
            {
                while (true)
                {
                    if (!string.IsNullOrEmpty(writer.ToString())) StatusText = writer.ToString();
                    Thread.Sleep(200);
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    writer.Write("hej");
                    Thread.Sleep(800);
                }
            });
            
            _viewMap.Add(OverviewSituationName, "pack://application:,,,/ScrumBoardPictures;component/BoardOverview.jpg");
            _viewMap.Add(StandupSituationName, "pack://application:,,,/ScrumBoardPictures;component/BoardStandup.jpg");
            _viewMap.Add(CloseupSituationName, "pack://application:,,,/ScrumBoardPictures;component/BoardCloseup.jpg");

            ImageUri = _viewMap["Overview"];



            /*var comHelper = new TcpHelper(writer);
            string[] situationNames = { CloseupSituationName, StandupSituationName };

            var frameworkClient = new Client(comHelper, null, situationNames);

            frameworkClient.SituationStateChangedEvent += (sender, args) => UpdatePicture(args.SituationName, args.State);*/

            //Choose a logging instance if any
            var log = Console.Out;

            //Instantiate a network helper. Here passing the logging target
            //alternatively instantiate as new TcpHelper(); if no logging is needed
            var comHelper = new TcpHelper(log);

            //

            //Instantiate the client
            var oconClient = new Client(comHelper, log, "situationName1", "SituationName2");

            //Subscribe a delegate to be run when a situation change event is fired
            oconClient.SituationStateChangedEvent += (sender, args) => ActOnChange(args.SituationName, args.State);
        }

        private void UpdatePicture(string name, bool state)
        {

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