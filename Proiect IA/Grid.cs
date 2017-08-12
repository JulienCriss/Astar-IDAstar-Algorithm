using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Proiect_IA
{
    /// <summary>
    /// Reprezinta harta jocului impreuna cu nodurile de pe harta
    /// </summary>
    class Grid
    {
        // harta unde vor fi plasate block-urile (vizual)
        public Panel gameWorld;

        // lista care contine nodurile ce conduc la solutia finala
        public List<Node> path;

        // nodurile din grid
        Node[,] grid;

        // diametrul unui nod in grid
        public int nodDiameter;
        // numarul de block-uri
        int numberOfBlocks;
        // dimensiunile grid-ului
        int gridSizeX; int gridSizeY;

        /// <summary>
        /// Returneaza toate nodurile de pe harta
        /// </summary>
        public Node[,] GetNodesGrid
        {
            get
            {
                return grid;
            }
        }

        /// <summary>
        /// C-tor pentru a crea nodurile
        /// </summary>
        /// <param name="_panel"></param>
        /// <param name="_numberOfBlocks"></param>
        public Grid(Panel _panel, int _numberOfBlocks)
        {
            this.gameWorld = _panel;
            //this.nodDiameter = _nodDiameter;
            this.numberOfBlocks = _numberOfBlocks;
        }

        /// <summary>
        /// Deseneaza grid-ul
        /// </summary>
        public void drawGrid()
        {
            // calculeaza in cate block-uri va fi impartita harta
            nodDiameter = gameWorld.Size.Width / numberOfBlocks;

            gridSizeX = gameWorld.Size.Width / nodDiameter;
            gridSizeY = gameWorld.Size.Height / nodDiameter;          
           
            // creaza grid-ul
            grid = new Node[gridSizeX, gridSizeY];

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Panel block = new Panel();
                    // creaza locatia undeva va fi desenat block-ul
                    Point pointLocation = new Point(x * nodDiameter, y * nodDiameter);
                    // setez marimea block-ului
                    block.Size = new Size(nodDiameter, nodDiameter);
                    // setez locatia
                    block.Location = pointLocation;

                    block.BorderStyle = BorderStyle.Fixed3D;
                    block.BackColor = Color.Transparent;

                    // adaug functionalitate daca sa fie sau nu accesbil (vizual)
                    block.Click += lbl_Click;
                    // il adaug pe harta
                    gameWorld.Controls.Add(block);

                    Point gridPoint = new Point(x, y);
                    // implictit toate blocurile sunt accesibile
                    grid[x, y] = new Node(true, gridPoint, block);
                }
            }
        }

        /// <summary>
        /// Returneaza un nod de pe harta de la o anumita locatie
        /// </summary>
        /// <param name="_point"></param>
        /// <returns></returns>
        public Node GetNodeFromMap(Point _point)
        {
            return grid[_point.X, _point.Y];
        }

        /// <summary>
        /// Returneaza o lista cu toti vecinii pe care ii are un nod
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public List<Node> GetNeighbours(Node parent)
        {
            List<Node> vecini = new List<Node>();

            // itereaza prin vecini nodului printr-un bloc de 3x3
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    // daca suntem pe nodul din mijloc inseamna ca e parintele insusi
                    if (x == 0 && y == 0)
                        continue;

                    int verificaX = parent.pozitieInJoc.X + x;
                    int verificaY = parent.pozitieInJoc.Y + y;

                    // daca nodurile nu se afla in afara hartii atunci adauga vecinii
                    if (verificaX >= 0 && verificaX < gridSizeX && verificaY >=0 && verificaY < gridSizeY)
                    {
                        vecini.Add(grid[verificaX, verificaY]);
                    }
                }
            }
            return vecini;
        }

        /// <summary>
        /// Schimba modul unui bloc daca acesta este sau nu accesibil in functie de dorinta utilizatorului
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbl_Click(object sender, System.EventArgs e)
        {
            Panel lbl = (Panel)sender;
            foreach (Node n in grid)
            {
                if (n.blockNode.Equals(lbl) && n.isReserved == false)
                {
                    // daca nodul este accesibil si utilizatorul da click pe block
                    // atunci block-ul devin innaccesbil
                    if (n.accesibil == true)
                    {
                        n.blockNode.BackgroundImage = Properties.Resources.stone;
                        n.blockNode.BackgroundImageLayout = ImageLayout.Stretch;
                        n.accesibil = false;
                    }
                    else
                    {
                        n.blockNode.BackgroundImage = Properties.Resources.block_hidden;
                        n.blockNode.BackgroundImageLayout = ImageLayout.None;
                        n.accesibil = true;
                    }
                }
            }
        }

        /// <summary>
        /// Face clear la un block care a fost utilizat in scopul de a fi nod de start sau nod obiectiv
        /// </summary>
        /// <param name="_point"></param>
        public void clearOldPosition(Point _point)
        {
            Node oldPosition = GetNodeFromMap(_point);
            oldPosition.blockNode.BackgroundImage = Properties.Resources.block_hidden;
            oldPosition.blockNode.BackgroundImageLayout = ImageLayout.None;
            oldPosition.isReserved = false;
        }
    }
}
