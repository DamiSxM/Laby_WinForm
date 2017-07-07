using System;
using System.Drawing;
using System.Windows.Forms;
using Laby_Interfaces;


namespace Laby_Affichage
{
    public class LabyPanel : Panel, IAffichage
    {
        LabyPictureBox _labyrinthe;
        PlayerPictureBox _player;

        public LabyPanel(int[,] maze)
        {
            int tileSize = 50;
            int displayTileSize = 9;

            Size = new Size(displayTileSize * tileSize, displayTileSize * tileSize);

            _labyrinthe = new LabyPictureBox(maze, tileSize, displayTileSize, displayTileSize);
            _labyrinthe.PositionChanged += Labyrinthe_PositionChanged; ;
            _labyrinthe.Location = new Point(0, 0);
            Controls.Add(_labyrinthe);

            _player = new PlayerPictureBox();
            _player.setLocation(Size);
            Controls.Add(_player);
            _player.Parent = _labyrinthe;
        }

        public event Position PositionChanged;
        void OnPositionChanged(int x, int y) { if (PositionChanged != null) PositionChanged(x, y); }
        private void Labyrinthe_PositionChanged(int x, int y)
        {
            OnPositionChanged(x, y); // Event PositionChanged
        }

        public void PersoMove(Direction d)
        {
            switch (d)
            {
                case Direction.LEFT:
                    _labyrinthe.GoLeft();
                    _player.GoLeft();
                    break;
                case Direction.UP:
                    _labyrinthe.GoUp();
                    _player.GoUp();
                    break;
                case Direction.RIGHT:
                    _labyrinthe.GoRight();
                    _player.GoRight();
                    break;
                case Direction.DOWN:
                    _labyrinthe.GoDown();
                    _player.GoDown();
                    break;
            }
        }
        public void PersoTeleport(int x, int y)
        {
            _labyrinthe.moveToTile(x, y);
        }
        public void PersoStop()
        {
            _player.Stop();
        }

        public void PlayerAdd(string ip, int x, int y)
        {
            _labyrinthe.addPlayer(ip, x, y);
        }
        public void PlayerMove(string ip, int x, int y)
        {
            _labyrinthe.movePlayer(ip, x, y);
        }
        public void PlayerRemove(string ip)
        {
            _labyrinthe.removePlayer(ip);
        }

        public void ItemAdd(int x, int y, string nom)
        {
            _labyrinthe.addItem(x, y, nom);
        }
        public void ItemRemove(int x, int y)
        {
            _labyrinthe.removeItem(x, y);
        }

        public void Warfog(int lvl)
        {
            if (lvl >= 0 & lvl <= 4) _labyrinthe.Warfog(lvl);
        }
    }
}
