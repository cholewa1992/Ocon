using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ScrumBoardPictures.Model
{
    public class ContextServiceStub : IContextService
    {
        public async void GetData(Action<BoardState, Exception> callback)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    int i = new Random().Next(1, 3);
                    callback((BoardState)i, null);
                }
            });

        }
    }
}