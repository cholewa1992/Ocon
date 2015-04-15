using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Ocon.Entity;
using Ocon.OconCommunication;

namespace Ocon
{

    public interface IOconSituation
    {
        Guid Id { get; set; }
        bool Evaluate(ICollection<IEntity> collection);
        void AddSubscriber(IOconPeer peer);
        void RemoveSubscriber(IOconPeer peer);
        ICollection<IOconPeer> GetSubscribersList();
        LambdaExpression Expression { get; set; }
    }

    public class Situation<T> : IOconSituation where T : IComparable<T>
    {
        private Guid _id = Guid.NewGuid();
        private readonly HashSet<IOconPeer> _peers = new HashSet<IOconPeer>(new PeerEquallityCompare());

        public LambdaExpression Expression { set; get; }

        public Situation(Expression<Func<ICollection<IEntity>,T>> expression)
        {
            Expression = expression;
        }

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Description { get; set; }

        public T Value { get; set; }

        public void AddSubscriber(IOconPeer peer)
        {
            _peers.Add(peer);
        }

        public void RemoveSubscriber(IOconPeer peer)
        {
            _peers.Remove(peer);
        }

        public ICollection<IOconPeer> GetSubscribersList()
        {
            return _peers;
        }

        public bool Evaluate(ICollection<IEntity> collection)
        {
            T oldValue = Value;
            Value = (T) Expression.Compile().DynamicInvoke(collection);
            if (!typeof(T).IsValueType && oldValue == null) return Value != null;
            return oldValue.CompareTo(Value) != 0;
        }

    }
}