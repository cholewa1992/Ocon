using System;
using System.Collections.Generic;

namespace ContextawareFramework
{
    public class ContextFilter
    {
        private List<IEntity> entities;


        public void AddEntity(IEntity entity)
        {
            entities.Add(entity);
        }

        public bool AddSensor(ISensor sensor)
        {


            return true;
        }

        public bool Subscribe(Type t)
        {



            return true;
        }
    }
}