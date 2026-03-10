CREATE DATABASE  IF NOT EXISTS `shoes` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `shoes`;
-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: shoes
-- ------------------------------------------------------
-- Server version	8.0.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `category`
--

DROP TABLE IF EXISTS `category`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `category` (
  `category_id` int NOT NULL AUTO_INCREMENT,
  `category_title` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `category`
--

LOCK TABLES `category` WRITE;
/*!40000 ALTER TABLE `category` DISABLE KEYS */;
INSERT INTO `category` VALUES (1,'Женская обувь'),(2,'Мужская обувь');
/*!40000 ALTER TABLE `category` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `item`
--

DROP TABLE IF EXISTS `item`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `item` (
  `order_id` int NOT NULL,
  `item_articul` varchar(7) NOT NULL,
  `item_count` int DEFAULT NULL,
  PRIMARY KEY (`order_id`,`item_articul`),
  KEY `tovar_fk_idx` (`item_articul`),
  CONSTRAINT `order_fk` FOREIGN KEY (`order_id`) REFERENCES `order` (`order_id`),
  CONSTRAINT `tovar_fk` FOREIGN KEY (`item_articul`) REFERENCES `tovar` (`tovar_articul`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `item`
--

LOCK TABLES `item` WRITE;
/*!40000 ALTER TABLE `item` DISABLE KEYS */;
INSERT INTO `item` VALUES (1,'F635R4',2),(1,'А112Т4',2),(2,'G783F5',1),(2,'H782T5',1),(3,'D572U8',10),(3,'J384T6',10),(4,'D329H3',4),(4,'F572H7',5),(5,'F635R4',2),(5,'А112Т4',2),(6,'G783F5',1),(6,'H782T5',1),(7,'D572U8',10),(7,'J384T6',10),(8,'D329H3',4),(8,'F572H7',5),(9,'B320R5',5),(9,'G432E4',1),(10,'E482R4',5),(10,'S213E3',5);
/*!40000 ALTER TABLE `item` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `manufactur`
--

DROP TABLE IF EXISTS `manufactur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manufactur` (
  `manufactur_id` int NOT NULL AUTO_INCREMENT,
  `manufactur_name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`manufactur_id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manufactur`
--

LOCK TABLES `manufactur` WRITE;
/*!40000 ALTER TABLE `manufactur` DISABLE KEYS */;
INSERT INTO `manufactur` VALUES (1,'Alessio Nesca'),(2,'CROSBY'),(3,'Kari'),(4,'Marco Tozzi'),(5,'Rieker'),(6,'Рос');
/*!40000 ALTER TABLE `manufactur` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order`
--

DROP TABLE IF EXISTS `order`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order` (
  `order_id` int NOT NULL AUTO_INCREMENT,
  `order_item` int DEFAULT NULL,
  `order_date` date DEFAULT NULL,
  `order_delivery_date` date DEFAULT NULL,
  `order_adress` int DEFAULT NULL,
  `order_user` int DEFAULT NULL,
  `order_code` int DEFAULT NULL,
  `order_status` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`order_id`),
  KEY `user_fk_idx` (`order_user`),
  KEY `pick_point_fk_idx` (`order_adress`),
  CONSTRAINT `pick_point_fk` FOREIGN KEY (`order_adress`) REFERENCES `pick_point` (`pick_point_id`),
  CONSTRAINT `user_fk` FOREIGN KEY (`order_user`) REFERENCES `user` (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order`
--

LOCK TABLES `order` WRITE;
/*!40000 ALTER TABLE `order` DISABLE KEYS */;
INSERT INTO `order` VALUES (1,1,'2025-02-27','2025-04-20',1,4,901,'Завершен'),(2,2,'2022-09-28','2025-04-21',11,1,902,'Завершен'),(3,3,'2025-03-21','2025-04-22',2,2,903,'Завершен'),(4,4,'2025-02-20','2025-04-23',11,3,904,'Завершен'),(5,5,'2025-03-17','2025-04-24',2,4,905,'Завершен'),(6,6,'2025-03-01','2025-04-25',15,1,906,'Завершен'),(7,7,'2025-03-17','2025-04-26',3,2,907,'Завершен'),(8,8,'2025-03-31','2025-04-27',19,3,908,'Новый'),(9,9,'2025-04-02','2025-04-28',5,4,909,'Новый'),(10,10,'2025-04-03','2025-04-29',19,4,910,'Новый');
/*!40000 ALTER TABLE `order` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `pick_point`
--

DROP TABLE IF EXISTS `pick_point`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `pick_point` (
  `pick_point_id` int NOT NULL AUTO_INCREMENT,
  `pick_point_index` varchar(6) DEFAULT NULL,
  `pick_point_city` varchar(65) DEFAULT NULL,
  `pick_point_street` varchar(85) DEFAULT NULL,
  `pick_point_house` int DEFAULT NULL,
  PRIMARY KEY (`pick_point_id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `pick_point`
--

LOCK TABLES `pick_point` WRITE;
/*!40000 ALTER TABLE `pick_point` DISABLE KEYS */;
INSERT INTO `pick_point` VALUES (1,'420151','г. Лесной','ул. Вишневая',32),(2,'125061','г. Лесной','ул. Подгорная',8),(3,'630370','г. Лесной','ул. Шоссейная',24),(4,'400562','г. Лесной','ул. Зеленая',32),(5,'614510','г. Лесной','ул. Маяковского',47),(6,'410542','г. Лесной','ул. Светлая',46),(7,'620839','г. Лесной','ул. Цветочная',8),(8,'443890','г. Лесной','ул. Коммунистическая',1),(9,'603379','г. Лесной','ул. Спортивная',46),(10,'603721','г. Лесной','ул. Гоголя',41),(11,'410172','г. Лесной','ул. Северная',13),(12,'614611','г. Лесной','ул. Молодежная',50),(13,'454311','г.Лесной','ул. Новая',19),(14,'660007','г.Лесной','ул. Октябрьская',19),(15,'603036','г. Лесной','ул. Садовая',4),(16,'394060','г.Лесной','ул. Фрунзе',43),(17,'410661','г. Лесной','ул. Школьная',50),(18,'625590','г. Лесной','ул. Коммунистическая',20),(19,'625683','г. Лесной','ул. 8 Марта',NULL),(20,'450983','г.Лесной','ул. Комсомольская',26),(21,'394782','г. Лесной','ул. Чехова',3),(22,'603002','г. Лесной','ул. Дзержинского',28),(23,'450558','г. Лесной','ул. Набережная',30),(24,'344288','г. Лесной','ул. Чехова',1),(25,'614164','г.Лесной','  ул. Степная',30),(26,'394242','г. Лесной','ул. Коммунистическая',43),(27,'660540','г. Лесной','ул. Солнечная',25),(28,'125837','г. Лесной','ул. Шоссейная',40),(29,'125703','г. Лесной','ул. Партизанская',49),(30,'625283','г. Лесной','ул. Победы',46),(31,'614753','г. Лесной','ул. Полевая',35),(32,'426030','г. Лесной','ул. Маяковского',44),(33,'450375','г. Лесной','ул. Клубная',44),(34,'625560','г. Лесной','ул. Некрасова',12),(35,'630201','г. Лесной','ул. Комсомольская',17),(36,'190949','г. Лесной','ул. Мичурина',26);
/*!40000 ALTER TABLE `pick_point` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `supplier`
--

DROP TABLE IF EXISTS `supplier`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `supplier` (
  `supplier_id` int NOT NULL AUTO_INCREMENT,
  `supplier_title` varchar(15) DEFAULT NULL,
  PRIMARY KEY (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `supplier`
--

LOCK TABLES `supplier` WRITE;
/*!40000 ALTER TABLE `supplier` DISABLE KEYS */;
INSERT INTO `supplier` VALUES (1,'Kari'),(2,'Обувь для вас');
/*!40000 ALTER TABLE `supplier` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `tovar`
--

DROP TABLE IF EXISTS `tovar`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tovar` (
  `tovar_articul` varchar(45) NOT NULL,
  `tovar_title` varchar(45) DEFAULT NULL,
  `tovar_unit` varchar(5) DEFAULT NULL,
  `tovar_cost` int DEFAULT NULL,
  `tovar_supplier` int DEFAULT NULL,
  `tovar_manufactor` int DEFAULT NULL,
  `tovar_category` int DEFAULT NULL,
  `tovar_sale` int DEFAULT NULL,
  `tovar_count` int DEFAULT NULL,
  `tovar_description` varchar(225) DEFAULT NULL,
  `tovar_photo` varchar(8) DEFAULT NULL,
  PRIMARY KEY (`tovar_articul`),
  KEY `supplier_fk_idx` (`tovar_supplier`),
  KEY `manufactur_fk_idx` (`tovar_manufactor`),
  KEY `category_fk_idx` (`tovar_category`),
  KEY `Item_fk_idx` (`tovar_articul`),
  CONSTRAINT `category_fk` FOREIGN KEY (`tovar_category`) REFERENCES `category` (`category_id`),
  CONSTRAINT `manufactur_fk` FOREIGN KEY (`tovar_manufactor`) REFERENCES `manufactur` (`manufactur_id`),
  CONSTRAINT `supplier_fk` FOREIGN KEY (`tovar_supplier`) REFERENCES `supplier` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `tovar`
--

LOCK TABLES `tovar` WRITE;
/*!40000 ALTER TABLE `tovar` DISABLE KEYS */;
INSERT INTO `tovar` VALUES ('B320R5','Туфли','шт.',4300,2,5,1,2,6,'Туфли Rieker женские демисезонные, размер 41, цвет коричневый','9.jpg'),('B431R5','Ботинки','шт.',2700,1,5,2,2,5,'Мужские кожаные ботинки/мужские ботинки','нет'),('C436G5','Ботинки','шт.',10200,1,1,1,15,9,'Ботинки женские, ARGO, размер 40','нет'),('D268G5','Туфли','шт.',4399,2,5,1,3,12,'Туфли Rieker женские демисезонные, размер 36, цвет коричневый','нет'),('D329H3','Полуботинки','шт.',1890,1,1,1,4,4,'Полуботинки Alessio Nesca женские 3-30797-47, размер 37, цвет: бордовый','8.jpg'),('D364R4','Туфли','шт.',12400,1,3,1,16,5,'Туфли Luiza Belly женские Kate-lazo черные из натуральной замши','нет'),('D572U8','Кроссовки','шт.',4100,2,6,2,3,6,'129615-4 Кроссовки мужские','6.jpg'),('E482R4','Полуботинки','шт.',1800,2,3,1,2,14,'Полуботинки kari женские MYZ20S-149, размер 41, цвет: черный','нет'),('F427R5','Ботинки','шт.',11800,1,5,1,15,11,'Ботинки на молнии с декоративной пряжкой FRAU','нет'),('F572H7','Туфли','шт.',2700,2,4,1,2,14,'Туфли Marco Tozzi женские летние, размер 39, цвет черный','7.jpg'),('F635R4','Ботинки','шт.',3244,1,4,1,2,13,'Ботинки Marco Tozzi женские демисезонные, размер 39, цвет бежевый','2.jpg'),('G432E4','Туфли','шт.',2800,1,3,1,3,15,'Туфли kari женские TR-YR-413017, размер 37, цвет: черный','10.jpg'),('G531F4','Ботинки','шт.',6600,2,3,1,12,9,'Ботинки женские зимние ROMER арт. 893167-01 Черный','нет'),('G783F5','Ботинки','шт.',5900,1,6,2,2,8,'Мужские ботинки Рос-Обувь кожаные с натуральным мехом','4.jpg'),('H535R5','Ботинки','шт.',2300,1,5,1,2,7,'Женские Ботинки демисезонные','нет'),('H782T5','Туфли','шт.',4499,2,3,2,4,5,'Туфли kari мужские классика MYZ21AW-450A, размер 43, цвет: черный','3.jpg'),('J384T6','Ботинки','шт.',3800,1,5,2,2,16,'B3430/14 Полуботинки мужские Rieker','5.jpg'),('J542F5','Тапочки','шт.',500,1,3,2,13,0,'Тапочки мужские Арт.70701-55-67син р.41','нет'),('K345R4','Полуботинки','шт.',2100,2,2,2,2,3,'407700/01-02 Полуботинки мужские CROSBY','нет'),('K358H6','Тапочки','шт.',599,1,5,2,20,2,'Тапочки мужские син р.41','нет'),('L754R4','Полуботинки','шт.',1700,2,3,1,2,7,'Полуботинки kari женские WB2020SS-26, размер 38, цвет: черный','нет'),('M542T5','Кроссовки','шт.',2800,1,5,2,18,3,'Кроссовки мужские TOFA','нет'),('N457T5','Полуботинки','шт.',4600,2,2,1,3,13,'Полуботинки Ботинки черные зимние, мех','нет'),('O754F4','Туфли','шт.',5400,2,5,1,4,18,'Туфли женские демисезонные Rieker артикул 55073-68/37','нет'),('P764G4','Туфли','шт.',6800,2,2,1,15,15,'Туфли женские, ARGO, размер 38','нет'),('S213E3','Полуботинки','шт.',2156,1,2,2,3,6,'407700/01-01 Полуботинки мужские CROSBY','нет'),('S326R5','Тапочки','шт.',9900,1,2,2,17,15,'Мужские кожаные тапочки \"Профиль С.Дали\" ','нет'),('S634B5','Кеды','шт.',5500,1,2,2,3,0,'Кеды Caprice мужские демисезонные, размер 42, цвет черный','нет'),('T324F5','Сапоги','шт.',4699,2,2,1,2,5,'Сапоги замша Цвет: синий','нет'),('А112Т4','Ботинки','шт.',4990,2,3,1,3,6,'Женские Ботинки демисезонные kari','1.jpg');
/*!40000 ALTER TABLE `tovar` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `user_role` varchar(65) DEFAULT NULL,
  `user_firstname` varchar(45) DEFAULT NULL,
  `user_name` varchar(45) DEFAULT NULL,
  `user_lastname` varchar(45) DEFAULT NULL,
  `user_login` varchar(225) DEFAULT NULL,
  `user_password` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `user`
--

LOCK TABLES `user` WRITE;
/*!40000 ALTER TABLE `user` DISABLE KEYS */;
INSERT INTO `user` VALUES (1,'Администратор','Никифорова','Весения','Николаевна','94d5ous@gmail.com','uzWC67'),(2,'Администратор','Сазонов','Руслан','Германович','uth4iz@mail.com','2L6KZG'),(3,'Администратор','Одинцов','Серафим','Артёмович','yzls62@outlook.com','JlFRCZ'),(4,'Менеджер','Степанов','Михаил','Артёмович','1diph5e@tutanota.com','8ntwUp'),(5,'Менеджер','Ворсин','Петр','Евгеньевич','tjde7c@yahoo.com','YOyhfR'),(6,'Менеджер','Старикова','Елена','Павловна','wpmrc3do@tutanota.com','RSbvHv'),(7,'Авторизированный клиент','Михайлюк','Анна','Вячеславовна','5d4zbu@tutanota.com','rwVDh9'),(8,'Авторизированный клиент','Ситдикова','Елена','Анатольевна','ptec8ym@yahoo.com','LdNyos'),(9,'Авторизированный клиент','Ворсин','Петр','Евгеньевич','1qz4kw@mail.com','gynQMT'),(10,'Авторизированный клиент','Старикова','Елена','Павловна','4np6se@mail.com','AtnDjr');
/*!40000 ALTER TABLE `user` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-01-19  0:04:36
