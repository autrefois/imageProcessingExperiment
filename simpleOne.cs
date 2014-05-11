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
        private Image theImage;
        private string file;
        
        private Bitmap theBitmapLeft;
        private Bitmap theBitmapRight;

        private Image redhot;
        private Bitmap bRedHot;
        public simpleOne()
        {
            InitializeComponent();
            btnSave.Enabled = false;
            btnProcess.Enabled = false;
            btnInverse.Enabled = false;
            btnExtract.Enabled = false;
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

            redhot = Image.FromFile("E:\\Facultate\\Anul IV\\Sem II\\Image Processing\\Sample Pics\\red_hot.jpg");
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

        // Contour detection - Sobel Edge detector

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
        private void btnCSSskeleton_Click(object sender, EventArgs e)
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


        // Thinning
        private void btnCSSthinning_Click(object sender, EventArgs e)
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

        private void btnErode_Click(object sender, EventArgs e)
        {
            // load the image
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            AForge.Imaging.Filters.Erosion filter = new AForge.Imaging.Filters.Erosion();
            // apply filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
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
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
            // format image
            // AForge.Imaging.Image.FormatImage(ref image);
            Bitmap image = img.Clone(new Rectangle(0, 0, img.Width, img.Height), PixelFormat.Format8bppIndexed);
            // create filter
            AForge.Imaging.Filters.Dilatation filter = new AForge.Imaging.Filters.Dilatation();
            // apply filter
            System.Drawing.Bitmap newImage = filter.Apply(image);
            picRight.Image = newImage;
            picRight.Refresh();
        }

        private void btnOpening_Click(object sender, EventArgs e)
        {
            // Erode + Dilate
            // load the image
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
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
            System.Drawing.Bitmap img = (Bitmap)Bitmap.FromFile(file);
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
        }

        // Color transformation

        private void btnColorTransf_Click(object sender, EventArgs e)
        {

        }

        private void btnBW_Click(object sender, EventArgs e)
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
            Threshold filter = new Threshold(100);
            // apply the filter
            filter.ApplyInPlace(image);

            picRight.Image = image;
            picRight.Refresh();
            
        }

    }
}
