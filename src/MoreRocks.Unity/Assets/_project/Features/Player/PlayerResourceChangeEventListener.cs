using Assets.Code.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Features.Player
{
    public class PlayerResourceChangeEventListener : GameEventListener<PlayerResourceChangeEventArgs>, IGameEventListener<PlayerResourceChangeEventArgs>
    {
    }
}
