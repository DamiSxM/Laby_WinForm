using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Labyrinthe
{
    class LabyPictureBox : PictureBox
    {
        int[,] _maze;
        int _tailleTiles;
        int _tailleMaze;

        Bitmap _labyBase;

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
        public int X { get { return _X; } set { MoveCenter(new Point(value, _Y)); } }
        public int Y { get { return _Y; } set { MoveCenter(new Point(_X, value)); } }

        public LabyPictureBox(int[,] maze, int tailleTiles, int cmbTuillesWidth, int cmbTuillesHeight) : base()
        {
            DoubleBuffered = true;
            _maze = maze;
            _tailleMaze = maze.GetLength(0);
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

            Generate(_maze);
        }

        public void Generate(int[,] maze)
        {
            _maze = maze;
            using (Graphics g = Graphics.FromImage(_labyBase))
            {
                for (int x = 0; x < maze.GetLength(0); x++)
                {
                    for (int y = 0; y < maze.GetLength(0); y++)
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

        delegate void PointCallback(Point p);

        delegate void StringPointCallback(string ip, Point p);

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

            Rectangle srcRect = new Rectangle(_offsetX, _offsetY, droite, bas);
            g.DrawImage(b, 0, 0, srcRect, GraphicsUnit.Pixel);
        }

        public void MoveCenter(Point p)
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
        public void MoveCenterPixel(Point p)
        {
            /*_offsetX = p.X - _centerX + (_tailleTiles / 2);
            _offsetY = p.Y - _centerY + (_tailleTiles / 2);*/
            _offsetX = p.X - _centerX;
            _offsetY = p.Y - _centerY;
            check_if_XnY_changes();
            RePaint();
        }

        public void GoLeft() { Moving(_offsetX - _velocity, _offsetY); }
        public void GoUp() { Moving(_offsetX, _offsetY - _velocity); }
        public void GoRight() { Moving(_offsetX + _velocity, _offsetY); }
        public void GoDown() { Moving(_offsetX, _offsetY + _velocity); }

        void Moving(int x, int y) // Ca Marche !
        {
            _offsetX = x;
            _offsetY = y;

            /*int left = -_centerX + (_tailleTiles / 2);
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
            }*/
            check_if_XnY_changes();
            RePaint();
        }

        public Point GetPositionPixel()
        {
            return new Point(_centerX + _offsetX, _centerY + _offsetY);
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
