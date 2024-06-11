using System;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Diagnostics;


namespace OOPiP2_6_7
{

    /// <summary>
    /// Логика взаимодействия с пользователем
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString;
        SqlDataAdapter adapter;
        DataTable medicineTable;
        public MainWindow()
        {
            InitializeComponent();
            connectionString = ConfigurationManager.ConnectionStrings["OOPiP2_6_7.Properties.Settings.PharmacyConnectionString"].ConnectionString;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string sql = "SELECT * FROM Medicine";
            medicineTable = new DataTable();
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                SqlCommand command = new SqlCommand(sql, connection);
                adapter = new SqlDataAdapter(command);
                // установка команды на добавление для вызова хранимой процедуры
                adapter.InsertCommand = new SqlCommand("sp_InsertMedicine", connection);
                adapter.InsertCommand.CommandType = CommandType.StoredProcedure;
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@medicinename", SqlDbType.VarChar, 50, "MedicineName"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@medicineform", SqlDbType.VarChar, 30, "MedicineForm"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@quantity", SqlDbType.Int, 0, "Quantity"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@unitofmeasure", SqlDbType.VarChar, 10, "UnitOfMeasure"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@dosage", SqlDbType.VarChar, 10, "Dosage"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@price", SqlDbType.Money, 0, "Price"));
                adapter.InsertCommand.Parameters.Add(new SqlParameter("@manufacturer", SqlDbType.VarChar, 50, "Manufacturer"));

                connection.Open();
                adapter.Fill(medicineTable);
                medicinesGrid.ItemsSource = medicineTable.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }
        private void UpdateDB()
        {
            foreach (DataRow row in medicineTable.Rows)
            {
                string medicineForm = row["MedicineForm"].ToString();
                // Добавьте вашу логику проверки медицинских форм здесь
                if (medicineForm != "Таблетка" && medicineForm != "Капсула" && medicineForm != "Порошок")
                {
                    MessageBox.Show("Недопустимая медицинская форма: " + medicineForm);
                    return; // Остановить обновление базы данных
                }
            }
            SqlCommandBuilder comandbuilder = new SqlCommandBuilder(adapter);
            adapter.UpdateCommand = comandbuilder.GetUpdateCommand();
            adapter.Update(medicineTable);
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateDB();
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (medicinesGrid.SelectedItems != null)
            {
                for (int i = 0; i < medicinesGrid.SelectedItems.Count; i++)
                {
                    DataRowView datarowView = medicinesGrid.SelectedItems[i] as DataRowView;
                    if (datarowView != null)
                    {
                        DataRow dataRow = (DataRow)datarowView.Row;
                        dataRow.Delete();
                    }
                }
            }
            UpdateDB();
        }
    }
}
