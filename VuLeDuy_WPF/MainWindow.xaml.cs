using BusinessObjects;
using Service;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using DataAccessLayer;

namespace VuLeDuy_WPF
{
    public partial class MainWindow : Window
    {
        private readonly IRoomInfoService roomService = new RoomInfoService();
        private readonly IRoomTypeService roomTypeService = new RoomTypeService();
        private readonly ICustomerService customerService = new CustomerService();
        private readonly IBookingService bookingService = new BookingService();
        private Customer? currentCustomer;
        private bool isAdmin;

        public MainWindow(string email)
        {
            InitializeComponent();

            try
            {
                string json = File.ReadAllText("appsettings.json");
                var config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);
                if (config != null && config.TryGetValue("AdminAccount", out var adminAccount))
                {
                    string? adminEmail = adminAccount.GetValueOrDefault("Email");
                    string? adminPassword = adminAccount.GetValueOrDefault("Password");

                    if (adminEmail != null && adminPassword != null && email == adminEmail && customerService.Authenticate(email, adminPassword))
                    {
                        isAdmin = true;
                        currentCustomer = null;
                    }
                    else
                    {
                        currentCustomer = customerService.GetByEmail(email);
                        isAdmin = false;
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi cấu hình: Không tìm thấy tài khoản admin trong appsettings.json.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc cấu hình: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            ConfigureUI();
            LoadData();
        }

        private void ConfigureUI()
        {
            tabRooms.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            tabCustomers.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            tabReport.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            tabProfile.Visibility = !isAdmin ? Visibility.Visible : Visibility.Collapsed;
            tabBookingHistory.Visibility = !isAdmin ? Visibility.Visible : Visibility.Collapsed;
            tabNewBooking.Visibility = !isAdmin ? Visibility.Visible : Visibility.Collapsed;

            if (isAdmin)
            {
                btnUpdateRoom.IsEnabled = false;
                btnDeleteRoom.IsEnabled = false;
                btnUpdateCustomer.IsEnabled = false;
                btnDeleteCustomer.IsEnabled = false;
            }
            else
            {
                btnCreateRoom.Visibility = Visibility.Collapsed;
                btnUpdateRoom.Visibility = Visibility.Collapsed;
                btnDeleteRoom.Visibility = Visibility.Collapsed;
                btnCreateCustomer.Visibility = Visibility.Collapsed;
                btnUpdateCustomer.Visibility = Visibility.Collapsed;
                btnDeleteCustomer.Visibility = Visibility.Collapsed;
                tabControl.SelectedItem = tabProfile; // Force Hồ Sơ tab for customers
            }

            tabControl.UpdateLayout();
            tabControl.Items.Refresh();
        }

        private void LoadData()
        {
            try
            {
                roomService.UpdateExpiredRoomStatus();

                if (isAdmin)
                {
                    dgRooms.ItemsSource = roomService.GetAll()?.ToList() ?? Enumerable.Empty<RoomInformation>();
                    cboRoomType.ItemsSource = roomTypeService.GetAll() ?? Enumerable.Empty<RoomType>();
                    dgCustomers.ItemsSource = customerService.GetAll() ?? Enumerable.Empty<Customer>();
                    var bookings = bookingService.GetAll()
                        .Select(b => new
                        {
                            b.BookingID,
                            b.StartDate,
                            b.TotalPrice,
                            CustomerFullName = customerService.GetByID(b.CustomerID)?.CustomerFullName ?? "Unknown"
                        })
                        .OrderByDescending(b => b.TotalPrice);
                    dgReport.ItemsSource = bookings ?? Enumerable.Empty<object>();
                }
                else if (currentCustomer != null)
                {
                    txtProfileFullName.Text = currentCustomer.CustomerFullName ?? string.Empty;
                    txtProfileEmail.Text = currentCustomer.EmailAddress ?? string.Empty;
                    txtProfileTelephone.Text = currentCustomer.Telephone ?? string.Empty;
                    dpProfileBirthday.SelectedDate = currentCustomer.CustomerBirthday;
                    var bookings = bookingService.GetByCustomerID(currentCustomer.CustomerID)
                        .Select(b => new
                        {
                            b.BookingID,
                            b.StartDate,
                            b.EndDate,
                            b.TotalPrice,
                            b.BookingStatus,
                            RoomNumber = roomService.GetByID(b.RoomID)?.RoomNumber ?? "Unknown"
                        });
                    dgBookingHistory.ItemsSource = bookings ?? Enumerable.Empty<object>();
                    LoadNewAvailableRooms();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadNewAvailableRooms()
        {
            try
            {
                var selectedDate = dpNewBookingDate.SelectedDate ?? DateTime.Today;
                dgNewAvailableRooms.ItemsSource = roomService.GetAvailableRooms(selectedDate)
                    .Select(r => new
                    {
                        r.RoomID,
                        r.RoomNumber,
                        r.RoomPricePerDate,
                        r.RoomMaxCapacity,
                        RoomTypeName = roomTypeService.GetByID(r.RoomTypeID)?.RoomTypeName ?? "Unknown"
                    }) ?? Enumerable.Empty<object>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải phòng trống: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgNewAvailableRooms_SelectionChanged(object sender, SelectionChangedEventArgs e) { }

        private int GenerateUniqueBookingID()
        {
            var bookings = InMemoryDatabase.Instance.bookings ?? new List<Booking>();
            int newId;
            Random random = new Random();
            do
            {
                newId = random.Next(1000, 9999);
            } while (bookings.Any(b => b.BookingID == newId));
            return newId;
        }

        private void BookRoom(DataGrid dgRooms, DatePicker dpStart, DatePicker dpEnd)
        {
            if (dgRooms.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng chọn một phòng để đặt.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!dpStart.SelectedDate.HasValue || !dpEnd.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và ngày kết thúc.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var bookingDate = dpStart.SelectedDate.Value;
            var endDate = dpEnd.SelectedDate.Value;

            if (bookingDate < DateTime.Today || endDate < bookingDate)
            {
                MessageBox.Show("Ngày đặt hoặc ngày kết thúc không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var selectedRoom = dgRooms.SelectedItem;
            var roomId = (int)selectedRoom.GetType().GetProperty("RoomID")!.GetValue(selectedRoom)!;
            var room = roomService.GetByID(roomId);
            if (room == null)
            {
                MessageBox.Show("Phòng không tồn tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (room.RoomStatus != 1)
            {
                MessageBox.Show("Phòng đã được đặt. Vui lòng chọn phòng khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var days = (endDate - bookingDate).Days + 1;
            var totalPrice = room.RoomPricePerDate * days;

            if (currentCustomer == null)
            {
                MessageBox.Show("Thông tin khách hàng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var booking = new Booking
            {
                BookingID = GenerateUniqueBookingID(),
                StartDate = bookingDate,
                EndDate = endDate,
                TotalPrice = totalPrice,
                CustomerID = currentCustomer.CustomerID,
                RoomID = roomId,
                BookingStatus = 1
            };

            try
            {
                bookingService.Add(booking);
                room.RoomStatus = 2;
                roomService.Update(room);
                MessageBox.Show("Đặt phòng thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt phòng: {ex.Message}\nStack Trace: {ex.StackTrace}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnNewBookRoom_Click(object sender, RoutedEventArgs e)
        {
            BookRoom(dgNewAvailableRooms, dpNewBookingDate, dpNewEndDate);
        }

        private void dpNewBookingDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadNewAvailableRooms();
        }

        private void dgRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgRooms.SelectedItem is RoomInformation room)
            {
                txtRoomID.Text = room.RoomID.ToString();
                txtRoomNumber.Text = room.RoomNumber ?? string.Empty;
                txtRoomPrice.Text = room.RoomPricePerDate.ToString();
                txtMaxCapacity.Text = room.RoomMaxCapacity.ToString();
                cboRoomType.SelectedValue = room.RoomTypeID;
                btnUpdateRoom.IsEnabled = true;
                btnDeleteRoom.IsEnabled = true;
            }
            else
            {
                txtRoomID.Text = string.Empty;
                txtRoomNumber.Text = string.Empty;
                txtRoomPrice.Text = string.Empty;
                txtMaxCapacity.Text = string.Empty;
                cboRoomType.SelectedValue = null;
                btnUpdateRoom.IsEnabled = false;
                btnDeleteRoom.IsEnabled = false;
            }
        }

        private void btnCreateRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomDialog dialog = new RoomDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    roomService.Add(dialog.Room);
                    LoadData();
                    dgRooms.Items.Refresh();
                    MessageBox.Show("Thêm phòng thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnUpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms.SelectedItem is not RoomInformation selectedRoom)
            {
                MessageBox.Show("Vui lòng chọn một phòng để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            RoomDialog dialog = new RoomDialog(selectedRoom);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    roomService.Update(dialog.Room);
                    LoadData();
                    MessageBox.Show("Cập nhật phòng thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            if (dgRooms.SelectedItem is not RoomInformation selectedRoom)
            {
                MessageBox.Show("Vui lòng chọn một phòng để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa phòng này?", "Xác Nhận Xóa", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    roomService.Delete(selectedRoom.RoomID);
                    LoadData();
                    MessageBox.Show("Xóa phòng thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void dgCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCustomers.SelectedItem is Customer customer)
            {
                txtCustomerID.Text = customer.CustomerID.ToString();
                txtFullName.Text = customer.CustomerFullName ?? string.Empty;
                txtEmail.Text = customer.EmailAddress ?? string.Empty;
                txtTelephone.Text = customer.Telephone ?? string.Empty;
                dpBirthday.SelectedDate = customer.CustomerBirthday;
                btnUpdateCustomer.IsEnabled = true;
                btnDeleteCustomer.IsEnabled = true;
            }
            else
            {
                txtCustomerID.Text = string.Empty;
                txtFullName.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtTelephone.Text = string.Empty;
                dpBirthday.SelectedDate = null;
                btnUpdateCustomer.IsEnabled = false;
                btnDeleteCustomer.IsEnabled = false;
            }
        }

        private void btnCreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerDialog dialog = new CustomerDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    customerService.Add(dialog.Customer);
                    LoadData();
                    MessageBox.Show("Thêm khách hàng thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi thêm khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is not Customer selectedCustomer)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để cập nhật.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CustomerDialog dialog = new CustomerDialog(selectedCustomer);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    customerService.Update(dialog.Customer);
                    LoadData();
                    MessageBox.Show("Cập nhật khách hàng thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (dgCustomers.SelectedItem is not Customer selectedCustomer)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xóa.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show("Bạn có chắc muốn xóa khách hàng này?", "Xác Nhận Xóa", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    customerService.Delete(selectedCustomer.CustomerID);
                    LoadData();
                    MessageBox.Show("Xóa khách hàng thành công!", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa khách hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtProfileFullName.Text) || string.IsNullOrEmpty(txtProfileEmail.Text) ||
                string.IsNullOrEmpty(txtProfileTelephone.Text) || !dpProfileBirthday.SelectedDate.HasValue)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (currentCustomer == null)
            {
                MessageBox.Show("Thông tin khách hàng không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                currentCustomer.CustomerFullName = txtProfileFullName.Text;
                currentCustomer.EmailAddress = txtProfileEmail.Text;
                currentCustomer.Telephone = txtProfileTelephone.Text;
                currentCustomer.CustomerBirthday = dpProfileBirthday.SelectedDate.Value;

                customerService.Update(currentCustomer);
                MessageBox.Show("Cập nhật hồ sơ thành công.", "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật hồ sơ: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}