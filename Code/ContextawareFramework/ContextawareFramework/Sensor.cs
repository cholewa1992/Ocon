using System;

namespace ContextawareFramework
{
    public class Sensor: ISensor
    {
        public event SensorEventHandler sensorevent;
        public delegate void SensorEventHandler(IEntity entity);
    }
}