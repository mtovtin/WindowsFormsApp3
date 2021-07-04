using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public string fileName = "";
        public string whereToWrite = "";
        public static string[,] matrix;
        public Form1()
        {
            InitializeComponent();
        }


        private (List<string[]>, bool) GetElements(string fileName = "")
        {
            var rows = new List<string[]>();
            bool validation = false;
            openFileDialog.InitialDirectory = @"C:\Users\Windows\Desktop";
            openFileDialog.FileName = fileName;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                validation = true;
                using (var reader = new StreamReader(openFileDialog.FileName))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var row = line.Split(new[] { ' ' });
                        if (rows.Count > 0 && row.Length != rows[0].Length)
                        {
                            MessageBox.Show($"Invalid Data", "Error");
                            validation = false;
                            break;
                        }

                        rows.Add(row);
                    }
                }
            }
            return (rows, validation);
        }


        private void FillGrid(List<string[]> rows, DataGridView grid)
        {
            grid.Rows.Clear();
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.DefaultCellStyle.BackColor = Color.White;
            }
            grid.ColumnCount = rows[0].Length;
            foreach (var row in rows)
            {
                grid.Rows.Add(row);
            }

        }
        private void FillMatrix(DataGridView grid)
        {

            matrix = new string[grid.RowCount, grid.ColumnCount];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] =Convert.ToString(grid[j, i].Value);
                }
            }
        }
    

        private void buttonLoadElements_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Оберіть файл", "Information", MessageBoxButtons.OK);
            var (rows, validation) = GetElements("matrix.txt");
            fileName = "matrix.txt";
            if (validation && rows.Count > 0)
            {
                FillGrid(rows, dataGridViewMatrix);
                FillMatrix(dataGridViewMatrix);
                ButtonApplyChanges.Enabled = true;
                ButtonShowChart.Enabled = true;
                buttonWriteToFile.Enabled = true;
               
            }
            else if (rows.Count == 0)
            {
                MessageBox.Show($"Файл пустий", "Error");
            }
        }
        private bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void buttonApplyChanges_Click(object sender, EventArgs e)
        {
            DataGridView grid = dataGridViewMatrix;
            FillMatrix(grid);
            foreach (DataGridViewColumn col in grid.Columns)
            {
                col.DefaultCellStyle.BackColor = Color.Red;
            }
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (!CheckDate(matrix[j,i]))
                    {
                        grid.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        break;
                    }
                   
                }
            }
        }

  
        private int[] FindNotDate()
        {
            int[] res = new int[dataGridViewMatrix.ColumnCount];
            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                res[i] = 0;
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    matrix[j, i] = Convert.ToString(dataGridViewMatrix[i, j].Value);
                    if (!CheckDate(matrix[j, i]))
                    {
                        res[i]++;
                    }
                }
            }
            return res;
        }

        private void buttonShowChart_Click(object sender, EventArgs e)
        {
            var chartForm = new ChartMatrix(FindNotDate());
            chartForm.Show();
        }

        private void buttonWriteToFile_Click(object sender, EventArgs e)
        {
            saveFileDialog.InitialDirectory = @"C:\Users\Windows\Desktop";
            saveFileDialog.FileName = "";
            saveFileDialog.Filter = "Text Files|*.txt";
            saveFileDialog.DefaultExt = "txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (var writer=new StreamWriter(saveFileDialog.FileName))
                {
                    foreach(DataGridViewRow row in dataGridViewMatrix.Rows)
                    {
                        foreach(DataGridViewCell cell in row.Cells)
                        {
                
                                writer.Write(cell.Value.ToString() + "\t");
  
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

    }
}
