using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SmartParking
{
    public partial class MainForm : Form
    {
        // ───────────────────────────────────────────────────────────
        // Class‐level fields for tracking selections & controls
        // ───────────────────────────────────────────────────────────
        private Label _successUsernameLabel;
        private string _cachedUsername = "";
        private Panel mainContainer;
        private Panel homeScreen;
        private Panel parkingMapScreen;
        private Panel reservationSuccessScreen;
        private int currentScreen = 0; // 0 = home, 1 = map, 2 = success

        // Vehicle & location caching
        private string _selectedVehicleType;
        private string _selectedLocation;
        private string _selectedSlotCode;

        // “Book Your Slot” button on screen 2
        private RoundedButton bookButton;

        // Back button on map screen
        private RoundedButton _mapBackBtn;

        // Parking‐slot selection tracking
        private Panel _selectedSlotPanel;

        // Success‐screen layout & nav buttons
        private RoundedPanel _contentCard;
        private Label _successVehicleLabel;
        private Label _successSpotLabel;
        private Label _successLocationLabel;
        private RoundedButton _resBackBtn;
        private RoundedButton _resHomeBtn;

        // Slot colors
        private readonly Color AvailableColor = Color.FromArgb(0, 0, 80);
        private readonly Color LockedColor = Color.Yellow;
        private readonly Color HoverColor = Color.FromArgb(0, 180, 0);
        private readonly Color SelectedColor = Color.Green;
        // Tracks which vehicle-icon panel is currently “clicked” on screen 1
        private RoundedPanel _selectedVehiclePanel = null;

        // Tracks which place-card panel is currently “clicked” on screen 1
        private RoundedPanel _selectedPlacePanel = null;
        private enum SlotType { Available, Locked }

        public MainForm()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 750);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SmartParking";
            ResumeLayout(false);
        }

        private void InitializeUI()
        {
            // Main container
            mainContainer = new Panel
            {
                BackColor = Color.LightBlue,
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };
            Controls.Add(mainContainer);

            CreateHomeScreen();
            CreateParkingMapScreen();
            CreateReservationSuccessScreen();

            ShowScreen(0);
        }

        private void ShowScreen(int screenIndex)
        {
            homeScreen.Visible = (screenIndex == 0);
            parkingMapScreen.Visible = (screenIndex == 1);
            reservationSuccessScreen.Visible = (screenIndex == 2);
            currentScreen = screenIndex;
        }

        #region Home Screen
        private RoundedTextBox _usernameBox; // add this to your class-level fields
        private void CreateHomeScreen()
        {
            homeScreen = new RoundedPanel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Visible = false
            };
            mainContainer.Controls.Add(homeScreen);

            // Header
            var headerPanel = new RoundedPanel
            {
                BackColor = Color.FromArgb(120, 80, 220),
                Dock = DockStyle.Top,
                Height = 250,
                Padding = new Padding(20)
            };
            homeScreen.Controls.Add(headerPanel);

            // Profile icon
           

            // Main header text
            var headerText = new Label
            {
                Text = "FIND THE BEST\r\nPARKING SPACE",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(300, 80),
                Location = new Point(20, 80)
            };
            headerPanel.Controls.Add(headerText);

            // Search box
            // Search box
            // Search box


            // Vehicle type
            var vehicleTypeLabel = new Label
            {
                Text = "Your vehicle type",
                Font = new Font("Arial", 12),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 270)
            };
            homeScreen.Controls.Add(vehicleTypeLabel);

            CreateVehicleOption("🚗", 20, 300);
            CreateVehicleOption("🏍️", 120, 300);
            CreateVehicleOption("🚲", 220, 300);

            // Recent places
            var recentPlacesLabel = new Label
            {
                Text = "Recent places",
                Font = new Font("Arial", 12),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(20, 380)
            };
            homeScreen.Controls.Add(recentPlacesLabel);

            CreateRecentPlaceCard(
                "Archiepiskopou Makariou III\nYpsonas 4182",
                20, 410,
                () => ShowScreen(1)
            );
            CreateRecentPlaceCard(
                "Spyrou Kyprianou Av. 74\nLim 4632",
                190, 410,
                () => ShowScreen(1)
            );

            // Bottom nav indicator
            var navbar = new Panel
            {
                BackColor = Color.White,
                Height = 5,
                Dock = DockStyle.Bottom
            };
            homeScreen.Controls.Add(navbar);

            var navLine = new Panel
            {
                BackColor = Color.Black,
                Size = new Size(200, 5),  // increased width from 60 to 200
                Location = new Point((navbar.Width - 200) / 2, 0)  // center inside navbar
            };
            navbar.Controls.Add(navLine);

            // Also, to keep it centered on resizing:
            navbar.Resize += (s, e) =>
            {
                navLine.Location = new Point((navbar.Width - navLine.Width) / 2, 0);
            };

        }

        private void CreateVehicleOption(string icon, int x, int y)
        {
            var defaultBorder = Color.Gray;
            var hoverBorder = Color.FromArgb(80, 160, 240);
            var clickBorder = Color.FromArgb(40, 120, 200);
            var defaultBack = Color.White;
            var hoverBack = Color.FromArgb(240, 240, 255);
            var clickBack = Color.FromArgb(230, 230, 255);

            var panel = new RoundedPanel
            {
                Size = new Size(80, 80),
                Location = new Point(x, y),
                BackColor = defaultBack,
                BorderColor = defaultBorder,
                BorderWidth = 2
            };

            var label = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 24),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            panel.Controls.Add(label);
            homeScreen.Controls.Add(panel);

            void OnEnter(object s, EventArgs e)
            {
                if (panel != _selectedVehiclePanel)
                {
                    panel.BorderColor = hoverBorder;
                    panel.BackColor = hoverBack;
                    panel.Invalidate();
                }
            }
            void OnLeave(object s, EventArgs e)
            {
                if (panel != _selectedVehiclePanel)
                {
                    panel.BorderColor = defaultBorder;
                    panel.BackColor = defaultBack;
                    panel.Invalidate();
                }
            }
            void OnClick(object s, EventArgs e)
            {
                if (_selectedVehiclePanel != null && _selectedVehiclePanel != panel)
                {
                    _selectedVehiclePanel.BorderColor = defaultBorder;
                    _selectedVehiclePanel.BackColor = defaultBack;
                    _selectedVehiclePanel.Invalidate();
                }
                _selectedVehiclePanel = panel;
                panel.BorderColor = clickBorder;
                panel.BackColor = clickBack;
                panel.Invalidate();

                _selectedVehicleType = icon;
            }

            panel.MouseEnter += OnEnter;
            panel.MouseLeave += OnLeave;
            panel.Click += OnClick;

            label.MouseEnter += OnEnter;
            label.MouseLeave += OnLeave;
            label.Click += OnClick;
        }

        private void CreateRecentPlaceCard(string address, int x, int y, Action onClick)
        {
            var defaultBorder = Color.Gray;
            var hoverBorder = Color.FromArgb(80, 160, 240);
            var clickBorder = Color.FromArgb(40, 120, 200);

            var panel = new RoundedPanel
            {
                Size = new Size(150, 150),
                Location = new Point(x, y),
                BackColor = Color.White,
                BorderColor = defaultBorder,
                BorderWidth = 2
            };

            var addrLabel = new Label
            {
                Text = address,
                Font = new Font("Arial", 9),
                ForeColor = Color.Black,
                Size = new Size(130, 60),
                Location = new Point(10, 10),
                BackColor = Color.Transparent
            };
            var preview = new PictureBox
            {
                Size = new Size(130, 70),
                Location = new Point(10, 70),
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            panel.Controls.Add(addrLabel);
            panel.Controls.Add(preview);
            homeScreen.Controls.Add(panel);

            void OnEnter(object s, EventArgs e)
            {
                if (panel != _selectedPlacePanel)
                {
                    panel.BorderColor = hoverBorder;
                    panel.Invalidate();
                }
            }
            void OnLeave(object s, EventArgs e)
            {
                if (panel != _selectedPlacePanel)
                {
                    panel.BorderColor = defaultBorder;
                    panel.Invalidate();
                }
            }
            void OnClick(object s, EventArgs e)
            {
                if (_selectedPlacePanel != null && _selectedPlacePanel != panel)
                {
                    _selectedPlacePanel.BorderColor = defaultBorder;
                    _selectedPlacePanel.Invalidate();
                }
                _selectedPlacePanel = panel;
                panel.BorderColor = clickBorder;
                panel.Invalidate();

                _selectedLocation = address;
                onClick();
            }

            panel.MouseEnter += OnEnter;
            panel.MouseLeave += OnLeave;
            panel.Click += OnClick;
            addrLabel.MouseEnter += OnEnter;
            addrLabel.MouseLeave += OnLeave;
            addrLabel.Click += OnClick;
            preview.MouseEnter += OnEnter;
            preview.MouseLeave += OnLeave;
            preview.Click += OnClick;
        }

        #endregion

        #region Parking Map Screen

        private void CreateParkingMapScreen()
        {
            parkingMapScreen = new Panel
            {
                BackColor = Color.FromArgb(40, 40, 40), 
                Dock = DockStyle.Fill,
                Visible = false
            };
            mainContainer.Controls.Add(parkingMapScreen);

            // Status bar
            var statusBar = new Panel
            {
                Height = 30,
                Dock = DockStyle.Top,
                BackColor = Color.Black
            };
            parkingMapScreen.Controls.Add(statusBar);

            var timeLabel = new Label
            {
                Text = DateTime.Now.ToString("h:mm"),
                ForeColor = Color.White,
                Font = new Font("Arial", 10),
                Location = new Point(20, 7),
                AutoSize = true
            };
            statusBar.Controls.Add(timeLabel);

            var batteryIcon = new PictureBox
            {
                Size = new Size(25, 12),
                Location = new Point(330, 9),
                BackColor = Color.Transparent,
                Image = CreateBatteryIcon()
            };
            statusBar.Controls.Add(batteryIcon);

            // App header
            var appHeader = new Panel
            {
                Height = 50,
                Dock = DockStyle.Top,
                BackColor = Color.Black
            };
            parkingMapScreen.Controls.Add(appHeader);

            var appLogo = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(10, 5),
                Image = CreateAppLogo(),
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            appHeader.Controls.Add(appLogo);

            var settingsBtn = new Button
            {
                Size = new Size(30, 30),
                Location = new Point(320, 10),
                FlatStyle = FlatStyle.Flat,
                Text = "⚙️",
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };
            settingsBtn.FlatAppearance.BorderSize = 0;
            appHeader.Controls.Add(settingsBtn);

            // Back button (top-right)
            _mapBackBtn = new RoundedButton
            {
                Text = "←",
                Font = new Font("Arial", 12),
                Size = new Size(30, 30),
                BackColor = Color.Red,       // ← make the bg red
                FlatStyle = FlatStyle.Flat
            };

            _mapBackBtn.FlatAppearance.BorderSize = 0;
            _mapBackBtn.Click += (s, e) => ShowScreen(0);
            parkingMapScreen.Controls.Add(_mapBackBtn);
            parkingMapScreen.Resize += (s, e) =>
            {
                _mapBackBtn.Location = new Point(
                    parkingMapScreen.ClientSize.Width - _mapBackBtn.Width - 10,
                    10
                );
            };

            // Parking grid & legend
            CreateParkingGrid();
            var legendPanel = new Panel
            {
                Height = 30,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            parkingMapScreen.Controls.Add(legendPanel);
            CreateLegendItem("SELECTED", Color.Green, 20, 5);
            CreateLegendItem("Locked", Color.Yellow, 140, 5);
            CreateLegendItem("Available", AvailableColor, 260, 5);

            // Book button
            bookButton = new RoundedButton
            {
                Text = "Book Your Slot",
                ForeColor = Color.White,
                BackColor = Color.Gray,
                Font = new Font("Arial", 12),
                Size = new Size(200, 40),
                Location = new Point(100, 640),
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            bookButton.FlatAppearance.BorderSize = 0;
            bookButton.Click += (s, e) =>
            {
                _successUsernameLabel.Text = _cachedUsername;
                _successVehicleLabel.Text = _selectedVehicleType;
                _successSpotLabel.Text = _selectedSlotCode;
                _successLocationLabel.Text = _selectedLocation;
                ShowScreen(2);
            };

            parkingMapScreen.Controls.Add(bookButton);
        }

        private void CreateParkingGrid()
        {
            // Top row A0–A5
            for (int i = 0; i < 6; i++)
                CreateParkingSlot(20 + i * 60, 120, $"A{i}",
                    i % 2 == 0 ? SlotType.Available : SlotType.Locked
                );
            // Middle row B0–B2
            for (int i = 0; i < 3; i++)
                CreateParkingSlot(80 + i * 60, 240, $"B{i}", SlotType.Available);
            // Bottom row C0–C5
            for (int i = 0; i < 6; i++)
                CreateParkingSlot(20 + i * 60, 360, $"C{i}",
                    i % 2 == 1 ? SlotType.Available : SlotType.Locked
                );
        }

        private void CreateParkingSlot(int x, int y, string code, SlotType type)
        {
            // 1) Set the “baseColor” for locked slots to DarkRed instead of Yellow:
            Color baseColor = type == SlotType.Locked
                ? Color.FromArgb(255, 165, 0)   // orange color (like orange)
                : AvailableColor;

            var slotPanel = new Panel
            {
                Size = new Size(50, 70),
                Location = new Point(x, y),
                BackColor = baseColor,
                Tag = baseColor
            };

            // 2) Make the code label text black if it’s a locked slot:
            var slotLabel = new Label
            {
                Text = code,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 8, FontStyle.Bold),
                ForeColor = type == SlotType.Locked
                            ? Color.Black  // ← locked slots get black text
                            : Color.White, // available/selected stay white
                BackColor = Color.Transparent
            };

            slotPanel.Controls.Add(slotLabel);
            parkingMapScreen.Controls.Add(slotPanel);

            void OnEnter(object s, EventArgs e)
            {
                if (type != SlotType.Locked && slotPanel != _selectedSlotPanel)
                    slotPanel.BackColor = HoverColor;
            }
            void OnLeave(object s, EventArgs e)
            {
                if (slotPanel != _selectedSlotPanel)
                    slotPanel.BackColor = (Color)slotPanel.Tag;
            }
            void OnClick(object s, EventArgs e)
            {
                if (type == SlotType.Locked)
                {
                   // Show alert if user taps an already reserved slot
                    MessageBox.Show(
                        "Sorry, this spot is already reserved.",
                        "Spot Unavailable",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                // Deselect previous
                if (_selectedSlotPanel != null)
                    _selectedSlotPanel.BackColor = (Color)_selectedSlotPanel.Tag;

                // Select this one
                _selectedSlotPanel = slotPanel;
                slotPanel.BackColor = SelectedColor;

                // Cache & enable booking
                _selectedSlotCode = code;
                bookButton.Enabled = true;
                bookButton.BackColor = SelectedColor;
            }

            slotPanel.Click += OnClick;
            slotLabel.Click += OnClick;

            slotPanel.MouseEnter += OnEnter;
            slotPanel.MouseLeave += OnLeave;
            slotPanel.Click += OnClick;
            slotLabel.MouseEnter += OnEnter;
            slotLabel.MouseLeave += OnLeave;
            slotLabel.Click += OnClick;
        }

        private void CreateLegendItem(string text, Color color, int x, int y)
        {
            var square = new Panel
            {
                Size = new Size(15, 15),
                Location = new Point(x, y),
                BackColor = color
            };
            var lbl = new Label
            {
                Text = text,
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(x + 20, y - 2),
                Font = new Font("Arial", 8)
            };
            parkingMapScreen.Controls.Add(square);
            parkingMapScreen.Controls.Add(lbl);
        }

        #endregion

        #region Reservation Success Screen

        private void CreateReservationSuccessScreen()
        {
            reservationSuccessScreen = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Visible = false,
                Padding = new Padding(10)
            };
            mainContainer.Controls.Add(reservationSuccessScreen);

            // Recenter card & nav on resize
            reservationSuccessScreen.Resize += (s, e) =>
            {
                if (_contentCard != null)
                {
                    _contentCard.Location = new Point(
                        (reservationSuccessScreen.ClientSize.Width - _contentCard.Width) / 2,
                        (reservationSuccessScreen.ClientSize.Height - _contentCard.Height) / 2
                    );
                }
                if (_resBackBtn != null && _resHomeBtn != null)
                {
                    int gap = 20;
                    int totalW = _resBackBtn.Width + _resHomeBtn.Width + gap;
                    int startX = (reservationSuccessScreen.ClientSize.Width - totalW) / 2;
                    int btnY = _contentCard.Bottom + 20;
                    _resBackBtn.Location = new Point(startX, btnY);
                    _resHomeBtn.Location = new Point(startX + _resBackBtn.Width + gap, btnY);
                }
            };

            // Content card
            int cardW = 300, cardH = 520;
            _contentCard = new RoundedPanel
            {
                Size = new Size(cardW, cardH),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            reservationSuccessScreen.Controls.Add(_contentCard);
            // store for Resize and initial centering

            reservationSuccessScreen.Controls.Add(_contentCard);

            // **Initial centering**  
            _contentCard.Location = new Point(
                (reservationSuccessScreen.ClientSize.Width - cardW) / 2,
                (reservationSuccessScreen.ClientSize.Height - cardH) / 2
            );

            // Header
            var headerLabel = new Label
            {
                Text = "Reservation successful for",
                Font = new Font("Arial", 12),
                ForeColor = Color.Gray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(cardW, 30),
                Location = new Point(0, 20)
            };
            _contentCard.Controls.Add(headerLabel);

            // Vehicle icon/text (large)
            _successVehicleLabel = new Label
            {
                AutoSize = false,
                Size = new Size(cardW, 60),
                Location = new Point(0, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Emoji", 36),
                ForeColor = Color.Black,
                Text = ""
            };
            _contentCard.Controls.Add(_successVehicleLabel);
            // Username label
            _successUsernameLabel = new Label
            {
                AutoSize = false,
                Size = new Size(cardW, 30),
                Location = new Point(0, 25),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Black,
                Text = ""
            };
            _contentCard.Controls.Add(_successUsernameLabel);

            // “Spot” title
            var spotTitle = new Label
            {
                Text = "Spot",
                Font = new Font("Arial", 12),
                ForeColor = Color.Gray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(cardW, 25),
                Location = new Point(0, 140)
            };
            _contentCard.Controls.Add(spotTitle);

            // Spot code
            _successSpotLabel = new Label
            {
                AutoSize = false,
                Size = new Size(cardW, 40),
                Location = new Point(0, 165),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 30, FontStyle.Bold),
                ForeColor = Color.Black,
                Text = ""
            };
            _contentCard.Controls.Add(_successSpotLabel);

            // Booking ID
            var bookingId = new Label
            {
                Text = "Booking ID: 324992429",
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(cardW, 20),
                Location = new Point(0, 215)
            };
            _contentCard.Controls.Add(bookingId);

            // QR code
            var qrSize = 150;
            var qrCode = new PictureBox
            {
                Size = new Size(qrSize, qrSize),
                Location = new Point((cardW - qrSize) / 2, 240),
                Image = CreateQRCode(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent
            };
            _contentCard.Controls.Add(qrCode);

            // Scan text
            var scanLabel = new Label
            {
                Text = "Scan barcode at the location",
                Font = new Font("Arial", 10),
                ForeColor = Color.Gray,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(cardW, 20),
                Location = new Point(0, 400)
            };
            _contentCard.Controls.Add(scanLabel);

            // Location
            _successLocationLabel = new Label
            {
                AutoSize = false,
                Size = new Size(cardW, 20),
                Location = new Point(0, 425),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10),
                ForeColor = Color.Black,
                Text = ""
            };
            _contentCard.Controls.Add(_successLocationLabel);

            // Nav buttons under card
            AddReservationNavButtons();

            // Force initial layout
            reservationSuccessScreen.PerformLayout();
            reservationSuccessScreen.Refresh();
        }

        private void AddReservationNavButtons()
        {
            int btnSize = 40, gap = 20;
            _resBackBtn = new RoundedButton
            {
                Text = "←",
                Font = new Font("Arial", 16),
                Size = new Size(40, 40),
                BackColor = Color.Red,       // ← make the bg red
                FlatStyle = FlatStyle.Flat
            };
            _resBackBtn.FlatAppearance.BorderSize = 0;
            _resBackBtn.Click += (s, e) => ShowScreen(1);
            reservationSuccessScreen.Controls.Add(_resBackBtn);
            var topBackBtn = new RoundedButton
            {
                Text = "←",
                Font = new Font("Arial", 12),
                Size = new Size(30, 30),
                BackColor = Color.Red,      // same red background
                FlatStyle = FlatStyle.Flat
            };
            topBackBtn.FlatAppearance.BorderSize = 0;
            topBackBtn.Click += (s, e) => ShowScreen(0);

            // position it centered just above bookButton
            topBackBtn.Location = new Point(
                bookButton.Location.X + (bookButton.Width - topBackBtn.Width) / 2,
                bookButton.Location.Y - topBackBtn.Height - 10
            );

            parkingMapScreen.Controls.Add(topBackBtn);
            _resHomeBtn = new RoundedButton
            {
                Text = "✕",
                Font = new Font("Arial", 16),
                Size = new Size(btnSize, btnSize),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White
            };
            _resHomeBtn.FlatAppearance.BorderSize = 0;
            _resHomeBtn.Click += (s, e) => ShowScreen(0);
            reservationSuccessScreen.Controls.Add(_resHomeBtn);
        }

        #endregion

        #region Graphics Helpers

        private Image CreateCircleImage(int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(Color.LightGray))
                    g.FillEllipse(brush, 0, 0, size - 1, size - 1);
            }
            return bmp;
        }

        private Image CreateSearchIcon(int size)
        {
            var bmp = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(Color.Gray, 2))
                {
                    g.DrawEllipse(pen, 2, 2, size - 10, size - 10);
                    g.DrawLine(pen, size - 4, size - 4, size - 8, size - 8);
                }
            }
            return bmp;
        }

        private Image CreateBatteryIcon()
        {
            var bmp = new Bitmap(25, 12);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawRectangle(Pens.White, 0, 0, 20, 10);
                g.FillRectangle(Brushes.White, 2, 2, 15, 6);
                g.FillRectangle(Brushes.White, 21, 3, 2, 4);
            }
            return bmp;
        }

        private Image CreateAppLogo()
        {
            var bmp = new Bitmap(40, 40);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(Brushes.Green, 0, 0, 40, 40);
                g.DrawString("P", new Font("Arial", 20, FontStyle.Bold), Brushes.White, 10, 5);
            }
            return bmp;
        }

        private Image CreateQRCode()
        {
            var bmp = new Bitmap(150, 150);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.White, 0, 0, 150, 150);
                var rand = new Random(42);
                for (int x = 0; x < 10; x++)
                    for (int y = 0; y < 10; y++)
                        if (rand.Next(2) == 0)
                            g.FillRectangle(Brushes.Black, x * 15, y * 15, 15, 15);

                // Markers
                g.FillRectangle(Brushes.Black, 15, 15, 30, 30);
                g.FillRectangle(Brushes.White, 22, 22, 15, 15);
                g.FillRectangle(Brushes.Black, 105, 15, 30, 30);
                g.FillRectangle(Brushes.White, 112, 22, 15, 15);
                g.FillRectangle(Brushes.Black, 15, 105, 30, 30);
                g.FillRectangle(Brushes.White, 22, 112, 15, 15);
            }
            return bmp;
        }

        #endregion

        #region Custom Controls

        public class RoundedPanel : Panel
        {
            private int _radius = 20;
            public Color BorderColor { get; set; } = Color.Gray;
            public int BorderWidth { get; set; } = 2;
            public RoundedPanel() { DoubleBuffered = true; }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, _radius, _radius, 180, 90);
                    path.AddArc(Width - _radius, 0, _radius, _radius, 270, 90);
                    path.AddArc(Width - _radius, Height - _radius, _radius, _radius, 0, 90);
                    path.AddArc(0, Height - _radius, _radius, _radius, 90, 90);
                    path.CloseAllFigures();

                    Region = new Region(path);
                    if (BorderWidth > 0 && BorderColor != Color.Transparent)
                        using (var pen = new Pen(BorderColor, BorderWidth))
                            g.DrawPath(pen, path);
                }
                base.OnPaint(e);
            }
        }

        public class RoundedButton : Button
        {
            private int _radius = 20;
            public RoundedButton()
            {
                FlatStyle = FlatStyle.Flat;
                DoubleBuffered = true;
                FlatAppearance.BorderSize = 0;
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, _radius, _radius, 180, 90);
                    path.AddArc(Width - _radius, 0, _radius, _radius, 270, 90);
                    path.AddArc(Width - _radius, Height - _radius, _radius, _radius, 0, 90);
                    path.AddArc(0, Height - _radius, _radius, _radius, 90, 90);
                    path.CloseAllFigures();
                    Region = new Region(path);
                }
                base.OnPaint(e);
            }
        }

        public class RoundedTextBox : TextBox
        {
            private int _radius = 15;
            public RoundedTextBox()
            {
                BorderStyle = BorderStyle.None;
                DoubleBuffered = true;
                Padding = new Padding(10, 5, 10, 5);
            }
            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);
                using (var path = new GraphicsPath())
                {
                    path.AddArc(0, 0, _radius, _radius, 180, 90);
                    path.AddArc(Width - _radius, 0, _radius, _radius, 270, 90);
                    path.AddArc(Width - _radius, Height - _radius, _radius, _radius, 0, 90);
                    path.AddArc(0, Height - _radius, _radius, _radius, 90, 90);
                    path.CloseAllFigures();
                    Region = new Region(path);
                }
            }
        }

        #endregion
    }
}
