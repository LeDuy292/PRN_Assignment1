using BusinessObjects;
using Service;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VuLeDuy_WPF
{
    public partial class RoomDialog : Window
    {
        private readonly IRoomInfoService roomService = new RoomInfoService();
        private readonly IRoomTypeService roomTypeService = new RoomTypeService();
        public RoomInformation Room { get; private set; }

        public RoomDialog()
        {
            InitializeComponent();
            Room = new RoomInformation();
            LoadRoomTypes();
            SetNewRoomID();
        }

        public RoomDialog(RoomInformation room)
        {
            InitializeComponent();
            Room = room ?? new RoomInformation();
            txtRoomID.Text = room?.RoomID.ToString() ?? "0";
            txtRoomNumber.Text = room?.RoomNumber ?? string.Empty;
            txtRoomPrice.Text = room?.RoomPricePerDate.ToString() ?? string.Empty;
            txtMaxCapacity.Text = room?.RoomMaxCapacity.ToString() ?? string.Empty;
            cboRoomType.SelectedValue = room?.RoomTypeID;
            LoadRoomTypes();
        }

        private void SetNewRoomID()
        {
            try
            {
                var rooms = roomService.GetAll()?.ToList() ?? new List<RoomInformation>();
                int newId = rooms.Any() ? rooms.Max(r => r.RoomID) + 1 : 1;
                txtRoomID.Text = newId.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo mã phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRoomTypes()
        {
            try
            {
                cboRoomType.ItemsSource = roomTypeService.GetAll() ?? Enumerable.Empty<RoomType>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải loại phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtRoomNumber.Text) ||
                !decimal.TryParse(txtRoomPrice.Text, out decimal price) ||
                !int.TryParse(txtMaxCapacity.Text, out int capacity) ||
                cboRoomType.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                Room = new RoomInformation
                {
                    RoomID = int.TryParse(txtRoomID.Text, out int id) ? id : 0,
                    RoomNumber = txtRoomNumber.Text,
                    RoomPricePerDate = price,
                    RoomMaxCapacity = capacity,
                    RoomTypeID = (int)cboRoomType.SelectedValue,
                    RoomStatus = 1 // Default: Trống
                };
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu phòng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}