--create database [MenuQ]

USE [MenuQ]
GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[AccountID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[RoleID] [int] NOT NULL,
	[Active] [bit] NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Admins]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admins](
	[AdminID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Position] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[AdminID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Areas]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Areas](
	[AreaID] [int] IDENTITY(1,1) NOT NULL,
	[AreaName] [nvarchar](50) NOT NULL,
	[Status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[AreaID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CancellationReasons]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CancellationReasons](
	[ReasonID] [int] IDENTITY(1,1) NOT NULL,
	[ReasonText] [nvarchar](200) NOT NULL,
	[Status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [nvarchar](100) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Employees]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employees](
	[EmployeeID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Position] [nvarchar](50) NULL,
	[HireDate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[InvoiceCode] [nvarchar](50) NOT NULL,
	[TotalAmount] [decimal](10, 2) NOT NULL,
	[PaymentMethod] [nvarchar](50) NOT NULL,
	[PaymentStatus] [bit] NOT NULL,
	[PaymentDate] [datetime] NULL,
	[CreatedAt] DATETIME NOT NULL DEFAULT (GETDATE()),
    [TableID] INT NOT NULL,
	[InvoiceStatus] INT NOT NULL DEFAULT 1,
PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MenuItems]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MenuItems](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryID] [int] NULL,
	[ItemName] [nvarchar](200) NOT NULL,
	[Descriptions] [nvarchar](max) NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[ImageURL] [nvarchar](max) NULL,
	[Status] [bit] NULL,
	[IsHot] [bit] NOT NULL,
	[IsNew] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperatingHours]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperatingHours](
	[OperatingHourID] [int] IDENTITY(1,1) NOT NULL,
	[RestaurantName] nvarchar(100),
	[ImageURL] [nvarchar](max) NULL,
	[OpeningTime] [time](7) NOT NULL,
	[ClosingTime] [time](7) NOT NULL,
	[IsOpen] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[OperatingHourID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [int] NULL,
	[ItemID] [int] NULL,
	[Quantity] [int] NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[Note] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Requests]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Requests](
	[RequestID] [int] IDENTITY(1,1) NOT NULL,
	[TableID] [int] NULL,
	[CustomerID] [int] NULL,
	[RequestTypeID] [int] NULL,
	[RequestStatusID] [int] NULL,
	[CreatedAt] [datetime] NULL,
	[CancellationReasonID] [int] NULL,
	[AccountID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestStatuses]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestStatuses](
	[RequestStatusID] [int] IDENTITY(1,1) NOT NULL,
	[StatusName] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RequestTypes]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestTypes](
	[RequestTypeID] [int] IDENTITY(1,1) NOT NULL,
	[RequestTypeName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RequestTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] IDENTITY(1,1) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceCalls]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceCalls](
	[ServiceCallID] [int] IDENTITY(1,1) NOT NULL,
	[RequestID] [int] NULL,
	[ReasonID] [int] NULL,
	[Note] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ServiceCallID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ServiceReasons]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceReasons](
	[ReasonID] [int] IDENTITY(1,1) NOT NULL,
	[ReasonText] [nvarchar](200) NOT NULL,
	[Status] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReasonID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tables]    Script Date: 3/2/2025 8:18:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tables](
	[TableID] [int] IDENTITY(1,1) NOT NULL,
	[AreaID] [int] NULL,
	[TableNumber] [nvarchar](10) NOT NULL,
	[Status] [bit] NULL,
	[SeatCapacity] [int] NOT NULL,
	[TableStatus] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TableID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Accounts__A9D10534D9992DC6]    Script Date: 3/2/2025 8:18:13 PM ******/
