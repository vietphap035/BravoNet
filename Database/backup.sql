-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: dacs1
-- ------------------------------------------------------
-- Server version	8.0.42

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `accounts`
--

DROP TABLE IF EXISTS `accounts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `accounts` (
  `UId` char(36) NOT NULL DEFAULT (uuid()),
  `username` varchar(50) NOT NULL,
  `pwd` varchar(100) NOT NULL,
  `roles` enum('customer','admin','staff') DEFAULT 'customer',
  `is_online` tinyint(1) DEFAULT '0',
  `last_login` datetime DEFAULT NULL,
  `last_logout` datetime DEFAULT NULL,
  PRIMARY KEY (`UId`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `accounts`
--

LOCK TABLES `accounts` WRITE;
/*!40000 ALTER TABLE `accounts` DISABLE KEYS */;
INSERT INTO `accounts` VALUES ('068ff82d-689d-11f0-85e3-505a65037030','phapvn','1','customer',0,NULL,NULL),('54a55dcf-5b57-11f0-9980-505a65037030','0775424803','1234','admin',0,NULL,NULL);
/*!40000 ALTER TABLE `accounts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customer_time`
--

DROP TABLE IF EXISTS `customer_time`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customer_time` (
  `UId` char(36) NOT NULL,
  `existing_time` int DEFAULT NULL,
  KEY `UId` (`UId`),
  CONSTRAINT `customer_time_ibfk_1` FOREIGN KEY (`UId`) REFERENCES `accounts` (`UId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customer_time`
--

LOCK TABLES `customer_time` WRITE;
/*!40000 ALTER TABLE `customer_time` DISABLE KEYS */;
INSERT INTO `customer_time` VALUES ('54a55dcf-5b57-11f0-9980-505a65037030',3494535),('068ff82d-689d-11f0-85e3-505a65037030',100);
/*!40000 ALTER TABLE `customer_time` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `nap_tien`
--

DROP TABLE IF EXISTS `nap_tien`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `nap_tien` (
  `deposit_id` int NOT NULL AUTO_INCREMENT,
  `UId` char(36) NOT NULL,
  `so_tien` decimal(10,0) NOT NULL,
  `thoi_gian_nap` datetime NOT NULL,
  PRIMARY KEY (`deposit_id`),
  KEY `UId` (`UId`),
  CONSTRAINT `nap_tien_ibfk_1` FOREIGN KEY (`UId`) REFERENCES `accounts` (`UId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `nap_tien`
--

LOCK TABLES `nap_tien` WRITE;
/*!40000 ALTER TABLE `nap_tien` DISABLE KEYS */;
INSERT INTO `nap_tien` VALUES (3,'068ff82d-689d-11f0-85e3-505a65037030',100000,'2025-07-24 21:46:48');
/*!40000 ALTER TABLE `nap_tien` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `order_id` char(36) NOT NULL DEFAULT (uuid()),
  `UId` char(36) DEFAULT NULL,
  `order_date` datetime DEFAULT CURRENT_TIMESTAMP,
  `order_status` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`order_id`),
  KEY `UId` (`UId`),
  CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`UId`) REFERENCES `accounts` (`UId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES ('104e4603-6868-11f0-85e3-505a65037030','54a55dcf-5b57-11f0-9980-505a65037030','2025-07-24 15:27:40',0);
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders_items`
--

DROP TABLE IF EXISTS `orders_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders_items` (
  `order_id` char(36) NOT NULL,
  `product_id` int NOT NULL,
  `quantity` int NOT NULL,
  `price_at_order` decimal(10,2) NOT NULL,
  PRIMARY KEY (`order_id`,`product_id`),
  KEY `product_id` (`product_id`),
  CONSTRAINT `orders_items_ibfk_1` FOREIGN KEY (`order_id`) REFERENCES `orders` (`order_id`),
  CONSTRAINT `orders_items_ibfk_2` FOREIGN KEY (`product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `orders_items_chk_1` CHECK ((`quantity` > 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders_items`
--

LOCK TABLES `orders_items` WRITE;
/*!40000 ALTER TABLE `orders_items` DISABLE KEYS */;
INSERT INTO `orders_items` VALUES ('104e4603-6868-11f0-85e3-505a65037030',1,1,100000.00);
/*!40000 ALTER TABLE `orders_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pc`
--

DROP TABLE IF EXISTS `pc`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pc` (
  `pc_code` char(36) NOT NULL DEFAULT (uuid()),
  `UId` char(36) DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT '0',
  `pc_number` int DEFAULT NULL,
  PRIMARY KEY (`pc_code`),
  KEY `UId` (`UId`),
  CONSTRAINT `pc_ibfk_1` FOREIGN KEY (`UId`) REFERENCES `accounts` (`UId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pc`
--

LOCK TABLES `pc` WRITE;
/*!40000 ALTER TABLE `pc` DISABLE KEYS */;
INSERT INTO `pc` VALUES ('05a00eb1-5bd0-11f0-9980-505a65037030',NULL,0,1),('05a0391d-5bd0-11f0-9980-505a65037030',NULL,0,2),('05a04e6a-5bd0-11f0-9980-505a65037030',NULL,0,3),('05a050c7-5bd0-11f0-9980-505a65037030',NULL,0,4),('05a0513c-5bd0-11f0-9980-505a65037030',NULL,0,5),('05a051c8-5bd0-11f0-9980-505a65037030',NULL,0,6),('05a0522f-5bd0-11f0-9980-505a65037030',NULL,0,7),('05a05295-5bd0-11f0-9980-505a65037030',NULL,0,8),('05a052fa-5bd0-11f0-9980-505a65037030',NULL,0,9),('05a0535c-5bd0-11f0-9980-505a65037030',NULL,0,10),('05a053e3-5bd0-11f0-9980-505a65037030',NULL,0,11),('05a05440-5bd0-11f0-9980-505a65037030',NULL,0,12),('05a0549f-5bd0-11f0-9980-505a65037030',NULL,0,13),('05a05500-5bd0-11f0-9980-505a65037030',NULL,0,14),('05a0555e-5bd0-11f0-9980-505a65037030',NULL,0,15),('05a055b8-5bd0-11f0-9980-505a65037030',NULL,0,16),('05a0560f-5bd0-11f0-9980-505a65037030',NULL,0,17),('05a05678-5bd0-11f0-9980-505a65037030',NULL,0,18),('05a056d1-5bd0-11f0-9980-505a65037030',NULL,0,19),('05a0572b-5bd0-11f0-9980-505a65037030',NULL,0,20),('05a05781-5bd0-11f0-9980-505a65037030',NULL,0,21),('05a057ec-5bd0-11f0-9980-505a65037030',NULL,0,22),('05a05845-5bd0-11f0-9980-505a65037030',NULL,0,23),('05a058a0-5bd0-11f0-9980-505a65037030',NULL,0,24);
/*!40000 ALTER TABLE `pc` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `product_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `price` decimal(10,2) NOT NULL,
  `quantity` int DEFAULT '0',
  PRIMARY KEY (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'sa xao sa ot',100000.00,10000),(2,'bun dau tuong ot',10000.00,1111),(3,'ot sao sa ot',10000.00,10101);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `staffs`
--

DROP TABLE IF EXISTS `staffs`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `staffs` (
  `UId` char(36) NOT NULL,
  `Full_name` varchar(60) NOT NULL,
  `basic_salary` decimal(10,2) NOT NULL,
  `bonus_salary` decimal(10,2) DEFAULT '0.00',
  `work_time` int DEFAULT '0',
  KEY `UId` (`UId`),
  CONSTRAINT `staffs_ibfk_1` FOREIGN KEY (`UId`) REFERENCES `accounts` (`UId`) ON DELETE CASCADE,
  CONSTRAINT `staffs_chk_1` CHECK ((`basic_salary` >= 0)),
  CONSTRAINT `staffs_chk_2` CHECK ((`bonus_salary` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `staffs`
--

LOCK TABLES `staffs` WRITE;
/*!40000 ALTER TABLE `staffs` DISABLE KEYS */;
/*!40000 ALTER TABLE `staffs` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-07-25 12:14:06
