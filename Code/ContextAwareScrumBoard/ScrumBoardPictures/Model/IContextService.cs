using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrumBoardPictures.Model
{
    public interface IContextService
    {
        void GetData(Action<BoardState, Exception> callback);
    }
}
