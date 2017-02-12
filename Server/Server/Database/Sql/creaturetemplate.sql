-- --------------------------------------------------------
-- Host:                         localhost
-- Server version:               5.7.17-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Dumping structure for table main.creaturetemplate
CREATE TABLE IF NOT EXISTS `creaturetemplate` (
  `templateId` int(11) NOT NULL AUTO_INCREMENT,
  `name` text,
  `health` int(11) DEFAULT NULL,
  `minlevel` int(11) DEFAULT NULL,
  `maxlevel` int(11) DEFAULT NULL,
  PRIMARY KEY (`templateId`),
  UNIQUE KEY `templateId` (`templateId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- Dumping data for table main.creaturetemplate: ~0 rows (approximately)
/*!40000 ALTER TABLE `creaturetemplate` DISABLE KEYS */;
REPLACE INTO `creaturetemplate` (`templateId`, `name`, `health`, `minlevel`, `maxlevel`) VALUES
	(1, 'Test Creature', 300, 20, 25);
/*!40000 ALTER TABLE `creaturetemplate` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
