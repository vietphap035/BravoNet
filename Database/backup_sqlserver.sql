-- 1. Tạo Database
CREATE DATABASE LTCSharp;
GO
USE LTCSharp;
GO

-- 2. Table: accounts
CREATE TABLE accounts (
    UId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    username NVARCHAR(50) NOT NULL,
    pwd NVARCHAR(100) NOT NULL,
    roles NVARCHAR(20) CHECK (roles IN ('customer', 'admin', 'staff')) DEFAULT 'customer',
    is_online TINYINT DEFAULT 0,
    last_login DATETIME DEFAULT NULL,
    last_logout DATETIME DEFAULT NULL,
    PRIMARY KEY (UId),
    UNIQUE (username)
);

-- 3. Table: customer_time
CREATE TABLE customer_time (
    UId UNIQUEIDENTIFIER NOT NULL,
    existing_time INT DEFAULT NULL,
    FOREIGN KEY (UId) REFERENCES accounts (UId)
);

-- 4. Table: products
CREATE TABLE products (
    product_id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    quantity INT DEFAULT 0
);

-- 5. Table: nap_tien
CREATE TABLE nap_tien (
    deposit_id INT IDENTITY(1,1) PRIMARY KEY,
    UId UNIQUEIDENTIFIER NOT NULL,
    so_tien DECIMAL(10, 0) NOT NULL,
    thoi_gian_nap DATETIME NOT NULL,
    FOREIGN KEY (UId) REFERENCES accounts (UId)
);

-- 6. Table: orders
CREATE TABLE orders (
    order_id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    UId UNIQUEIDENTIFIER DEFAULT NULL,
    order_date DATETIME DEFAULT GETDATE(),
    order_status TINYINT DEFAULT 0,
    PRIMARY KEY (order_id),
    FOREIGN KEY (UId) REFERENCES accounts (UId)
);

-- 7. Table: orders_items
CREATE TABLE orders_items (
    order_id UNIQUEIDENTIFIER NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL CHECK (quantity > 0),
    price_at_order DECIMAL(10, 2) NOT NULL,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders (order_id),
    FOREIGN KEY (product_id) REFERENCES products (product_id)
);

-- 8. Table: pc
CREATE TABLE pc (
    pc_code UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    UId UNIQUEIDENTIFIER DEFAULT NULL,
    is_active TINYINT DEFAULT 0,
    pc_number INT DEFAULT NULL,
    PRIMARY KEY (pc_code),
    FOREIGN KEY (UId) REFERENCES accounts (UId)
);

-- 9. Table: staffs
CREATE TABLE staffs (
    UId UNIQUEIDENTIFIER NOT NULL,
    Full_name NVARCHAR(60) NOT NULL,
    basic_salary DECIMAL(10, 2) NOT NULL CHECK (basic_salary >= 0),
    bonus_salary DECIMAL(10, 2) DEFAULT 0.00 CHECK (bonus_salary >= 0),
    work_time INT DEFAULT 0,
    FOREIGN KEY (UId) REFERENCES accounts (UId) ON DELETE CASCADE
);
GO

-- ------------------------------------------------------
-- NHẬP DỮ LIỆU MẪU (Dữ liệu từ file dump của bạn)
-- ------------------------------------------------------

-- Chèn vào accounts
INSERT INTO accounts (UId, username, pwd, roles) VALUES 
('068ff82d-689d-11f0-85e3-505a65037030', 'phapvn', '1', 'customer'),
('54a55dcf-5b57-11f0-9980-505a65037030', '0775424803', '1234', 'admin');

-- Chèn vào customer_time
INSERT INTO customer_time (UId, existing_time) VALUES 
('54a55dcf-5b57-11f0-9980-505a65037030', 3494535),
('068ff82d-689d-11f0-85e3-505a65037030', 100);

-- Chèn vào products
INSERT INTO products (name, price, quantity) VALUES 
(N'sa xao sa ot', 100000.00, 10000),
(N'bun dau tuong ot', 10000.00, 1111),
(N'ot sao sa ot', 10000.00, 10101);

-- Chèn vào nap_tien
INSERT INTO nap_tien (UId, so_tien, thoi_gian_nap) VALUES 
('068ff82d-689d-11f0-85e3-505a65037030', 100000, '2025-07-24 21:46:48');

-- Chèn vào orders
INSERT INTO orders (order_id, UId, order_date, order_status) VALUES 
('104e4603-6868-11f0-85e3-505a65037030', '54a55dcf-5b57-11f0-9980-505a65037030', '2025-07-24 15:27:40', 0);

-- Chèn vào orders_items (Lấy product_id 1 từ bảng products)
INSERT INTO orders_items (order_id, product_id, quantity, price_at_order) VALUES 
('104e4603-6868-11f0-85e3-505a65037030', 1, 1, 100000.00);

-- Chèn vào pc
INSERT INTO pc (pc_number) VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10);