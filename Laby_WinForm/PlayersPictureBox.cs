using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Labyrinthe
{
    class PlayersPictureBox : PictureBox
    {
        int _tailleGrille, _tailleCellule, _offsetX = 0, _offsetY = 0;
        Bitmap _playersBitmap;
        Hashtable _players = new Hashtable();

        delegate void StringPointCallback(string ip, Point p);

        public PlayersPictureBox(int tailleGrille, int tailleCellule, int width, int height) : base()
        {
            _playersBitmap = new Bitmap(tailleGrille * tailleCellule, tailleGrille * tailleCellule);

            _tailleGrille = tailleGrille;
            _tailleCellule = tailleCellule;

            BackColor = Color.Transparent;
            Width = width * tailleCellule;
            Height = height * tailleCellule;

            Image = new Bitmap(Width, Height);
        }

        public void MoveLeft(int distance)
        {
            _offsetX -= distance;
            Invalidate();
        }
        internal void MoveUp(int distance)
        {
            _offsetY -= distance;
            Invalidate();
        }
        internal void MoveRight(int distance)
        {
            _offsetX += distance;
            Invalidate();
        }
        internal void MoveDown(int distance)
        {
            _offsetY += distance;
            Invalidate();
        }
        public new void MoveCenter(Point p)
        {
            _offsetX = p.X * _tailleCellule - Width / 2 + _tailleCellule / 2;
            _offsetY = p.Y * _tailleCellule - Height / 2 + _tailleCellule / 2;
            Invalidate();
        }
        public new void Move(string ip, Point p)
        {
            if (InvokeRequired) Invoke(new StringPointCallback(Move), new object[] { ip, p });
            else
            {
                _players.Remove(ip);
                _players.Add(ip, p);
                Invalidate();
            }
        }

        public void Add(string ip, Point p)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("AddAddAdd {0} {1} {2}", ip, p.X, p.Y));

            if (!_players.ContainsKey(ip))
            {
                if (InvokeRequired) Invoke(new StringPointCallback(Add), new object[] { ip, p });
                else
                {
                    _players.Add(ip, p);
                    //Rectangle r = new Rectangle(p.X * _tailleCellule, p.Y * _tailleCellule, _tailleCellule, _tailleCellule);
                    using (Graphics g = Graphics.FromImage(_playersBitmap))
                    {
                        p.X = p.X * _tailleCellule;
                        p.Y = p.Y * _tailleCellule;
                        Bitmap tmp = new Bitmap(Properties.Resources.Combattante_Rose_Bas_1);
                        g.DrawImage(tmp, p);
                    }
                    Invalidate();
                    /*if (r.IntersectsWith(new Rectangle(0, 0, Width, Height)))
                        Invalidate(r);*/
                }
            }
            /*Rectangle r = new Rectangle(x * _tailleCellule, y * _tailleCellule, _tailleCellule, _tailleCellule);
            using (Graphics g = Graphics.FromImage(_playersBitmap))
            {
                g.DrawImage(i, r);
            }
            if (r.IntersectsWith(new Rectangle(0, 0, Width, Height)))
                Invalidate(r);*/
        }
        public void Remove(string ip)
        {
            Point p = (Point)_players[ip];
            Rectangle r = new Rectangle(p.X * _tailleCellule, p.Y * _tailleCellule, _tailleCellule, _tailleCellule);
            using (Graphics g = Graphics.FromImage(_playersBitmap))
            {
                g.Clip = new Region(r);
                g.Clear(Color.Transparent);
            }
            _players.Remove(ip);
            Invalidate();
            /*if (r.IntersectsWith(new Rectangle(0, 0, Width, Height)))
                Invalidate(r);*/
        }

        public new void Invalidate()
        {
            int droite, bas;
            if (_offsetX + Width < Width) droite = Width;
            else droite = _offsetX + Width;
            if (_offsetY + Height < Height) bas = Height;
            else bas = _offsetY + Height;

            Rectangle srcRect = new Rectangle(_offsetX, _offsetY, droite, bas);

            using (Graphics g = Graphics.FromImage(Image))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(_playersBitmap, 0, 0, srcRect, GraphicsUnit.Pixel);
            }
            base.Invalidate();
        }
        public new void Invalidate(Rectangle r)
        {
            int droite, bas;
            if (_offsetX + Width < Width) droite = Width;
            else droite = _offsetX + Width;
            if (_offsetY + Height < Height) bas = Height;
            else bas = _offsetY + Height;

            Rectangle destRect = new Rectangle(new Point(0, 0), Size);
            Rectangle srcRect = new Rectangle(_offsetX, _offsetY, droite, bas);

            using (Graphics g = Graphics.FromImage(Image))
            {
                g.Clear(Color.Transparent);
                g.DrawImage(_playersBitmap, 0, 0, srcRect, GraphicsUnit.Pixel);
            }
            base.Invalidate(r);
        }
        public bool ContainsPlayer(string ip)
        {
            return _players.ContainsKey(ip);
        }
    }
}
