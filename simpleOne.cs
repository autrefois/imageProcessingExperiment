using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;
using AForge.Controls;
using AForge.Imaging.Filters;

namespace simpleOne
{
    public partial class simpleOne : Form
    {
        // Lab 1
        private Image theImage;
        private string file;
        
        private Bitmap theBitmapLeft;
        private Bitmap theBitmapRight;

        // Lab 3
        private Image redhot;
        private Bitmap bRedHot;

        // Lab 5
        double X, Y, Z, L, a, b, u, v;
        Color[][] structuralElement;

        public simpleOne()
        {
            InitializeComponent();
       /*     btnSave.Enabled = false;
            btnProcess.Enabled = false;
            btnInverse.Enabled = false;
            btnExtract.Enabled = false;
        * */

            structuralElement = new Color[2][];
            for (int x = 0; x < structuralElement.Length; x++)
            {
                structuralElement[x] = new Color[2];
            }

            structuralElement[0][0] = Color.FromArgb(10, 10, 10);
            structuralElement[0][1] = Color.FromArgb(10, 10, 10);
            structuralElement[1][0] = Color.FromArgb(10, 10, 10);
            structuralElement[1][1] = Color.FromArgb(10, 10, 10);
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
                
        }

        /** Lab 1 - Simple Processing */
        private void btnOpenPic_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            file = openFileDialog.FileName;
            Console.WriteLine(file);
            theImage = Image.FromFile(file);
            theBitmapLeft = new Bitmap(theImage);
            picLeft.Image = theBitmapLeft;
            picLeft.Refresh();
            btnProcess.Enabled = true;
            btnInverse.Enabled = true;
            btnExtract.Enabled = true;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                for (int i = 0; i < theBitmapLeft.Height; i++)
                    for (int j = 0; j < theBitmapLeft.Width; j++)
                    {
                        Color c = theBitmapLeft.GetPixel(j, i);
                        theBitmapRight.SetPixel(j, i, Color.FromArgb((c.R + 100) % 256, (c.G * 2) % 256, c.B));
                    }

                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.ShowDialog();
            theBitmapRight.Save(saveFileDialog.FileName + ".bmp");
        }

        private void btnSwap_Click(object sender, EventArgs e)
        {
            theBitmapLeft = theBitmapRight;
            picLeft.Image = theBitmapLeft;
            picLeft.Refresh();
        }

        /** Lab 2 - Punctual transformation */

        private void btnInverse_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                //   int a = 100, b = 120;

