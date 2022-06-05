using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace OTTER
{
    public static class Staticka
    {
        public static string Vrijeme = "Trajanje: ";
        public static string Zetoni = "Skupljeno: ";
        public static string Ukupno = "Rezultat: ";

        public static int Random1()
        {
            Random rnd = new Random();
            return rnd.Next(1, 3);
        }
        public static int Random2()
        {
            Random rnd = new Random();
            return rnd.Next(1, 3);
        }

    }
}
