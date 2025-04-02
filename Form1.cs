using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace prKol_ind2._1_Shengals
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnProcessFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                MessageBox.Show("Пожалуйста, укажите путь к файлу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(txtFilePath.Text))
            {
                MessageBox.Show("Указанный файл не существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                ProcessEmployeeFile(txtFilePath.Text);
                MessageBox.Show("Файл успешно обработан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessEmployeeFile(string filePath)
        {
            List<Employee> employees = new List<Employee>();
            List<string> errors = new List<string>();
            int lineNumber = 0;

            foreach (string line in File.ReadLines(filePath))
            {
                lineNumber++;
                try
                {
                    string[] parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 6)
                    {
                        errors.Add($"Строка {lineNumber}: неверное количество элементов (ожидается 6, получено {parts.Length})");
                        continue;
                    }

                    var employee = new Employee
                    {
                        LastName = parts[0].Trim(),
                        FirstName = parts[1].Trim(),
                        MiddleName = parts[2].Trim(),
                        Gender = parts[3].Trim(),
                        Age = int.Parse(parts[4].Trim()),
                        Salary = decimal.Parse(parts[5].Trim())
                    };

                    employees.Add(employee);
                }
                catch (FormatException)
                {
                    errors.Add($"Строка {lineNumber}: неверный формат числа (возраст или зарплата)");
                }
                catch (Exception ex)
                {
                    errors.Add($"Строка {lineNumber}: {ex.Message}");
                }
            }

            if (errors.Any())
            {
                MessageBox.Show($"Обнаружены ошибки в файле:\n{string.Join("\n", errors.Take(5))}" +
                    (errors.Count > 5 ? "\n..." : ""), "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            var lowSalaryQueue = new Queue<Employee>(employees.Where(e => e.Salary < 10000));
            var otherSalaryQueue = new Queue<Employee>(employees.Where(e => e.Salary >= 10000));

            txtResult.Clear();
            txtResult.AppendText("=== Сотрудники с зарплатой < 10000 ===\n");
            PrintEmployees(lowSalaryQueue);

            txtResult.AppendText("\n=== Остальные сотрудники ===\n");
            PrintEmployees(otherSalaryQueue);
        }

        private void PrintEmployees(Queue<Employee> employees)
        {
            while (employees.Count > 0)
            {
                var employee = employees.Dequeue();
                txtResult.AppendText($"{employee.LastName} {employee.FirstName} {employee.MiddleName}, " +
                    $"{employee.Gender}, {employee.Age} лет, зарплата: {employee.Salary:C}\n");
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                }
            }
        }
    }
}
