using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Constructor.Global;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System.Threading;

namespace Constructor
{

    public partial class Main : Form
    {
        Loading loading;
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_VOLUME = 0x00000002;
        public Main()
        {
            InitializeComponent();
        }

        private void GetCustomerIDand()
        {
            //ATB0_GEN4_CTS_3GA004J-A_RevA0.tgz

            char[] Client = Global_V.PATHATBO[3].ToCharArray();
            if (Char.IsLetter(Client[1]) && Char.IsLetter(Client[2]) && Char.IsDigit(Client[3]))
            {
                Global.Global_V.IDCustomer = Client[1].ToString();
                Global.Global_V.IDHarness = Client[3].ToString() + Client[4].ToString() + Client[5].ToString();
                Global.Global_V.IDBoardType = Client[6].ToString() + Client[7].ToString() + Client[8].ToString(); ;

            }

            else if (Char.IsLetter(Client[1]) && Char.IsLetter(Client[2]) && Char.IsLetter(Client[3]))
            {
                Global.Global_V.IDCustomer = Client[1].ToString() + Client[2].ToString();
                Global.Global_V.IDHarness = Client[4].ToString() + Client[5].ToString() + Client[6].ToString();
                Global.Global_V.IDBoardType = Client[7].ToString() + Client[8].ToString() + Client[9].ToString();
            }



            Dictionary<string, string> customerDictionary = new Dictionary<string, string>()
            {
             {"A","GM"},{"AA","PACCAR"}, {"AB","VOLKSWAGEN"}, {"AC","NAVISTAR INTERNATIONAL"}, {"B","BMW"},
             {"C","HONDA"}, {"D","STELLANTIS"}, {"E","FISKER"}, {"F","FORD"}, {"FA","FORD"}, {"G","GM"}, {"H","HYUNDAI"},
             {"I","ISUZU"}, {"J","ALLISON TRANSMISSION"}, {"K","FIAT"}, {"L","FREIGHTLINER"}, {"M","MITSUBISHI"},
             {"N","NISSAN"}, {"O","LUCID MOTORS"}, {"P","WORKHORSE"}, {"Q","CASE NEW HOLLAND"}, {"R","TESLA INC"},
             {"S","A123 SYSTEMS"}, {"T","TOYOTA"}, {"U","MISCELLANEOUS"}, {"V","VOLVO"}, {"W","CUMMINS"}, {"X","HARLEY DAVIDSON"},
             {"Y","JOHN DEERE"},{"Z","MACK-TRUCK"}};



            label14.Text = customerDictionary[Global.Global_V.IDCustomer];
            label12.Text = Global.Global_V.IDHarness;
            label18.Text = Global.Global_V.IDBoardType;

        }
        private void USBDeteccion()
        {

            label6.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            string ATBOPath = Global.Global_V.USBId + ":\\ATBO\\";


            try
            {
                int u;
                string[] ATBO = Directory.GetFiles(@"" + ATBOPath, "*.tgz"); //Path+Name
                if (ATBO.Length > 20)
                {
                    u = 0;
                   
                }
                else
                {
                    u = 1;
                }
                Global.Global_V.ATBOFile = ATBO[u].Replace(ATBOPath, ""); //File Name

                label7.Text = Global.Global_V.ATBOFile;

                pictureBox1.Visible = false;
                pictureBox3.Visible = false;
                pictureBox7.Visible = true;

                label2.Text = "Se encontraron los siguientes archivos:";
                label2.ForeColor = Color.CadetBlue;
                label3.Text = "Archivos listos para Cargar:";
                label3.ForeColor = Color.CadetBlue;


                char[] delimiterChars = { ' ', ',', '.', ':', '\t', '_' };
                string text = Global_V.ATBOFile;
                string[] words = text.Split(delimiterChars);
                System.Console.WriteLine($"{words.Length} words in text:");
                Global_V.PATHATBO = words;


                char[] Year = Global_V.PATHATBO[3].ToCharArray();
                label16.Text = "202" + Year[0].ToString();

                char[] Rev = Global_V.PATHATBO[4].ToCharArray();
                label20.Text = Rev[0].ToString() + Rev[1].ToString() + Rev[2].ToString() + Rev[3].ToString() + Rev[4].ToString();



                GetCustomerIDand();

            }
            catch (Exception)
            {
                pictureBox1.Visible = true;
                pictureBox3.Visible = true;
                pictureBox7.Visible = false;
                label10.Visible = false;
                label2.Text = "USB Error";
                label3.Text = "";
                label10.Text = "";
                label2.ForeColor = Color.RosyBrown;
                label7.Text = "Secuencia ATBO no encontrada: "+ Global_V.USBId + ":\\";
                label12.Text = "";
                label14.Text = "";
                label16.Text = "";
                label18.Text = "";
                label20.Text = "";
                Global_V.IDHarness = string.Empty;
                Global_V.IDCustomer = string.Empty;
                Global_V.IDBoardType = string.Empty;

            }

            try
            {

                string EHVPath = Global.Global_V.USBId + ":\\ATBO\\";


                string[] EHV = Directory.GetFiles(@"" + EHVPath, "*.tgz"); //Path+Name
                int v;
                if (EHV.Length > 20)
                {
                    v = 1;

                }
                else
                {
                    v = 0;
                }
                Global.Global_V.VisualAidsFile = EHV[v].Replace(EHVPath, ""); //File Name
                label8.Text = Global.Global_V.VisualAidsFile;

                if (Global.Global_V.VisualAidsFile != null)
                {
                    label11.Text = "Se encontraron ayudas visuales " + Global.Global_V.VisualAidsFile;
                }

            }
            catch
            {
                label8.Text = "Ayudas visuales no encontradas: " + Global_V.USBId + ":\\";
                label11.Text = "No se encontraron ayudas visuales";
            }

        }