ALTER TABLE [dbo].[Accounts] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Accounts__C9F284560A8B600F]    Script Date: 3/2/2025 8:18:13 PM ******/
ALTER TABLE [dbo].[Accounts] ADD UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Admins__349DA587DDCEFFDA]    Script Date: 3/2/2025 8:18:13 PM ******/
ALTER TABLE [dbo].[Admins] ADD UNIQUE NONCLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ__Employee__349DA5875C9BEB11]    Script Date: 3/2/2025 8:18:13 PM ******/
ALTER TABLE [dbo].[Employees] ADD UNIQUE NONCLUSTERED 
(
	[AccountID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Invoices__0D9D7FF348F0B973]    Script Date: 3/2/2025 8:18:13 PM ******/
ALTER TABLE [dbo].[Invoices] ADD UNIQUE NONCLUSTERED 
(
	[InvoiceCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Areas] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[CancellationReasons] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[MenuItems] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[MenuItems] ADD  DEFAULT ((0)) FOR [IsHot]
GO
ALTER TABLE [dbo].[MenuItems] ADD  DEFAULT ((0)) FOR [IsNew]
GO
ALTER TABLE [dbo].[OperatingHours] ADD  DEFAULT ((1)) FOR [IsOpen]
GO
ALTER TABLE [dbo].[Requests] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ServiceReasons] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Tables] ADD  DEFAULT ((1)) FOR [Status]
GO
ALTER TABLE [dbo].[Tables] ADD  DEFAULT ((4)) FOR [SeatCapacity]
GO
ALTER TABLE [dbo].[Tables] ADD  DEFAULT ('Available') FOR [TableStatus]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[Admins]  WITH CHECK ADD FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Employees]  WITH CHECK ADD FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[Requests] ([RequestID])
GO
ALTER TABLE [dbo].[MenuItems]  WITH CHECK ADD FOREIGN KEY([CategoryID])
REFERENCES [dbo].[Categories] ([CategoryID])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([ItemID])
REFERENCES [dbo].[MenuItems] ([ItemID])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[Requests] ([RequestID])
GO
ALTER TABLE [dbo].[Requests]  WITH CHECK ADD FOREIGN KEY([AccountID])
REFERENCES [dbo].[Accounts] ([AccountID])
GO
ALTER TABLE [dbo].[Requests]  WITH CHECK ADD FOREIGN KEY([CancellationReasonID])
REFERENCES [dbo].[CancellationReasons] ([ReasonID])
GO
ALTER TABLE [dbo].[Requests]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Requests]  WITH CHECK ADD FOREIGN KEY([RequestTypeID])
REFERENCES [dbo].[RequestTypes] ([RequestTypeID])
GO
ALTER TABLE [dbo].[Requests]  WITH CHECK ADD FOREIGN KEY([RequestStatusID])
REFERENCES [dbo].[RequestStatuses] ([RequestStatusID])
GO
ALTER TABLE [dbo].[Requests]  WITH CHECK ADD FOREIGN KEY([TableID])
REFERENCES [dbo].[Tables] ([TableID])
GO
ALTER TABLE [dbo].[ServiceCalls]  WITH CHECK ADD FOREIGN KEY([ReasonID])
REFERENCES [dbo].[ServiceReasons] ([ReasonID])
GO
ALTER TABLE [dbo].[ServiceCalls]  WITH CHECK ADD FOREIGN KEY([RequestID])
REFERENCES [dbo].[Requests] ([RequestID])
GO
ALTER TABLE [dbo].[Tables]  WITH CHECK ADD FOREIGN KEY([AreaID])
REFERENCES [dbo].[Areas] ([AreaID])
GO


-- Insert into Roles
INSERT INTO Roles (RoleName)
VALUES
('Admin'),
('Employee'),
('Customer');

-- Insert into Areas
INSERT INTO Areas (AreaName, Status)
VALUES
(N'Tầng 1', 1),
(N'Tầng 2', 1),(N'Tầng 3', 1),
(N'Khu VIP', 1);
-- Insert into OperatingHours
INSERT INTO OperatingHours (OpeningTime, ClosingTime, IsOpen)
VALUES
('08:00', '22:00', 1);

-- Insert into CancellationReasons
INSERT INTO CancellationReasons (ReasonText, Status)
VALUES
(N'Khách đổi món', 1),
(N'Hết hàng', 1),
(N'Khách order nhầm', 1),
(N'Khác', 1);

-- Insert into Categories
INSERT INTO Categories (CategoryName, Status)
VALUES
(N'Cà Phê', 1),
(N'Trà', 1),
(N'Freeze', 1),
(N'Bánh ngọt', 1);

-- Insert into ServiceReasons
INSERT INTO ServiceReasons (ReasonText, Status)
VALUES
(N'Gọi thêm món', 1),
(N'Yêu cầu thanh toán', 1),
(N'Cần gọi phục vụ', 1),
(N'Đồ ăn chưa lên đủ', 1);

