using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Calculators
{
    public partial class Form1 : Form
    {
        string connectionString = "Data Source=DESKTOP-FS1G69B\\SQLEXPRESS;Initial Catalog=CalculatorDB;Integrated Security=True;Encrypt=False;";

        public Form1()
        {
            InitializeComponent();

            this.Click += (s, e) => History.Visible = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            History.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text += "1";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += "2";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += "3";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text += "4";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text += "5";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text += "6";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Text += "7";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Text += "8";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text += "9";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.Text += ".";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox1.Text += "0";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text += "00";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            PerformOperation("+");
        }

        private void button15_Click(object sender, EventArgs e)
        {
            PerformOperation("-");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            PerformOperation("x");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            PerformOperation("/");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void button18_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            CalculateResult();
        }

        private void PerformOperation(string operation)
        {
            if (!string.IsNullOrEmpty(textBox1.Text))
            {
                label2.Text = textBox1.Text;
                label1.Text = operation;
                textBox1.Text = "";
            }
        }

        private void CalculateResult()
        {
            if (!string.IsNullOrEmpty(label1.Text) && !string.IsNullOrEmpty(textBox1.Text))
            {
                double operand1 = Convert.ToDouble(label2.Text);
                double operand2 = Convert.ToDouble(textBox1.Text);
                double result = 0; 

                switch (label1.Text)
                {
                    case "+":
                        result = operand1 + operand2;
                        InsertData("Addition", operand1, operand2, result);
                        break;
                    case "-":
                        result = operand1 - operand2;
                        InsertData("Subtraction", operand1, operand2, result);
                        break;
                    case "x":
                        result = operand1 * operand2;
                        InsertData("Multiplication", operand1, operand2, result);
                        break;
                    case "/":
                        if (operand2 != 0)
                        {
                            result = operand1 / operand2;
                            InsertData("Division", operand1, operand2, result);
                        }
                        else
                        {
                            MessageBox.Show("Cannot divide by zero.");
                        }
                        break;
                }

                textBox1.Text = result.ToString();
                label1.Text = "=";
                label2.Text = "";
            }
        }


        private void ClearAll()
        {
            textBox1.Text = "";
            label1.Text = "";
            label2.Text = "";
        }

        private void InsertData(string operation, double operand1, double operand2, double result)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlInsert = $"INSERT INTO {operation}_table (Operand1, Operand2, Result) VALUES (@Operand1, @Operand2, @Result)";

                    using (SqlCommand command = new SqlCommand(sqlInsert, connection))
                    {
                        command.Parameters.AddWithValue("@Operand1", operand1);
                        command.Parameters.AddWithValue("@Operand2", operand2);
                        command.Parameters.AddWithValue("@Result", result);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inserting data into database: " + ex.Message);
            }
        }
        private void button22_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string[] tables = { "Addition_table", "Subtraction_table", "Multiplication_table", "Division_table" /* Add other table names as needed */ };

                    foreach (string table in tables)
                    {
                        string sqlDelete = $"TRUNCATE TABLE {table}";

                        using (SqlCommand command = new SqlCommand(sqlDelete, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("All records deleted successfully.");
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting data from database: " + ex.Message);
            }
        }


        private void button21_Click(object sender, EventArgs e)
        {
            try
            {
                History.Visible = !History.Visible;

                History.Items.Clear();

                if (History.Visible)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string[] tables = { "Addition_table", "Subtraction_table", "Multiplication_table", "Division_table" /* Add other table names as needed */ };

                        foreach (string table in tables)
                        {
                            string sqlSelect = $"SELECT * FROM {table}";

                            using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                            {
                                SqlDataReader reader = command.ExecuteReader();

                                while (reader.Read())
                                {
                                    double operand1 = Convert.ToDouble(reader["Operand1"]);
                                    double operand2 = Convert.ToDouble(reader["Operand2"]);
                                    double result = Convert.ToDouble(reader["Result"]);

                                    string operationSign = GetOperationSign(table);

                                    string historyItem = $"{operand1} {operationSign} {operand2} = {result}";
                                    History.Items.Add(historyItem);
                                }

                                reader.Close();
                            }
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data from database: " + ex.Message);
            }
        }

        private string GetOperationSign(string tableName)
        {
            switch (tableName)
            {
                case "Addition_table":
                    return "+";
                case "Subtraction_table":
                    return "-";
                case "Multiplication_table":
                    return "*";
                case "Division_table":
                    return "/";
                default:
                    return "?";
            }
        }



    }
}
