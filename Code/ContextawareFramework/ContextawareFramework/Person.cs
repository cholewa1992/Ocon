using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace ContextawareFramework
{
    public class Person: IEntity
    {
        public int i;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}