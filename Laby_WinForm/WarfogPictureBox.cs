using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Labyrinthe
{
    class WarfogPictureBox : PictureBox
    {
        public WarfogPictureBox(int tailleGrille, int tailleCellule, int width, int height) : base()
        {
            BackColor = Color.Transparent;
            Width = width * tailleCellule;
            Height = height * tailleCellule;
            ChangeLvl(0);
        }

        public void ChangeLvl(int lvl)
        {
            switch (lvl)
            {
                case 0:
                    Image = Properties.Resources.warfog_vision_1;
                    break;
                case 1:
                    Image = Properties.Resources.warfog_vision_2;
                    break;
                case 2:
                    Image = Properties.Resources.warfog_vision_3;
                    break;
                case 3:
                    Image = Properties.Resources.warfog_vision_4;
                    break;
                case 4:
                    Image = Properties.Resources.warfog_vision_5;
                    break;
                default:
                    break;
            }
            Invalidate();
        }

    }
}
