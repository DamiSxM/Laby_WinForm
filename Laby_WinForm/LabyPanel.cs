using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
//using Laby_Interfaces;


//namespace Laby_Affichage
namespace Labyrinthe
{
    public class LabyPanel : Panel, IAffichage
    {
        LabyPictureBox _labyrinthe;
        ItemsPictureBox _items;
        //PictureBox _players;
        //PictureBox _warfog;
        PersoPictureBox _perso;
        Label _debug;

        public LabyPanel(int[,] maze)
        {
            int tileSize = 50;
            int displayTileSize = 9;

            Size = new Size(displayTileSize * tileSize, displayTileSize * tileSize);

            _labyrinthe = new LabyPictureBox(maze, tileSize, displayTileSize, displayTileSize);
            _labyrinthe.PositionChanged += Labyrinthe_PositionChanged; ;
            _labyrinthe.Location = new Point(0, 0);
            Controls.Add(_labyrinthe);

            _items = new ItemsPictureBox(maze.GetLength(0), tileSize, displayTileSize, displayTileSize);
            _items.Location = new Point(0, 0);
            Controls.Add(_items);
            _items.Parent = _labyrinthe;
            
            _perso = new PersoPictureBox();
            _perso.setLocation(Size);
            Controls.Add(_perso);
            _perso.Parent = _items;

            _debug = new Label();
            _debug.ForeColor = Color.Red;
            _debug.BackColor = Color.Transparent;
            _debug.Width = 100;
            _debug.Text = "Debug";
            _debug.Location = new Point(10, 10);
            Controls.Add(_debug);
            _debug.Parent = _items;
        }

        public event Position PositionChanged;
        void OnPositionChanged(int x, int y) { if (PositionChanged != null) PositionChanged(x, y); }
        private void Labyrinthe_PositionChanged(int x, int y)
        {
            OnPositionChanged(x, y); // Event PositionChanged
        }

        public void LabyUpdate()
        {
            _labyrinthe.generateLaby();
        }

        delegate void DebugCallback(string text);
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("LabyPanel.Debug : {0}", message));

            if (_debug.InvokeRequired)
            {
                DebugCallback d = new DebugCallback(Debug);
                Invoke(d, new object[] { message });
            }
            else
            {
                _debug.Text = message;
            }
        }

        public Point PersoGetPosition()
        {
            return new Point(_labyrinthe.X, _labyrinthe.Y);
        }

        public void PersoMove(Direction d, int vitesse)
        {
            switch (d)
            {
                case Direction.LEFT:
                    _labyrinthe.GoLeft();
                    _items.MoveLeft(vitesse);
                    _perso.GoLeft();
                    break;
                case Direction.UP:
                    _labyrinthe.GoUp();
                    _items.MoveUp(vitesse);
                    _perso.GoUp();
                    break;
                case Direction.RIGHT:
                    _labyrinthe.GoRight();
                    _items.MoveRight(vitesse);
                    _perso.GoRight();
                    break;
                case Direction.DOWN:
                    _labyrinthe.GoDown();
                    _items.MoveDown(vitesse);
                    _perso.GoDown();
                    break;
            }
            //_items.Move(PersoGetPosition().X, PersoGetPosition().Y);
        }
        /*public void PersoMoveLeft()
        {
            _labyrinthe.GoLeft();
            _perso.GoLeft();
            _items.Move(PersoGetPosition().X, PersoGetPosition().Y);
        }
        public void PersoMoveUp()
        {
            _labyrinthe.GoUp();
            _perso.GoUp();
            _items.Move(PersoGetPosition().X, PersoGetPosition().Y);
        }
        public void PersoMoveRight()
        {
            _labyrinthe.GoRight();
            _perso.GoRight();
            _items.Move(PersoGetPosition().X, PersoGetPosition().Y);
        }
        public void PersoMoveDown()
        {
            _labyrinthe.GoDown();
            _perso.GoDown();
            _items.Move(PersoGetPosition().X, PersoGetPosition().Y);
        }*/
        public void PersoTeleport(Point p)
        {
            _labyrinthe.moveToTile(p);
            _items.Move(p.X, p.Y);
        }
        public void PersoStop()
        {
            _perso.Stop();
        }

        public bool PlayerExists(string ip)
        {
            return _labyrinthe.containsPlayer(ip);
        }
        public void PlayerAdd(string ip, Point p)
        {
            _labyrinthe.addPlayer(ip, p);
        }
        public void PlayerMove(string ip, Point p)
        {
            _labyrinthe.movePlayer(ip, p);
        }
        public void PlayerRemove(string ip)
        {
            _labyrinthe.removePlayer(ip);
        }

        public void ItemsInit(Hashtable ht)
        {
            _labyrinthe.ItemsInit(ht);

            _items.Init(ht);

        }
        public void ItemAdd(Point p, Loot nom)
        {
            _labyrinthe.addItem(p, nom);
            string n = "";
            switch (nom)
            {
                case Loot.CRATE: n = "escalierPierre"; break;
                case Loot.COIN: n = "escalierPierre"; break;
            }
            _items.Add(new Bitmap(Properties.Resources.ResourceManager.GetObject(n) as Image), p.X, p.Y);
        }
        public void ItemRemove(Point p)
        {
            _labyrinthe.ItemRemove(p);
            _items.Remove(p.X, p.Y);
        }

        public void Warfog(int lvl)
        {
            if (lvl >= 0 & lvl <= 4) _labyrinthe.Warfog(lvl);
        }
    }
}