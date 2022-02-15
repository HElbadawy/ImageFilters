using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ZGraphTools;

namespace ImageFilters
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string OpenedFilePath;
        byte[,] ImageMatrix;
        int N = 1;
        int T = 1;
        int filter = 0;
        int sort = 0;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
        }

        private void btnZGraph_Click(object sender, EventArgs e)
        {
            int N = 20;
            double[] x_values = new double[N/2];
            double[] y_values1 = new double[N/2];
            double[] y_values2 = new double[N/2];

            ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);

            for (int i = 1; i < N; i += 2)
            {
                int Start = System.Environment.TickCount;
                ImageOperations.AlphaTrim(ImageMatrix, i, 0, 0);
                int End = System.Environment.TickCount;
                double Time = End - Start;
                Time /= 1000;
                x_values[i / 2] = i;
                y_values1[i / 2] = Time;
            }

            ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);

            for (int i = 1; i < N; i += 2)
            {
                int Start = System.Environment.TickCount;
                ImageOperations.AlphaTrim(ImageMatrix, i, 0, 1);
                int End = System.Environment.TickCount;
                double Time = End - Start;
                Time /= 1000;
                y_values2[i / 2] = Time;
            }

            //Create a graph and add two curves to it
            ZGraphForm alphaGraph = new ZGraphForm("Alpha Trim", "N", "Time");
            alphaGraph.add_curve("Alpha trim with counting sort", x_values, y_values1, Color.Blue);
            alphaGraph.add_curve("Alpha trim with heap sort", x_values, y_values2, Color.Red);
            alphaGraph.Show();

            for (int i = 1; i < N; i += 2)
            {
                int Start = System.Environment.TickCount;
                ImageOperations.AdaptiveMedianFilter(ImageMatrix, i, 0);
                int End = System.Environment.TickCount;
                double Time = End - Start;
                Time /= 1000;
                y_values1[i / 2] = Time;
            }

            ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);

            for (int i = 1; i < N; i += 2)
            {
                int Start = System.Environment.TickCount;
                ImageOperations.AdaptiveMedianFilter(ImageMatrix, i, 1);
                int End = System.Environment.TickCount;
                double Time = End - Start;
                Time /= 1000;
                y_values2[i/2] = Time;
            }

            ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);

            ZGraphForm MedianGraph = new ZGraphForm("Adaptive Median", "N", "Time");
            MedianGraph.add_curve("Adaptive median with counting sort", x_values, y_values1, Color.Blue);
            MedianGraph.add_curve("Adaptive median with quick sort", x_values, y_values2, Color.Red);
            MedianGraph.Show();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Visible = true;
            if (comboBox1.SelectedIndex == 0)
            {
                label1.Visible = true;
                numericUpDown1.Visible = true;
                label2.Visible = true;
                numericUpDown2.Visible = true;
                filter = 0;

                comboBox2.Items.Clear();

                comboBox2.Items.Add("Counting Sort");
                comboBox2.Items.Add("Heap Sort");
            }
            else {
                label1.Visible = true;
                numericUpDown1.Visible = true;
                label2.Visible = false;
                numericUpDown2.Visible = false;
                filter = 1;

                comboBox2.Items.Clear();

                comboBox2.Items.Add("Counting Sort");
                comboBox2.Items.Add("Quick Sort");
            }
        }

        private void Display()
        {
            if (filter == 0)
            {
                AlphaTrim();
            }
            else
            {
                AdaptiveMedian();
            }
        }

        private void AlphaTrim()
        {
            ImageOperations.DisplayImage(ImageOperations.AlphaTrim(ImageMatrix, N, T, sort), pictureBox2);
        }

        private void AdaptiveMedian()
        {
            ImageOperations.DisplayImage(ImageOperations.AdaptiveMedianFilter(ImageMatrix, N, sort), pictureBox2);
        }

        private void btnGen_Click(object sender, EventArgs e)
        {
            Display();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            N = (int)numericUpDown1.Value;
            Display();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            T = (int)numericUpDown2.Value;
            Display();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            sort = comboBox2.SelectedIndex;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}