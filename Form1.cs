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

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Выберите файл с данными сотрудников"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    List<Employee> employees = ReadEmployeesFromFile(openFileDialog.FileName);
                    DisplayEmployees(employees);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private List<Employee> ReadEmployeesFromFile(string filePath)
        {
            List<Employee> employees = new List<Employee>();

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден.");
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 6)
                {
                    throw new FormatException($"Некорректный формат строки: {line}. Ожидается 6 полей.");
                }

                if (!int.TryParse(parts[4], out int age))
                {
                    throw new FormatException($"Некорректный возраст в строке: {line}");
                }

                if (!decimal.TryParse(parts[5], out decimal salary))
                {
                    throw new FormatException($"Некорректная зарплата в строке: {line}");
                }

                employees.Add(new Employee
                {
                    LastName = parts[0].Trim(),
                    FirstName = parts[1].Trim(),
                    MiddleName = parts[2].Trim(),
                    Gender = parts[3].Trim(),
                    Age = age,
                    Salary = salary
                });
            }

            return employees;
        }

        private void DisplayEmployees(List<Employee> employees)
        {
            txtOutput.Clear();

            foreach (var emp in employees)
            {
                if (emp.Salary < 10000)
                {
                    txtOutput.AppendText(emp.ToString() + Environment.NewLine);
                }
            }
            foreach (var emp in employees)
            {
                if (emp.Salary >= 10000)
                {
                    txtOutput.AppendText(emp.ToString() + Environment.NewLine);
                }
            }
        }
    }
}
