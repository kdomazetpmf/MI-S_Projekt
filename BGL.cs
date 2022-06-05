using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Speech.Recognition;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events

        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {
                foreach (Sprite sprite in allSprites)
                {
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = true;
            sensing.MouseDown = true;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            //sensing.MouseDown = false;
            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            //sensing.MouseX = e.X;
            //sensing.MouseY = e.Y;
            //Sensing.Mouse.x = e.X;
            //Sensing.Mouse.y = e.Y;
            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;

        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }
        }

        #endregion
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

        #endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
        #region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        #endregion

        //sound
        #region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        #endregion

        //file
        #region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        #endregion

        //mouse & keys
        #region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        //detekcija
        SpeechRecognitionEngine recognizer;

        /* Game variables */

        public GlavniLik paco;

        Zeton zeton1;
        Zeton zeton2;

        Prepreka prepreka1;
        Prepreka prepreka2;

        Zivot noviZivot;

        Sprite triZivota;
        Sprite dvaZivota;
        Sprite jedanZivot;
        Sprite nulaZivota;

        Sprite oblak;
        Sprite stablo;
        Sprite priprema;

        /* Initialization */

        private void SetupGame()
        {
            //1. setup stage
            SetStageTitle("DuckDuckRun");
            setBackgroundPicture("backgrounds\\pozadinaIzbornik.png");
            setPictureLayout("stretch");

            //2. add sprites
            triZivota = new Sprite("sprites\\3srca.png", 530, 30);
            Game.AddSprite(triZivota);
            triZivota.SetSize(25);
            triZivota.SetVisible(false);

            dvaZivota = new Sprite("sprites\\2srca.png", 530, 30);
            Game.AddSprite(dvaZivota);
            dvaZivota.SetSize(25);
            dvaZivota.SetVisible(false);

            jedanZivot = new Sprite("sprites\\1srce.png", 530, 30);
            Game.AddSprite(jedanZivot);
            jedanZivot.SetSize(25);
            jedanZivot.SetVisible(false);

            nulaZivota = new Sprite("sprites\\0srca.png", 530, 30);
            Game.AddSprite(nulaZivota);
            nulaZivota.SetSize(25);
            nulaZivota.SetVisible(false);

            oblak = new Sprite("sprites\\oblak.png", 250, 80);
            Game.AddSprite(oblak);
            oblak.SetSize(60);
            oblak.SetVisible(false);
            oblak.SetHeading(270);

            stablo = new Sprite("sprites\\stablo.png", 800, 200);
            Game.AddSprite(stablo);
            stablo.SetSize(40);
            stablo.SetVisible(false);
            stablo.SetHeading(270);

            priprema = new Sprite("sprites\\priprema.png", 0, 0);
            Game.AddSprite(priprema);
            priprema.SetSize(100);
            priprema.SetVisible(false);

            zeton1 = new Zeton("sprites\\zeton.png", 800, 300);
            Game.AddSprite(zeton1);
            zeton1.SetSize(10);
            zeton1.SetVisible(false);
            zeton1.SetHeading(270);

            zeton2 = new Zeton("sprites\\zeton.png", 800, 300);
            Game.AddSprite(zeton2);
            zeton2.SetSize(10);
            zeton2.SetVisible(false);
            zeton2.SetHeading(270);

            prepreka1 = new Prepreka("sprites\\casa.png", 800, 300);
            Game.AddSprite(prepreka1);
            prepreka1.SetSize(10);
            prepreka1.AddCostumes("sprites\\kamen.png");
            prepreka1.SetVisible(false);
            prepreka1.SetHeading(270);

            prepreka2 = new Prepreka("sprites\\kamen.png", 800, 300);
            Game.AddSprite(prepreka2);
            prepreka2.SetSize(10);
            prepreka2.AddCostumes("sprites\\casa.png");
            prepreka2.SetVisible(false);
            prepreka2.SetHeading(270);

            noviZivot = new Zivot("sprites\\srce.png", 800, 300);
            Game.AddSprite(noviZivot);
            noviZivot.SetSize(10);
            noviZivot.SetVisible(false);
            noviZivot.SetHeading(270);

            paco = new GlavniLik("sprites\\glavnilik.png", 50, 235);
            Game.AddSprite(paco);
            paco.SetSize(25);
            paco.SetVisible(false);

            lblZetoni.Visible = false;
            lblVrijeme.Visible = false;
            lblKonacniBodovi.Visible = false;
            //3. scripts that start
            Game.StartScript(PocetakIgre);

            //highscore
             using (StreamReader sr = File.OpenText("dat.txt"))
                {
                    string linija;
                    List<double> rezultati = new List<double>();
                    while ((linija = sr.ReadLine()) != null)
                    {
                        double br = double.Parse(linija);
                        bool alreadyExist = rezultati.Contains(br);
                        if (!alreadyExist)
                            rezultati.Add(br);

                    }
                    rezultati.Sort();
                    rezultati.Reverse();
                    if (rezultati.Count() == 0)
                    {
                        label1.Text = "Score: ";
                    }
                    else if (rezultati.Count() == 1)
                    {
                        label1.Text = "Prvo mjesto:" + rezultati[0];

                    }
                    else if (rezultati.Count() == 2)
                    {
                        label1.Text = "Prvo mjesto:" + rezultati[0] + "\nDrugo mjesto" + rezultati[1];
                    }
                    else
                    {
                        label1.Text = "Prvo mjesto: " + rezultati[0] + "\nDrugo mjesto: " + rezultati[1] + "\nTreće mjesto : " + rezultati[2];
                    }
             }

            Wait(7);
        }

        /* Scripts */

        //#############################
        //##### VOICE RECOGNITION #####
        //#############################

        //boolovi
        bool voiceIzlaz = false;
        bool voiceNova = false;
        bool voiceKreni = false;
        bool voiceSkoci = false;

        //detekcija
        private void loadSpeechRecognition()
        {
            recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));

            var c = getChoiceLibrary();
            var gp = new GrammarBuilder(c);
            var g = new Grammar(gp);
            // Create and load a dictation grammar.  
            recognizer.LoadGrammar(g);

            // Add a handler for the speech recognized event.  
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            // Configure input to the speech recognizer.  
            recognizer.SetInputToDefaultAudioDevice();

            // Start asynchronous, continuous speech recognition.  
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "eezlaz")
            {
                voiceIzlaz = true;
            }
            else if (e.Result.Text == "nova eegra")
            {
                voiceNova = true;
            }
            else if (e.Result.Text == "krenee")
            {
                voiceKreni = true;
            }
            else if (e.Result.Text == "skochee")
            {
                voiceSkoci = true;
            }
        }

        //glasovne naredbe
        private Choices getChoiceLibrary()
        {
            Choices myChoices = new Choices();

            myChoices.Add("eezlaz");
            myChoices.Add("nova eegra");
            myChoices.Add("krenee");
            myChoices.Add("skochee");
            
            return myChoices;
        }

        //#############################

        private int PocetakIgre()
        {
            loadSpeechRecognition();
            while (START)
            {
                if (sensing.KeyPressed(Keys.N) || voiceNova)
                {
                    setBackgroundPicture("backgrounds\\pozadina.png");
                    paco.SetVisible(true);
                    oblak.SetVisible(true);
                    stablo.SetVisible(true);
                    priprema.SetVisible(true);
                    prepreka1.SetVisible(true);
                    prepreka2.SetVisible(true);
                    zeton1.SetVisible(true);
                    zeton2.SetVisible(true);
                    noviZivot.SetVisible(true);

                    lblKonacniBodovi.Invoke((MethodInvoker)(() => lblKonacniBodovi.Visible = false));
                    lblZetoni.Invoke((MethodInvoker)(() => lblZetoni.Visible = true));
                    lblVrijeme.Invoke((MethodInvoker)(() => lblVrijeme.Visible = true));


                    Game.StartScript(Priprema);
                    break;
                }
                else if (sensing.KeyPressed(Keys.E) || voiceIzlaz)
                {
                    Application.Exit();
                }
            }
            return 0;
        }
        private int Priprema()
        {
            while (START)
            {
                if (sensing.KeyPressed(Keys.Space) || voiceKreni)
                {
                    priprema.SetVisible(false);
                    Wait(0.5);
                    Game.StartScript(IgraPokrenuta);
                    Game.StartScript(PomiciPozadinu);
                    Game.StartScript(Generator);
                    Game.StartScript(Stoperica);
                    break;
                }
            }
            return 0;
        }
        private int IgraPokrenuta()
        {
            while (START)
            {
                lblZetoni.Invoke((MethodInvoker)(() => lblZetoni.Text = Staticka.Zetoni + paco.SkupljeniZetoni.ToString()));

                if (paco.Zdravlje == 0)
                {
                    Wait(3);
                    Game.StartScript(KrajIgre);
                }
                if ((sensing.KeyPressed(Keys.Space) && paco.Skok == true) || (voiceSkoci && paco.Skok == true))
                {
                    Game.StartScript(Skoci);
                    paco.Skok = false;
                    voiceSkoci = false;
                }
                if (paco.TouchingSprite(zeton1) && paco.DiraZeton == false)
                {
                    paco.DiraZeton = true;
                    zeton1.SetVisible(false);
                    paco.DodajZetoneBezUdarca();
                    paco.SkupljeniZetoni += zeton1.Vrijednost;
                    Wait(1);
                    paco.DiraZeton = false;
                }
                if (paco.TouchingSprite(zeton2) && paco.DiraZeton == false)
                {
                    paco.DiraZeton = true;
                    zeton2.SetVisible(false);
                    paco.DodajZetoneBezUdarca();
                    paco.SkupljeniZetoni += zeton2.Vrijednost;
                    Wait(1);
                    paco.DiraZeton = false;
                }
                if ((paco.TouchingSprite(prepreka1) || paco.TouchingSprite(prepreka2)) && (paco.DiraPrepreku == false))
                {
                    paco.DiraPrepreku = true;
                    paco.VratiZetoneBezUdarca();
                    Game.StartScript(Treperi);
                    paco.Zdravlje -= prepreka1.Ozljeda;
                    Wait(1);
                    paco.DiraPrepreku = false;
                }
                if (paco.TouchingSprite(noviZivot) && paco.DiraSrce == false)
                {
                    paco.DiraSrce = true;
                    noviZivot.SetVisible(false);
                    paco.Zdravlje += noviZivot.Regeneracija;
                    Wait(1);
                    paco.DiraSrce = false;
                }
                Game.StartScript(PrikazZivota);
                paco.RacunajRezultat();
                
            }
            return 0;
        }
        private int Skoci()
        {
            while (START)
            {
                for (int i = 0; i < 1; i++)
                {
                    paco.Y -= 7;
                    Wait(0.01);
                }
                for (int i = 0; i < 2; i++)
                {
                    paco.Y -= 6;
                    Wait(0.01);
                }
                for (int i = 0; i < 4; i++)
                {
                    paco.Y -= 5;
                    Wait(0.01);
                }
                for (int i = 0; i < 6; i++)
                {
                    paco.Y -= 4;
                    Wait(0.01);
                }
                for (int i = 0; i < 7; i++)
                {
                    paco.Y -= 3;
                    Wait(0.01);
                }
                for (int i = 0; i < 8; i++)
                {
                    paco.Y -= 2;
                    Wait(0.01);
                }
                for (int i = 0; i < 10; i++)
                {
                    paco.Y -= 1;
                    Wait(0.01);
                }

                for (int i = 0; i < 10; i++)
                {
                    paco.Y += 1;
                    Wait(0.01);
                }
                for (int i = 0; i < 8; i++)
                {
                    paco.Y += 2;
                    Wait(0.01);
                }
                for (int i = 0; i < 7; i++)
                {
                    paco.Y += 3;
                    Wait(0.01);
                }
                for (int i = 0; i < 6; i++)
                {
                    paco.Y += 4;
                    Wait(0.01);
                }
                for (int i = 0; i < 4; i++)
                {
                    paco.Y += 5;
                    Wait(0.01);
                }
                for (int i = 0; i < 2; i++)
                {
                    paco.Y += 6;
                    Wait(0.01);
                }
                for (int i = 0; i < 1; i++)
                {
                    paco.Y += 7;
                    Wait(0.01);
                }

                paco.Skok = true;
                break;
            }
            return 0;
        }
        private int Treperi()
        {
            while (START)
            {
                for (int i = 0; i < 6; i++)
                {
                    paco.SetVisible(false);
                    Wait(0.1);
                    paco.SetVisible(true);
                    Wait(0.1);
                }
                break;
            }
            return 0;
        }
        private int PrikazZivota()
        {
            while (START)
            {
                if (paco.Zdravlje == 0)
                {
                    nulaZivota.SetVisible(true);
                    jedanZivot.SetVisible(false);
                    dvaZivota.SetVisible(false);
                    triZivota.SetVisible(false);
                }
                if (paco.Zdravlje == 1)
                {
                    nulaZivota.SetVisible(false);
                    jedanZivot.SetVisible(true);
                    dvaZivota.SetVisible(false);
                    triZivota.SetVisible(false);
                }
                if (paco.Zdravlje == 2)
                {
                    nulaZivota.SetVisible(false);
                    jedanZivot.SetVisible(false);
                    dvaZivota.SetVisible(true);
                    triZivota.SetVisible(false);
                }
                if (paco.Zdravlje == 3)
                {
                    nulaZivota.SetVisible(false);
                    jedanZivot.SetVisible(false);
                    dvaZivota.SetVisible(false);
                    triZivota.SetVisible(true);
                }
                break;
            }
            return 0;
        }
        private int Stoperica()
        {
            while (START)
            {
                int i = 0;
                while (paco.Zdravlje > 0)
                {
                    lblVrijeme.Invoke((MethodInvoker)(() => lblVrijeme.Text = Staticka.Vrijeme + i.ToString() + " s"));
                    paco.DodajSekundu();
                    Wait(1);
                    i++;
                }
            }
            return 0;
        }
        private int PomiciPozadinu()
        {
            while (paco.Zdravlje > 0)
            {
                stablo.MoveSteps(3);
                if (stablo.X <= -200)
                {
                    stablo.X = 800;
                }
                oblak.MoveSteps(2);
                if (oblak.X <= -200)
                {
                    oblak.X = 800;
                }
                Wait(0.03);
            }
            return 0;
        }
        private int IspaliPrepreku1()
        {
            while (START)
            {
                prepreka1.MoveSteps(4);
                if (prepreka1.X == -200)
                {
                    prepreka1.X = 800;
                    prepreka1.NextCostume();
                    break;
                }
                Wait(0.01);
            }
            return 0;
        }
        private int IspaliPrepreku2()
        {
            while (START)
            {
                prepreka2.MoveSteps(4);
                if (prepreka2.X == -200)
                {
                    prepreka2.X = 800;
                    prepreka2.NextCostume();
                    break;
                }
                Wait(0.01);
            }
            return 0;
        }
        private int IspaliZeton1()
        {
            zeton1.SetVisible(true);
            while (START)
            {
                zeton1.MoveSteps(4);
                if (zeton1.X == -200)
                {
                    zeton1.X = 800;
                    break;
                }
                Wait(0.01);
            }
            return 0;
        }
        private int IspaliZeton2()
        {
            zeton2.SetVisible(true);
            while (START)
            {
                zeton2.MoveSteps(4);
                if (zeton2.X == -200)
                {
                    zeton2.X = 800;
                    break;
                }
                Wait(0.01);
            }
            return 0;
        }
        private int IspaliSrce()
        {
            noviZivot.SetVisible(true);
            while (START)
            {
                noviZivot.MoveSteps(4);
                if (noviZivot.X == -200)
                {
                    noviZivot.X = 800;
                    break;
                }
                Wait(0.01);
            }
            return 0;
        }
        private int PrviDio()
        {
            while (START)
            {
                if (paco.SkupljeniZetoniBezUdarca == 25)
                {
                    Game.StartScript(IspaliSrce);
                    paco.VratiZetoneBezUdarca();
                }
                else
                {
                    if (Staticka.Random1() == 1)
                    {
                        Game.StartScript(IspaliZeton1);
                    }
                    else if (Staticka.Random1() == 2)
                    {
                        Game.StartScript(IspaliPrepreku1);
                    }
                }
                break;
            }
            return 0;
        }
        private int DrugiDio()
        {
            while (START)
            {
                if (Staticka.Random2() == 1)
                {
                    Game.StartScript(IspaliZeton2);
                }
                else if (Staticka.Random2() == 2)
                {
                    Game.StartScript(IspaliPrepreku2);
                }
                break;
            }
            return 0;
        }
        private int Generator()
        {
            while (paco.Zdravlje > 0)
            {
                Game.StartScript(PrviDio);
                Wait(2.25);
                Game.StartScript(DrugiDio);
                Wait(2.25);
            }
            return 0;
        }
        private int KrajIgre()
        {
            while (true)
            {
                START = false;
                setBackgroundPicture("backgrounds\\pozadinaKraj.png");
                paco.SetVisible(false);
                oblak.SetVisible(false);
                stablo.SetVisible(false);
                priprema.SetVisible(false);
                prepreka1.SetVisible(false);
                prepreka2.SetVisible(false);
                zeton1.SetVisible(false);
                zeton2.SetVisible(false);
                noviZivot.SetVisible(false);

                nulaZivota.SetVisible(false);
                jedanZivot.SetVisible(false);
                dvaZivota.SetVisible(false);
                triZivota.SetVisible(false);

                lblZetoni.Invoke((MethodInvoker)(() => lblZetoni.Visible = false));
                lblVrijeme.Invoke((MethodInvoker)(() => lblVrijeme.Visible = false));
                lblKonacniBodovi.Invoke((MethodInvoker)(() => lblKonacniBodovi.Visible = true));
                lblKonacniBodovi.Invoke((MethodInvoker)(() => lblKonacniBodovi.Text = Staticka.Ukupno + Math.Round(paco.UkupniBodovi, 5).ToString() + " BTC"));

                using (StreamWriter sw = File.AppendText("dat.txt"))
                {
                    sw.WriteLine( Math.Round(paco.UkupniBodovi, 5).ToString());
                }
                Wait(7);
                Application.Restart();
                break;

            }
            return 0;
        }

        /* ------------ GAME CODE END ------------ */

    }
}
