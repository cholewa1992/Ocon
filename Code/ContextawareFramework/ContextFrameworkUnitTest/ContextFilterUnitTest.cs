using System;
using System.Collections.Generic;
using ContextawareFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContextFrameworkUnitTest
{
    [TestClass]
    public class ContextFilterUnitTest
    {


        [TestMethod]
        public void AddSubscription_Valid_SuccessfulAdd()
        {
            var filter = new ContextFilter();
            var guid = new Guid();

            filter.AddSituation(new Situation(entities => false){Name = "s1"}, 
                new Situation(entities => false){Name = "s2"}, 
                new Situation(entities => false){Name = "s3"});

            
        }
    }
}