                for (int i = 0; i < theBitmapLeft.Height; i++)
                    for (int j = 0; j < theBitmapLeft.Width; j++)
                    {
                        Color c = theBitmapLeft.GetPixel(j, i);

                        /*
                        int g = c.R;
                        if (g < a ) g = 0;
                        else
                            if (g > b ) g = 0;

                        theBitmapRight.SetPixel(j, i, Color.FromArgb(2*g, 2*g, 2*g)); */

                        theBitmapRight.SetPixel(j, i, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                    }

                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            //   int gray = ((pixels[i] & 0xff) + ((pixels[i] & 0xff00) >> 8) + ((pixels[i] & 0xff0000) >> 16)) / 3;
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                int bit = 6; // extract the 7th bit

                for (int i = 0; i < theBitmapLeft.Height; i++)
                    for (int j = 0; j < theBitmapLeft.Width; j++)
                    {
                        Color c = theBitmapLeft.GetPixel(j, i);
                        byte r = c.R;
                        byte g = c.G;
                        byte b = c.B;

                        if (r != Math.Pow(2, bit)) r = 0;
                        if (g != Math.Pow(2, bit)) g = 0;
                        if (b != Math.Pow(2, bit)) b = 0;

                        theBitmapRight.SetPixel(j, i, Color.FromArgb((int)r, (int)g, (int)b));
                    }

                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnGamma_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                float x = (float) tb_howMuch.Value / 40;

                Gamma(x - 0.02, x, x);

                Console.WriteLine(tb_howMuch.Value + " + X is " + x);

                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void Gamma(double r, double g, double b)
        {
            byte A, R, G, B;
            Color pixelColor;

            byte[] redGamma = new byte[256];
            byte[] greenGamma = new byte[256];
            byte[] blueGamma = new byte[256];

            for (int k = 0; k < 256; ++k)
            {
                // Calculate gamma  = 255 * (i / 255)^1/gamma 
                redGamma[k] = (byte)Math.Min(255, (int)((255.0
                    * Math.Pow(k / 255.0, 1.0 / r))));
                greenGamma[k] = (byte)Math.Min(255, (int)((255.0
                    * Math.Pow(k / 255.0, 1.0 / g))));
                blueGamma[k] = (byte)Math.Min(255, (int)((255.0
                    * Math.Pow(k / 255.0, 1.0 / b))));
            }

            for (int y = 0; y < theBitmapRight.Height; y++)
            {
                for (int x = 0; x < theBitmapRight.Width; x++)
                {
                    pixelColor = theBitmapRight.GetPixel(x, y);
                    A = pixelColor.A;
                    R = redGamma[pixelColor.R];
                    G = greenGamma[pixelColor.G];
                    B = blueGamma[pixelColor.B];
                    theBitmapRight.SetPixel(x, y, Color.FromArgb((int)A, (int)R, (int)G, (int)B));
                }
            }
        }

        private void btnBinarize_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                for (int i = 0; i < theBitmapRight.Width; i++)
                {
                    for (int j = 0; j < theBitmapRight.Height; j++)
                    {
                        Color pixel = theBitmapRight.GetPixel(i, j);
                        byte R, G, B;

                        // Fetching RGB Values
                        R = pixel.R;
                        G = pixel.G;
                        B = pixel.B;

                        // Converting Black tones 
                        if ((R < Color.Black.R + 110)
                            && (G < Color.Black.G + 110)
                            && (B < Color.Black.B + 110))
                        {
                            theBitmapRight.SetPixel(i, j, Color.Black);
                            continue;
                        }

                        // Converting Yellow tones
                        if ((R > Color.Yellow.R - 110)
                                && (G > Color.Yellow.G - 110)
                                && (B < Color.Yellow.B + 110))
                        {
                            theBitmapRight.SetPixel(i, j, Color.Black);
                            continue;
                        }

                        // Converting White tones
                        if ((R > Color.White.R - 110)
                            && (G > Color.White.G - 110)
                            && (B > Color.White.B - 110))
                        {
                            theBitmapRight.SetPixel(i, j, Color.Black);
                            continue;
                        }

                        theBitmapRight.SetPixel(i, j, Color.White);
                    }
                }


                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /** Lab 3 - Spatial transformation */

        private void btnSegmentation_Click(object sender, EventArgs e)
        {

            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                for (int i = 0; i < theBitmapRight.Width; i++)
                {
                    for (int j = 0; j < theBitmapRight.Height; j++)
                    {
                        Color pixel = theBitmapRight.GetPixel(i, j);
                        byte R, G, B;

                        // Fetching RGB Values
                        R = pixel.R;
                        G = pixel.G;
                        B = pixel.B;

                        // Converting Black tones 
                        if ((R < Color.Black.R + 110)
                            && (G < Color.Black.G + 110)
                            && (B < Color.Black.B + 110))
                        {
                            theBitmapRight.SetPixel(i, j, Color.Red);
                            continue;
                        }

                        // Converting Yellow tones
                        if ((R > Color.Yellow.R - 110)
                                && (G > Color.Yellow.G - 110)
                                && (B < Color.Yellow.B + 110))
                        {
                            theBitmapRight.SetPixel(i, j, Color.Yellow);
                            continue;
                        }

                        // Converting White tones
                        if ((R > Color.White.R - 110)
                            && (G > Color.White.G - 110)
                            && (B > Color.White.B - 110))
                        {
                            theBitmapRight.SetPixel(i, j, Color.Gray);
                            continue;
                        }

                        theBitmapRight.SetPixel(i, j, Color.Black);
                    }
                }


                picRight.Image = theBitmapRight;
                picRight.Refresh();
			


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void btn_PseudoColor_Click(object sender, EventArgs e)
        {
            theBitmapRight = new Bitmap(theBitmapLeft);

            int w = theBitmapRight.Width;
            int h = theBitmapRight.Height;

            redhot = Image.FromFile("E:\\Facultate\\Anul IV\\Sem II\\Image Processing\\PicsToUse\\red_hot.jpg");
            bRedHot = new Bitmap(redhot);

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color c = theBitmapRight.GetPixel(i, j);

                    int r = c.R;
                    int g = c.G;
                    int b = c.B;

                    if (r == g && g == b)
                    {
                        Color color = searchColorInLUT(r);
                        theBitmapRight.SetPixel(i, j, Color.FromArgb(color.R, color.G, color.B));
                    }
                }
            }

            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private Color searchColorInLUT(int x)
        {

            Color equivalentColor = bRedHot.GetPixel(x, 10);

            return equivalentColor;
        }

        private void btnDilate_Click(object sender, EventArgs e)
        {

            int f = tb_howMuch.Value; //10; // how much dilation
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                for (int i = 0; i < theBitmapLeft.Width; i++)
                    for (int j = 0; j < theBitmapLeft.Height; j++)
                    {

                        Color c = theBitmapLeft.GetPixel(i, j);

                        if (c.R == 0)
                        {
                            try
                            {
                                for (int k = 1; k < f; k++)
                                {
                                    theBitmapRight.SetPixel(i - k, j, Color.Black);
                                    theBitmapRight.SetPixel(i + k, j, Color.Black);
                                    theBitmapRight.SetPixel(i, j - k, Color.Black);
                                    theBitmapRight.SetPixel(i, j + k, Color.Black);
                                    theBitmapRight.SetPixel(i - k, j - k, Color.Black);
                                    theBitmapRight.SetPixel(i + k, j + k, Color.Black);
                                    theBitmapRight.SetPixel(i + k, j - k, Color.Black);
                                    theBitmapRight.SetPixel(i - k, j + k, Color.Black);
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }

                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void btnContours_Click(object sender, EventArgs e)
        {
            theBitmapRight = contours(theBitmapLeft);
            picRight.Image = theBitmapRight;
            picRight.Refresh();   
        }

        private Bitmap contours(Bitmap source)
        {
            Bitmap newBitmap = new Bitmap(source.Width, source.Height);

            for (int i = 1; i < source.Width - 1; i++)
            {
                double x = (double)tb_howMuch.Value / 40;
                for (int j = 1; j < source.Height - 1; j++)
                {
                    int vr = contR(i, j, x);
                    if (vr < 0)
                        vr = 0;
                    else if (vr > 255)
                        vr = 255;

                    int vg = contG(i, j, x);
                    if (vg < 0)
                        vg = 0;
                    else if (vg > 255)
                        vg = 255;

                    int vb = contB(i, j, x);
                    if (vb < 0)
                        vb = 0;
                    else if (vb > 255)
                        vb = 255;

                    newBitmap.SetPixel(i, j, Color.FromArgb(255, vr, vg, vb));
                }
            }
            return newBitmap;
        }

        private int contR(int i, int j, double l)
        {
            int r = theBitmapLeft.GetPixel(i,j).R;
            int f = (theBitmapLeft.GetPixel(i, j).R + theBitmapLeft.GetPixel(i, j).G + theBitmapLeft.GetPixel(i, j).B) / 3;
            int v = r + Convert.ToInt32(l * (r - f));
            return v;
        }

        private int contG(int i, int j, double l)
        {
            int r = theBitmapLeft.GetPixel(i, j).G;
            int f = (theBitmapLeft.GetPixel(i, j).R + theBitmapLeft.GetPixel(i, j).G + theBitmapLeft.GetPixel(i, j).B) / 3;
            int v = r + Convert.ToInt32(l * (r - f));
            return v;
        }

        private int contB(int i, int j, double l)
        {
            int r = theBitmapLeft.GetPixel(i, j).B;
            int f = (theBitmapLeft.GetPixel(i, j).R + theBitmapLeft.GetPixel(i, j).G + theBitmapLeft.GetPixel(i, j).B) / 3;
            int v = r + Convert.ToInt32(l * (r - f));
            return v;
        }

        private void tb_howMuch_Scroll(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnDirectional_Click(object sender, EventArgs e)
        {
            theBitmapRight = DirectionalFilter(theBitmapLeft, Convert.ToInt32(cmbDirectional.SelectedItem));
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private Bitmap DirectionalFilter(Bitmap source, int n)
        {
            Bitmap newBitmap = new Bitmap(source.Width, source.Height);

            for (int x = n / 2; x < source.Width - n / 2; x++)
            {
                for (int y = n / 2; y < source.Height - n / 2; y++)
                {
                    newBitmap.SetPixel(x, y, Color.FromArgb(DirR(x, y, n), DirG(x, y, n), DirB(x, y, n)));
                }
            }

            return newBitmap;
        }

        int DirR(int r , int c, int n)
        {
            int[] valfa = new int[4];
            int[] optimalDir = new int[4];
            Color color = theBitmapLeft.GetPixel(0, 2);

            int[,] matrix = new int[n, n];

            for (int i = r - n / 2; i <= r + n / 2; i++)
            {
                for (int j = c - n / 2; j <= c + n / 2; j++)
                {
                    if (i - j == r - c)
                        valfa[0] = valfa[0] + theBitmapLeft.GetPixel(i, j).R;
                    if (i + j == r + c)
                        valfa[1] = valfa[1] + theBitmapLeft.GetPixel(i, j).R;
                    if (i == r)
                        valfa[2] = valfa[2] + theBitmapLeft.GetPixel(i, j).R;
                    if (j == c)
                        valfa[3] = valfa[3] + theBitmapLeft.GetPixel(i, j).R;
                }
                
            }

            for (int i = 0; i < 4; i++)
            {
                valfa[i] = valfa[i] / n;
            }


            for (int i = 0; i < 4; i++)
            {
                optimalDir[i] = Math.Abs(theBitmapLeft.GetPixel(r, c).R - valfa[i]);
            }

            int min = optimalDir[0];
            int indice = 0;

            for (int j = 1; j < 4; j++)
            {
                if (min > optimalDir[j])
                {
                    min = optimalDir[j];
                    indice = j;
                }
            }

            return valfa[indice];
        }

        int DirG(int r, int c,int n)
        {
            int[] valfa = new int[4];
            int[] optimalDir = new int[4];
            Color color = theBitmapLeft.GetPixel(0, 2);

            int[,] matrix = new int[n, n];

            for (int i = r - n / 2; i <= r + n / 2; i++)
            {
                for (int j = c - n / 2; j <= c + n / 2; j++)
                {
                    if (i - j == r - c)
                        valfa[0] = valfa[0] + theBitmapLeft.GetPixel(i, j).G;
                    if (i + j == r + c)
                        valfa[1] = valfa[1] + theBitmapLeft.GetPixel(i, j).G;
                    if (i == r)
                        valfa[2] = valfa[2] + theBitmapLeft.GetPixel(i, j).G;
                    if (j == c)
                        valfa[3] = valfa[3] + theBitmapLeft.GetPixel(i, j).G;
                }

            }

            for (int i = 0; i < 4; i++)
            {
                valfa[i] = valfa[i] / n;
            }


            for (int i = 0; i < 4; i++)
            {
                optimalDir[i] = Math.Abs(theBitmapLeft.GetPixel(r, c).G - valfa[i]);
            }

            int min = optimalDir[0];
            int indice = 0;

            for (int j = 1; j < 4; j++)
            {
                if (min > optimalDir[j])
                {
                    min = optimalDir[j];
                    indice = j;
                }
            }

            return valfa[indice];
        }

        int DirB(int r, int c, int n)
        {
            int[] valfa = new int[4];
            int[] optimalDir = new int[4];
            Color color = theBitmapLeft.GetPixel(0, 2);

            int[,] matrix = new int[n, n];

            for (int i = r - n / 2; i <= r + n / 2; i++)
            {
                for (int j = c - n / 2; j <= c + n / 2; j++)
                {
                    if (i - j == r - c)
                        valfa[0] = valfa[0] + theBitmapLeft.GetPixel(i, j).B;
                    if (i + j == r + c)
                        valfa[1] = valfa[1] + theBitmapLeft.GetPixel(i, j).B;
                    if (i == r)
                        valfa[2] = valfa[2] + theBitmapLeft.GetPixel(i, j).B;
                    if (j == c)
                        valfa[3] = valfa[3] + theBitmapLeft.GetPixel(i, j).B;
                }

            }

            for (int i = 0; i < 4; i++)
            {
                valfa[i] = valfa[i] / n;
            }


            for (int i = 0; i < 4; i++)
            {
                optimalDir[i] = Math.Abs(theBitmapLeft.GetPixel(r, c).R - valfa[i]);
            }

            int min = optimalDir[0];
            int indice = 0;

            for (int j = 1; j < 4; j++)
            {
                if (min > optimalDir[j])
                {
                    min = optimalDir[j];
                    indice = j;
                }
            }

            return valfa[indice];
        }

        // Biomedical images
        private void btnLaplacian_Click(object sender, EventArgs e)
        {
            theBitmapRight = laplacian(theBitmapLeft);
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private Bitmap laplacian(Bitmap source)
        {
            Bitmap newBitmap = new Bitmap(source.Width, source.Height);
            for (int i = 1; i < source.Width - 1; i++)
            {

                for (int j = 1; j < source.Height - 1; j++)
                {
                    int vr = LapR(i, j);
                    if (vr < 0)
                        vr = 0;
                    else if (vr > 255)
                        vr = 255;

                    int vg = LapG(i, j);
                    if (vg < 0)
                        vg = 0;
                    else if (vg > 255)
                        vg = 255;

                    int vb = LapB(i, j);
                    if (vb < 0)
                        vb = 0;
                    else if (vb > 255)
                        vb = 255;

                    newBitmap.SetPixel(i, j, Color.FromArgb(255, vr, vg, vb));
                }
            }

            return newBitmap;
        }

        /*
         * -1 -1 -1
         * -1  9 -1
         * -1 -1 -1
         * */

        int LapR(int i, int j)
        {
            int s = 0;
            for (int ii = -1; ii <= 1; ii++)
                for (int jj = -1; jj <= 1; jj++)
                    if ((ii == 0) && (jj == 0))
                        s += 9 * theBitmapLeft.GetPixel(i + ii, j + jj).R;
                    else
                        s += (-1) * theBitmapLeft.GetPixel(i + ii, j + jj).R;

            return s;
        }

        int LapG(int i, int j)
        {
            int s = 0;
            for (int ii = -1; ii <= 1; ii++)
                for (int jj = -1; jj <= 1; jj++)
                    if ((ii == 0) && (jj == 0))
                        s += 9 * theBitmapLeft.GetPixel(i + ii, j + jj).G;
                    else
                        s += (-1) * theBitmapLeft.GetPixel(i + ii, j + jj).G;

            return s;
        }

        int LapB(int i, int j)
        {
            int s = 0;
            for (int ii = -1; ii <= 1; ii++)
                for (int jj = -1; jj <= 1; jj++)
                    if ((ii == 0) && (jj == 0))
                        s += 9 * theBitmapLeft.GetPixel(i + ii, j + jj).B;
                    else
                        s += (-1) * theBitmapLeft.GetPixel(i + ii, j + jj).B;

            return s;
        }

        /** Lab 4 */

        // Contour detection - Sobel Edge Detector

        public Bitmap FilterProcessImage(Bitmap image)
        {
            Bitmap ret = new Bitmap(image.Width, image.Height);
            /**
             * For each pixel on the image, both templates are applied to the 3X3 area around the pixel. 
             * The grayscale values of surrounding pixels are multiplied by corresponding matrix values and then summed together 
             */

            for (int i = 1; i < image.Width - 1; i++)
            {
                for (int j = 1; j < image.Height - 1; j++)
                {
                    Color cr = image.GetPixel(i + 1, j);
                    Color cl = image.GetPixel(i - 1, j);
                    Color cu = image.GetPixel(i, j - 1);
                    Color cd = image.GetPixel(i, j + 1);
                    Color cld = image.GetPixel(i - 1, j + 1);
                    Color clu = image.GetPixel(i - 1, j - 1);
                    Color crd = image.GetPixel(i + 1, j + 1);
                    Color cru = image.GetPixel(i + 1, j - 1);
                    int power = getMaxD(cr.R, cl.R, cu.R, cd.R, cld.R, clu.R, cru.R, crd.R);
                    if (power > 50)
                        ret.SetPixel(i, j, Color.Black);
                    else
                        ret.SetPixel(i, j, Color.White);
                }
            }
            return ret;
        }

        private int getD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd, int[,] matrix)
        {
            return Math.Abs(matrix[0, 0] * clu + matrix[0, 1] * cu + matrix[0, 2] * cru
               + matrix[1, 0] * cl + matrix[1, 2] * cr
                  + matrix[2, 0] * cld + matrix[2, 1] * cd + matrix[2, 2] * crd);
        }

        private int getMaxD(int cr, int cl, int cu, int cd, int cld, int clu, int cru, int crd)
        {
            int max = int.MinValue;
                                for (int i = 0; i < templates.Count; i++)
                                {
                                    int newVal = getD(cr, cl, cu, cd, cld, clu, cru, crd, templates[i]);
                                    if (newVal > max)
                                        max = newVal;
                                }
                                return max;
       }

       private List<int[,]> templates = new List<int[,]> 
                    {
                       new int[,] {{ -3, -3, 5 }, { -3, 0, 5 }, { -3, -3, 5 } },
                       new int[,] {{ -3, 5, 5 }, { -3, 0, 5 }, { -3, -3, -3 } },
                       new int[,] {{ 5, 5, 5 }, { -3, 0, -3 }, { -3, -3, -3 } },
                       new int[,] {{ 5, 5, -3 }, { 5, 0, -3 }, { -3, -3, -3 } },
                       new int[,] {{ 5, -3, -3 }, { 5, 0, -3 }, { 5, -3, -3 } },
                       new int[,] {{ -3, -3, -3 }, { 5, 0, -3 }, { 5, 5, -3 } },
                       new int[,] {{ -3, -3, -3 }, { -3, 0, -3 }, { 5, 5, 5 } },
                       new int[,] {{ -3, -3, -3 }, { -3, 0, 5 }, { -3, 5, 5 } }
                    };

       private Bitmap ConvertToGray(Bitmap source)
       {
           Bitmap newBitmap = new Bitmap(source.Width, source.Height);
           for (int x = 0; x < newBitmap.Width; x++)
           {
               for (int y = 0; y < newBitmap.Height; y++)
               {
                   Color c = source.GetPixel(x, y);
                   int m = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                   newBitmap.SetPixel(x, y, Color.FromArgb(m, m, m));
               }
           }
           return newBitmap;
       }
       private Bitmap binarize(Bitmap source)
       {
           Bitmap newBitmap = new Bitmap(source.Width, source.Height);
           byte R, G, B;
           Color pixelColor;

           source = ConvertToGray(source);
           int threshold = 0;

           for (int x = 0; x < source.Width; x++)
           {
               for (int y = 0; y < source.Height; y++)
               {
                   threshold = threshold + source.GetPixel(x, y).R;
               }
           }
           threshold = threshold / source.Width / source.Height;
           for (int x = 0; x < source.Width; x++)
           {

               for (int y = 0; y < source.Height; y++)
               {
                   pixelColor = source.GetPixel(x, y);
                   if (pixelColor.R < threshold)
                       R = 0;
                   else
                       R = 255;
                   if (pixelColor.G < threshold)
                       G = 0;
                   else
                       G = 255;
                   if (pixelColor.B < threshold)
                       B = 0;
                   else
                       B = 255;
                   newBitmap.SetPixel(x, y, Color.FromArgb((int)R, (int)G, (int)B));
               }
           }

           return newBitmap;
       }
       private Bitmap getContour(Bitmap source)
       {
           Bitmap newBitmap = new Bitmap(source);
           for (int x = 1; x < newBitmap.Width - 1; x++)
           {
               for (int y = 1; y < newBitmap.Height - 1; y++)
               {

                   int color = newBitmap.GetPixel(x, y).R;
                   if (color == 255)
                   {
                       if (isContour(x, y, newBitmap) != true)
                       {
                           newBitmap.SetPixel(x, y, Color.FromArgb(125, 125, 125));
                       }
                       else
                       {
                           newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                       }
                   }
                   else
                   {
                       newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                   }
               }
           }

           for (int x = 0; x < newBitmap.Width; x++)
           {
               for (int y = 0; y < newBitmap.Height; y++)
               {
                   if (newBitmap.GetPixel(x, y).R == 125)
                       newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
               }
           }

           return newBitmap;
       }

      Boolean isContour(int i, int j, Bitmap source)
      {
            Boolean isContour = false;
            for (int ii = -1; ii <= 1; ii++)
                for (int jj = -1; jj <= 1; jj++)
                    if ((source.GetPixel(i + ii, j + jj).R == 0))
                    {
                        if (!((ii == 0) && (jj == 0)))
                            isContour = true;
                    }
            return isContour;
      }
        private void btnCSScountour_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = binarize(theBitmapLeft);
                Bitmap aux = getContour(theBitmapRight);

                picRight.Image = aux;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        // Skeletonization
        // Using the AForge framework

        private void AFSkeleton()
        {
            // load the image
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);

        //    theBitmapRight = new Bitmap(theBitmapLeft);

            // Creating the filter
            AForge.Imaging.Filters.FiltersSequence filterSequence =
    new AForge.Imaging.Filters.FiltersSequence();
            // add 8 thinning filters with different structuring elements
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thinning));
            // create filter iterator for 10 iterations
            AForge.Imaging.Filters.FilterIterator filter =
                new AForge.Imaging.Filters.FilterIterator(filterSequence, 7);
            // apply the filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
            theBitmapRight = newImage;
            picRight.Image = newImage;
        }

        // Skeletonization - from scratch
        private void mySkeleton()
        {
            theBitmapRight = new Bitmap(theBitmapLeft);
            btnSave.Enabled = true;

            int w = theBitmapLeft.Width;
            int h = theBitmapLeft.Height;

            int[][] a = new int[w][];
            for (int x = 0; x < a.Length; x++)
            {
                a[x] = new int[h];
            }

            int[][] aux = new int[w][];
            for (int x = 0; x < a.Length; x++)
            {
                aux[x] = new int[h];
            }

            int[][] s = new int[w][];
            for (int x = 0; x < a.Length; x++)
            {
                s[x] = new int[h];
            }

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Color c = theBitmapRight.GetPixel(i, j);

                    int r = c.R;
                    int g = c.G;
                    int b = c.B;

                    int gray = (r + g + b) / 3;

                    if (gray >= 225)
                    {
                        a[i][j] = 0;
                    }
                    else
                    {
                        a[i][j] = 1;
                    }
                }
            }

            int k = 1;
            int st = 1;

            while (st == 1)
            {
                for (int i = 1; i < w - 1; i++)
                {
                    for (int j = 1; j < h - 1; j++)
                    {
                        aux[i][j] = a[i][j];
                    }
                }

                for (int i = 1; i < w - 1; i++)
                {
                    for (int j = 1; j < h - 1; j++)
                    {
                        if (k == aux[i][j])
                        {
                            a[i][j] = Math.Min(
                                    Math.Min(aux[i + 1][j], aux[i - 1][j]),
                                    Math.Min(aux[i][j + 1], aux[i][j - 1]))
                                    + 1;
                        }
                    }
                }

                int rp = 0;
                for (int i = 1; i < w - 1; i++)
                {
                    for (int j = 1; j < h - 1; j++)
                    {
                        if (aux[i][j] != a[i][j])
                        {
                            rp = 1;
                        }
                    }
                }

                if (rp == 0) st = 0;
                k++;
            }

            for (int i = 1; i < w - 1; i++)
            {
                for (int j = 1; j < h - 1; j++)
                {
                    if (a[i][j] >= a[i + 1][j] && a[i][j] >= a[i - 1][j]
                        && a[i][j] >= a[i][j - 1] && a[i][j] >= a[i][j + 1])
                    {
                        s[i][j] = 1;
                        theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 0, 0, 0));
                    }
                    else
                    {
                        s[i][j] = 0;
                        theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255, 255));
                    }
                }
            }

            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private void btnCSSskeleton_Click(object sender, EventArgs e)
        {

            // AFSkeleton();
            mySkeleton();
        }



        // Thinning

        private void AFThin()
        {
            // load the image
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            short[,] se = new short[,] {
                    { -1, -1, -1 },
                    {  1,  1,  0 },
                    { -1, -1, -1 }
                };
            // create filter
           // HitAndMiss filter = new HitAndMiss(se, HitAndMiss.Modes.Thinning);

            AForge.Imaging.Filters.FiltersSequence filterSequence =
    new AForge.Imaging.Filters.FiltersSequence();
            // add 8 thinning filters with different structuring elements
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } },
                HitAndMiss.Modes.Thinning));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thinning));
            // create filter iterator for 10 iterations
            AForge.Imaging.Filters.FilterIterator filter =
                new AForge.Imaging.Filters.FilterIterator(filterSequence, 10);

            // apply the filter
            filter.Apply(image);

            picRight.Image = image;
            picRight.Refresh();
        }

        private Bitmap myThin(Bitmap source)
        {
            Bitmap newBitmap = new Bitmap(source);
            Boolean isDone = false;

            while (!isDone)
            {
                isDone = true;
                for (int x = 1; x < newBitmap.Width - 1; x++)
                {
                    for (int y = 1; y < newBitmap.Height - 1; y++)
                    {

                        int color = newBitmap.GetPixel(x, y).R;
                        if (color == 255)
                        {
                            if (isRemovable(x, y, newBitmap))
                            {
                                isDone = false;
                                newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                            }
                            else
                            {
                                newBitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                            }
                        }
                        else
                        {
                            newBitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                        }
                    }
                }
            }
            return newBitmap;
        }

        Boolean isRemovable(int i, int j, Bitmap source)
        {
            int[] vecA = new int[9];
            Boolean ok = false;
            int Bp1 = 0;
            int Ap1 = 0;

            vecA[0] = source.GetPixel(i - 1, j + 1).R;
            vecA[1] = source.GetPixel(i - 1, j).R;
            vecA[2] = source.GetPixel(i - 1, j - 1).R;
            vecA[3] = source.GetPixel(i, j - 1).R;
            vecA[4] = source.GetPixel(i + 1, j - 1).R;
            vecA[5] = source.GetPixel(i + 1, j).R;
            vecA[6] = source.GetPixel(i + 1, j + 1).R;
            vecA[7] = source.GetPixel(i, j + 1).R;
            vecA[8] = source.GetPixel(i - 1, j + 1).R;

            for (int k = 0; k < 8; k++)
            {
                if ((vecA[k] == 255) && (vecA[k + 1] == 0))
                    Ap1++;
            }

            for (int ii = -1; ii <= 1; ii++)
            {
                for (int jj = -1; jj <= 1; jj++)
                {
                    if ((source.GetPixel(i + ii, j + jj).R == 255))
                        Bp1++;
                }
            }
            Bp1 = Bp1 - 1;


            if ((Bp1 >= 2) && (Bp1 < 6))
            {
                if (Ap1 == 1)
                    ok = true;
            }
            return ok;
        }
        private void btnCSSthinning_Click(object sender, EventArgs e)
        {
         //   AFThin();
            theBitmapRight = binarize(theBitmapLeft);
            Bitmap aux = myThin(theBitmapRight);

            picRight.Image = aux;
            picRight.Refresh();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        /** Lab 5 */

        private void btnGrayscale_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft.Width, theBitmapLeft.Height);
                btnSave.Enabled = true;

                Graphics g = Graphics.FromImage(theBitmapRight);

                // Create grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                    new float[][]
                    {
                         new float[] {.3f, .3f, .3f, 0, 0},
                         new float[] {.59f, .59f, .59f, 0, 0},
                         new float[] {.11f, .11f, .11f, 0, 0},
                         new float[] {0, 0, 0, 1, 0},
                         new float[] {0, 0, 0, 0, 1}
                    }
                    );
                //create some image attributes
                ImageAttributes attributes = new ImageAttributes();

                //set the color matrix attribute
                attributes.SetColorMatrix(colorMatrix);

                //draw the original image on the new image
                //using the grayscale color matrix
                g.DrawImage(theBitmapLeft, new Rectangle(0, 0, theBitmapLeft.Width, theBitmapLeft.Height),
                   0, 0, theBitmapLeft.Width, theBitmapLeft.Height, GraphicsUnit.Pixel, attributes);

                //dispose the Graphics object
                g.Dispose();
                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnBW_Click(object sender, EventArgs e)
        {
            // load the image
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            Threshold filter = new Threshold(100);
            // apply the filter
            filter.Apply(image);

            theBitmapRight = image;

            picRight.Image = image;
            picRight.Refresh();

        }

        /** Duality of morphological transformations can be proved by
         * erosion and dilation.
         * i.e. Erosion and dilation are duals of each other with respect to set complementation and reflection.
         * 
         * */


        private void btnErode_Click(object sender, EventArgs e)
        {
            if (this.rbBinary.Checked)
            {
                Bitmap image = theBitmapLeft.Clone(new Rectangle(0, 0, theBitmapLeft.Width, theBitmapLeft.Height), PixelFormat.Format8bppIndexed);
                // create filter
                BinaryErosion3x3 filter = new BinaryErosion3x3();
                // apply filter
                theBitmapRight = filter.Apply(image);
            }
            else
            {
                theBitmapRight = eroBitmap(theBitmapLeft);
            }

            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private void btnDilateBW_Click(object sender, EventArgs e)
        {
            /*
            Image<Gray, Byte> src = new Image<Gray, Byte>(file);
            Image<Gray, Byte> dst = new Image<Gray, Byte>(src.Width, src.Height);
            dst = src.Dilate(2);

            picRight.Image = dst.ToBitmap();
            picRight.Refresh();
             * 
             * 
             * */

            byte[,] se = new byte[3, 3];

     
            if (this.rbBinary.Checked)
            {
                Bitmap image = theBitmapLeft.Clone(new Rectangle(0, 0, theBitmapLeft.Width, theBitmapLeft.Height), PixelFormat.Format8bppIndexed);
                // create filter
                BinaryDilatation3x3 filter = new BinaryDilatation3x3();
                // apply filter
                theBitmapRight = filter.Apply(image);
            }
            else
            {
                theBitmapRight = dilBitmap(theBitmapLeft);
            }

            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private void btnOpening_Click(object sender, EventArgs e)
        {
            // Erode + Dilate
            // load the image
            if (this.rbBinary.Checked)
            {
                System.Drawing.Bitmap img = theBitmapLeft; // (Bitmap)Bitmap.FromFile(file);
                // format image
                // AForge.Imaging.Image.FormatImage(ref image);
                Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
                // create filter
                Opening filter = new Opening();
                // apply filter
                theBitmapRight =  filter.Apply(image);
            }
            else
            {
                Bitmap aux;
                aux = eroBitmap(theBitmapLeft);
                theBitmapRight = dilBitmap(aux);
            }
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private void btnClosing_Click(object sender, EventArgs e)
        {
            // Dilate + Erode
            // load the image
            if (this.rbBinary.Checked)
            {
                System.Drawing.Bitmap img = theBitmapLeft;  // (Bitmap)Bitmap.FromFile(file);
                // format image
                // AForge.Imaging.Image.FormatImage(ref image);
                Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
                // create filter
                Closing filter = new Closing();
                // apply filter
                theBitmapRight = filter.Apply(image);
            }
            else
            {
                Bitmap aux;
                aux = dilBitmap(theBitmapLeft);
                theBitmapRight = eroBitmap(aux);
            }
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private void AFThick()
        {
            // load the image
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);

            //    theBitmapRight = new Bitmap(theBitmapLeft);

            // Creating the filter
            AForge.Imaging.Filters.FiltersSequence filterSequence =
    new AForge.Imaging.Filters.FiltersSequence();
            // add 8 thinning filters with different structuring elements
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } },
                HitAndMiss.Modes.Thickening));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thickening));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } },
                HitAndMiss.Modes.Thickening));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } },
                HitAndMiss.Modes.Thickening));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } },
                HitAndMiss.Modes.Thickening));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } },
                HitAndMiss.Modes.Thickening));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } },
                HitAndMiss.Modes.Thickening));
            filterSequence.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thickening));
            // create filter iterator for 10 iterations
            AForge.Imaging.Filters.FilterIterator filter =
                new AForge.Imaging.Filters.FilterIterator(filterSequence, 100);
            // apply the filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
            picRight.Image = newImage;
        }

        private void btnThick_Click(object sender, EventArgs e)
        {
            
            // load the image
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            short[,] se = new short[,] {
                    { -1, -1, -1 },
                    {  1,  1,  0 },
                    { -1, -1, -1 }
                };
            // create filter
            HitAndMiss filter = new HitAndMiss(se, HitAndMiss.Modes.Thickening);
            // apply the filter
            filter.ApplyInPlace(image);

            picRight.Image = image;
            picRight.Refresh(); 

           // AFThick();
        }

        // Color transformation

        public void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        /** From Lecture 9, slide 12 */
        private void RGBtoXYZ(Color c/*, double X, double Y, double Z*/)
        {
            double r = c.R ;/// 255;
            double g = c.G ;/// 255;
            double b = c.B ;/// 255;

            Console.WriteLine("R = " + r + "; G = " + g + "; B = " + b);

            if ( r > 0.04045 ) r = Math.Pow((( r + 0.055 ) / 1.055 ), 2.4);
                    else r = r / 12.92;
            if ( g > 0.04045 ) g =  Math.Pow(( ( g + 0.055 ) / 1.055 ), 2.4);
                    else  g = g / 12.92;
            if ( b > 0.04045 ) b = Math.Pow(( ( b + 0.055 ) / 1.055 ) , 2.4);
                    else b = b / 12.92;

            r = r * 100;
            g = g * 100;
            b = b * 100;


            // Observer. = 2°, Illuminant = D65
            X = r * 0.4124 + g * 0.3576 + b * 0.1805;
            Y = r * 0.2126 + g * 0.7152 + b * 0.0722;
            Z = r * 0.0193 + g * 0.1192 + b * 0.9505;
        }

        private Color XYZtoRGB()
        {
            // Observer = 2°, Illuminant = D65)
            double var_X = X / 100;        
            double var_Y = Y / 100;       
            double var_Z = Z / 100;

            Console.WriteLine("X = " + var_X + "; Y = " + var_Y + "; Z = " + var_Z);

            int var_R = (int) (var_X *  3.2406 + var_Y * -1.5372 + var_Z * -0.4986);
            int var_G = (int) (var_X * -0.9689 + var_Y *  1.8758 + var_Z *  0.0415);
            int var_B = (int) (var_X *  0.0557 + var_Y * -0.2040 + var_Z *  1.0570);

            if ( var_R > 0.0031308 ) 
                var_R = (int) (1.055 * ( Math.Pow(var_R, ( 1 / 2.4 )) ) - 0.055);
              else var_R = (int) (12.92 * var_R);
            if ( var_G > 0.0031308 ) 
                var_G = (int) (1.055 * ( Math.Pow(var_G, ( 1 / 2.4 ))) - 0.055);
              else var_G = (int) (12.92 * var_G);
            if ( var_B > 0.0031308 ) 
                var_B = (int) (1.055 * ( Math.Pow(var_B, ( 1 / 2.4 ))) - 0.055);
              else var_B = (int) (12.92 * var_B);

            Color c = Color.FromArgb(var_R , var_G , var_B );

            return c;

        }

        private void XYZtoLAB(/*double X, double Y, double Z, double L, double a, double b*/)
        {
            double ref_X =  95.047;
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            // Observer= 2°, Illuminant= D65
            double var_X = X / ref_X;         
            double var_Y = Y / ref_Y;          
            double var_Z = Z / ref_Z;          

            if ( var_X > 0.008856 ) var_X = Math.Pow(var_X, 1/3);
                else var_X = ( 7.787 * var_X ) + (16 / 116);
            if ( var_Y > 0.008856 ) var_Y = Math.Pow(var_Y, 1/3);
                else var_Y = ( 7.787 * var_Y ) + (16 / 116);
            if (var_Z > 0.008856) var_Z = Math.Pow(var_Z, 1 / 3);
                else var_Z = (7.787 * var_Z) + (16 / 116);

            L = ( 116 * var_Y ) - 16;
            a = 500 * ( var_X - var_Y );
            b = 200 * ( var_Y - var_Z );
        }

        private void XYZtoLUV(/*double X, double Y, double Z, double L, double u, double v*/)
        {
            double var_U = ( 4 * X ) / ( X + ( 15 * Y ) + ( 3 * Z ) );
            double var_V = ( 9 * Y ) / ( X + ( 15 * Y ) + ( 3 * Z ) );

            double var_Y = Y / 100;

            if (var_Y > 0.008856) var_Y = Math.Pow(var_Y, 1 / 3);
                else var_Y = (7.787 * var_Y) + (16 / 116);

            // Observer= 2°, Illuminant= D65
            double ref_X =  95.047;     
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            double ref_U = ( 4 * ref_X ) / ( ref_X + ( 15 * ref_Y ) + ( 3 * ref_Z ) );
            double ref_V = ( 9 * ref_Y ) / ( ref_X + ( 15 * ref_Y ) + ( 3 * ref_Z ) );

            L = ( 116 * var_Y ) - 16;
            u = 13 * L * ( var_U - ref_U );
            v = 13 * L * ( var_V - ref_V );
        }

        private void LUVtoXYZ()
        {
            double var_Y = ( L + 16 ) / 116;
            if (Math.Pow(var_Y, 3) > 0.008856)
                var_Y = Math.Pow(var_Y, 3);
            else var_Y = (var_Y - 16 / 116) / 7.787;

            // Observer= 2°, Illuminant= D65
            double ref_X =  95.047 ;     
            double ref_Y = 100.000;
            double ref_Z = 108.883;

            double ref_U = (4 * ref_X) / (ref_X + (15 * ref_Y) + (3 * ref_Z));
            double ref_V = (9 * ref_Y) / (ref_X + (15 * ref_Y) + (3 * ref_Z));

            double var_U = u / (13 * L) + ref_U;
            double var_V = u / (13 * L) + ref_V;

            Y = var_Y * 100;
            X = -(9 * Y * var_U) / ((var_U - 4) * var_V - var_U * var_V);
            Z = (9 * Y - (15 * var_V * Y) - (var_V * X)) / (3 * var_V);
        }

        private void btnColorTransf_Click(object sender, EventArgs e)
        {
            // RGB to XYZ

            try
            {
               
                if ((txtR.Text.Equals("")) && (txtG.Text.Equals("")) && (txtB.Text.Equals("")))
                {
                    theBitmapRight = new Bitmap(theBitmapLeft);
                    btnSave.Enabled = true;

                    //   for (int i = 0; i < theBitmapLeft.Height; i++)
                    //       for (int j = 0; j < theBitmapLeft.Width; j++)
                    int i = 0;
                    int j = 0;
                    {
                        Color c = theBitmapLeft.GetPixel(j, i);
                        RGBtoXYZ(c);
                       // Console.WriteLine("X = " + X + "; Y = " + Y + "; Z = " + Z);
                        
                        //       XYZtoLAB();
                        //       Console.WriteLine("L = " + L + "; a = " + a + "; b = " + b);

                    }
                }

                else
                {
                    int R = int.Parse(txtR.Text);
                    int G = int.Parse(txtG.Text);
                    int B = int.Parse(txtB.Text);

                    Color c = Color.FromArgb(R,G,B);

                    RGBtoXYZ(c);
                }

                Console.WriteLine("X = " + X + "; Y = " + Y + "; Z = " + Z);
                txtX.Text = X.ToString();
                txtY.Text = Y.ToString();
                txtZ.Text = Z.ToString();
                
              //  picRight.Image = theBitmapRight;
              //  picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnXYZtoRGB_Click(object sender, EventArgs e)
        {
            X = Double.Parse(txtX.Text);
            Y = Double.Parse(txtY.Text);
            Z = Double.Parse(txtZ.Text);

            Color c = XYZtoRGB();

            txtR.Text = c.R.ToString();
            txtG.Text = c.G.ToString();
            txtB.Text = c.B.ToString();
        }

        private void btnXYZtoLuv_Click(object sender, EventArgs e)
        {
            X = Double.Parse(txtX.Text);
            Y = Double.Parse(txtY.Text);
            Z = Double.Parse(txtZ.Text);
            
            XYZtoLUV();
            Console.WriteLine("L = " + L + "; u = " + u + "; v = " + v);

            txtL.Text = L.ToString();
            txtU.Text = u.ToString();
            txtV.Text = v.ToString();
        }

        private void btnLuvtoXYZ_Click(object sender, EventArgs e)
        {
            int L = int.Parse(txtL.Text);
            int u = int.Parse(txtU.Text);
            int v = int.Parse(txtV.Text);


            LUVtoXYZ();

            Console.WriteLine("X = " + X + "; Y = " + Y + "; Z = " + Z);
            txtX.Text = X.ToString();
            txtY.Text = Y.ToString();
            txtZ.Text = Z.ToString();
        }

        private Bitmap dilBitmap(Bitmap inputBitmap)
        {
            Bitmap bit = new Bitmap(inputBitmap);
            int w = inputBitmap.Width;
            int h = inputBitmap.Height;

            for (int i = 1; i < w - 1; i++)
            {
                for (int j = 1; j < h - 1; j++)
                {
                    Color c;
                    int max = 0;

                    for (int ii = 0; ii < structuralElement.Length; ii++)
                    {
                        for (int jj = 0; jj < structuralElement.Length; jj++)
                        {
                            if (i - ii >= 0 && j - jj >= 0)
                            {
                                Color temp = inputBitmap.GetPixel(i - ii, j - jj);
                                if (max < temp.R + structuralElement[ii][jj].R)
                                {
                                    max = temp.R + structuralElement[ii][jj].R;
                                }
                            }
                        }
                    }

                    max = max % 255;
                 //   if (max > 255)
                 //       max = 255;

                    c = Color.FromArgb(max, max, max);

                    bit.SetPixel(i, j, c);
                }
            }

            return bit;
        }

        private Bitmap eroBitmap(Bitmap inputBitmap)
        {
            Bitmap bit = new Bitmap(inputBitmap);
            int w = inputBitmap.Width;
            int h = inputBitmap.Height;

            for (int i = 1; i < w - 1; i++)
            {
                for (int j = 1; j < h - 1; j++)
                {
                    Color c;
                    int min = int.MaxValue;

                    for (int ii = 0; ii < structuralElement.Length; ii++)
                    {
                        for (int jj = 0; jj < structuralElement.Length; jj++)
                        {
                            if (i - ii >= 0 && j - jj >= 0)
                            {
                                Color temp = inputBitmap.GetPixel(i - ii, j - jj);
                                if (min > temp.R - structuralElement[ii][jj].R)
                                {
                                    min = temp.R - structuralElement[ii][jj].R;
                                }
                            }
                        }
                    }

                    min = Math.Abs(min) % 255; 
                   // if (min < 0)
                   //     min = 0;

                    c = Color.FromArgb(min, min, min);

                    bit.SetPixel(i, j, c);
                }
            }

            return bit;
        }

        public Bitmap DilateC(Bitmap Image)
        {
            int Size = 5;
            System.Drawing.Bitmap TempBitmap = Image;
            System.Drawing.Bitmap NewBitmap = new System.Drawing.Bitmap(TempBitmap.Width, TempBitmap.Height);
            System.Drawing.Graphics NewGraphics = System.Drawing.Graphics.FromImage(NewBitmap);
            NewGraphics.DrawImage(TempBitmap, new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), System.Drawing.GraphicsUnit.Pixel);
            NewGraphics.Dispose();
            Random TempRandom = new Random();
            int ApetureMin = -(Size / 2);
            int ApetureMax = (Size / 2);
            for (int x = 0; x < NewBitmap.Width; ++x)
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    int RValue = 0;
                    int GValue = 0;
                    int BValue = 0;
                    for (int x2 = ApetureMin; x2 < ApetureMax; ++x2)
                    {
                        int TempX = x + x2;
                        if (TempX >= 0 && TempX < NewBitmap.Width)
                        {
                            for (int y2 = ApetureMin; y2 < ApetureMax; ++y2)
                            {
                                int TempY = y + y2;
                                if (TempY >= 0 && TempY < NewBitmap.Height)
                                {
                                    Color TempColor = TempBitmap.GetPixel(TempX, TempY);
                                    if (TempColor.R > RValue)
                                        RValue = TempColor.R;
                                    if (TempColor.G > GValue)
                                        GValue = TempColor.G;
                                    if (TempColor.B > BValue)
                                        BValue = TempColor.B;
                                }
                            }
                        }
                    }
                    Color TempPixel = Color.FromArgb(RValue, GValue, BValue);
                    NewBitmap.SetPixel(x, y, TempPixel);
                }
            }
            return NewBitmap;
        }

        public Bitmap ErodeC(Bitmap Image)
        {
            int Size = 5;
            System.Drawing.Bitmap TempBitmap = Image;
            System.Drawing.Bitmap NewBitmap = new System.Drawing.Bitmap(TempBitmap.Width, TempBitmap.Height);
            System.Drawing.Graphics NewGraphics = System.Drawing.Graphics.FromImage(NewBitmap);
            NewGraphics.DrawImage(TempBitmap, new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), System.Drawing.GraphicsUnit.Pixel);
            NewGraphics.Dispose();
            int r, g, b;
            for (int x = 0; x < NewBitmap.Width; ++x)
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    Color c = TempBitmap.GetPixel(x, y);
                    r = c.R;
                    g = c.G;
                    b = c.B;

                    //Erosion
                    for (int aa = -1; aa < 2; aa++)
                    {
                        for (int bb = -1; bb < 2; bb++)
                        {
                            try
                            {
                                Color col2 = TempBitmap.GetPixel(x + aa, y + bb);
                                r = Math.Min(r, col2.R);
                                g = Math.Min(g, col2.G);
                                b = Math.Min(b, col2.B);
                            }
                            catch
                            {
                            }
                        }
                    }

                    NewBitmap.SetPixel(x, y, Color.FromArgb(255, r, g, b));
                }
            }
            return NewBitmap;
        }

        private Bitmap morphGradient(Bitmap dilBitmap, Bitmap eroBitmap)
        {
            Bitmap bit = new Bitmap(theBitmapLeft);
            btnSave.Enabled = true;

            int w = theBitmapLeft.Width;
            int h = theBitmapLeft.Height;

            for (int i = 1; i < w - 1; i++)
            {
                for (int j = 1; j < h - 1; j++)
                {
                    Color d = dilBitmap.GetPixel(i, j);
                    Color e = eroBitmap.GetPixel(i, j);
                    Color o = bit.GetPixel(i, j);

                    int r = Math.Abs((d.R - e.R)) % 255;
                    int g = Math.Abs(d.G - e.G) % 255;
                    int b = Math.Abs(d.B - e.B) % 255;

                    bit.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            }

            return bit;
        }

        private void btnColorGradient_Click(object sender, EventArgs e)
        {
            theBitmapRight = morphGradient(DilateC(theBitmapLeft), ErodeC(theBitmapLeft));
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private void btnDilateColor_Click(object sender, EventArgs e)
        {
            theBitmapRight = DilateC(theBitmapLeft); 
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

        private void btnErodeColor_Click(object sender, EventArgs e)
        {
            theBitmapRight = ErodeC(theBitmapLeft);
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }



    }
}
