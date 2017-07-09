using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
//using Laby_Interfaces;

//namespace Laby_Affichage
namespace Labyrinthe
{
    class LabyPictureBox : PictureBox
    {
        int[,] _maze;
        int _tailleTiles;
        int _tailleMaze;

        Bitmap _labyBase;
        Bitmap _labyItems;
        Bitmap _labyPlayers;
        Image _warfog;

        Hashtable _items = new Hashtable();
        Hashtable _players = new Hashtable();

        int _width;
        int _height;
        int _offsetX = 0;
        int _offsetY = 0;
        int _centerX;
        int _centerY;
        int _X;
        int _Y;

        int _velocity = 2;

        public event Position PositionChanged;
        void OnPositionChanged(int x, int y) { if (PositionChanged != null) PositionChanged(x, y); }

        public int Velocity { get { return _velocity; } set { _velocity = value; } }
        public int X { get { return _X; } set { moveToTile(new Point(value, _Y)); } }
        public int Y { get { return _Y; } set { moveToTile(new Point(_X, value)); } }

        public LabyPictureBox(int[,] maze, int tailleTiles, int cmbTuillesWidth, int cmbTuillesHeight) : base()
        {
            DoubleBuffered = true;
            _maze = maze;
            _tailleMaze = maze.GetUpperBound(0);
            _tailleTiles = tailleTiles;

            _width = cmbTuillesWidth * _tailleTiles;
            _height = cmbTuillesHeight * _tailleTiles;
            _centerX = _width / 2;
            _centerY = _height / 2;
            _X = ((_centerX + _offsetX) / _tailleTiles);
            _Y = ((_centerY + _offsetY) / _tailleTiles);

            Size = new Size(_width, _height);

            Image = new Bitmap(cmbTuillesWidth * _tailleTiles, cmbTuillesHeight * _tailleTiles);
            _labyBase = new Bitmap(_tailleMaze * _tailleTiles, _tailleMaze * _tailleTiles);
            _labyItems = new Bitmap(_tailleMaze * _tailleTiles, _tailleMaze * _tailleTiles);
            _labyPlayers = new Bitmap(_tailleMaze * _tailleTiles, _tailleMaze * _tailleTiles);

            Warfog(0);

            generateLaby();
        }

        public void generateLaby()
        {
            using (Graphics g = Graphics.FromImage(_labyBase))
            {
                for (int x = 0; x < _tailleMaze - 1; x++)
                {
                    for (int y = 0; y < _tailleMaze - 1; y++)
                    {
                        Bitmap tmp = new Bitmap(_tailleTiles, _tailleTiles);
                        if (_maze[x, y] == 0) tmp = new Bitmap(Properties.Resources.solSable);
                        if (_maze[x, y] == 1) tmp = new Bitmap(Properties.Resources.murs);
                        g.DrawImage(tmp, (x * _tailleTiles), (y * _tailleTiles), _tailleTiles, _tailleTiles);
                    }
                }
            }
            RePaint();
        }


        public void ItemsInit(Hashtable ht)
        {
            _items = ht;
            createBitMapItems();
            RePaint();
        }

        public void addItem(Point p, Loot nom)
        {
            if (!_items.ContainsKey(p))
            {
                _items.Add(p, nom);
                //createBitMapItems();
                using (Graphics g = Graphics.FromImage(_labyItems))
                {
                    string n = "";
                    switch (nom)
                    {
                        case Loot.CRATE: n = "crate"; break;
                        case Loot.COIN: n = "coin"; break;
                    }
                    p.X = p.X * _tailleTiles;
                    p.Y = p.Y * _tailleTiles;
                    Bitmap tmp = new Bitmap(Properties.Resources.ResourceManager.GetObject(n) as Image);
                    g.DrawImage(tmp, p);
                }
                RePaint();
            }
        }

        delegate void PointCallback(Point p);
        public void ItemRemove(Point p)
        {
            if (InvokeRequired) Invoke(new PointCallback(ItemRemove), new object[] { p });
            else
            {
                _items.Remove(p);
                using (Graphics g = Graphics.FromImage(_labyItems))
                {
                    g.Clip = new Region(new Rectangle(p.X * _tailleTiles, p.Y * _tailleTiles, _tailleTiles, _tailleTiles));
                    g.Clear(Color.Transparent);


                    using (System.Media.SoundPlayer audio = new System.Media.SoundPlayer(Properties.Resources.snd_coin))
                    {
                        audio.Play();
                    }

                }
                RePaint();
            }
        }

