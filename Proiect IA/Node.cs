using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Proiect_IA
{
    /// <summary>
    /// Reprezinta un nod din harta
    /// </summary>
    
    enum SEARCHRETURN { BOUND, FOUND, NOT_FOUND };

    class Node
    {
        // daca nodul poate fi parcurs sau nu
        public bool accesibil { get; set; }
        // seteaza flag daca nodul este rezervat sau nu pentru seeker sau target
        public bool isReserved { get; set; }
        // pozitia pe harta
        public Point pozitieInJoc { get; set; }
        // block-ul vizual pe care il reprezinta
        public Panel blockNode;
        // parintete de care apartine
        public Node parentNode;
        // gCost -> reprezinta distanta de la nodul de start
        public int gCost;
        // hCost (heuristic) reprezinta dinstanta pana la nodul tinta
        public int hCost;

        /* IDA* implementare */ 
        // statusul in care se afla nodul
        public SEARCHRETURN status;
        public List<Node> noduriVizitate;

        /// <summary>
        /// C-tor vid
        /// </summary>
        public Node()
        {
            noduriVizitate = new List<Node>();
        }
        
        /// <summary>
        /// C-tor pentru crearea unui nod
        /// </summary>
        /// <param name="_accesibil"></param>
        /// <param name="_pozitie"></param>
        /// <param name="_panel"></param>
        public Node(bool _accesibil, Point _pozitie, Panel _panel)
        {
            this.accesibil = _accesibil;
            this.pozitieInJoc = _pozitie;
            this.blockNode = _panel;
        }

        /// <summary>
        /// Functie ce reprezinta costul unei mutari
        /// </summary>
        public int fCostFunction
        {
            get
            {
                return gCost + hCost;
            }
        }
    }
}
