﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labyrinthe
{
    public class ItemsPictureBox : PictureBox
    {
        int _tailleGrille, _tailleCellule, _offsetX = 0, _offsetY = 0;
        Bitmap _itemsBitmap;

        Hashtable _items;

        public ItemsPictureBox(int tailleGrille, int tailleCellule, int width, int height) : base()
        {
            _itemsBitmap = new Bitmap(tailleGrille * tailleCellule, tailleGrille * tailleCellule);

            _tailleGrille = tailleGrille;
            _tailleCellule = tailleCellule;

            BackColor = Color.Transparent;
            Width = width;
            Height = height;
            Image = new Bitmap(Width, Height);
        }

        public void Init(Hashtable ht)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("Init !!!!!!!!!!!!!!!!!!!!"));
            Point p; string n = "";
            foreach (DictionaryEntry entry in ht)
            {
                p = (Point)entry.Key;
                switch ((Loot)entry.Value)
                {
                    case Loot.CRATE: n = "crate"; break;
                    case Loot.COIN: n = "coin"; break;
                }
                Add(new Bitmap(Properties.Resources.ResourceManager.GetObject(n) as Image), p.X, p.Y);
            }
            Invalidate();
        }

        public new void Move(int x, int y)
        {
            _offsetX = x - Width / 2;
            _offsetY = y - Height / 2;
            Invalidate();
        }

        public void Add(Image i, int x, int y)
        {
            Rectangle r = new Rectangle(x * _tailleCellule, y * _tailleCellule, _tailleCellule, _tailleCellule);
            using (Graphics g = Graphics.FromImage(_itemsBitmap))
            {
                g.DrawImage(i, r);
            }
            Invalidate();
            /*if(r.IntersectsWith(new Rectangle(0, 0, Width, Height)))
                Invalidate(r);*/
        }
        public void Remove(int x, int y)
        {
            Rectangle r = new Rectangle(x * _tailleCellule, y * _tailleCellule, _tailleCellule, _tailleCellule);
            using (Graphics g = Graphics.FromImage(_itemsBitmap))
            {
                g.Clip = new Region(r);
                g.Clear(Color.Transparent);
            }
            if (r.IntersectsWith(new Rectangle(0, 0, Width, Height)))
                Invalidate(r);
        }

        public new void Invalidate()
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("Invalidate !!!!!!!!!!!!!!!!!!!!"));
            int droite, bas;
            if (_offsetX + Width < Width) droite = Width;
            else droite = _offsetX + Width;
            if (_offsetY + Height < Height) bas = Height;
            else bas = _offsetY + Height;

            Rectangle destRect = new Rectangle(new Point(0, 0), Size);
            Rectangle srcRect = new Rectangle(_offsetX, _offsetY, droite, bas);

            using (Graphics g = Graphics.FromImage(Image))
            {
                g.DrawImage(_itemsBitmap, 0, 0, srcRect, GraphicsUnit.Pixel);
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
                g.DrawImage(_itemsBitmap, 0, 0, srcRect, GraphicsUnit.Pixel);
            }
            base.Invalidate(r);
        }
    }
}
