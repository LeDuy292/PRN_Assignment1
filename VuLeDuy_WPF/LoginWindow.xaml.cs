using Service;
using System.Windows;

namespace VuLeDuy_WPF
{
    public partial class LoginWindow : Window
    {
        private readonly ICustomerService customerService = new CustomerService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Vui lòng nhập email và mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (customerService.Authenticate(email, password))
            {
                MainWindow mainWindow = new MainWindow(email);
                mainWindow.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Email hoặc mật khẩu không đúng.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}