-- Insert into Customers
INSERT INTO Customers (CustomerName, PhoneNumber, CreatedAt)
VALUES
('Trần Văn Bảo', '0901234567', GETDATE()),
('Lê Thị Xuân', '0912345678', GETDATE()),
('Nguyễn Văn Hiếu', '0922334455', GETDATE()),
('Phạm Thị Hoà', '0977554433', GETDATE());


-- Insert into Accounts. Password "Kuroneko"
INSERT INTO Accounts (Email, Password, UserName, PhoneNumber, RoleID, Active, CreatedAt)
VALUES
('admin@gmail.com', 'YWG0/U9EjSFSyFodYZOFjw==:tXL3wkW/NQN5w+Lcsta+gkN9ke/j9vQFxxK4LZZpkBU=', 'admin', '0987654321', 1, 1, GETDATE()),
('employee1@gmail.com', 'YWG0/U9EjSFSyFodYZOFjw==:tXL3wkW/NQN5w+Lcsta+gkN9ke/j9vQFxxK4LZZpkBU=', 'employee1', '0981234567', 2, 1, GETDATE()),
('chef1@gmail.com', 'YWG0/U9EjSFSyFodYZOFjw==:tXL3wkW/NQN5w+Lcsta+gkN9ke/j9vQFxxK4LZZpkBU=', 'chef1', '0988887777', 2, 1, GETDATE()),
('cashier1@gmail.com', 'YWG0/U9EjSFSyFodYZOFjw==:tXL3wkW/NQN5w+Lcsta+gkN9ke/j9vQFxxK4LZZpkBU=', 'cashier1', '0977776666', 2, 1, GETDATE());

-- Insert into Admins
INSERT INTO Admins (AccountID, FullName, Position)
VALUES
(1, N'Phạm Gia Bảo', 'Quản Lý');

-- Insert into Employees
INSERT INTO Employees (AccountID, FullName, Position, HireDate)
VALUES
(2, N'Hạ Xuân Hậu', N'Phục vụ', GETDATE()),
(3, N'Lê Đình Hiếu', N'Đầu bếp', GETDATE()),
(4, N'Lê Văn Kiên', N'Thu ngân', GETDATE());
-- Insert into RequestStatuses
INSERT INTO RequestStatuses (StatusName)
VALUES
('Pending'),
('InProcess'),
('Accepted'),
('Cancelled');

-- Insert into RequestTypes
INSERT INTO RequestTypes (RequestTypeName)
VALUES
('Food Order'),
('Waiter Asssistant'),
('Checkout');

INSERT INTO ServiceReasons (ReasonText, Status)
VALUES
(N'DEFAULT', 1)

-- Insert into Tables
INSERT INTO Tables (AreaID, TableNumber, Status, SeatCapacity, TableStatus)
VALUES
(1, 'A1', 1, 4, 'Available'),
(2, 'B1', 1, 6, 'Available'),
(1, 'A2', 1, 2, 'Available'),
(2, 'B2', 1, 8, 'Available'),
(3, 'C1', 1, 10, 'Available');


