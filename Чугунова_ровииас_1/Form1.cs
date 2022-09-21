using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Чугунова_ровииас_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            capture = new VideoCapture();
            
        }
        private Image<Bgr, byte> sourceImage;
        private void imageBox1_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла

            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;
                sourceImage = new Image<Bgr, byte>(fileName);

            }
            imageBox1.Image = sourceImage.Resize(300, 300, Inter.Linear);
            double cannyThreshold = (int)shit1.Value;
            double cannyThresholdLinking = (int)shit2.Value;
            imageBox2.Image = Filter(sourceImage, cannyThreshold, cannyThresholdLinking).Resize(300, 300, Inter.Linear); 



        }

        private void imageBox2_Click(object sender, EventArgs e)
        {

        }
        private VideoCapture capture;
        

        private void Camera()
        {
            // инициализация веб-камеры
            
            capture.ImageGrabbed += ProcessFrame;

            capture.Start();
            var frame = capture.QueryFrame();
            sourceImage = frame.ToImage<Bgr, byte>();
            imageBox1.Image = sourceImage.Resize(300, 300, Inter.Linear);
            double cannyThreshold = (double)shit1.Value;
            double cannyThresholdLinking = (double)shit2.Value;
            imageBox2.Image = Filter(sourceImage, cannyThreshold, cannyThresholdLinking).Resize(300, 300, Inter.Linear);
            capture.Stop();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if( button2.Text == "остановить")
            {
                button2.Text = "Камера";
                timer1.Enabled = false;
            }
            else
            {
                button2.Text = "Остановить";
                timer1.Enabled = true;
            }
        }
        private void InitializwTimer()
        {

            capture = new VideoCapture();
            timer1.Interval = 100;
            timer1.Tick += new EventHandler(timer1_Tick);

            timer1.Enabled = true;

            button2.Text = "Остановить";
            button2.Click += new EventHandler(button2_Click);
        }
        // захват кадра из видеопотока
        private void ProcessFrame(object sender, EventArgs e)
        {
            var frame = new Mat();
            capture.Retrieve(frame); // получение текущего кадра
            Image<Bgr, byte> sourceImage = frame.ToImage<Bgr, byte>();
           

        }
        private void ProcessFrameVideo(object sender, EventArgs e)
        {
            var frame = new Mat();
            capture.Retrieve(frame); // получение текущего кадра
            var picture = frame.ToImage<Bgr, byte>();
            var picture2 = frame.ToImage<Bgr, byte>();
            imageBox1.Image = picture.Resize(640, 480, Inter.Linear);
            double cannyThreshold = (int)shit1.Value;
            double cannyThresholdLinking = (int)shit2.Value;
            imageBox2.Image = Filter(picture2, cannyThreshold, cannyThresholdLinking).Resize(640, 480, Inter.Linear);
            //imageBox1.Image = frame.ToImage<Bgr, byte>();
            //imageBox2.Image = frame.ToImage<Gray,byte>();
            //Thread.Sleep((int)capture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files (*mp4, *.flv)| *.mp4;*.flv";
            var result = openFileDialog.ShowDialog(); // открытие диалога выбора файла
            if (result == DialogResult.OK) // открытие выбранного файла
            {
                string fileName = openFileDialog.FileName;
                capture = new VideoCapture(fileName);
                capture.ImageGrabbed += ProcessFrameVideo;
                capture.Start(); // начало обработки видеопотока
                

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Camera();
        }

       

        private static Image<Bgr, byte> Filter(Image<Bgr, byte> source, double Threshold, double ThresholdLinking)
        {
            
            Image<Gray, byte> grayImage = source.Convert<Gray, byte>();
            var tempImage = grayImage.PyrDown();// понижает качество
            var destImage = tempImage.PyrUp();// повышает качество
            
           
            Image<Gray, byte> cannyEdges = destImage.Canny(Threshold, ThresholdLinking);
            var cannyEdgesBgr = cannyEdges.Convert<Bgr, byte>();
            var resultImage = source.Sub(cannyEdgesBgr); // попиксельное вычитание
                                                         //обход по каналам
            for (int channel = 0; channel < resultImage.NumberOfChannels; channel++)
                for (int x = 0; x < resultImage.Width; x++)
                    for (int y = 0; y < resultImage.Height; y++) // обход по пискелям
                    {
                        // получение цвета пикселя
                        byte color = resultImage.Data[y, x, channel];
                        if (color <= 50)
                            color = 0;
                        else if (color <= 100)
                            color = 25;
                        else if (color <= 150)
                            color = 180;
                        else if (color <= 200)
                            color = 210;
                        else
                            color = 255;
                        resultImage.Data[y, x, channel] = color; // изменение цвета пикселя
                    }
            return resultImage;
        }

        private void shit1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
