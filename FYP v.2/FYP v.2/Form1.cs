using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace FYP_v._2
{
    public partial class Form1 : Form
    {
        List<Seed> reference = new List<Seed>();
        
        List<Particle> particles = new List<Particle>();
        Particle GBest = new Particle();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string line;

            StreamReader read = new StreamReader("test.txt");
            label1.Text = "Number of white pixel in Result Image";
            label2.Text = "Number of white pixel in Ground Truth image";
            label3.Text = ":GBest after each iteration";
            label4.Text = "Test Image";
            label5.Text = "Given Image";
            label6.Text = "Number of black pixel in Result Image";
            label7.Text = "Number of black pixel in Ground Truth Image";
            while ((line = read.ReadLine()) != null)
            {

                string[] split = line.Split(';');//need to redo data collection to enable the split function
                Point nPoint = new Point(Convert.ToInt16(split[1]), Convert.ToInt16(split[2]));
                Color myColor = Color.FromArgb(Convert.ToInt16(split[3]), Convert.ToInt16(split[4]), Convert.ToInt16(split[5]));
                Seed ref_seed = new Seed(nPoint, myColor, 0);


                reference.Add(ref_seed);
            }

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void UploadIMG_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.tif;)|*.jpg; *.jpeg; *.tif";
            if (open.ShowDialog() == DialogResult.OK)
            {
                String strFilename = open.FileName;
                Image img = Image.FromFile(strFilename);
                Bitmap bit = new Bitmap(img);
                pictureBox1.Image = bit;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }



        #region PSO


        private void PSO_Click(object sender, EventArgs e)
        {
            //starting PSO sequence
            richTextBox1.Clear();

            Bitmap tempo = (Bitmap)pictureBox1.Image;
            Random r = new Random();
            int swarm = 50;
            int epoch = 50;

            #region Initialize
            for (int i = 0; i < swarm; i++) //for num = set particles
            //start num
            {
                List<Seed> random_list = new List<Seed>();

                for (int j = 0; j < 10; j++)//choose 10 random seeds
                {
                    Point loc = new Point(r.Next(tempo.Width), r.Next(tempo.Height));
                    Color tempColor = tempo.GetPixel(loc.X, loc.Y);
                    Seed tempSeed = new Seed(loc, tempColor, 0);
                    tempSeed.fitness = FitnessFunction(tempSeed);
                    random_list.Add(tempSeed);
                }

                //initializing particle
                Particle tempart = new Particle();
                tempart = new Particle(random_list, tempart, 0, 0);
                
                particles.Add(tempart);

                //addition of all seeds fitness into the particle fitness
                //fitness update

                for (int j = 0; j < particles.Count; j++)
                {
                    for (int k = 0; k < particles[j].seeds.Count; k++)
                    {
                        particles[j].fitness = particles[j].seeds[0].fitness
                            + particles[j].seeds[1].fitness
                            + particles[j].seeds[2].fitness
                            + particles[j].seeds[3].fitness
                            + particles[j].seeds[4].fitness
                            + particles[j].seeds[5].fitness
                            + particles[j].seeds[6].fitness
                            + particles[j].seeds[7].fitness
                            + particles[j].seeds[8].fitness
                            + particles[j].seeds[9].fitness;

                        particles[j].PBest = particles[j];
                    }
                    
                }
            #endregion
                //velocity check
                
                particles.Sort((x, y) => y.fitness.CompareTo(x.fitness)); 
                // sort particles from highest fitness to lowest fitness
                GBest = particles[0]; 
                // set GBest to highest sorted fitness value


            }


            #region Epoch
            // starting epoch iteration sequence
            for (int i = 0; i < epoch; i++)
            {
                for (int j = 0; j < particles.Count; j++)
                {
                    // FollowGBest sequence
                    particles[j] = FollowGBest(particles[j], GBest);

                    // fitness update = first time  
                    for (int k = 0; k < particles[j].seeds.Count; k++)
                    {
                       particles[j].fitness = particles[j].seeds[0].fitness
                            + particles[j].seeds[1].fitness
                            + particles[j].seeds[2].fitness
                            + particles[j].seeds[3].fitness
                            + particles[j].seeds[4].fitness
                            + particles[j].seeds[5].fitness
                            + particles[j].seeds[6].fitness
                            + particles[j].seeds[7].fitness
                            + particles[j].seeds[8].fitness
                            + particles[j].seeds[9].fitness;
                    }

                    

                    // FollowPbest sequence
                    particles[j] = FollowPBest(particles[j], particles[j].PBest);
                    // fitness update = second time
                    for (int k = 0; k < particles[j].seeds.Count; k++)
                    {
                        particles[j].fitness = particles[j].seeds[0].fitness
                             + particles[j].seeds[1].fitness
                             + particles[j].seeds[2].fitness
                             + particles[j].seeds[3].fitness
                             + particles[j].seeds[4].fitness
                             + particles[j].seeds[5].fitness
                             + particles[j].seeds[6].fitness
                             + particles[j].seeds[7].fitness
                             + particles[j].seeds[8].fitness
                             + particles[j].seeds[9].fitness;
                    }

                }
                particles.Sort((x, y) => y.fitness.CompareTo(x.fitness)); // sort particles from highest fitness to lowest fitness
                GBest = particles[0]; // set GBest to highest sorted fitness value
                richTextBox1.AppendText("\n" + particles[0].fitness);
            }
            
            #endregion
        }
        #endregion

        

        private void RegGrow_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            Bitmap BaseImg = new Bitmap(bmp.Width, bmp.Height);

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int i = 0; i < particles[0].seeds.Count; i++)
                    {
                        
                        if ((particles[0].seeds[i].pixel.R - bmp.GetPixel(x, y).R) <= 3)
                            BaseImg.SetPixel(x, y, Color.Black);
                        if ((particles[0].seeds[i].pixel.G - bmp.GetPixel(x, y).G) <= 3)
                            BaseImg.SetPixel(x, y, Color.Black);
                        if ((particles[0].seeds[i].pixel.B - bmp.GetPixel(x, y).B) <= 3)
                            BaseImg.SetPixel(x, y, Color.Black);

                        else
                        {
                            if ((particles[0].seeds[i].pixel.R - bmp.GetPixel(x, y).R) > 50)
                                BaseImg.SetPixel(x, y, Color.Black);
                            else
                            BaseImg.SetPixel(x, y, Color.White);
                        }
                        
                        pictureBox1.Image = BaseImg;
                        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }

            //pictureBox1.Image = BaseImg;

        }

        #region Ground Truth
        private void GroundTruth_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.tif; *.gif)|*.jpg; *.jpeg; *.tif; *.gif";
            if (open.ShowDialog() == DialogResult.OK)
            {
                String strFilename = open.FileName;
                Image img = Image.FromFile(strFilename);
                Bitmap bit = new Bitmap(img);
                pictureBox2.Image = bit;
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            Bitmap one = (Bitmap)pictureBox1.Image;
            Bitmap two = (Bitmap)pictureBox2.Image;
            int calculate1 = 0;
            int calculate2 = 0;
            int countPixel = 0;
            int calculate3 = 0;

            for (int x = 0; x < one.Width; x++)
            {
                for (int y = 0; y < one.Height; y++)
                {
                    
                    if (one.GetPixel(x, y).R == 255)
                        if (one.GetPixel(x, y).R == two.GetPixel(x, y).R)
                            if (one.GetPixel(x, y).G == 255)
                                if (one.GetPixel(x, y).G == two.GetPixel(x, y).G)
                                    if (one.GetPixel(x, y).B == 255)
                                        if (one.GetPixel(x, y).B == two.GetPixel(x, y).B)
                                            calculate1 = calculate1 + 1;
                }
            }

            for (int x = 0; x < one.Width; x++)
            {
                for (int y = 0; y < one.Height; y++)
                {

                    if (one.GetPixel(x, y).R == 0)
                        if (one.GetPixel(x, y).R == two.GetPixel(x, y).R)
                            if (one.GetPixel(x, y).G == 0)
                                if (one.GetPixel(x, y).G == two.GetPixel(x, y).G)
                                    if (one.GetPixel(x, y).B == 0)
                                        if (one.GetPixel(x, y).B == two.GetPixel(x, y).B)
                                            calculate2 = calculate2 + 1;
                }
            }

            for (int x = 0; x < one.Width; x++)
            {
                for (int y = 0; y < one.Height; y++)
                {

                    if (one.GetPixel(x, y).R == 255)
                        if (one.GetPixel(x, y).R != two.GetPixel(x, y).R)
                            if (one.GetPixel(x, y).G == 255)
                                if (one.GetPixel(x, y).G != two.GetPixel(x, y).G)
                                    if (one.GetPixel(x, y).B == 255)
                                        if (one.GetPixel(x, y).B != two.GetPixel(x, y).B)
                                            countPixel = countPixel + 1;
                }
            }

            for (int x = 0; x < one.Width; x++)
            {
                for (int y = 0; y < one.Height; y++)
                {

                    if (one.GetPixel(x, y).R == 0)
                        if (one.GetPixel(x, y).R != two.GetPixel(x, y).R)
                            if (one.GetPixel(x, y).G == 0)
                                if (one.GetPixel(x, y).G != two.GetPixel(x, y).G)
                                    if (one.GetPixel(x, y).B == 0)
                                        if (one.GetPixel(x, y).B != two.GetPixel(x, y).B)
                                            calculate3 = calculate3 + 1;
                }
            }
            
            textBox1.Text = "" + calculate1 + "";
            textBox2.Text = "" + calculate2 + "";
            textBox3.Text = "" + countPixel + "";
            textBox4.Text = "" + calculate3 + "";
        }
        #endregion


        #region Training Data
        private void Mask_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.gif; *.tif)|*.gif; *.tif";
            if (open.ShowDialog() == DialogResult.OK)
            {
                String strFilename2 = open.FileName;
                Image mask = Image.FromFile(strFilename2);
                Bitmap bitMask = new Bitmap(mask);
                Bitmap bit = (Bitmap)pictureBox1.Image;
                Bitmap result = new Bitmap(bit.Width, bit.Height);

                int wid = Math.Min(bit.Width, bitMask.Width);
                int hgt = Math.Min(bit.Height, bitMask.Height);

                for (int x = 0; x < wid; x++)
                {
                    for (int y = 0; y < hgt; y++)
                    {
                        Color color1 = bit.GetPixel(x, y);
                        Color color2 = bitMask.GetPixel(x, y);

                        if (color2.R <= 20)
                            result.SetPixel(x, y, Color.White);
                        else
                            result.SetPixel(x, y, color1);
                    }

                    pictureBox1.Image = result;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

                }


            }
        }

        private void Overlay_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.gif)|*.gif";
            if (open.ShowDialog() == DialogResult.OK)
            {
                String strFilename3 = open.FileName;
                Image overlay = Image.FromFile(strFilename3);
                Bitmap bitOver = new Bitmap(overlay);
                Bitmap baseImage = (Bitmap)pictureBox1.Image;
                Bitmap overlay2 = new Bitmap(baseImage.Width, baseImage.Height);

                for (int i = 0; i < baseImage.Width; i++)
                {
                    for (int j = 0; j < baseImage.Height; j++)
                    {
                        if (bitOver.GetPixel(i, j).R <= 20)
                            overlay2.SetPixel(i, j, Color.White);
                        else
                            overlay2.SetPixel(i, j, baseImage.GetPixel(i, j));
                    }
                }


                pictureBox1.Image = overlay2;

            }
        }

        private void GetRGB_Click(object sender, EventArgs e)
        {
            Bitmap RGBget = (Bitmap)pictureBox1.Image;
            int imwid = RGBget.Width;
            int imhgt = RGBget.Height;
            int total = imwid * imhgt;

            Process notepad = new Process();
            notepad.StartInfo.FileName = @"C:\Windows\Notepad.exe";
            notepad.Start();

            StreamWriter writer = new StreamWriter("test9.txt");


            for (int y = 0; y < imhgt; y++)
            {
                for (int x = 0; x < imwid; x++)
                {
                    Color pixelColor = RGBget.GetPixel(x, y);

                    if (RGBget.GetPixel(x, y).R < 255)
                    {
                        if (RGBget.GetPixel(x, y).G < 250)
                        {
                            if (RGBget.GetPixel(x, y).B < 250)
                            {
                                writer.WriteLine(";" + x + ";" + y + ";" + pixelColor.R + ";" + pixelColor.G + ";" + pixelColor.B);
                            }
                        }
                    }

                }
            }
        }

        #endregion

        

        #region Function
        public int FitnessFunction(Seed x)
        {
            for (int i = 0; i < reference.Count; i++)
            {
                if ((reference[i].pixel.R - x.pixel.R) == 0)
                    x.fitness = x.fitness + 1;
            }

            return x.fitness;
        }

        #region FollowGBest
        public Particle FollowGBest(Particle x, Particle GBest)
        {
            //for (int i = 0; i < x.seeds.Count; i++)
            //{
            #region start
            x.velocity = GBest.fitness - x.fitness;

            if (x.velocity == 0)
            {
                if (GBest.seeds[9].fitness < 400)
                x.seeds[9] = GBest.seeds[9];
                if (GBest.seeds[8].fitness < 400)
                x.seeds[8] = GBest.seeds[8];
                if (GBest.seeds[7].fitness < 400)
                x.seeds[7] = GBest.seeds[7];
                if (GBest.seeds[6].fitness < 400)
                x.seeds[6] = GBest.seeds[6];
                if (GBest.seeds[5].fitness < 400)
                x.seeds[5] = GBest.seeds[5];
                if (GBest.seeds[4].fitness < 400)
                x.seeds[4] = GBest.seeds[4];
                if (GBest.seeds[3].fitness < 400)
                x.seeds[3] = GBest.seeds[3];
                if (GBest.seeds[2].fitness < 400)
                x.seeds[2] = GBest.seeds[2];
                if (GBest.seeds[1].fitness < 400)
                x.seeds[1] = GBest.seeds[1];
                if (GBest.seeds[0].fitness < 400)
                x.seeds[0] = GBest.seeds[0];
            }

            if (x.velocity > 0)
            {
                if (x.velocity < 10)
                {
                    if (GBest.seeds[0].fitness < 400)
                    x.seeds[9] = GBest.seeds[0];
                }
            }

            if (x.velocity > 10)
            {
                if (x.velocity < 20)
                {
                    if (GBest.seeds[9].fitness < 400)
                    x.seeds[8] = GBest.seeds[9];
                }
            }

            if (x.velocity > 20)
            {
                if (x.velocity < 30)
                {
                    if (GBest.seeds[8].fitness < 400)
                    x.seeds[7] = GBest.seeds[8];
                }
            }

            if (x.velocity > 30)
            {
                if (x.velocity < 40)
                {
                    if (GBest.seeds[7].fitness < 400)
                    x.seeds[6] = GBest.seeds[7];
                }
            }

            if (x.velocity > 40)
            {
                if (x.velocity < 50)
                {
                    if (GBest.seeds[6].fitness < 400)
                    x.seeds[5] = GBest.seeds[6];
                }
            }

            if (x.velocity > 50)
            {
                if (x.velocity < 60)
                {
                    if (GBest.seeds[5].fitness < 400)
                    x.seeds[4] = GBest.seeds[5];
                }
            }

            if (x.velocity > 60)
            {
                if (x.velocity < 70)
                {
                    if (GBest.seeds[4].fitness < 400)
                    x.seeds[3] = GBest.seeds[4];
                }
            }

            if (x.velocity > 70)
            {
                if (x.velocity < 80)
                {
                    if (GBest.seeds[3].fitness < 400)
                    x.seeds[2] = GBest.seeds[3];
                }
            }

            if (x.velocity > 80)
            {
                if (x.velocity < 90)
                {
                    if (GBest.seeds[2].fitness < 400)
                    x.seeds[1] = GBest.seeds[2];
                }
            }

            if (x.velocity > 90)
            {
                if (x.velocity < 100)
                {
                    if (GBest.seeds[1].fitness < 400)
                    x.seeds[0] = GBest.seeds[1];
                }
            }

            if (x.velocity > 100)
            {
                if (x.velocity < 110)
                {
                    if (GBest.seeds[0].fitness < 400)
                    x.seeds[9] = GBest.seeds[0];
                }
            }

            if (x.velocity > 110)
            {
                if (x.velocity < 120)
                {
                    x.seeds[9] = GBest.seeds[0];
                    x.seeds[1] = GBest.seeds[9];
                }
            }

            if (x.velocity > 120)
            {
                if (x.velocity < 130)
                {
                    x.seeds[1] = GBest.seeds[9];
                    x.seeds[2] = GBest.seeds[8];
                }
            }

            if (x.velocity > 130)
            {
                if (x.velocity < 140)
                {
                    x.seeds[2] = GBest.seeds[8];
                    x.seeds[3] = GBest.seeds[7];
                }
            }

            if (x.velocity > 140)
            {
                if (x.velocity < 150)
                {
                    x.seeds[3] = GBest.seeds[7];
                    x.seeds[4] = GBest.seeds[6];
                }
            }

            if (x.velocity > 150)
            {
                if (x.velocity < 160)
                {
                    x.seeds[4] = GBest.seeds[6];
                    x.seeds[5] = GBest.seeds[5];
                }
            }

            if (x.velocity > 160)
            {
                if (x.velocity < 170)
                {
                    x.seeds[5] = GBest.seeds[5];
                    x.seeds[6] = GBest.seeds[4];
                }
            }

            if (x.velocity > 170)
            {
                if (x.velocity < 180)
                {
                    x.seeds[6] = GBest.seeds[4];
                    x.seeds[7] = GBest.seeds[3];
                }
            }

            if (x.velocity > 180)
            {
                if (x.velocity < 190)
                {
                    x.seeds[7] = GBest.seeds[3];
                    x.seeds[8] = GBest.seeds[2];
                }
            }

            if (x.velocity > 190)
            {
                if (x.velocity < 200)
                {
                    x.seeds[8] = GBest.seeds[2];
                    x.seeds[9] = GBest.seeds[1];
                }
            }

            if (x.velocity > 200)
            {
                if (x.velocity < 220)
                {
                    x.seeds[9] = GBest.seeds[1];
                    x.seeds[0] = GBest.seeds[0];
                }
            }

            if (x.velocity > 220)
            {
                if (x.velocity < 240)
                {
                    x.seeds[0] = GBest.seeds[0];
                    x.seeds[1] = GBest.seeds[9];
                }
            }

            if (x.velocity > 240)
            {
                if (x.velocity < 260)
                {
                    x.seeds[9] = GBest.seeds[0];
                    x.seeds[8] = GBest.seeds[1];
                    x.seeds[7] = GBest.seeds[2];
                }
            }

            if (x.velocity > 260)
            {
                if (x.velocity < 280)
                {
                    x.seeds[8] = GBest.seeds[1];
                    x.seeds[7] = GBest.seeds[2];
                    x.seeds[6] = GBest.seeds[3];
                }
            }

            if (x.velocity > 280)
            {
                if (x.velocity < 300)
                {
                    x.seeds[7] = GBest.seeds[2];
                    x.seeds[6] = GBest.seeds[3];
                    x.seeds[5] = GBest.seeds[4];
                }
            }

            if (x.velocity > 300)
            {
                if (x.velocity < 320)
                {
                    x.seeds[6] = GBest.seeds[3];
                    x.seeds[5] = GBest.seeds[4];
                    x.seeds[4] = GBest.seeds[5];
                }
            }

            if (x.velocity > 320)
            {
                if (x.velocity < 340)
                {
                    x.seeds[5] = GBest.seeds[4];
                    x.seeds[4] = GBest.seeds[5];
                    x.seeds[3] = GBest.seeds[6];
                }
            }

            if (x.velocity > 340)
            {
                if (x.velocity < 360)
                {
                    x.seeds[4] = GBest.seeds[5];
                    x.seeds[3] = GBest.seeds[6];
                    x.seeds[2] = GBest.seeds[7];
                }
            }

            if (x.velocity > 360)
            {
                if (x.velocity < 380)
                {
                    x.seeds[3] = GBest.seeds[6];
                    x.seeds[2] = GBest.seeds[7];
                    x.seeds[1] = GBest.seeds[8];
                }
            }

            if (x.velocity > 380)
            {
                if (x.velocity < 400)
                {
                    x.seeds[2] = GBest.seeds[7];
                    x.seeds[1] = GBest.seeds[8];
                    x.seeds[0] = GBest.seeds[9];
                }
            }

            if (x.velocity > 400)
            {
                if (x.velocity < 420)
                {
                    x.seeds[1] = GBest.seeds[8];
                    x.seeds[0] = GBest.seeds[9];
                    x.seeds[9] = GBest.seeds[0];
                }
            }

            if (x.velocity > 420)
            {
                if (x.velocity < 440)
                {
                    x.seeds[0] = GBest.seeds[9];
                    x.seeds[9] = GBest.seeds[0];
                    x.seeds[8] = GBest.seeds[1];
                }
            }

            if (x.velocity > 440)
            {
                if (x.velocity < 460)
                {
                    x.seeds[9] = GBest.seeds[0];
                    x.seeds[8] = GBest.seeds[1];
                    x.seeds[7] = GBest.seeds[2];
                }
            }

            if (x.velocity > 460)
            {
                if (x.velocity < 480)
                {
                    x.seeds[8] = GBest.seeds[1];
                    x.seeds[7] = GBest.seeds[2];
                    x.seeds[6] = GBest.seeds[3];
                }
            }

            if (x.velocity > 480)
            {
                if (x.velocity < 500)
                {
                    x.seeds[7] = GBest.seeds[2];
                    x.seeds[6] = GBest.seeds[3];
                    x.seeds[5] = GBest.seeds[4];
                }
            }

            if (x.velocity > 500)
            {
                if (x.velocity < 550)
                {
                    x.seeds[9] = GBest.seeds[0];
                    x.seeds[8] = GBest.seeds[1];
                    x.seeds[7] = GBest.seeds[2];
                    x.seeds[6] = GBest.seeds[3];
                }
            }

            if (x.velocity > 550)
            {
                if (x.velocity < 600)
                {
                    x.seeds[8] = GBest.seeds[1];
                    x.seeds[7] = GBest.seeds[2];
                    x.seeds[6] = GBest.seeds[3];
                    x.seeds[5] = GBest.seeds[4];
                }
            }

            if (x.velocity > 600)
            {
                if (x.velocity < 650)
                {
                    x.seeds[7] = GBest.seeds[2];
                    x.seeds[6] = GBest.seeds[3];
                    x.seeds[5] = GBest.seeds[4];
                    x.seeds[4] = GBest.seeds[5];
                }
            }

            if (x.velocity > 650)
            {
                if (x.velocity < 700)
                {
                    x.seeds[6] = GBest.seeds[3];
                    x.seeds[5] = GBest.seeds[4];
                    x.seeds[4] = GBest.seeds[5];
                    x.seeds[3] = GBest.seeds[6];
                }
            }

            if (x.velocity > 700)
            {
                if (x.velocity < 750)
                {
                    x.seeds[5] = GBest.seeds[4];
                    x.seeds[4] = GBest.seeds[5];
                    x.seeds[3] = GBest.seeds[6];
                    x.seeds[2] = GBest.seeds[7];
                }
            }

            if (x.velocity > 750)
            {
                if (x.velocity < 800)
                {
                    x.seeds[4] = GBest.seeds[5];
                    x.seeds[3] = GBest.seeds[6];
                    x.seeds[2] = GBest.seeds[7];
                    x.seeds[1] = GBest.seeds[8];
                }
            }

            if (x.velocity > 800)
            {
                if (x.velocity < 850)
                {
                    x.seeds[3] = GBest.seeds[6];
                    x.seeds[2] = GBest.seeds[7];
                    x.seeds[1] = GBest.seeds[8];
                    x.seeds[0] = GBest.seeds[9];
                }
            }

            if (x.velocity > 850)
            {
                if (x.velocity < 900)
                {
                    x.seeds[2] = GBest.seeds[7];
                    x.seeds[1] = GBest.seeds[8];
                    x.seeds[0] = GBest.seeds[9];
                    x.seeds[9] = GBest.seeds[0];
                }
            }

            if (x.velocity > 900)
            {
                if (x.velocity < 950)
                {
                    x.seeds[1] = GBest.seeds[8];
                    x.seeds[0] = GBest.seeds[9];
                    x.seeds[9] = GBest.seeds[0];
                    x.seeds[8] = GBest.seeds[1];
                }
            }

            if (x.velocity > 950)
            {
                if (x.velocity < 1000)
                {
                    x.seeds[0] = GBest.seeds[9];
                    x.seeds[9] = GBest.seeds[0];
                    x.seeds[8] = GBest.seeds[1];
                    x.seeds[7] = GBest.seeds[2];
                }
            }
            //}
            return x;

            #endregion
        }

        #endregion

        #region FollowPBest
        public Particle FollowPBest(Particle x, Particle PBest)
        {
            #region start
            

            if (x.velocity == 0)
            {
                x.seeds[9] = x.PBest.seeds[9];
                x.seeds[8] = x.PBest.seeds[8];
                x.seeds[7] = x.PBest.seeds[7];
                x.seeds[6] = x.PBest.seeds[6];
                x.seeds[5] = x.PBest.seeds[5];
                x.seeds[4] = x.PBest.seeds[4];
                x.seeds[3] = x.PBest.seeds[3];
                x.seeds[2] = x.PBest.seeds[2];
                x.seeds[1] = x.PBest.seeds[1];
                x.seeds[0] = x.PBest.seeds[0];
            }

            if (x.velocity > 0)
            {
                if (x.velocity < 20)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                }
            }

            if (x.velocity > 20)
            {
                if (x.velocity < 30)
                {
                    x.seeds[8] = x.PBest.seeds[9];
                }
            }

            if (x.velocity > 30)
            {
                if (x.velocity < 40)
                {
                    x.seeds[7] = x.PBest.seeds[8];
                }
            }

            if (x.velocity > 40)
            {
                if (x.velocity < 60)
                {
                    x.seeds[6] = x.PBest.seeds[0];
                }
            }

            if (x.velocity > 60)
            {
                if (x.velocity < 80)
                {
                    x.seeds[5] = x.PBest.seeds[9];
                }
            }

            if (x.velocity > 80)
            {
                if (x.velocity < 100)
                {
                    x.seeds[9] = x.PBest.seeds[9];
                }
            }

            if (x.velocity > 100)
            {
                if (x.velocity < 120)
                {
                    x.seeds[9] = x.PBest.seeds[1];
                    x.seeds[8] = x.PBest.seeds[2];
                }
            }

            if (x.velocity > 120)
            {
                if (x.velocity < 140)
                {
                    x.seeds[9] = x.PBest.seeds[3];
                    x.seeds[8] = x.PBest.seeds[4];
                    x.seeds[7] = x.PBest.seeds[5];
                }
            }

            if (x.velocity > 140)
            {
                if (x.velocity < 160)
                {
                    x.seeds[9] = x.PBest.seeds[6];
                    x.seeds[8] = x.PBest.seeds[7];
                    x.seeds[7] = x.PBest.seeds[8];
                }
            }

            if (x.velocity > 160)
            {
                if (x.velocity < 180)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                    x.seeds[8] = x.PBest.seeds[1];
                    x.seeds[7] = x.PBest.seeds[2];
                }
            }

            if (x.velocity > 180)
            {
                if (x.velocity < 200)
                {
                    x.seeds[9] = x.PBest.seeds[5];
                    x.seeds[8] = x.PBest.seeds[6];
                    x.seeds[7] = x.PBest.seeds[7];
                }
            }

            if (x.velocity > 200)
            {
                if (x.velocity < 240)
                {
                    x.seeds[9] = x.PBest.seeds[1];
                    x.seeds[8] = x.PBest.seeds[2];
                    x.seeds[7] = x.PBest.seeds[3];
                }
            }

            if (x.velocity > 240)
            {
                if (x.velocity < 280)
                {
                    x.seeds[9] = x.PBest.seeds[8];
                    x.seeds[8] = x.PBest.seeds[9];
                    x.seeds[7] = x.PBest.seeds[0];
                    x.seeds[6] = x.PBest.seeds[1];
                }
            }

            if (x.velocity > 280)
            {
                if (x.velocity < 320)
                {
                    x.seeds[9] = x.PBest.seeds[4];
                    x.seeds[8] = x.PBest.seeds[5];
                    x.seeds[7] = x.PBest.seeds[6];
                    x.seeds[6] = x.PBest.seeds[7];
                }
            }

            if (x.velocity > 320)
            {
                if (x.velocity < 360)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                    x.seeds[8] = x.PBest.seeds[1];
                    x.seeds[7] = x.PBest.seeds[2];
                    x.seeds[6] = x.PBest.seeds[3];
                }
            }

            if (x.velocity > 400)
            {
                if (x.velocity < 450)
                {
                    x.seeds[9] = x.PBest.seeds[9];
                    x.seeds[8] = x.PBest.seeds[8];
                    x.seeds[7] = x.PBest.seeds[7];
                    x.seeds[6] = x.PBest.seeds[6];
                }
            }

            if (x.velocity > 450)
            {
                if (x.velocity < 500)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                    x.seeds[8] = x.PBest.seeds[1];
                    x.seeds[7] = x.PBest.seeds[2];
                    x.seeds[6] = x.PBest.seeds[3];
                    x.seeds[5] = x.PBest.seeds[4];
                }
            }

            if (x.velocity > 500)
            {
                if (x.velocity < 550)
                {
                    x.seeds[9] = x.PBest.seeds[5];
                    x.seeds[8] = x.PBest.seeds[6];
                    x.seeds[7] = x.PBest.seeds[7];
                    x.seeds[6] = x.PBest.seeds[8];
                    x.seeds[5] = x.PBest.seeds[9];
                }
            }

            if (x.velocity > 550)
            {
                if (x.velocity < 600)
                {
                    x.seeds[9] = x.PBest.seeds[8];
                    x.seeds[8] = x.PBest.seeds[7];
                    x.seeds[7] = x.PBest.seeds[6];
                    x.seeds[6] = x.PBest.seeds[5];
                    x.seeds[5] = x.PBest.seeds[4];
                }
            }

            if (x.velocity > 600)
            {
                if (x.velocity < 650)
                {
                    x.seeds[9] = x.PBest.seeds[3];
                    x.seeds[8] = x.PBest.seeds[2];
                    x.seeds[7] = x.PBest.seeds[1];
                    x.seeds[6] = x.PBest.seeds[0];
                    x.seeds[5] = x.PBest.seeds[9];
                }
            }

            if (x.velocity > 650)
            {
                if (x.velocity < 700)
                {
                    x.seeds[9] = x.PBest.seeds[8];
                    x.seeds[8] = x.PBest.seeds[7];
                    x.seeds[7] = x.PBest.seeds[5];
                    x.seeds[6] = x.PBest.seeds[4];
                    x.seeds[5] = x.PBest.seeds[2];
                }
            }

            if (x.velocity > 700)
            {
                if (x.velocity < 750)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                    x.seeds[7] = x.PBest.seeds[1];
                    x.seeds[5] = x.PBest.seeds[2];
                    x.seeds[3] = x.PBest.seeds[3];
                    x.seeds[1] = x.PBest.seeds[4];
                }
            }

            if (x.velocity > 750)
            {
                if (x.velocity < 800)
                {
                    x.seeds[8] = x.PBest.seeds[2];
                    x.seeds[6] = x.PBest.seeds[3];
                    x.seeds[4] = x.PBest.seeds[4];
                    x.seeds[2] = x.PBest.seeds[5];
                    x.seeds[0] = x.PBest.seeds[6];
                }
            }

            if (x.velocity > 800)
            {
                if (x.velocity < 850)
                {
                    x.seeds[6] = x.PBest.seeds[4];
                    x.seeds[5] = x.PBest.seeds[3];
                    x.seeds[4] = x.PBest.seeds[2];
                    x.seeds[3] = x.PBest.seeds[1];
                    x.seeds[2] = x.PBest.seeds[0];
                }
            }

            if (x.velocity > 850)
            {
                if (x.velocity < 900)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                    x.seeds[8] = x.PBest.seeds[1];
                    x.seeds[7] = x.PBest.seeds[2];
                }
            }

            if (x.velocity > 900)
            {
                if (x.velocity < 1000)
                {
                    x.seeds[6] = x.PBest.seeds[3];
                    x.seeds[5] = x.PBest.seeds[2];
                    x.seeds[4] = x.PBest.seeds[1];
                }
            }


            //reverse seed
            if (x.velocity > 1000)
            {
                if (x.velocity < 1100)
                {
                    x.seeds[9] = x.PBest.seeds[4];
                    x.seeds[7] = x.PBest.seeds[2];
                    x.seeds[5] = x.PBest.seeds[0];
                }
            }

            if (x.velocity > 1100)
            {
                if (x.velocity < 1200)
                {
                    x.seeds[8] = x.PBest.seeds[7];
                    x.seeds[6] = x.PBest.seeds[5];
                    x.seeds[4] = x.PBest.seeds[3];
                }
            }

            if (x.velocity > 1200)
            {
                if (x.velocity < 1300)
                {
                    x.seeds[7] = x.PBest.seeds[0];
                    x.seeds[5] = x.PBest.seeds[8];
                    x.seeds[3] = x.PBest.seeds[6];
                }
            }

            if (x.velocity > 1300)
            {
                if (x.velocity < 1400)
                {
                    x.seeds[6] = x.PBest.seeds[3];
                    x.seeds[5] = x.PBest.seeds[2];
                    x.seeds[4] = x.PBest.seeds[1];
                }
            }

            if (x.velocity > 1400)
            {
                if (x.velocity < 1500)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                    x.seeds[8] = x.PBest.seeds[9];
                    x.seeds[7] = x.PBest.seeds[8];
                }
            }

            if (x.velocity > 1500)
            {
                if (x.velocity < 1600)
                {
                    x.seeds[9] = x.PBest.seeds[4];
                    x.seeds[7] = x.PBest.seeds[2];
                    x.seeds[5] = x.PBest.seeds[0];
                    x.seeds[3] = x.PBest.seeds[8];
                }
            }

            if (x.velocity > 1600)
            {
                if (x.velocity < 1700)
                {
                    x.seeds[8] = x.PBest.seeds[2];
                    x.seeds[6] = x.PBest.seeds[0];
                    x.seeds[4] = x.PBest.seeds[8];
                    x.seeds[2] = x.PBest.seeds[6];
                }
            }

            if (x.velocity > 1700)
            {
                if (x.velocity < 1800)
                {
                    x.seeds[7] = x.PBest.seeds[8];
                    x.seeds[5] = x.PBest.seeds[6];
                    x.seeds[3] = x.PBest.seeds[4];
                    x.seeds[1] = x.PBest.seeds[2];
                }
            }

            if (x.velocity > 1800)
            {
                if (x.velocity < 1900)
                {
                    x.seeds[6] = x.PBest.seeds[5];
                    x.seeds[4] = x.PBest.seeds[3];
                    x.seeds[2] = x.PBest.seeds[1];
                    x.seeds[0] = x.PBest.seeds[9];
                }
            }

            if (x.velocity > 1900)
            {
                if (x.velocity < 2000)
                {
                    x.seeds[7] = x.PBest.seeds[8];
                    x.seeds[5] = x.PBest.seeds[6];
                    x.seeds[3] = x.PBest.seeds[4];
                }
            }

            if (x.velocity > 2000)
            {
                if (x.velocity < 2100)
                {
                    x.seeds[9] = x.PBest.seeds[0];
                    x.seeds[7] = x.PBest.seeds[8];
                    x.seeds[5] = x.PBest.seeds[6];
                }
            }

            if (x.velocity > 2100)
            {
                if (x.velocity < 2200)
                {
                    x.seeds[9] = x.PBest.seeds[8];
                    x.seeds[7] = x.PBest.seeds[6];
                    x.seeds[5] = x.PBest.seeds[5];
                }
            }

            if (x.velocity > 2200)
            {
                if (x.velocity < 2300)
                {
                    x.seeds[6] = x.PBest.seeds[3];
                    x.seeds[4] = x.PBest.seeds[2];
                    x.seeds[2] = x.PBest.seeds[1];
                }
            }

            if (x.velocity > 2300)
            {
                if (x.velocity < 2400)
                {
                    x.seeds[1] = x.PBest.seeds[3];
                    x.seeds[2] = x.PBest.seeds[2];
                    x.seeds[3] = x.PBest.seeds[1];
                }
            }

            if (x.velocity > 2400)
            {
                if (x.velocity < 2500)
                {
                    x.seeds[5] = x.PBest.seeds[3];
                    x.seeds[7] = x.PBest.seeds[2];
                    x.seeds[9] = x.PBest.seeds[1];
                }
            }

            if (x.velocity > 2500)
            {
                if (x.velocity < 2600)
                {
                    x.seeds[6] = x.PBest.seeds[3];
                    x.seeds[4] = x.PBest.seeds[2];
                    x.seeds[2] = x.PBest.seeds[1];
                    x.seeds[0] = x.PBest.seeds[0];
                }
            }

            if (x.velocity > 2600)
            {
                if (x.velocity < 2700)
                {
                    x.seeds[5] = x.PBest.seeds[1];
                    x.seeds[3] = x.PBest.seeds[5];
                    x.seeds[1] = x.PBest.seeds[1];
                    x.seeds[9] = x.PBest.seeds[5];
                }
            }

            if (x.velocity > 2700)
            {
                if (x.velocity < 2800)
                {
                    x.seeds[6] = x.PBest.seeds[3];
                    x.seeds[4] = x.PBest.seeds[2];
                    x.seeds[2] = x.PBest.seeds[3];
                    x.seeds[3] = x.PBest.seeds[2];
                }
            }
            //}
            return x;

            #endregion
        }
        #endregion

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }







        #endregion

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Clear_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            richTextBox1.Text = null;
            textBox1.Text = null;
            textBox2.Text = null;

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void menuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    #region Classes
    public class Seed // creating seed which store the informations of the pixel's color and location
    {
        public Point loc;
        public Color pixel;
        public int fitness;

        public Seed()
        {

        }

        public Seed(Point loc, Color pixel, int fitness)
        {
            this.loc = loc;
            this.pixel = pixel;
            this.fitness = fitness;

            fitness = 0;
        }
    }

    public class Particle //to create particle which keep the information of the seeds
    {
        public List<Seed> seeds;
        public Particle PBest;
        public int fitness;
        public int velocity;

        public Particle()
        {

        }

        public Particle(List<Seed> seeds, Particle PBest, int fitness, int velocity)
        {
            this.seeds = seeds;
            this.PBest = PBest;
            this.fitness = fitness;
        }
    }


    #endregion
}

// need to do multithreading
// review the coding
// make sure to understand the whole process
// need to revamp the whole process
