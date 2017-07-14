using Labyrinthe;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Labyrinthe
{
    public class LabyPanel : Panel, IAffichage
    {
        Maze _maze;

        LabyPictureBox _labyrinthe;
        ItemsPictureBox _items;
        PlayersPictureBox _players;
        WarfogPictureBox _warfog;
        PersoPictureBox _perso;
        Label _debug;

        int _cellSize;

        public LabyPanel(Maze maze)
        {
            _maze = maze;
            _cellSize = 50;
            int displayTileSize = 9;

            Size = new Size(displayTileSize * _cellSize, displayTileSize * _cellSize);

            _labyrinthe = new LabyPictureBox(_maze.Labyrinthe, _cellSize, displayTileSize, displayTileSize);
            _labyrinthe.PositionChanged += Labyrinthe_PositionChanged; ;
            _labyrinthe.Location = new Point(0, 0);
            Controls.Add(_labyrinthe);

            _items = new ItemsPictureBox(_maze.Labyrinthe.GetLength(0), _cellSize, displayTileSize, displayTileSize);
            _items.Location = new Point(0, 0);
            Controls.Add(_items);
            _items.Parent = _labyrinthe;

            _players = new PlayersPictureBox(_maze.Labyrinthe.GetLength(0), _cellSize, displayTileSize, displayTileSize);
            _players.Location = new Point(0, 0);
            Controls.Add(_players);
            _players.Parent = _items;

            _warfog = new WarfogPictureBox(_maze.Labyrinthe.GetLength(0), _cellSize, displayTileSize, displayTileSize);
            _warfog.Location = new Point(0, 0);
            Controls.Add(_warfog);
            _warfog.Parent = _players;

            _perso = new PersoPictureBox();
            _perso.setLocation(Size);
            Controls.Add(_perso);
            _perso.Parent = _warfog;

            _debug = new Label();
            _debug.ForeColor = Color.Red;
            _debug.BackColor = Color.Transparent;
            _debug.Width = 100;
            _debug.Text = "Debug";
            _debug.Location = new Point(10, 10);
            Controls.Add(_debug);
            _debug.Parent = _warfog;
        }

        public event Position PositionChanged;
        void OnPositionChanged(int x, int y) { if (PositionChanged != null) PositionChanged(x, y); }
        private void Labyrinthe_PositionChanged(int x, int y)
        {
            OnPositionChanged(x, y); // Event PositionChanged
        }

        public void LabyUpdate()
        {
            _labyrinthe.Generate(_maze.Labyrinthe);
        }
        public int GetCellSize()
        {
            return _cellSize;
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
        public Point PersoGetPositionPixel()
        {
            return _labyrinthe.GetPositionPixel();
        }
        public void PersoMove(Direction d, Point p)
        {
            switch (d)
            {
                case Direction.LEFT:
                    _perso.GoLeft();
                    break;
                case Direction.UP:
                    _perso.GoUp();
                    break;
                case Direction.RIGHT:
                    _perso.GoRight();
                    break;
                case Direction.DOWN:
                    _perso.GoDown();
                    break;
            }
            _labyrinthe.MoveCenterPixel(p);
            _items.MoveCenterPixel(p);
            _players.MoveCenterPixel(p);
        }
        public void PersoMove(Direction d, int vitesse)
        {
            switch (d)
            {
                case Direction.LEFT:
                    _labyrinthe.GoLeft();
                    _items.MoveLeft(vitesse);
                    _players.MoveLeft(vitesse);
                    _perso.GoLeft();
                    break;
                case Direction.UP:
                    _labyrinthe.GoUp();
                    _items.MoveUp(vitesse);
                    _players.MoveUp(vitesse);
                    _perso.GoUp();
                    break;
                case Direction.RIGHT:
                    _labyrinthe.GoRight();
                    _items.MoveRight(vitesse);
                    _players.MoveRight(vitesse);
                    _perso.GoRight();
                    break;
                case Direction.DOWN:
                    _labyrinthe.GoDown();
                    _items.MoveDown(vitesse);
                    _players.MoveDown(vitesse);
                    _perso.GoDown();
                    break;
            }
            //_items.Move(PersoGetPosition().X, PersoGetPosition().Y);
        }
        public void PersoTeleport(Point p)
        {
            _labyrinthe.MoveCenter(p);
            _items.MoveCenter(p);
            _players.MoveCenter(p);
        }
        public void PersoStop()
        {
            _perso.Stop();
        }

        public bool PlayerExists(string ip)
        {
            return _players.ContainsPlayer(ip);
        }
        public void PlayerAdd(string ip, Point p)
        {
            _players.Add(ip, p);
        }
        public void PlayerMove(string ip, Point p)
        {
            _players.Move(ip, p);
        }
        public void PlayerRemove(string ip)
        {
            _players.Remove(ip);
        }

        public void ItemsInit(Hashtable ht)
        {
            _items.Init(ht);
        }
        public void ItemAdd(Point p, Loot nom)
        {
            string n = "";
            switch (nom)
            {
                case Loot.CRATE: n = "crate"; break;
                case Loot.COIN: n = "coin"; break;
            }
            _items.Add(new Bitmap(Properties.Resources.ResourceManager.GetObject(n) as Image), p.X, p.Y);
        }
        public void ItemRemove(Point p)
        {
            _items.Remove(p.X, p.Y);

            using (System.Media.SoundPlayer audio = new System.Media.SoundPlayer(Properties.Resources.snd_coin))
                audio.Play();
        }

        public void Warfog(int lvl)
        {
            if (lvl >= 0 & lvl <= 4) _warfog.ChangeLvl(lvl);
        }
    }
}