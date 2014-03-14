using System;
using System.IO;
using System.Linq;
using Microsoft.Kinect;

namespace TestWidget
{
    internal class Kinect
    {
        private KinectSensor _sensor;
        public event EventHandler<KinectEventArgs> KinectEvent;

        internal class KinectEventArgs : EventArgs
        {
            public bool PeoplePresent { get; private set; }
            public int NumberOfPeople { get; set; }

            public KinectEventArgs(bool peoplePresent, int numberOfPeople)
            {
                PeoplePresent = peoplePresent;
                NumberOfPeople = numberOfPeople;
            }
        }

        public void StartKinect()
        {
            Console.Write("Looking for sensor");
            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    Console.WriteLine("\nFound sensor");
                    _sensor = potentialSensor;
                    break;
                }
                Console.Write(".");
            }

            if (null != _sensor)
            {
                _sensor.SkeletonStream.Enable();
                _sensor.SkeletonFrameReady += SensorSkeletonFrameReady;

                try
                {
                    _sensor.Start();
                }
                catch (IOException)
                {
                    _sensor = null;
                }
            }
        }

        public void FireTestEvent()
        {
            var i = new Random().Next(6);
            Console.WriteLine("TestEvent: " + i);
            KinectEvent(this, new KinectEventArgs(true,i));
        }

        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            var skeletons = new Skeleton[0];
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }

                //if (skeletons.Length == 0) return;

                //foreach (var skel in skeletons.Where(skel => skel.TrackingState == SkeletonTrackingState.Tracked))
                //{
                //
                //

                if (skeletons.Any())
                {
                    KinectEvent(this, new KinectEventArgs(true, skeletons.Count(skel => skel.TrackingState == SkeletonTrackingState.Tracked || skel.TrackingState == SkeletonTrackingState.PositionOnly)));
                }
                else
                {
                    KinectEvent(this, new KinectEventArgs(false, 0));
                }

            }
        }

        public void Close()
        {
            if (null != _sensor)
            {
                _sensor.Stop();
            }
        }
    }
}