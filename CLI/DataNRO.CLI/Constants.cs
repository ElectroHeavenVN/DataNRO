using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataNRO.CLI
{
    internal class Constants
    {
        internal static readonly int NPC_CHEST = 3;
        internal static readonly int NPC_SENZU_BEAN_TREE = 4;
        internal static readonly int NPC_NOTIFICATOR = 5;
        internal static readonly int NPC_ZONE = 6;
        internal static readonly int NPC_SHENRON = 24;
        internal static readonly int NPC_PORUNGA = 27;
        internal static readonly int NPC_MABU_EGG = 50;
        internal static readonly int NPC_MABU_WATERMELON = 51;

        internal static readonly int[] EXCLUDED_NPCS = new int[]
        {
            NPC_CHEST,
            NPC_SENZU_BEAN_TREE,
            NPC_NOTIFICATOR,
            NPC_ZONE,
            NPC_SHENRON,
            NPC_PORUNGA,
            NPC_MABU_EGG,
            NPC_MABU_WATERMELON
        };
    }
}
