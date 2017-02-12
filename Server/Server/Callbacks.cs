using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    /// <summary>
    /// This class will be used so other classes like worldhandlers can get informed when something happens
    /// an example could be:
    /// that the creature handler needs to know when a players joins so i would just add itself to OnPlayerEnterMap
    /// </summary>
    public static class Callback {
        public delegate void OnPlayerEnterMap(int AccountSocketId, int MapId);
        public static event OnPlayerEnterMap PlayerEnteredMap;

        public static void CallPlayerEnteredMap(int AccountSocketId, int MapId) {
            PlayerEnteredMap(AccountSocketId, MapId);
        }
    }
}