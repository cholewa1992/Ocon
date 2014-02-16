using System;

namespace ContextawareFramework
{
    public class Sensor: ISensor
    {
        public event SensorEventHandler sensorevent;
        public delegate void SensorEventHandler(IEntity entity);

        public IEntity person = new Person();

        public void FireEvent(int i)
        {
            person.i = i;

            if (sensorevent != null)
            {
                sensorevent(person);
            }
        }
    }
}