        private void createBitMapItems() // générer l'image items
        {
            Point p;
            string n = "";
            using (Graphics g = Graphics.FromImage(_labyItems))
            {
                g.Clear(Color.Transparent);
                foreach (DictionaryEntry entry in _items)
                {
                    p = (Point)entry.Key;
                    switch ((Loot)entry.Value)
                    {
                        case Loot.CRATE: n = "crate"; break;
                        case Loot.COIN: n = "coin"; break;
                    }
                    p.X = p.X * _tailleTiles;
                    p.Y = p.Y * _tailleTiles;
                    Bitmap tmp = new Bitmap(Properties.Resources.ResourceManager.GetObject(n) as Image);
                    g.DrawImage(tmp, p);
                }
            }
        }

        public bool containsPlayer(string ip)
        {
            return _players.ContainsKey(ip);
        }

        delegate void StringPointCallback(string ip, Point p);
        public void addPlayer(string ip, Point p)
        {
            if (!_players.ContainsKey(ip))
            {
                if (InvokeRequired) Invoke(new StringPointCallback(addPlayer), new object[] { ip, p });
                else
                {
                    _players.Add(ip, p);
                    using (Graphics g = Graphics.FromImage(_labyPlayers))
                    {
                        p.X = p.X * _tailleTiles;
                        p.Y = p.Y * _tailleTiles;
                        Bitmap tmp = new Bitmap(Properties.Resources.Combattante_Rose_Bas_1);
                        g.DrawImage(tmp, p);
                    }
                    RePaint();
                }
            }
        }
        public void movePlayer(string ip, Point p)
        {
            if (InvokeRequired) Invoke(new StringPointCallback(movePlayer), new object[] { ip, p });
            else
            {
                _players.Remove(ip);
                _players.Add(ip, p);
                createBitMapPlayers();
                RePaint();
            }
        }

        public void removePlayer(string ip)
        {
            _players.Remove(ip);
            createBitMapPlayers();
            RePaint();
        }


        private void createBitMapPlayers() // générer l'image players
        {
            Point p;
            using (Graphics g = Graphics.FromImage(_labyPlayers))
            {
                g.Clear(Color.Transparent);
                foreach (DictionaryEntry entry in _players)
                {
                    p = (Point)entry.Value;
                    p.X = p.X * _tailleTiles;
                    p.Y = p.Y * _tailleTiles;
                    Bitmap tmp = new Bitmap(Properties.Resources.Combattante_Rose_Bas_1);
                    g.DrawImage(tmp, p);
                }
            }
        }

        delegate void RePaintCallback();
        public void RePaint()
        {
            if (InvokeRequired) Invoke(new RePaintCallback(RePaint));
            else
            {
                using (Graphics g = Graphics.FromImage(Image))
                {
                    affichageFond(g);

                    affichageBitmap(g, _labyBase);      // Laby
                    affichageBitmap(g, _labyItems);     // Items
                    affichageBitmap(g, _labyPlayers);   // Players

                    affichageFog(g);
                }
                Invalidate();
            }
        }

