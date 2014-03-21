﻿using System;
using System.Net;
using System.Runtime.InteropServices;
using NetworkHelper;
using Newtonsoft.Json;

namespace ContextawareFramework
{
    public class ContextCentral
    {

        private readonly ICommunicationHelper _comHelper;
        private readonly ContextFilter _contextFilter;



        public ContextCentral(ContextFilter contextFilter, ICommunicationHelper communicationHelper)
        {
            _contextFilter = contextFilter;
            _comHelper = communicationHelper;
        }

        

        public void Initialize()
        {
            _comHelper.StartListen();
            _comHelper.Broadcast();
            


        }


        
    }
}