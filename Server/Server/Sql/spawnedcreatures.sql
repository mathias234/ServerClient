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

-- Dumping structure for table main.spawnedcreatures
CREATE TABLE IF NOT EXISTS `spawnedcreatures` (
  `instanceId` int(11) NOT NULL AUTO_INCREMENT,
  `templateId` int(11) NOT NULL DEFAULT '0',
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '0',
  `mapid` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`instanceId`),
  UNIQUE KEY `instanceId` (`instanceId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- Dumping data for table main.spawnedcreatures: ~0 rows (approximately)
/*!40000 ALTER TABLE `spawnedcreatures` DISABLE KEYS */;
REPLACE INTO `spawnedcreatures` (`instanceId`, `templateId`, `x`, `y`, `z`, `mapid`) VALUES
	(1, 1, 57.42, 58.73, 35.88, 1);
/*!40000 ALTER TABLE `spawnedcreatures` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
