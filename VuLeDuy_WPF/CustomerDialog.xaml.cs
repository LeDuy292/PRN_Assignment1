using BusinessObjects;
using Service;
using System;
using System.Linq;
using System.Windows;

namespace VuLeDuy_WPF
{
    public partial class CustomerDialog : Window
    {
        private readonly ICustomerService customerService = new CustomerService();
        public Customer Customer { get; private set; }

        public CustomerDialog()
        {
            InitializeComponent();
            Customer = new Customer();
            SetNewCustomerID();
        }

        public CustomerDialog(Customer customer)
        {
            InitializeComponent();
            Customer = customer ?? new Customer();
            txtCustomerID.Text = customer?.CustomerID.ToString() ?? "0";
            txtFullName.Text = customer?.CustomerFullName ?? string.Empty;
            txtEmail.Text = customer?.EmailAddress ?? string.Empty;
            txtTelephone.Text = customer?.Telephone ?? string.Empty;
            dpBirthday.SelectedDate = customer?.CustomerBirthday;
            txtPassword.Text = customer?.Password ?? string.Empty;
        }

        private void SetNewCustomerID()
        {
            try
            {
                var customers = customerService.GetAll()?.ToList() ?? new List<Customer>();
                int newId = customers.Any() ? customers.Max(c => c.CustomerID) + 1 : 1;
                txtCustomerID.Text = newId.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mã khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtFullName.Text) || string.IsNullOrEmpty(txtEmail.Text) ||
                string.IsNullOrEmpty(txtTelephone.Text) || !dpBirthday.SelectedDate.HasValue ||
                string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Customer = new Customer
                {
                    CustomerID = int.TryParse(txtCustomerID.Text, out int id) ? id : 0,
                    CustomerFullName = txtFullName.Text,
                    EmailAddress = txtEmail.Text,
                    Telephone = txtTelephone.Text,
                    CustomerBirthday = dpBirthday.SelectedDate.Value,
                    CustomerStatus = 1, // Default: Hoạt Động
                    Password = txtPassword.Text
                };
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}