        private void EXtATBOfile()
        {
            FileInfo tarFileInfo = new FileInfo(Global.Global_V.USBId + ":\\ATBO\\" + Global_V.ATBOFile);
            DirectoryInfo targetDirectory = new DirectoryInfo(Global.Global_V.Folder);
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }
            using (Stream sourceStream = new GZipInputStream(tarFileInfo.OpenRead()))
            {
                using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(sourceStream, TarBuffer.DefaultBlockFactor))
                {
                    tarArchive.ExtractContents(targetDirectory.FullName);
                }

            }

        }

        private void EXtEHVfile()
        {
            FileInfo tarFileInfo = new FileInfo(Global.Global_V.USBId + ":\\ATBO\\" + Global_V.VisualAidsFile);
            DirectoryInfo targetDirectory = new DirectoryInfo(Global.Global_V.LinkEHV);
            if (!targetDirectory.Exists)
            {
                targetDirectory.Create();
            }
            using (Stream sourceStream = new GZipInputStream(tarFileInfo.OpenRead()))
            {
                using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(sourceStream, TarBuffer.DefaultBlockFactor))
                {
                    tarArchive.ExtractContents(targetDirectory.FullName);
                }

            }

        }

        private void Constructor()
        {

            char[] Year = Global_V.PATHATBO[3].ToCharArray();
            char[] Rev = Global_V.PATHATBO[4].ToCharArray();
            Global.Global_V.Folder = Global.Global_V.LinkATBO + "/" + "202" + Year[0].ToString() + "/" + Rev[3].ToString() + Rev[4].ToString();

            //Secuence ABTO
            if (!Directory.Exists(Global.Global_V.Folder))
            {
                Console.WriteLine("Se creo el directorio: {0}", Global.Global_V.Folder);
                DirectoryInfo di = Directory.CreateDirectory(Global.Global_V.Folder);
            }
            else
            {
                Console.WriteLine("Ya se tenia: " + Global.Global_V.LinkATBO);
                DirectoryInfo directory = new DirectoryInfo(Global_V.Folder);
                foreach (FileInfo file in directory.EnumerateFiles())
                {
                    file.Delete();
                }
                DirectoryInfo di = Directory.CreateDirectory(Global.Global_V.Folder);
                Console.WriteLine("Se rescribio: " + Global.Global_V.LinkATBO);
            }
            //Visual files
            if (!Directory.Exists(Global.Global_V.LinkEHV))
            {
                Console.WriteLine("Se creo el directorio: {0}", Global.Global_V.LinkEHV);
                DirectoryInfo di = Directory.CreateDirectory(Global.Global_V.LinkEHV);

            }
            else { Console.WriteLine("Ya se tenia: " + Global.Global_V.LinkEHV); }
            //Extract sequence
            try
            { EXtATBOfile(); }
            catch
            { Console.WriteLine("Archivos no extraídos"); }
            //Extract visual aids
            try
            { EXtEHVfile(); }
            catch
            { Console.WriteLine("Archivos no extraídos"); }

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            TopMost = false;
            label10.Visible = false; label6.Visible = false; label7.Visible = false;//Visual Tags
            label8.Visible = false; label9.Visible = false; panel2.Enabled = false; //"
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        { USBDeteccion(); }

        private void pictureBox3_Click(object sender, EventArgs e)
        { }

        protected override void WndProc(ref Message m)
        {
            USBDeteccion();
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch ((int)m.WParam)
                    {
                        case DBT_DEVICEARRIVAL:
                            listBox1.Items.Add("Se ingreso USB");
                            int devType = Marshal.ReadInt32(m.LParam, 4);
                            if (devType == DBT_DEVTYP_VOLUME)
                            {
                                DevBroadcastVolume vol;
                                vol = (DevBroadcastVolume)Marshal.PtrToStructure(m.LParam,
                                   typeof(DevBroadcastVolume));
                                switch (vol.Mask)
                                {
                                    case 1: Global_V.USBId = "A"; break;
                                    case 2: Global_V.USBId = "B"; break;
                                    case 4: Global_V.USBId = "C"; break;
                                    case 8: Global_V.USBId = "D"; break;
                                    case 16: Global_V.USBId = "E"; break;
                                    case 32: Global_V.USBId = "F"; break;
                                    case 64: Global_V.USBId = "G"; break;
                                    case 128: Global_V.USBId = "H"; break;
                                    case 256: Global_V.USBId = "I"; break;
                                    case 512: Global_V.USBId = "J"; break;
                                    case 1024: Global_V.USBId = "K"; break;
                                    case 2048: Global_V.USBId = "L"; break;
                                    case 4096: Global_V.USBId = "M"; break;
                                    case 8192: Global_V.USBId = "N"; break;
                                    case 16384: Global_V.USBId = "O"; break;
                                    case 32768: Global_V.USBId = "P"; break;
                                    case 35536: Global_V.USBId = "Q"; break;
                                    case 131072: Global_V.USBId = "R"; break;
                                    default: Global_V.USBId = string.Empty; break;
                                }
                                listBox1.Items.Add("Mask is " + vol.Mask);
                            }
                            break;

                        case DBT_DEVICEREMOVECOMPLETE:
                            listBox1.Items.Add("Device Removed");
                            break;
                    }
                    break;
            }
        }

        private async void pictureBox7_Click(object sender, EventArgs e)
        {
            try
            {
                show();
                Task otask = new Task(sleep);
                otask.Start();
                await otask;
                Constructor();
                hide();
                label10.Visible = true; label10.Text = "Secuencia cargada correctamente"; label10.ForeColor = Color.OliveDrab;
                pictureBox7.Visible = false;
            }
            catch { label10.Visible = true; label10.Text = "Problema inesperado, reiniciar proceso."; label10.ForeColor = Color.RosyBrown; }
        }
        public void sleep()
        { Thread.Sleep(1900); }
        public void show()
        { loading = new Loading(); loading.Show(); }
        public void hide()
        { if (loading != null) { loading.Close(); } }

        private void pictureBox8_Click(object sender, EventArgs e)
        { Application.Exit(); }
    }
}