-- Insert into MenuItems
INSERT INTO MenuItems (CategoryID, ItemName, Descriptions, Price, ImageURL, Status, IsHot, IsNew)
VALUES
-- Cà phê (CategoryID = 1)
(1, N'Cà Phê Sữa Đá', N'Cà phê đậm đà với sữa đặc', 39000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/3_MENU_NGUYEN_BAN/thumbs/270_crop_Phin_Sua_Da.jpg', 1, 1, 0),
(1, N'Cà Phê Đen Đá', N'Cà phê nguyên chất, không đường', 35000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/3_MENU_NGUYEN_BAN/thumbs/270_crop_Phin_Den_Da.jpg', 1, 1, 0),
(1, N'Bạc Xỉu', N'Cà phê pha với nhiều sữa', 39000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/3_MENU_NGUYEN_BAN/thumbs/270_crop_Bac_Xiu.jpg', 1, 1, 0),
(1, N'Espresso', N'Cà phê Ý đậm đặc', 45000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/06_2023/thumbs/270_crop_HLC_New_logo_5.1_Products__PHINDI_KEM_SUA.jpg', 1, 1, 0),
(1, N'Americano', N'Cà phê pha loãng với nước nóng', 45000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/04_2023/New_product/thumbs/270_crop_HLC_New_logo_5.1_Products__MOCHA.jpg', 1, 0, 0),
(1, N'Cappuccino', N'Cà phê sữa bọt béo ngậy', 49000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/04_2023/New_product/thumbs/270_crop_HLC_New_logo_5.1_Products__CARAMEL_MACCHIATTO.jpg', 1, 0, 0),
(1, N'Latte', N'Cà phê sữa nhẹ nhàng', 49000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/04_2023/New_product/thumbs/270_crop_HLC_New_logo_5.1_Products__CAPPUCINO.jpg', 1, 0, 0),
(1, N'Mocha', N'Cà phê sữa kết hợp với chocolate', 55000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/04_2023/New_product/thumbs/270_crop_HLC_New_logo_5.1_Products__AMERICANO_NONG.jpg', 1, 0, 1),

-- Trà (CategoryID = 2)
(2, N'Trà Sen Vàng', N'Trà thanh mát với hạt sen', 49000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/06_2023/thumbs/270_crop_HLC_New_logo_5.1_Products__TSV.jpg', 1, 0, 1),
(2, N'Trà Xanh Đào', N'Trà xanh thơm mát kết hợp với đào', 49000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/06_2023/thumbs/270_crop_HLC_New_logo_5.1_Products__TRA_XANH_DAU_DO.jpg', 1, 0, 0),
(2, N'Trà Thạch Vải', N'Trà hoa quả với thạch vải', 49000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/06_2023/thumbs/270_crop_HLC_New_logo_5.1_Products__TRA_TACH_VAI.jpg', 1, 0, 0),
(2, N'Trà Sữa Trân Châu', N'Trà sữa kết hợp trân châu đen', 45000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/3_MENU_NGUYEN_BAN/thumbs/270_crop_Freeze_Cookie___Cream.jpg', 1, 1, 0),

-- Freeze (CategoryID = 3)
(3, N'Freeze Trà Xanh', N'Đá xay trà xanh thơm ngon', 59000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/06_2023/thumbs/270_crop_HLC_New_logo_5.1_Products__FREEZE_TRA_XANH.jpg', 1, 0, 1),
(3, N'Freeze Chocolate', N'Chocolate đá xay béo ngậy', 59000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/04_2023/New_product/thumbs/270_crop_HLC_New_logo_5.1_Products__FREEZE_CHOCO.jpg', 1, 0, 0),
(3, N'Freeze Caramel Phin', N'Đá xay caramel hòa quyện với cà phê', 65000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/06_2023/thumbs/270_crop_HLC_New_logo_5.1_Products__CARAMEL_FREEZE_PHINDI.jpg', 1, 1, 1),

-- Bánh ngọt (CategoryID = 4)
(4, N'Bánh Mousse Chocolate', N'Bánh mousse chocolate mềm mịn', 39000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/4_FOOD_MENU/thumbs/270_crop_CROP_Cake_2png.jpg', 1, 0, 1),
(4, N'Bánh Tiramisu', N'Bánh tiramisu thơm ngon', 45000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/4_FOOD_MENU/thumbs/270_crop_CROP_Cake_3png.jpg', 1, 0, 0),
(4, N'Bánh Croissant', N'Bánh sừng bò bơ thơm béo', 29000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/4_FOOD_MENU/thumbs/270_crop_CROP_Cake_1png.jpg', 1, 0, 0),
(4, N'Bánh Mì Que', N'Bánh mì que giòn rụm', 25000, 'https://www.highlandscoffee.com.vn/vnt_upload/product/HLCPOSTOFFICE_DRAFT/PNG_FINAL/4_FOOD_MENU/thumbs/270_crop_CROP_Cake_2png.jpg', 1, 0, 0);

CREATE TABLE [dbo].[VnPayTransactions] (
    [Id]            INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [OrderDescription] NVARCHAR(255),
    [TransactionId] NVARCHAR(100),
    [OrderId]       NVARCHAR(100),
    [PaymentMethod] NVARCHAR(50),
    [PaymentId]     NVARCHAR(100),
    [DateCreated]   DATETIME      
);
