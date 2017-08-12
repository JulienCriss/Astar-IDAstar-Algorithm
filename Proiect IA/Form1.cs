using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Proiect_IA
{
    public partial class Form1 : Form
    {
        static Grid map;
        // marimea grid-ului
        public int gridSizeX = 0;
        public int gridSizeY = 0;

        // retine index-ul pozitiei vechi ale nodului de start si nodul target
        int oldSeekerPosition = -1;
        int oldTargetPosition = -1;

        // toate pozitile pe care le poate lua nodul de start si nodul target dupa desenarea grid-ului
        Dictionary<int, Point> seekerPositions = new Dictionary<int, Point>();
        Dictionary<int, Point> targetPositions = new Dictionary<int, Point>();

        Node startNode;
        Node targetNode;
        // flag daca jocul poate incepe sau nu
        bool gameStart = false;


        public Form1()
        {
            InitializeComponent();
            addConfigurationDesign();
        }
        /// <summary>
        /// Seteaza configurarile in interfata
        /// - Ce marimi poatea avea grid-ul
        /// - Ce algoritm se poate folosi
        /// </summary>
        public void addConfigurationDesign()
        {
            comboBoxGridSize.Items.Insert(0, "5 x 3");
            comboBoxGridSize.Items.Insert(1, "10 x 6");
            comboBoxGridSize.Items.Insert(2, "20 x 14");
            comboBoxGridSize.Items.Insert(3, "30 x 21");
            comboBoxGridSize.Items.Insert(4, "42 x 28");

            comboBoxAlgorithm.Items.Insert(0, "A*");
            comboBoxAlgorithm.Items.Insert(1, "IDA*");

            comboBoxSeekerPos.DropDownHeight = comboBoxSeekerPos.ItemHeight * 7;
            comboBoxTarget.DropDownHeight = comboBoxTarget.ItemHeight * 7;

        }

        /// <summary>
        /// Reseteaza rezultatele pentru o noua rulare
        /// </summary>
        public void clearResults()
        {
            labelNoMoves.Text = "0";
            labelTime.Text = "0.00";

            gridSizeX = 0;
            gridSizeY = 0;

            oldSeekerPosition = -1;
            oldTargetPosition = -1;

            seekerPositions.Clear();
            targetPositions.Clear();

            gameStart = false;

            // daca harta a fost deja desenata, sterge tot si redeseneaza
            if (panelGame.Controls.Count > 0)
            {
                panelGame.Hide();
                panelGame.Controls.Clear();
                panelGame.Show();
            }
        }

        /// <summary>
        /// Seteaza dimensiunea gridului
        /// </summary>
        /// <param name="gridDimension"></param>
        public void drawGridGame(int gridDimension)
        {
            // daca harta a fost deja desenata, sterge tot si redeseneaza
            if (panelGame.Controls.Count > 0)
            {
                panelGame.Hide();
                panelGame.Controls.Clear();
                panelGame.Show();
            }

            map = new Grid(panelGame, gridDimension);
            map.drawGrid();
            panelGame = map.gameWorld;
        }

        /// <summary>
        /// Seteaza unde va fi asezat nodul de start (Seeker Player)
        /// </summary>
        /// <param name="_point"></param>
        public void SetStartNode(Point _point)
        {
            startNode = map.GetNodeFromMap(_point);
            startNode.blockNode.BackgroundImage = Properties.Resources.bunny;
            startNode.blockNode.BackgroundImageLayout = ImageLayout.Stretch;
            startNode.isReserved = true;
        }

        /// <summary>
        /// Seteaza unde va fi asezat nodul target (Target)
        /// </summary>
        /// <param name="_point"></param>
        public void SetTargetNode(Point _point)
        {
            targetNode = map.GetNodeFromMap(_point);
            targetNode.blockNode.BackgroundImage = Properties.Resources.carrots;
            targetNode.blockNode.BackgroundImageLayout = ImageLayout.Stretch;
            targetNode.isReserved = true;
        }

        /// <summary>
        /// Start Joc In functie de algoritmul pe care utilizatorul la selectat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBoxAlgorithm.SelectedIndex)
            {
                // calculeaza solutia cu A*
                case 0:
                    {
                        AStar gameAStar = new AStar();
                        foreach (Node n in map.GetNodesGrid)
                        {
                            if (n.blockNode.BackColor == Color.DarkRed || n.blockNode.BackColor == Color.Crimson)
                                n.blockNode.BackColor = Color.Transparent;
                        }

                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        gameAStar.AStarFindPath(map, startNode.pozitieInJoc, targetNode.pozitieInJoc);
                        watch.Stop();

                        labelTime.Text = watch.ElapsedMilliseconds.ToString();
                        labelNoMoves.Text = gameAStar.numberOfSteps.ToString();
                        labelBLockSize.Text = String.Format("{0}x{1}", map.nodDiameter, map.nodDiameter);
                        break;
                    }
                case 1:
                    {
                        IDAStar gameIDAStar = new IDAStar();
                        foreach (Node n in map.GetNodesGrid)
                        {
                            if (n.blockNode.BackColor == Color.DarkRed || n.blockNode.BackColor == Color.Crimson)
                                n.blockNode.BackColor = Color.Transparent;
                        }

                        List<Node> nodes = new List<Node>();
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        gameIDAStar.IDA_Star(map, startNode.pozitieInJoc, targetNode.pozitieInJoc);
                        watch.Stop();

                        labelTime.Text = watch.ElapsedMilliseconds.ToString();
                        labelNoMoves.Text = gameIDAStar.numberOfSteps.ToString();
                        labelBLockSize.Text = String.Format("{0}x{1}", map.nodDiameter, map.nodDiameter);
                        break;
                    }
            }

            
        }

        /// <summary>
        /// Seteaza pozitiile in interfata pe care le poate lua Seeker si Target
        /// </summary>
        /// <param name="_gridSizeX"></param>
        /// <param name="_gridSizeY"></param>
        public void setDesignPositions(int _gridSizeX, int _gridSizeY)
        {
            // clear comboBox before Insert
            comboBoxSeekerPos.Items.Clear();
            comboBoxTarget.Items.Clear();

            // clear all Position
            seekerPositions.Clear();
            targetPositions.Clear();

            int contor = 0;
            for (int x = 0; x < _gridSizeX; x++)
            {
                for (int y = 0; y < _gridSizeY; y++)
                {
                    comboBoxSeekerPos.Items.Insert(contor, String.Format("{0}x{1}", x + 1, y + 1));
                    comboBoxTarget.Items.Insert(contor, String.Format("{0}x{1}", x + 1, y + 1));

                    seekerPositions.Add(contor, new Point(x, y));
                    targetPositions.Add(contor, new Point(x, y));
                    contor++;
                }
            }
        }

        /// <summary>
        /// Schimba dimensiunea Gridului
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxGridSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIndex = comboBoxGridSize.SelectedIndex;
            if (selectIndex == -1)
            {
                gameStart = false;
                MessageBox.Show("Grid Size is not set");
            }
            else
            {
                if (!comboBoxSeekerPos.Enabled)
                    comboBoxSeekerPos.Enabled = true;
                if (!comboBoxTarget.Enabled)
                    comboBoxTarget.Enabled = true;

                switch (selectIndex)
                {
                    case 0:
                        {
                            drawGridGame(5);
                            gridSizeX = 5; gridSizeY = 3;
                            break;
                        }
                    case 1:
                        {
                            drawGridGame(10);
                            gridSizeX = 10; gridSizeY = 6;
                            break;
                        }
                    case 2:
                        {
                            drawGridGame(20);
                            gridSizeX = 20; gridSizeY = 14;
                            break;
                        }
                    case 3:
                        {
                            drawGridGame(30);
                            gridSizeX = 30; gridSizeY = 21;
                            break;
                        }
                    case 4:
                        {
                            drawGridGame(42);
                            gridSizeX = 42; gridSizeY = 28;
                            break;
                        }
                }
                setDesignPositions(gridSizeX, gridSizeY);
                labelBLockSize.Text = String.Format("{0} x {1}", map.nodDiameter, map.nodDiameter);
            }

        }

        /// <summary>
        /// Verifica daca configurarile pentru joc sunt facute corect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            int selectIndexGrid = comboBoxGridSize.SelectedIndex;
            int selectIndexAlgorithm = comboBoxAlgorithm.SelectedIndex;
            int selectIndexSeeker = comboBoxSeekerPos.SelectedIndex;
            int selectIndexTarget = comboBoxTarget.SelectedIndex;

            if (gameStart == false || selectIndexGrid == -1 || selectIndexAlgorithm == -1 || selectIndexSeeker == 1 || selectIndexTarget == -1)
            {
                gameStart = false;
                MessageBox.Show("Configurarile jocului nu sunt facute corect.\nVerificati din nou");
            }
            else
            {
                gameStart = true;
                MessageBox.Show("Setari aplicate cu succes!\nAcum poti testa jocul.");
            }
        }

        /// <summary>
        /// Seteaza NODUL - SEEKER in functie de selectia utilizatorului
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxSeekerPos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSeekerPos.SelectedIndex != -1)
            {
                if (comboBoxSeekerPos.SelectedIndex == comboBoxTarget.SelectedIndex)
                {
                    gameStart = false;
                    MessageBox.Show("Iepurasul si hrana nu pot fi acelasi nod!\nSelectati noduri diferite.");
                }
                else
                {
                    gameStart = true;
                    if (oldSeekerPosition == -1)
                    {
                        oldSeekerPosition = comboBoxSeekerPos.SelectedIndex;
                        SetStartNode(seekerPositions[oldSeekerPosition]);
                    }
                    else
                    {
                        map.clearOldPosition(seekerPositions[oldSeekerPosition]);
                        oldSeekerPosition = comboBoxSeekerPos.SelectedIndex;
                        SetStartNode(seekerPositions[oldSeekerPosition]);
                    }
                }
            }
        }

        /// <summary>
        /// Seteaza NODUL - TARGET in functie de selectia utilizatorului
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxTarget_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxTarget.SelectedIndex != -1)
            {
                if (comboBoxSeekerPos.SelectedIndex == comboBoxTarget.SelectedIndex)
                {
                    gameStart = false;
                    MessageBox.Show("Iepurasul si hrana nu pot fi pe acelasi nod!\nSelectati noduri diferite.");
                }
                else
                {
                    gameStart = true;
                    if (oldTargetPosition == -1)
                    {
                        oldTargetPosition = comboBoxTarget.SelectedIndex;
                        SetTargetNode(targetPositions[oldTargetPosition]);
                    }
                    else
                    {
                        map.clearOldPosition(targetPositions[oldTargetPosition]);
                        oldTargetPosition = comboBoxTarget.SelectedIndex;
                        SetTargetNode(targetPositions[oldTargetPosition]);
                    }
                }
            }
        }

        /// <summary>
        /// Seteaza algorimtul care va fi folosit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAlgorithm.SelectedIndex == -1)
            {
                gameStart = false;
            }
            else
            {
                gameStart = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog();
        }
    }
}
