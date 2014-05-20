using System.IO;
using Ocon.Helper;
using Ocon.OconCommunication;

namespace Ocon
{
    public class OconCentral
    {
        private readonly IOconCom _comHelper;
        private readonly OconContextFilter _contextFilter;
        private readonly TextWriter _log;

        /// <summary>
        ///     Constructs the object given contextFilter and oconCom
        /// </summary>
        /// <param name="contextFilter"></param>
        /// <param name="oconCom"></param>
        public OconCentral(OconContextFilter contextFilter, IOconCom oconCom,
            TextWriter log = null)
        {
            _contextFilter = contextFilter;
            _comHelper = oconCom;
            _log = log;
            Initialize();
        }

        /// <summary>
        ///     Registers events and starts communication
        /// </summary>
        private void Initialize()
        {
            // Set up
            _contextFilter.SituationStateChanged +=
                (sender, args) => _comHelper.SendSituationState(args.Situation, args.Subscriber);

            // Set up events
            _comHelper.IncommingEntityEvent += (sender, args) =>
            {
                _contextFilter.TrackEntity(args.Entity);
                Logger.Write(_log, "Incoming entity event: " + args.Entity.Name);
            };

            _comHelper.IncommingSituationSubscribtionEvent += (sender, args) =>
            {
                _contextFilter.Subscribe(args.Peer, args.SituationIdentifier);
                Logger.Write(_log,
                    "Incoming situation subscribtion on: " + args.SituationIdentifier + " form:" + args.Peer.Guid);
            };


            // Start listening for widgets and clients
            _comHelper.StartListen();

            // Multicast presence to widgets
            _comHelper.Broadcast();
        }
    }
}
