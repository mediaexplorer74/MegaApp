
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CG.Web.MegaApiClient; // !

namespace MegaApp.Model
{
    class MegaClient
    {
        public static MegaApiClient client = new MegaApiClient(); // our super client 

        // Ветка-Родитель =)
        public static INode parent;

        public static IEnumerable<INode> nodes;

        // Массив INode (емкость -- 10000)
        public static INode[] arNodes = new INode[10000];
    }
}
