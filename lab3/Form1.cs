using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using org.mariuszgromada.math.mxparser;
using org.mariuszgromada.math.mxparser.mathcollection;
using org.mariuszgromada.math.mxparser.parsertokens;
using org.mariuszgromada.math.mxparser.syntaxchecker;

namespace lab3 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e) {
            if (tbInput.Text == string.Empty || tbRightBorder.Text == string.Empty || tbLeftBorder.Text == string.Empty) {
                MessageBox.Show("Пожалуйста, введите данные");
                return;
            }
            chart1.ChartAreas[0].AxisX.Minimum = Convert.ToInt32(tbLeftBorder.Text);
            chart1.ChartAreas[0].AxisX.Maximum = Convert.ToInt32(tbRightBorder.Text);

            DrawChart();
            DrawGolden();
        }

        #region Ньютони
        private double e = 0.01;
        private double gLeftBorder;
        private double gRightBorder;
        private double k2;
        private double k1;
        private double x1;
        private double x2;
        private double F1;
        private double F2;
        
        private double derivateRigthBorder;
        private double derivateLeftBorder;
        private double GoldenNewton() {
            k2 = 0; k1 = 0; x1 = 0; x2 = 0; F1 = 0; F2 = 0; gX = 0; gY = 0;
            gLeftBorder = Convert.ToDouble(tbLeftBorder.Text);
            gRightBorder = Convert.ToDouble(tbRightBorder.Text);

            k2 = ( Math.Sqrt(5) - 1 ) / 2;
            k1 = 1 - k2;

            x1 = gLeftBorder + k1 * ( gRightBorder - gLeftBorder );
            x2 = gLeftBorder + k2 * ( gRightBorder - gLeftBorder );

            F1 = parseMath(x1);
            F2 = parseMath(x2);

            while(true) {
                derivateLeftBorder = parseDerivate(gLeftBorder);
                derivateRigthBorder = parseDerivate(gRightBorder);

                if(derivateRigthBorder <= -200) {
                    flag = true;
                    return derivateRigthBorder;
                }
                for (double i = gLeftBorder; i < gLeftBorder + 3; i += 0.1) {
                    if(parseDerivate(i) <= -200) {
                        flag = true;
                        return i;
                    }
                }
                //1
                if(Math.Abs(gRightBorder - gLeftBorder) < e) {
                    return ( gLeftBorder + gRightBorder ) / 2;
                }
                if(F1 < F2) {
                    gRightBorder = x2;
                    x2 = x1;
                    F2 = F1;

                    x1 = gLeftBorder + k1 * ( gRightBorder - gLeftBorder );
                    F1 = parseMath(x1);
                } else {
                    gLeftBorder = x1;
                    x1 = x2;
                    F1 = F2;

                    x2 = gLeftBorder + k2 * ( gRightBorder - gLeftBorder );
                    F2 = parseMath(x2);
                }
            }
        }
        #endregion

        #region строим ньютона
        private double gX;
        private double gY;
        private bool flag = false;
        private void DrawGolden() {
            GoldenNewton();
            this.chart1.Series[1].Points.Clear();
            gX = GoldenNewton();
            gY = parseMath(gX);
            chart1.ChartAreas[0].AxisY.Minimum = gY;

            this.chart1.Series[1].Points.AddXY(gX, gY);
            if (!flag) {
                MessageBox.Show($"Минимальная точка {gX}, {gY}");
            } else {
                MessageBox.Show($"Точка стремится к бесконечности в {gX}");
            }
        }
        #endregion

        #region построение графика

        private double leftBorder;
        private double rightBorder;
        private double step = 0.1;
        private double x;
        private double y = 0;

        private void DrawChart() {
            leftBorder = Convert.ToDouble(tbLeftBorder.Text);
            rightBorder = Convert.ToDouble(tbRightBorder.Text);//Правая граница
            x = leftBorder;

            //Очистка графика
            this.chart1.Series[0].Points.Clear();

            while (x <= rightBorder) {
                y = parseMath(x);
                if (double.IsNaN(y)) {
                    x++;
                    continue;
                }
                this.chart1.Series[0].Points.AddXY(x, y);
                x += step;
            }
        }
        #endregion

        #region парсинг
        private double parseMath(double point) {
            Argument x = new Argument("x");
            x.setArgumentValue(point);
            Expression expression = new Expression(tbInput.Text.ToString(), x);
            return expression.calculate();
        }
        #endregion

        #region Производная вычислять
        private double parseDerivate(double point) {
            Argument x = new Argument("x");
            x.setArgumentValue(point);
            string formula = tbInput.Text.ToString();
            string derivateFormula = "der(" + formula + ",x)";
            Expression expression = new Expression(derivateFormula, x);
            return expression.calculate();
        }

        #endregion
    }
}
