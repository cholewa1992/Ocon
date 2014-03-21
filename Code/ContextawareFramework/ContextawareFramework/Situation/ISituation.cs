using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace ContextawareFramework
{
    public interface ISituation
    {

        
        Guid Id { get; }
        
        string Description { get; set; }
        
        Predicate<ICollection<IEntity>> SituationPredicate { get; set; }

        ICollection<IPAddress> SubscribersAddresses { get; set; }


    }

}
