using System;
using System.Threading;
using System.Threading.Tasks;
using ContextawareFramework;

namespace TestWidget
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var testPerson = new Person
            {
                Name = "Jacob Cholewa",
                Description = "I'm a cool person!"
            };

            var w = new Widget();
            w.TrackEntity(testPerson);

            Task.Run(() =>
            {
                while (true)
                {
                    testPerson.i = new Random().Next(10);
                    Console.WriteLine(testPerson.i);
                    w.Notify(testPerson);
                    Thread.Sleep(500);
                }
            });

            Console.ReadLine();
        }
    }
}