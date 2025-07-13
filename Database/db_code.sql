drop database dacs1;
create database dacs1;
use dacs1;
CREATE TABLE accounts (
    UId CHAR(36) PRIMARY KEY DEFAULT (UUID()),
    username VARCHAR(50) NOT NULL UNIQUE,
    pwd VARCHAR(100) NOT NULL,
    roles ENUM('customer', 'admin', 'staff') DEFAULT 'customer',
    is_online BOOLEAN DEFAULT FALSE
);
CREATE TABLE pc (
    pc_code CHAR(36) PRIMARY KEY DEFAULT (UUID()),
    UId CHAR(36),
    FOREIGN KEY (UId) REFERENCES accounts(UId)
);
CREATE TABLE orders (
    order_id CHAR(36) PRIMARY KEY DEFAULT (UUID()),
    UId CHAR(36),
    order_date DATETIME DEFAULT CURRENT_TIMESTAMP,
    order_status BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (UId) REFERENCES accounts(UId)
);
CREATE TABLE products (
    product_id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL,
    price DECIMAL(10,2) NOT NULL
);
CREATE TABLE orders_items (
    order_id CHAR(36),
    product_id INT,
    quantity INT NOT NULL CHECK (quantity > 0),
    price_at_order DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders(order_id),
    FOREIGN KEY (product_id) REFERENCES products(product_id)
);
CREATE TABLE staffs (
    staff_id CHAR(36) PRIMARY KEY DEFAULT (UUID()),
    UId CHAR(36) NOT NULL,
    Full_name varchar(60) not null,
    basic_salary DECIMAL(10,2) NOT NULL CHECK (basic_salary >= 0),
    bonus_salary DECIMAL(10,2) DEFAULT 0 CHECK (bonus_salary >= 0),
    work_time TIME,
    FOREIGN KEY (UId) REFERENCES accounts(UId) ON DELETE CASCADE
);
create table customer_time(
UId CHAR(36) NOT NULL,
existing_time Time not null,
FOREIGN KEY (UId) REFERENCES accounts(UId)
)
