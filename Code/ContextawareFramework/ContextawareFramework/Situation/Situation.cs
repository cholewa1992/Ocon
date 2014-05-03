using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace ContextawareFramework
{
    [Serializable()]
    public class Situation: ISituation
    {

        public Situation(SerializationInfo info, StreamingContext ctxt)
        {
            Id = (Guid) info.GetValue("Id", typeof(Guid));
            SubscribersAddresse = (Guid)info.GetValue("SubscribersAddresse", typeof(Guid));
            SituationPredicate =
                (Predicate<ICollection<IEntity>>)
                    info.GetValue("SituationPredicate", typeof (Predicate<ICollection<IEntity>>));
            State = (bool) info.GetValue("State", typeof (bool));
            Description = (string) info.GetValue("Description", typeof (string));



            info.AddValue("Id", Id);
            info.AddValue("SubscribersAddresse", SubscribersAddresse);
            info.AddValue("SituationPredicate", SituationPredicate);
            info.AddValue("State", State);
            info.AddValue("Description", Description);
        }

        private Guid _id = Guid.NewGuid();

        public string Name { get; set; }

        public Guid Id
        {
            get { return _id; }
            private set { _id = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(value), "Description argument can't be null or empty");
                _description = value;
            }
        }

        public bool State { get; set; }


        private Predicate<ICollection<IEntity>> _situationPredicate;
        public Predicate<ICollection<IEntity>> SituationPredicate
        {
            get { return _situationPredicate; }
            set
            {
                _situationPredicate = value;
            }
        }

        public Guid SubscribersAddresse { get; set; }


        public Situation(Predicate<ICollection<IEntity>> situationPredicate)
        {
            SituationPredicate = situationPredicate;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("SubscribersAddresse", SubscribersAddresse);
            info.AddValue("SituationPredicate", SituationPredicate);
            info.AddValue("State", State);
            info.AddValue("Description", Description);
        }
    }
}