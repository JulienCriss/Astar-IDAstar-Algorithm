using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Proiect_IA
{
    class IDAStar
    {
        public Grid grid;
        public static List<Node> visitedNodes;
        public static int minValue;
        public int numberOfSteps = 0;

        public static int costLimit = 0;

        /// <summary>
        /// C-tor IDAStar
        /// </summary>
        public IDAStar()
        {
            visitedNodes = new List<Node>();
            costLimit = minValue = int.MaxValue;
        }
        /// <summary>
        /// Algoritmul IDA*
        /// </summary>
        /// <param name="_gridMap"></param>
        /// <param name="startPoint"></param>
        /// <param name="targetPoint"></param>
        /// <returns></returns>
        public void IDA_Star(Grid _gridMap, Point startPoint, Point targetPoint)
        {
            numberOfSteps = 0;

            // preia harta
            grid = _gridMap;

            // preia nodul de start
            Node startNode = grid.GetNodeFromMap(startPoint);

            // preia nodul tinta
            Node targetNode = grid.GetNodeFromMap(targetPoint);

            // seteaza costul nodului de start
            startNode.gCost = 0;

            // seteaza costul euristic
            startNode.hCost = GetDistanceHeuristic(startNode, targetNode);

            // seteaza cutt-off limit sau treshold pentru functia f()
            int fCostLimit = startNode.hCost;

            while (true)
            {
                //incrementeaza numarul de pasi
                ++numberOfSteps;

                // incepe cautarea
                Node foundTemp = search_path(_gridMap, startNode, targetNode, startNode.gCost, fCostLimit);
                switch (foundTemp.status)
                {
                    // a gasit o noua limita a f() costului
                    case SEARCHRETURN.BOUND:
                        {
                            fCostLimit = foundTemp.hCost;
                            break;
                        }
                    // a gasit calea
                    case SEARCHRETURN.FOUND:
                        {
                            RetracePath(startNode, targetNode);
                            return;
                        }
                    // nu a fost gasita nici o cale
                    case SEARCHRETURN.NOT_FOUND:
                        return;
                }
            }
        }

        /// <summary>
        /// Cautare in adancime in functie de valoarea de cutt-off (costul limită)
        /// </summary>
        /// <param name="_gridMap"></param>
        /// <param name="_currentNode"></param>
        /// <param name="currentCost"></param>
        /// <param name="_FcostLimit"></param>
        /// <param name="h"></param>
        /// <param name="_targetNode"></param>
        /// <returns></returns>
        public Node search_path(Grid _gridMap, Node _currentNode, Node _targetNode, int _gCost, int _fCostLimit)
        {
            //incrementeaza numarul de pasi
            //numberOfSteps++;

            Node ret = new Node();

            if (_currentNode == _targetNode)
            {
                ret.status = SEARCHRETURN.FOUND;
                //ret.noduriVizitate = currentNode.noduriVizitate;
                return ret;
            }

            //int new_fCostLimit = gCost + currentNode.hCost;
            int new_fCostLimit = _gCost + GetDistanceHeuristic(_currentNode, _targetNode);

            if (new_fCostLimit > _fCostLimit)
            {
                ret.status = SEARCHRETURN.BOUND;
                ret.hCost = new_fCostLimit;
                return ret;
            }

            // adauga nodul la noduri explorate
            if (!visitedNodes.Contains(_currentNode))
                visitedNodes.Add(_currentNode);

            // parcurgem vecini fiecarui nod si ne extindem 
            foreach (Node vecin in _gridMap.GetNeighbours(_currentNode))
            {

                // daca blocul nu este accesbil sau nodul a fost deja explorat
                if (!vecin.accesibil || visitedNodes.Contains(vecin))
                    continue;

                // daca nodul nu a fost vizitat
                if (!visitedNodes.Contains(vecin))
                {
                    // mod vizual pentru noduri explorate
                    // vecin.blockNode.BackColor = Color.Crimson;

                    // calculam costul catre vecin
                    int costNouCatreVecin = _gCost + GetDistanceHeuristic(_currentNode, vecin);

                    if (costNouCatreVecin < vecin.gCost || !visitedNodes.Contains(vecin))
                    {
                        // calculaza gCost pentru vecin
                        vecin.gCost = costNouCatreVecin;
                        // calculeaza hCost pentru vecin
                        vecin.hCost = GetDistanceHeuristic(vecin, _targetNode);
                        // seteaza parintele vecin
                        vecin.parentNode = _currentNode;
                    }

                    // cautam in adancime
                    Node t = search_path(_gridMap, vecin, _targetNode, costNouCatreVecin, _fCostLimit);
                    switch (t.status)
                    {
                        case SEARCHRETURN.BOUND:
                            {
                                if (t.hCost < costLimit)
                                {
                                    costLimit = t.hCost;
                                }
                                break;
                            }
                        case SEARCHRETURN.FOUND:
                            {
                                return t;
                            }
                        case SEARCHRETURN.NOT_FOUND:
                            {
                                continue;
                            }
                    }
                }
            }

            if (costLimit == minValue)
            {
                ret.status = SEARCHRETURN.NOT_FOUND;
            }
            else
            {
                ret.hCost = costLimit;
                ret.status = SEARCHRETURN.BOUND;
            }

            visitedNodes.Remove(_currentNode);
            return ret;
        }

        /// <summary>
        /// Calculeaza distanta dintre doua noduri (euristica)
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        int GetDistanceHeuristic(Node nodeA, Node nodeB)
        {
            int distX = Math.Abs(nodeA.pozitieInJoc.X - nodeB.pozitieInJoc.X);
            int distY = Math.Abs(nodeA.pozitieInJoc.Y - nodeB.pozitieInJoc.Y);

            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);

            return 14 * distX + 10 * (distY - distX);
        }

        /// <summary>
        /// Deseneaza calea pana la nodul tinta
        /// </summary>
        /// <param name="startNode"></param>
        /// <param name="targetNode"></param>
        void RetracePath(Node startNode, Node targetNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = targetNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.parentNode;
            }

            path.Reverse();
            if (path.Count > 0)
            {
                // colorare solitie gasita dintre nodul de start si nodul tinta
                for (int i = 0; i < path.Count; i++)
                {
                    path[i].blockNode.BackColor = Color.DarkRed;
                }
                grid.path = path;
            }
            else
            {
                MessageBox.Show("Nu se poate gasi o cale!\nDrumul este blocat de obstacole!");
            }
        }
    }
}
