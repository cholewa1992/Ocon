using System;
using System.Collections.Generic;
using ContextawareFramework;
using NetworkHelper;

namespace Client
{
    public class Client
    {
        private readonly ICommunicationHelper _comHelper;

        public Client(ICommunicationHelper comHelper)
        {
            _comHelper = comHelper;
        }

        public event EventHandler ContextEvent;



        public void RegisterSituation(ISituation predicates)
        {
            
        }
    }
}
