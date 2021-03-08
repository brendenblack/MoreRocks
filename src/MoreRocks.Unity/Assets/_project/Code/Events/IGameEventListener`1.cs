using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Code.Events
{
    public interface IGameEventListener<TArgs>
    {
        void OnEventRaised(TArgs args);
    }
}
