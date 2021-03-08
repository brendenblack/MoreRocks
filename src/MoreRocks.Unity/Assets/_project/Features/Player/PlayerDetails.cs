using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._project.Features.Player
{
    public class PlayerDetails : MonoBehaviour
    {
        public Code.Player Player { get; private set; }

        public int TeamId;

        private void Awake()
        {
            // TODO: fetch from somewhere
        }

        private void Start()
        {
            // TODO: testing purposes
            if (Player == null)
            {
                Player = new Code.Player(Guid.NewGuid().ToString());
            }
        }
    }
}