        void affichageFond(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, Size.Width, Size.Height));
        }
        void affichageBitmap(Graphics g, Bitmap b)
        {
            int droite, bas;
            if (_offsetY + _height < _height) bas = _height;
            else bas = _offsetY + _height;
            if (_offsetX + _width < _width) droite = _width;
            else droite = _offsetX + _width;

            Rectangle destRect = new Rectangle(new Point(0, 0), Size);
            Rectangle srcRect = new Rectangle(_offsetX, _offsetY, droite, bas);

            g.DrawImage(b, 0, 0, srcRect, GraphicsUnit.Pixel);
        }
        void affichageFog(Graphics g)
        {
            g.DrawImage(_warfog, 0, 0, Size.Width, Size.Height);
        }

        public void Warfog(int lvl)
        {
            switch (lvl)
            {
                case 0:
                    _warfog = Properties.Resources.warfog_vision_1;
                    break;
                case 1:
                    _warfog = Properties.Resources.warfog_vision_2;
                    break;
                case 2:
                    _warfog = Properties.Resources.warfog_vision_3;
                    break;
                case 3:
                    _warfog = Properties.Resources.warfog_vision_4;
                    break;
                case 4:
                    _warfog = Properties.Resources.warfog_vision_5;
                    break;
                default:
                    break;
            }
            RePaint();
        }

        bool canPlayerMove() // Marche pô....
        {
            int centerOnMazeX1 = ((_centerX + _offsetX - (_tailleTiles / 2)) / _tailleTiles);
            int centerOnMazeX2 = ((_centerX + _offsetX + (_tailleTiles / 2)) / _tailleTiles);

            int centerOnMazeY1 = ((_centerY + _offsetY - (_tailleTiles / 2)) / _tailleTiles);
            int centerOnMazeY2 = ((_centerY + _offsetY + (_tailleTiles / 2)) / _tailleTiles);

            Debug.WriteLine("centerOnMazeX1 {0}, centerOnMazeY1 {1}", centerOnMazeX1, centerOnMazeY1);
            Debug.WriteLine("centerOnMazeX2 {0}, centerOnMazeY2 {1}", centerOnMazeX2, centerOnMazeY2);

            int topleft = 0;
            int bottomright = 0;

            if (centerOnMazeX1 >= 0 & centerOnMazeX1 < _maze.GetUpperBound(0))
                if (centerOnMazeY1 >= 0 & centerOnMazeY1 < _maze.GetUpperBound(0))
                    topleft = _maze[centerOnMazeX1, centerOnMazeY1];
                else return false;
            else return false;

            if (centerOnMazeX2 >= 0 & centerOnMazeX2 < _maze.GetUpperBound(0))
                if (centerOnMazeY2 >= 0 & centerOnMazeY2 < _maze.GetUpperBound(0))
                    bottomright = _maze[centerOnMazeX2, centerOnMazeY2];
                else return false;
            else return false;

            Debug.WriteLine("_offsetX {0}, X {1}", _offsetX, _X);
            Debug.WriteLine("_offsetY {0}, Y {1}", _offsetY, _Y);

            //return !(topleft + bottomright == 0);
            return true;
        }

        public void moveToTile(Point p)
        {
            int x = p.X;
            int y = p.Y;
            int tailleMax = _maze.GetUpperBound(0) * _tailleTiles;
            if (((x + y) >= 0) & ((x + y) < tailleMax * 2))
            {
                _offsetX = (x * _tailleTiles) - _centerX + (_tailleTiles / 2);
                _offsetY = (y * _tailleTiles) - _centerY + (_tailleTiles / 2);
            }
            check_if_XnY_changes();
            RePaint();
        }

        public void GoLeft() { Moving(_offsetX - _velocity, _offsetY); }
        public void GoUp() { Moving(_offsetX, _offsetY - _velocity); }
        public void GoRight() { Moving(_offsetX + _velocity, _offsetY); }
        public void GoDown() { Moving(_offsetX, _offsetY + _velocity); }

        void Moving(int x, int y) // Ca Marche !
        {
            int left = -_centerX + (_tailleTiles / 2);
            int up = -_centerY + (_tailleTiles / 2);
            int right = ((_maze.GetUpperBound(0) - 1) * _tailleTiles) - _centerX - (_tailleTiles / 2);
            int down = ((_maze.GetUpperBound(0) - 1) * _tailleTiles) - _centerY - (_tailleTiles / 2);
            if (x < left) _offsetX = left;
            else
            {
                if (x > right) _offsetX = right;
                else
                {
                    if (_maze[((_centerX + x) / _tailleTiles), ((_centerY + _offsetY) / _tailleTiles)] == 0)
                        _offsetX = x;
                }
            }
            if (y < up) _offsetY = up;
            else
            {
                if (y > down) _offsetY = down;
                else
                {
                    bool center = false, bottom = false;
                    if (_maze[((_centerX + _offsetX) / _tailleTiles), ((_centerY + y) / _tailleTiles)] == 0)
                        center = true;
                    if (_maze[((_centerX + _offsetX) / _tailleTiles), ((_centerY + y + (_tailleTiles / 2)) / _tailleTiles)] == 0)
                        bottom = true;

                    if (center & bottom) _offsetY = y;
                }
            }
            check_if_XnY_changes();
            RePaint();
        }

        void check_if_XnY_changes() // Test....
        {
            int x = ((_centerX + _offsetX) / _tailleTiles);
            int y = ((_centerY + _offsetY) / _tailleTiles);
            if ((x + y) != (_X + _Y))
            {
                _X = x;
                _Y = y;
                OnPositionChanged(x, y); // Event PositionChanged
            }
        }
    }
}
