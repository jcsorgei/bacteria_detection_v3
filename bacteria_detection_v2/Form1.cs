using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bacteria_detection_v2
{
    public partial class Form1 : Form
    {
        public int imgCount = 1;

        public double ThreshMin = 180;
        public double ThreshMax = 255;
        public int ThreshMode=0;
        public Image <Gray, byte> _01Markers;
        public Image<Gray, byte> _02Markers;
        public Image<Gray, byte> _03Markers;
        public Image<Gray, byte> markers;

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            var img = new Bitmap(pictureBox1.Image)
                .ToImage<Bgr, byte>();
            if (ThreshMode==0) {
                markers = new Bitmap(pictureBox1.Image)
                .ToImage<Gray, byte>()
                .ThresholdBinary(new Gray(ThreshMin), new Gray(255));
            }
            else if (ThreshMode==1) {
                markers = new Bitmap(pictureBox1.Image)
                .ToImage<Gray, byte>()
                .ThresholdBinaryInv(new Gray(ThreshMin), new Gray(255));
            }
            else  {
                markers = new Bitmap(pictureBox1.Image).ToImage<Gray, byte>();
                CvInvoke.Threshold(markers, markers, 0, 255,ThresholdType.Otsu);
            }
            
                
            
               
            _01Markers = markers;
            _01Markers.Save("01_markers.png");
            Mat kernel1 = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new Size(21, 21), new Point(10, 10));// csinálunk egy elipszist aminek a mérete 51 és a közepe 25 

            CvInvoke.MorphologyEx(markers, markers, MorphOp.Erode, kernel1, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(1.0)); // egy erode transzformációt csinálunk a grayscale képen, hogy eltüntessük a kisebb imperfekciókat a képen
            _02Markers = markers;
            _02Markers.Save("02_markers.png");
            /*Mat distanceTransofrm = new Mat();
            CvInvoke.DistanceTransform(mask2, distanceTransofrm, null, Emgu.CV.CvEnum.DistType.L2, 3);
            distanceTransofrm.Save("1distancet1.png");
            CvInvoke.Normalize(distanceTransofrm, distanceTransofrm, 0, 255, Emgu.CV.CvEnum.NormType.MinMax);
            distanceTransofrm.Save("2distancet2.png");*/
            /*var markers = distanceTransofrm.ToImage<Gray, byte>()
                .ThresholdBinary(new Gray(50), new Gray(255));
            markers.Save("3markers3.png");*/

            var labels = new Image<Gray, Int32>(markers.Size);
            //labels.Save("_04_labels.pgm");
            //img.Save("_04a_img.tif");
            //Mat plLabel = new Mat();
            markers.Data[10, 10,0] = 255; //berajzolunk egy újabb elemet a képre, hogy a watershed ezt az elemet nyújtsa ki a háttérnek
            CvInvoke.ConnectedComponents(markers, labels);
            //plLabel.ConvertTo(labels, Emgu.CV.CvEnum.DepthType.Cv32F);
            //Image<Gray, byte> finalMarkers = labels.Convert<Gray, byte>();
            //markers.Save("4finalmarkers4.png");
            //labels.ThresholdBinary(new Gray(1), new Gray(255));
            //labels.Convert<Gray, Int32>();

            CvInvoke.Watershed(img, labels);

            Image<Gray, Byte> umat = labels.Convert<Gray, Byte>();


            _03Markers = umat;
            pictureBox2.Image = umat.ToBitmap();

            /* Image<Gray, byte> boundaries = labels.Convert<byte>(delegate (Int32 x)
             {
                 return (byte)(x == -1 ? 255 : 0);
             });
             boundaries.Save("6boundaries6.png");


             boundaries._Dilate(1);
             boundaries.Save("7boundaries7.png");
             img.SetValue(new Bgr(0, 0, 255), boundaries);
             pictureBox1.Image = img.ToBitmap();
             pictureBox2.Image = mask2.ToBitmap();
             //mask2.Save("./mask.png");
             img.Save("result.png");
             mask2.Save("mask.png");*/
        }

        private void watershedImage(Bitmap img)
        {
            var srcImg = img
                .ToImage<Bgr, byte>();
            if (ThreshMode == 0)
            {
                markers = img
                .ToImage<Gray, byte>()
                .ThresholdBinary(new Gray(ThreshMin), new Gray(255));
            }
            else if (ThreshMode == 1)
            {
                markers = img
                .ToImage<Gray, byte>()
                .ThresholdBinaryInv(new Gray(ThreshMin), new Gray(255));
            }
            else
            {
                markers = img.ToImage<Gray, byte>();
                CvInvoke.Threshold(markers, markers, 0, 255, ThresholdType.Otsu);
            }

            Directory.CreateDirectory($".\\{imgCount}");

            _01Markers = markers;
            _01Markers.Save($".\\{imgCount}\\01_markers.png");
            Mat kernel1 = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new Size(21, 21), new Point(10, 10));// csinálunk egy elipszist aminek a mérete 51 és a közepe 25 

            CvInvoke.MorphologyEx(markers, markers, MorphOp.Erode, kernel1, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(1.0)); // egy erode transzformációt csinálunk a grayscale képen, hogy eltüntessük a kisebb imperfekciókat a képen
            _02Markers = markers;
            _02Markers.Save($".\\{imgCount}\\02_markers.png");
            

            var labels = new Image<Gray, Int32>(markers.Size);
            
            markers.Data[10, 10, 0] = 255; //berajzolunk egy újabb elemet a képre, hogy a watershed ezt az elemet nyújtsa ki a háttérnek
            CvInvoke.ConnectedComponents(markers, labels);
            

            CvInvoke.Watershed(srcImg, labels);

            Image<Gray, Byte> umat = labels.Convert<Gray, Byte>();


            _03Markers = umat;
            _03Markers.Save($".\\{imgCount}\\03_markers.png");
            pictureBox2.Image = umat.ToBitmap();
            imgCount++;
        }

        private void watershedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            }
        }

        private void segmentationToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            ThreshMin = trackBar1.Value;
            button1_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _01Markers.Save("_01_mask_default.png");
            _02Markers.Save("_02_markers_morphologyex.png");
            _03Markers.Save("_03_segmented.png");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                ThreshMode = 0;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                button1_Click(sender, e);
                trackBar1.Enabled = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                ThreshMode = 1;
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                button1_Click(sender, e);
                trackBar1.Enabled = true;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                ThreshMode = 3;
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                button1_Click(sender, e);
                trackBar1.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(@"images");
            FileInfo[] images = d.GetFiles("*.png");

            foreach (var image in images)
            {
                var img = new Bitmap($".\\images\\{image.Name}");
                watershedImage(img);
            }
        }
    }
}
