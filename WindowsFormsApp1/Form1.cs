using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private MySqlConnection connection;
        private string connectionString = "server=localhost;user=root;database=computeraccessoriesdb;port=3306;password=Jkweko90-";

        public Form1()
        {
            InitializeComponent();
            connection = new MySqlConnection(connectionString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxTables.Items.AddRange(new string[] { "GraphicAdapters", "CdDevices", "HardDrives" });
            comboBoxTables.SelectedIndex = 0;
            LoadData(comboBoxTables.SelectedItem.ToString());
        }

        private void LoadData(string tableName)
        {
            string query = $"SELECT * FROM {tableName}";
            MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
            DataTable table = new DataTable();
            adapter.Fill(table);
            dataGridView.DataSource = table;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EditRecord(comboBoxTables.SelectedItem.ToString());
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells[0].Value);
                EditRecord(comboBoxTables.SelectedItem.ToString(), id);
            }
        }

        private void EditRecord(string tableName, int? id = null)
        {
            string type = "", manufacturer = "", capacity = "", frequency = "";
            int memory = 0;

            if (id.HasValue)
            {
                string query = $"SELECT * FROM {tableName} WHERE id = {id}";
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    type = reader["type"].ToString();
                    manufacturer = reader["manufacturer"].ToString();
                    if (tableName == "GraphicAdapters")
                        memory = Convert.ToInt32(reader["memory"]);
                    else if (tableName == "HardDrives")
                        capacity = reader["capacity"].ToString();
                    else if (tableName == "CdDevices")
                        frequency = reader["frequency"].ToString();
                }
                connection.Close();
            }

            Form editForm = new Form();
            TextBox txtType = new TextBox() { Text = type, Top = 10, Left = 100, Width = 200 };
            TextBox txtManufacturer = new TextBox() { Text = manufacturer, Top = 40, Left = 100, Width = 200 };
            TextBox txtMemory = null;
            TextBox txtCapacity = null;
            TextBox txtFrequency = null;

            if (tableName == "GraphicAdapters")
                txtMemory = new TextBox() { Text = memory.ToString(), Top = 70, Left = 100, Width = 200 };
            else if (tableName == "HardDrives")
                txtCapacity = new TextBox() { Text = capacity, Top = 70, Left = 100, Width = 200 };
            else if (tableName == "CdDevices")
                txtFrequency = new TextBox() { Text = frequency, Top = 70, Left = 100, Width = 200 };

            Button btnSave = new Button() { Text = "Save", Top = 100, Left = 100 };
            btnSave.Click += (s, ev) =>
            {
                type = txtType.Text;
                manufacturer = txtManufacturer.Text;
                if (tableName == "GraphicAdapters")
                    memory = Convert.ToInt32(txtMemory.Text);
                else if (tableName == "HardDrives")
                    capacity = txtCapacity.Text;
                else if (tableName == "CdDevices")
                    frequency = txtFrequency.Text;

                string query;
                if (id.HasValue)
                {
                    if (tableName == "GraphicAdapters")
                        query = $"UPDATE {tableName} SET type = '{type}', manufacturer = '{manufacturer}', memory = {memory} WHERE id = {id}";
                    else if (tableName == "HardDrives")
                        query = $"UPDATE {tableName} SET type = '{type}', manufacturer = '{manufacturer}', capacity = '{capacity}' WHERE id = {id}";
                    else if (tableName == "CdDevices")
                        query = $"UPDATE {tableName} SET type = '{type}', manufacturer = '{manufacturer}', frequency = '{frequency}' WHERE id = {id}";
                    else
                        query = $"UPDATE {tableName} SET type = '{type}', manufacturer = '{manufacturer}' WHERE id = {id}";
                }
                else
                {
                    if (tableName == "GraphicAdapters")
                        query = $"INSERT INTO {tableName} (type, manufacturer, memory) VALUES ('{type}', '{manufacturer}', {memory})";
                    else if (tableName == "HardDrives")
                        query = $"INSERT INTO {tableName} (type, manufacturer, capacity) VALUES ('{type}', '{manufacturer}', '{capacity}')";
                    else if (tableName == "CdDevices")
                        query = $"INSERT INTO {tableName} (type, manufacturer, frequency) VALUES ('{type}', '{manufacturer}', '{frequency}')";
                    else
                        query = $"INSERT INTO {tableName} (type, manufacturer) VALUES ('{type}', '{manufacturer}')";
                }

                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
                editForm.Close();
                LoadData(tableName);
            };

            editForm.Controls.Add(new Label() { Text = "Type", Top = 10, Left = 10 });
            editForm.Controls.Add(txtType);
            editForm.Controls.Add(new Label() { Text = "Manufacturer", Top = 40, Left = 10 });
            editForm.Controls.Add(txtManufacturer);
            if (tableName == "GraphicAdapters")
            {
                editForm.Controls.Add(new Label() { Text = "Memory", Top = 70, Left = 10 });
                editForm.Controls.Add(txtMemory);
            }
            else if (tableName == "HardDrives")
            {
                editForm.Controls.Add(new Label() { Text = "Capacity", Top = 70, Left = 10 });
                editForm.Controls.Add(txtCapacity);
            }
            else if (tableName == "CdDevices")
            {
                editForm.Controls.Add(new Label() { Text = "Frequency", Top = 70, Left = 10 });
                editForm.Controls.Add(txtFrequency);
            }
            editForm.Controls.Add(btnSave);
            editForm.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView.SelectedRows[0].Cells[0].Value);
                string tableName = comboBoxTables.SelectedItem.ToString();
                DeleteRecord(id, tableName);
            }
        }

        private void DeleteRecord(int id, string tableName)
        {
            string query = $"DELETE FROM {tableName} WHERE id = {id}";
            MySqlCommand command = new MySqlCommand(query, connection);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            LoadData(tableName);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string tableName = comboBoxTables.SelectedItem.ToString();
            Search(tableName);
        }

        private void Search(string tableName)
        {
            Form searchForm = new Form();
            TextBox txtType = new TextBox() { Top = 10, Left = 100, Width = 200 };
            TextBox txtManufacturer = new TextBox() { Top = 40, Left = 100, Width = 200 };
            TextBox txtCapacity = null; 
            TextBox txtFrequency = null; 

            Button btnSearch = new Button() { Text = "Search", Top = 70, Left = 100 };
            btnSearch.Click += (s, ev) =>
            {
                string type = txtType.Text;
                string manufacturer = txtManufacturer.Text;
                string capacity = txtCapacity?.Text ?? ""; 
                string frequency = txtFrequency?.Text ?? ""; 

                string query = $"SELECT * FROM {tableName} WHERE type LIKE '%{type}%' AND manufacturer LIKE '%{manufacturer}%'";

                if (tableName == "HardDrives")
                {
                    query += $" AND capacity LIKE '%{capacity}%'";
                }
                else if (tableName == "CdDevices")
                {
                    query += $" AND frequency LIKE '%{frequency}%'";
                }

                MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView.DataSource = table;
                searchForm.Close();
            };

            searchForm.Controls.Add(new Label() { Text = "Type", Top = 10, Left = 10 });
            searchForm.Controls.Add(txtType);
            searchForm.Controls.Add(new Label() { Text = "Manufacturer", Top = 40, Left = 10 });
            searchForm.Controls.Add(txtManufacturer);

            if (tableName == "HardDrives")
            {
                txtCapacity = new TextBox() { Top = 70, Left = 100, Width = 200 };
                searchForm.Controls.Add(new Label() { Text = "Capacity", Top = 70, Left = 10 });
                searchForm.Controls.Add(txtCapacity);
            }
            else if (tableName == "CdDevices")
            {
                txtFrequency = new TextBox() { Top = 70, Left = 100, Width = 200 };
                searchForm.Controls.Add(new Label() { Text = "Frequency", Top = 70, Left = 10 });
                searchForm.Controls.Add(txtFrequency);
            }

            searchForm.Controls.Add(btnSearch);
            searchForm.ShowDialog();
        }

        private void comboBoxTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tableName = comboBoxTables.SelectedItem.ToString();
            LoadData(tableName);
        }
    }
}
