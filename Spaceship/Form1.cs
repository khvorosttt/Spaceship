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
using System.Windows.Forms.DataVisualization.Charting;

namespace Spaceship
{
    public partial class Form1 : Form
    {
        Chart chart;
        double M = 6*Math.Pow(10,24);
        double dt = 0.01;
        double gamma = 6.672 * Math.Pow(10, -11);
        double t_end;

        private void In(out double[] x, out double[] y, out double[] vx, out double[] vy, out double[] r, out double[] t, out double t_end)
        {
            StreamReader fileIn = new StreamReader("Inlet2.in");
            string[] data = fileIn.ReadLine().Trim().Split(' ');
            t_end = double.Parse(data[0]);
            int N = (int)(t_end / dt);
            x = new double[N + 1];
            y = new double[N + 1];
            vx = new double[N + 1];
            vy = new double[N + 1];
            r = new double[N + 1];
            t = new double[N + 1];
            x[0] = 0;
            y[0] = 6.4 * Math.Pow(10, 6);
            r[0] = Math.Sqrt(Math.Pow(x[0], 2) + Math.Pow(y[0], 2));
            vx[0] = double.Parse(data[1]);
            vy[0] = double.Parse(data[2]);
            fileIn.Close();
        }

        public void Calc(ref double[] x, ref double[] y, ref double[] vx, ref double[] vy, ref double[] r, ref double[] t)
        {
            for(int i = 0; i < x.Length-1; i++)
            {
                t[i + 1] = t[i] + dt;
                vx[i + 1] = vx[i] - (gamma * M * x[i] * dt) / Math.Pow(r[i], 3);
                vy[i + 1] = vy[i] - (gamma * M * y[i] * dt) / Math.Pow(r[i], 3);
                x[i + 1] = x[i] + vx[i] * dt;
                y[i + 1] = y[i] + vy[i] * dt;                
                r[i+1]= Math.Sqrt(x[i+1]*x[i+1] +y[i+1]*y[i+1]);
            }
        }

        private void Out(double[] x, double[] y, double[] vx, double[] vy, double[] r, double[] t)
        {
            StreamWriter fileOut = new StreamWriter("Outlet.out");
            fileOut.WriteLine("t x y vx vy r");
            for (int i = 0; i < x.Length; i++)
            {
                fileOut.WriteLine(t[i] + " " + x[i] + " " + y[i] + " " + vx[i] + " " + vy[i] + " " + r[i]);
            }
            fileOut.Close();
        }

        private void CreateChart(double[] y)
        {
            chart = new Chart();
            chart.Parent = this;
            chart.SetBounds(10, 10, ClientSize.Width - 20,
                ClientSize.Height - 20);
            ChartArea area = new ChartArea();
            area.Name = "Spaceship";
            double min, max;
            MM(y, out min, out max);
            area.AxisY.Minimum = min;
            area.AxisY.Maximum = max;
            chart.ChartAreas.Add(area);
            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Spline;
            series1.BorderWidth = 1;
            series1.LegendText = "y";
            chart.Series.Add(series1);            
            Legend legend = new Legend();
            chart.Legends.Add(legend);
        }

        public void MM(double[] y, out double min, out double max)
        {
            min = double.MaxValue;
            max = double.MinValue;
            for(int i = 0; i < y.Length; i++)
            {
                if (y[i] < min)
                {
                    min = y[i];
                }
                if (y[i] > max)
                {
                    max = y[i];
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            double[] x;
            double[] y;
            double[] vx;
            double[] vy;
            double[] r;
            double[] t;
            In(out x, out y, out vx, out vy, out r, out t, out t_end);
            Calc(ref x, ref y, ref vx, ref vy, ref r, ref t);
            Out(x, y, vx, vy, r, t);
            CreateChart(y);
            DataPoint[] dp = new DataPoint[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                string info ="t = "+t[i]+"\n"+"x = " + x[i] + "\n" + "y = " + y[i] + "\n" + "vx = " + vx[i] + "\n" + "vy =" + vy[i]+"\n"+"r = "+r[i];
                dp[i] = new DataPoint(x[i], y[i]);
                dp[i].MarkerStyle = MarkerStyle.Circle;
                dp[i].ToolTip = info;         
                chart.Series[0].Points.Add(dp[i]);
            }
        }
    }
}
