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

            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

		int w = theBitmapLeft.Width;
		int h = theBitmapLeft.Height;

		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				Color c = theBitmapLeft.GetPixel(i, j);
				int m;

				int mc = (c.R + c.G + c.B) / 3;
				m = mc;
				if (mc <= 128 && mc >= 0)
					m = 0;
				if (mc > 128 & mc < 255)
					m = 255;
                theBitmapRight.SetPixel(i, j, Color.FromArgb(m, m, m));
			}
		}

        //int[] x_uri = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
        //int[] y_uri = new int[] { 1, 0, -1, -1, -1, 0, 1, 1 };


        //for (int i = 0; i < theBitmapRight.Width; i++)
        //{
        //    for (int j = 0; j < theBitmapRight.Height; j++)
        //    {

        //        int s = 0;
        //        Color c = theBitmapRight.GetPixel(i, j);
        //        int r = c.R;
        //        int g = c.G;
        //        int b = c.B;

        //        if (r != 0 || g != 0 || b != 0) {
        //            for (int k = 0; k < 8; k++) {
        //                if (i + x_uri[k] >= 0
        //                        && i + x_uri[k] < theBitmapRight.Width
        //                        && j + y_uri[k] >= 0
        //                        && j + y_uri[k] < theBitmapRight.Height)
        //                {
        //                    c = theBitmapRight.GetPixel(i + x_uri[k], j
        //                            + y_uri[k]);
        //                    int gri = c.R;
        //                    if (gri == 255) {
        //                        s++;
        //                    }
        //                }
        //            }
        //        }

        //        if (s == 0 || s == 1 || s == 7 || s == 8) {
        //            theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255));
        //        }

        //    }
        //} 

                picRight.Image = theBitmapRight;
                picRight.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void tb_howMuch_Scroll(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

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

    

        private void btnCSScountour_Click(object sender, EventArgs e)
        {
            try
            {
                theBitmapRight = new Bitmap(theBitmapLeft);
                btnSave.Enabled = true;

                theBitmapRight = FilterProcessImage(theBitmapLeft);


                picRight.Image = theBitmapRight;
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
                new AForge.Imaging.Filters.FilterIterator(filterSequence, 10);
            // apply the filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
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
            HitAndMiss filter = new HitAndMiss(se, HitAndMiss.Modes.Thinning);
            // apply the filter
            filter.ApplyInPlace(image);

            picRight.Image = image;
            picRight.Refresh();
        }

        private void myThin()
        {
            theBitmapRight = new Bitmap(theBitmapLeft);
            btnSave.Enabled = true;

            int[] x_uri = new int[] { -1, -1, -1, 0, 1, 1, 1, 0 };
            int[] y_uri = new int[] { 1, 0, -1, -1, -1, 0, 1, 1 };

            int w = theBitmapLeft.Width;
            int h = theBitmapLeft.Height;

            for (int i = 1; i < w - 1; i++)
            {
                for (int j = 1; j < h - 1; j++)
                {
                    int s = 0;
                    Color c = theBitmapRight.GetPixel(i, j);

                    int gray = c.R;

                    if (gray == 255)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            if (i + x_uri[k] >= 0 
                                && i + x_uri[k] < w
                                && j + y_uri[k] >= 0
                                && j + y_uri[k] < h)
                            {
                                c = theBitmapLeft.GetPixel(i + x_uri[k], j + y_uri[k]);
                                gray = (c.R);
                                if (gray == 255)
                                {
                                    s++;
                                }
                            }
                        }
                    }

                    if (s == 1)
                    {
                        c = theBitmapLeft.GetPixel(i, j - 1);
                        int westGray = c.R;
                        c = theBitmapLeft.GetPixel(i, j + 1);
                        int eastGray = c.R;
                        c = theBitmapLeft.GetPixel(i - 1, j);
                        int northGray = c.R;
                        c = theBitmapLeft.GetPixel(i + 1, j);
                        int southGray = c.R;

                        if (westGray == 255 || eastGray == 255 || northGray == 255 || southGray == 255)
                            theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255));

                    }

                    if (s == 2)
                    {
                        c = theBitmapLeft.GetPixel(i, j - 1);
                        int westGray = c.R;
                        c = theBitmapLeft.GetPixel(i, j + 1);
                        int eastGray = c.R;
                        c = theBitmapLeft.GetPixel(i - 1, j);
                        int northGray = c.R;
                        c = theBitmapLeft.GetPixel(i + 1, j);
                        int southGray = c.R;

                        c = theBitmapLeft.GetPixel(i + 1, j - 1);
                        int southWestGray = c.R;
                        c = theBitmapLeft.GetPixel(i + 1, j + 1);
                        int southEastGray = c.R;
                        c = theBitmapLeft.GetPixel(i - 1, j - 1);
                        int northWestGray = c.R;
                        c = theBitmapLeft.GetPixel(i - 1, j + 1);
                        int northEastGray = c.R;

                        if (northEastGray == 255 && northWestGray == 255)
                        {
                            theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                            theBitmapRight.SetPixel(i - 1, j, Color.FromArgb(0, 0, 0));
                        }

                        if (northWestGray == 255 && southWestGray == 255)
                        {
                            theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                            theBitmapRight.SetPixel(i, j - 1, Color.FromArgb(0, 0, 0));
                        }

                        if (southWestGray == 255 && southEastGray == 255)
                        {
                            theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                            theBitmapRight.SetPixel(i + 1, j, Color.FromArgb(0, 0, 0));
                        }

                        if (southEastGray == 255 && northEastGray == 255)
                        {
                            theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                            theBitmapRight.SetPixel(i, j + 1, Color.FromArgb(0, 0, 0));
                        }

                        if (eastGray == 255 && northGray == 255 || 
                            eastGray == 255 && southGray == 255 || 
                            northGray == 255 && westGray == 255 ||
                            westGray == 255 && southGray == 255)
                        {
                            theBitmapRight.SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        }
                    }
                }
            }

            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }
        private void btnCSSthinning_Click(object sender, EventArgs e)
        {
            // AFThin();
            myThin();
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
            // load the image
            System.Drawing.Bitmap img = theBitmapLeft; // (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            AForge.Imaging.Filters.Erosion filter = new AForge.Imaging.Filters.Erosion();
            // apply filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
            theBitmapRight = newImage;
            picRight.Image = newImage;
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

            // load the image
            System.Drawing.Bitmap img = theBitmapLeft; // (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            AForge.Imaging.Filters.Dilatation filter = new AForge.Imaging.Filters.Dilatation();
            // apply filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
            theBitmapRight = newImage;
            picRight.Image = newImage;
            picRight.Refresh();
        }

        private void btnOpening_Click(object sender, EventArgs e)
        {
            // Erode + Dilate
            // load the image
            System.Drawing.Bitmap img = theBitmapLeft; // (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            Opening filter = new Opening();
            // apply filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
            picRight.Image = newImage;
            picRight.Refresh();

        }

        private void btnClosing_Click(object sender, EventArgs e)
        {
            // Dilate + Erode
            // load the image
            System.Drawing.Bitmap img = theBitmapLeft;  // (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            Closing filter = new Closing();
            // apply filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
            picRight.Image = newImage;
            picRight.Refresh();
        }

        private void btnThick_Click(object sender, EventArgs e)
        {
            // load the image
            System.Drawing.Bitmap img = theBitmapLeft; // (Bitmap)Bitmap.FromFile(file);
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
                    if (max > 255)
                        max = 255;

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
                    if (min < 0)
                        min = 0;

                    c = Color.FromArgb(min, min, min);

                    bit.SetPixel(i, j, c);
                }
            }

            return bit;
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

                    int v = d.R - e.R;
                    if (v > 255) v = 255;
                    if (v < 0) v = 0;

                    bit.SetPixel(i, j, Color.FromArgb(v, v, v));
                }
            }

            return bit;
        }

        private void btnColorGradient_Click(object sender, EventArgs e)
        {
            theBitmapRight = morphGradient(dilBitmap(theBitmapLeft), eroBitmap(theBitmapLeft));
            picRight.Image = theBitmapRight;
            picRight.Refresh();
        }

    }
}
