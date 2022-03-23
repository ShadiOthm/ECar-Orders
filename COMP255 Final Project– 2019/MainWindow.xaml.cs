using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace COMP255_Final_Project__2019
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SaveRecButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = Properties.Settings.Default.ECarOrdersConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                //IsDataValid function use
                //if (IsDataValidOrder() == true)
                  if ( true)
                    {
                        if (OrderNumberText.IsReadOnly)
                    {
                        command.CommandText = "UPDATE CarOrders set " +
                             " OrderDate=N'" + OrderDateText.Text.Trim() + "'" +
                             " ,CustomerName=N'" + CustomerNametext.Text.Trim() + "'" +
                             " ,CustomerEmail=N'" + CustomerEmailText.Text.Trim() + "'" +
                             " ,CustomerAddress=N'" + CustAddressText.Text.Trim() + "'" +
                             " ,InProduction=N'" + InProductionChickBox.IsChecked + "'" +
                            " ,DeliveryDate=N'" + DeliveryDateText.Text.Trim() + "'" +
                            " where OrderNumber=" + OrderNumberText.Text.Trim();
                    }
                    else // new
                    {

                        // need to select max to make a new primary key ....


                        command.CommandText = "insert into CarOrders(OrderDate , CustomerName, CustomerEmail,CustomerAddress,InProduction,DeliveryDate,OrderNumber) values ( " +
                         " N'" + OrderDateText.Text.Trim() + "'" +
                         " ,N'" + CustomerNametext.Text.Trim() + "'" +
                         " ,N'" + CustomerEmailText.Text.Trim() + "'" +
                         " ,N'" + CustAddressText.Text.Trim() + "'" +
                         " ,N'" + InProductionChickBox.IsChecked + "'" +
                        " ,N'" + DeliveryDateText.Text.Trim() + "'" +
                        " , " + OrderNumberText.Text.Trim() + ")";
                    }
                    int result = command.ExecuteNonQuery();

                    connection.Close();
                    ClearControls();
                    MessageBox.Show("Operation Completed Successfully!");

                    DeleteRecButton.Visibility = Visibility.Visible;
                    NewRecButton.Visibility = Visibility.Visible;
                    LoadCarOrders();
                }
                else
                {
                    ErrorMessageLabel.Content = "Invalid Data";
                }
                }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCarOrders();
        }

        private void LoadCarOrders()
        {
            CarOrdersList.SelectedItem = null;
            CarOrdersList.Items.Clear();
            string connectionString = Properties.Settings.Default.ECarOrdersConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "select * from CarOrders";


                DataTable dtCarOrders = new DataTable();

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dtCarOrders);
                CarOrder order;
                foreach (DataRow row in dtCarOrders.Rows)
                {
                    order = new CarOrder();
                    OrderNumberText.IsReadOnly = true;
                    order.CustomerAddress = row["CustomerAddress"].ToString();
                    order.CustomerEmail = row["CustomerEmail"].ToString();
                    order.CustomerName = row["CustomerName"].ToString();
                    order.DeliveryDate = DateTime.Parse(row["DeliveryDate"].ToString());
                    order.InProduction = bool.Parse(row["InProduction"].ToString());
                    order.OrderDate = DateTime.Parse(row["OrderDate"].ToString());
                    order.OrderNumber = int.Parse(row["OrderNumber"].ToString());
                    CarOrdersList.Items.Add(order);
                }

            }
        }

        private void CarOrdersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CarOrder currentOrder = (CarOrder)CarOrdersList.SelectedItem;

            DisplayCarOrderDetails(currentOrder);

            LoadCarOptions();
        }

        private void LoadCarOptions()
        {
            
            decimal SubTotal = 0;
            decimal PST = 0;
            decimal GST = 0;
            decimal Total = 0;
            CarOrderOptionsList.SelectedItem = null;
            CarOrderOptionsList.Items.Clear();
            string connectionString = Properties.Settings.Default.ECarOrdersConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "select * from CarOrderOptions where OrderNumber='"+OrderNumberText.Text.Trim()+"'";


                DataTable dtCarOrderOptions = new DataTable();

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                adapter.Fill(dtCarOrderOptions);
                CarOrderOption orderOption;
                CarOrderOptionsList.Items.Add($"{"OptionID",5}  {"Option Name",25}{"Option Description",50} {"Option Price",20}");
                CarOrderOptionsList.Items.Add("==========================================================");
                foreach (DataRow row in dtCarOrderOptions.Rows)
                {
                    
                    orderOption = new CarOrderOption();
                    OrderNumberText.IsReadOnly = true;
                    orderOption.OptionDescription = row["OptionDescription"].ToString();
                    orderOption.OrderNumber = int.Parse(row["OrderNumber"].ToString());
                    orderOption.OptionID = int.Parse(row["OptionID"].ToString());
                    orderOption.OptionName = row["OptionName"].ToString();
                    orderOption.OptionPrice = decimal.Parse(row["OptionPrice"].ToString());
                  
                    CarOrderOptionsList.Items.Add(orderOption);

                    SubTotal += decimal.Parse(row["OptionPrice"].ToString());
                }

                SubtotalText.Text = SubTotal.ToString("N2");
                GST = SubTotal * 5 / 100;
                PST = SubTotal * 6 / 100;

                PSTText.Text = PST.ToString("N2");
                GSTText.Text = GST.ToString("N2");
                Total = SubTotal + GST + PST;

                OrderTotalText.Text = Total.ToString("N2");


            }
        }

        private void DisplayCarOrderDetails(CarOrder currentOrder)
        {
            if (currentOrder != null)
            {
                OrderNumberText.Text = currentOrder.OrderNumber.ToString();
                OrderDateText.Text = currentOrder.OrderDate.ToString();
                CustomerNametext.Text = currentOrder.CustomerName.Trim();

                CustomerEmailText.Text = currentOrder.CustomerEmail.Trim();
                CustAddressText.Text = currentOrder.CustomerAddress.Trim();
                InProductionChickBox.IsChecked = currentOrder.InProduction;
                DeliveryDateText.Text = currentOrder.DeliveryDate.ToString();
            }

        }

        private void NewRecButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            if (IsDataValidOrder() == true) { 
            DeleteRecButton.Visibility = Visibility.Hidden;
            NewRecButton.Visibility = Visibility.Hidden;
            ClearControls();
                DeleteRecButton.Visibility = Visibility.Visible;
                NewRecButton.Visibility = Visibility.Visible;

            }
            else
            {
                ErrorMessageLabel.Content = "Invalid data";
            }
            
        }

        private void ClearControls()
        {
            OrderNumberText.Text = "";
            OrderDateText.Text = "";
            CustomerNametext.Text = "";
            CustomerEmailText.Text = "";
            CustAddressText.Text = "";
            InProductionChickBox.IsChecked = false;
            DeliveryDateText.Text = "";

            OrderNumberText.IsReadOnly = false;
        }

        private void DeleteRecButton_Click(object sender, RoutedEventArgs e)
        {
            if (OrderNumberText.Text.Trim() != "")
            {
                // Create and open a connection
                string connectionString = Properties.Settings.Default.ECarOrdersConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                     command.CommandText = "delete from CarOrders  " +
                             
                            " where OrderNumber=" + OrderNumberText.Text.Trim();
                    
                    int result = command.ExecuteNonQuery();

                    connection.Close();
                    ClearControls();
                    MessageBox.Show("Operation Completed Successfully!");
                }

                LoadCarOrders();
            }
        }

        private void CarOrderOptionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            
            CarOrderOption currentOrderOption = (CarOrderOption)CarOrderOptionsList.SelectedItem;

            DisplayCarOrderOptionDetails(currentOrderOption);
        }

        private void DisplayCarOrderOptionDetails(CarOrderOption currentOrderOption)
        {
            if (currentOrderOption != null)
            {
                OptionIDText.Text = currentOrderOption.OptionID.ToString();
                OptionDescText.Text = currentOrderOption.OptionDescription.ToString();
                OptionNameText.Text = currentOrderOption.OptionName.Trim();

                OptionpriceText.Text = currentOrderOption.OptionPrice.ToString("N2").Trim();
                
            }

        }

        private void NewOptButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessageLabel.Content = "";
            if (IsDataValidOption() == true) { 
            NewOptButton.Visibility = Visibility.Hidden;
            DeleteOptButton.Visibility = Visibility.Hidden;
            ClearCarOptionsControls();
                NewOptButton.Visibility = Visibility.Visible;
                DeleteOptButton.Visibility = Visibility.Visible;
            }
            else
            {
                ErrorMessageLabel.Content = "Invalid data";
            }

        }

        private void ClearCarOptionsControls()
        {
            OptionIDText.Text = "";
            OptionNameText.Text = "";
            OptionDescText.Text = "";
            OptionpriceText.Text = "0";


            OptionIDText.IsReadOnly = false;
        }

        private void DeleteOptButton_Click(object sender, RoutedEventArgs e)
        {
            if (OptionIDText.Text.Trim() != "")
            {
                string connectionString = Properties.Settings.Default.ECarOrdersConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand();
                    command.Connection = connection;

                    command.CommandText = "delete from CarOrderOptions  " +

                           " where OptionID=" + OptionIDText.Text.Trim();

                    int result = command.ExecuteNonQuery();

                    connection.Close();
                    
                    MessageBox.Show("Operation Completed Successfully!");
                    ClearControls();
                }

                LoadCarOptions();
            }
        }

        private void SaveOptButton_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = Properties.Settings.Default.ECarOrdersConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                //if (IsDataValidOption() == true)
                 if ( true)
                    {
                        if (OptionIDText.IsReadOnly)
                    {
                        command.CommandText = "UPDATE CarOrderOptions set " +
                             " OptionName=N'" + OptionNameText.Text.Trim() + "'" +
                             " ,OptionDescription=N'" + OptionDescText.Text.Trim() + "'" +
                             " ,Optionprice=N'" + OptionpriceText.Text.Trim() + "'" +
                            " where OptionID=" + OptionIDText.Text.Trim();
                    }
                    else // new
                    {
                        command.CommandText = "insert into CarOrderOptions(OptionID , OrderNumber, OptionName,OptionDescription,OptionPrice) values ( " +
                         " N'" + OptionIDText.Text.Trim() + "'" +
                         " ,N'" + OrderNumberText.Text.Trim() + "'" +
                         " ,N'" + OptionNameText.Text.Trim() + "'" +
                         " ,N'" + OptionDescText.Text.Trim() + "'" +
                        " , " + OptionpriceText.Text.Trim() + ")";
                    }
                    int result = command.ExecuteNonQuery();

                    connection.Close();
                    ClearCarOptionsControls();
                    MessageBox.Show("Operation Completed Successfully!");


                    DeleteOptButton.Visibility = Visibility.Visible;
                    NewOptButton.Visibility = Visibility.Visible;
                    LoadCarOptions();
                }
                else
                {
                    ErrorMessageLabel.Content = "Invalid Data";
                }
            }
        }

        public bool IsDataValidOrder()
        {
            try
            {
                if (OrderNumberText.Text == "") return false;
                //int number = int.Parse(OrderNumberText.Text);
                if (OrderNumberText.Text == "") return false;
                if (OrderDateText.Text == "") return false;
                if (CustomerNametext.Text == "") return false;
                if (CustomerEmailText.Text == "") return false;
                if (CustAddressText.Text == "") return false;
                if (DeliveryDateText.Text == "") return false;
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool IsDataValidOption()
        {
            try
            {
                if (OrderNumberText.Text == "") return false;

                //int number;
                //if (int.TryParse(OrderNumberText.Text, out number)) return false;

                if (OptionIDText.Text == "") return false;
                if (OptionNameText.Text == "") return false;
                if (OptionDescText.Text == "") return false;
                if (OptionpriceText.Text == "") return false;
                
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
    
}
