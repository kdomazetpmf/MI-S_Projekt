using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OTTER
{
    public class GlavniLik : Sprite
    {
        public GlavniLik(string path, int x, int y) : base(path, x, y)
        {
            this.zdravlje = 3;
            this.skok = true;
            this.diraPrepreku = false;
            this.diraZeton = false;
            this.diraSrce = false;

            this.skupljeniZetoniBezUdarca = 0;
            this.skupljeniZetoni = 0;
            this.protekloSekundi = 0;
            this.ukupniBodovi = 0.0;
        }

        private int zdravlje;
        public int Zdravlje
        {
            get
            {
                return zdravlje;
            }
            set
            {
                if (value > 3)
                {
                    zdravlje = 3;
                }
                else
                {
                    zdravlje = value;
                }
            }
        }

        private bool skok;
        public bool Skok
        {
            get
            {
                return skok;
            }
            set
            {
                skok = value;
            }
        }

        private int skupljeniZetoniBezUdarca;
        public int SkupljeniZetoniBezUdarca
        {
            get
            {
                return skupljeniZetoniBezUdarca;
            }
            set
            {
                if (value > 25)
                {
                    skupljeniZetoniBezUdarca = 1;
                }
                else
                {
                    skupljeniZetoniBezUdarca = value;
                }
            }
        }

        private int skupljeniZetoni;
        public int SkupljeniZetoni
        {
            get
            {
                return skupljeniZetoni;
            }
            set
            {
                skupljeniZetoni = value;
            }
        }

        private int protekloSekundi;
        public int ProtekloSekundi
        {
            get
            {
                return protekloSekundi;
            }
            set
            {
                protekloSekundi = value;
            }
        }

        private double ukupniBodovi;
        public double UkupniBodovi
        {
            get
            {
                return ukupniBodovi;
            }
            set
            {
                ukupniBodovi = value;
            }
        }

        private bool diraPrepreku;
        public bool DiraPrepreku
        {
            get
            {
                return diraPrepreku;
            }
            set
            {
                diraPrepreku = value;
            }
        }

        private bool diraZeton;
        public bool DiraZeton
        {
            get
            {
                return diraZeton;
            }
            set
            {
                diraZeton = value;
            }
        }

        private bool diraSrce;
        public bool DiraSrce
        {
            get
            {
                return diraSrce;
            }
            set
            {
                diraSrce = value;
            }
        }

        public void DodajSekundu()
        {
            this.ProtekloSekundi += 1;
        }
        public void DodajZetoneBezUdarca()
        {
            this.SkupljeniZetoniBezUdarca += 1;
        }
        public int VratiZetoneBezUdarca()
        {
            this.SkupljeniZetoniBezUdarca = 0;
            return SkupljeniZetoniBezUdarca;
        }
        public double RacunajRezultat()
        {
            this.ukupniBodovi = ((double)this.SkupljeniZetoni * this.ProtekloSekundi) / 100000;
            return this.UkupniBodovi;
        }
    }
}
