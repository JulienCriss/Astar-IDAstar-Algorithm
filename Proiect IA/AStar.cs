using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Proiect_IA
{
    class AStar
    {
        public Grid grid;
        public int numberOfSteps = 0;

        /// <summary>
        /// Alogoritmul A*
        /// </summary>
        /// <param name="map"></param>
        /// <param name="startPoint"></param>
        /// <param name="targetPoint"></param>
        public void AStarFindPath(Grid map, Point startPoint, Point targetPoint)
        {
            numberOfSteps = 0;
            // preia harta
            grid = map;

            // preia nodul de start
            Node startNode = grid.GetNodeFromMap(startPoint);

            // preia nodul tinta
            Node targetNode = grid.GetNodeFromMap(targetPoint);

            // va retine nodurile care trebuie evaluate
            List<Node> openSet = new List<Node>();

            // va retine nodurile deja evaluate/explorate
            HashSet<Node> closeSet = new HashSet<Node>();

            // adauga nodul de unde se incepe cautarea
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                //incrementeaza numarul de pasi
                numberOfSteps++;

                Node nodCurent = openSet[0];
                // cauta nodul cu fCost cel mai mic
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCostFunction < nodCurent.fCostFunction || openSet[i].fCostFunction == nodCurent.fCostFunction && openSet[i].hCost < nodCurent.hCost)
                    {
                        nodCurent = openSet[i];
                    }
                }

                openSet.Remove(nodCurent);
                closeSet.Add(nodCurent);

                if (nodCurent == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                foreach (Node vecin in grid.GetNeighbours(nodCurent))
                {
                    if (!vecin.accesibil || closeSet.Contains(vecin))
                        continue;
                    // mod vizual pentru noduri explorate
                    // vecin.blockNode.BackColor = Color.Crimson;

                    int costNouCatreVecin = nodCurent.gCost + GetDistance(nodCurent, vecin);
                    if (costNouCatreVecin < vecin.gCost || !openSet.Contains(vecin))
                    {
                        // calculaza gCost pentru vecin
                        vecin.gCost = costNouCatreVecin;
                        // calculeaza hCost pentru vecin
                        vecin.hCost = GetDistance(vecin, targetNode);
                        // seteaza parintele fiecarui vecin
                        vecin.parentNode = nodCurent;

                        // daca vecinul nu se afla in lista de noduri explorate, atunci il adaugam
                        if (!openSet.Contains(vecin))
                            openSet.Add(vecin);
                    }
                }
            }
            // daca lista open set este goala atunci drumul este blocat
            if (openSet.Count == 0)
            {
                MessageBox.Show("Nu se poate gasi o cale!\nDrumul este blocat de obstacole!");
            }
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
        }

        /// <summary>
        /// Calculeaza distanta dintre doua noduri
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns></returns>
        int GetDistance(Node nodeA, Node nodeB)
        {
            int distX = Math.Abs(nodeA.pozitieInJoc.X - nodeB.pozitieInJoc.X);
            int distY = Math.Abs(nodeA.pozitieInJoc.Y - nodeB.pozitieInJoc.Y);

            if (distX > distY)
                return 14 * distY + 10 * (distX - distY);

            return 14 * distX + 10 * (distY - distX);
        }
    }
}
