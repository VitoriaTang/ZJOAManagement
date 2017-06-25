﻿-- ----------------------------
-- __migrationhistory
-- ---------------------------- 
CREATE TABLE `__migrationhistory` (
  `MigrationId` varchar(100) NOT NULL,
  `ContextKey` varchar(200) NOT NULL,
  `Model` longblob NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`,`ContextKey`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ;

-- ----------------------------
-- action_additionals
-- ---------------------------- 
CREATE TABLE `action_additionals` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Content` varchar(512) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 ;

-- ----------------------------
-- action_operators
-- ---------------------------- 
CREATE TABLE `action_operators` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ActionId` int(11) NOT NULL,
  `Operator` varchar(16) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=1186516 DEFAULT CHARSET=utf8;

-- ----------------------------
-- aspnetroles
-- ---------------------------- 
CREATE TABLE `aspnetroles` (
  `Id` varchar(128) NOT NULL,
  `Name` longtext NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 ;

-- ----------------------------
-- aspnetuserclaims
-- ---------------------------- 
CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` longtext,
  `ClaimValue` longtext,
  `User_Id` varchar(128) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id` (`Id`),
  KEY `User_Id` (`User_Id`),
  CONSTRAINT `IdentityUserClaim_User` FOREIGN KEY (`User_Id`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION) ENGINE=InnoDB DEFAULT CHARSET=utf8 ;

-- ----------------------------
-- aspnetuserlogins
-- ----------------------------
CREATE TABLE `aspnetuserlogins` (
  `UserId` varchar(128) NOT NULL,
  `LoginProvider` varchar(128) NOT NULL,
  `ProviderKey` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`LoginProvider`,`ProviderKey`),
  CONSTRAINT `IdentityUserLogin_User` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION) ENGINE=InnoDB DEFAULT CHARSET=utf8 ;

-- ----------------------------
-- aspnetuserroles
-- ----------------------------
CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(128) NOT NULL,
  `RoleId` varchar(128) NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IdentityUserRole_Role` (`RoleId`),
  CONSTRAINT `IdentityUserRole_Role` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION,
  CONSTRAINT `IdentityUserRole_User` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE ON UPDATE NO ACTION
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- aspnetusers
-- ----------------------------
CREATE TABLE `aspnetusers` (
  `Id` varchar(128) NOT NULL,
  `UserName` longtext,
  `PasswordHash` longtext,
  `SecurityStamp` longtext,
  `Email` longtext,
  `EncPassword` longtext,
  `Discriminator` varchar(128) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- departments
-- ----------------------------
CREATE TABLE `departments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `ParentId` int(11) NOT NULL DEFAULT '0',
  `ManagerId` int(11) NOT NULL DEFAULT '0',
  `Telephone` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8;

-- ----------------------------
-- employeedepartments
-- ----------------------------
CREATE TABLE `employeedepartments` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Employee_Id` int(11) NOT NULL,
  `Department_Id` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8;

-- ----------------------------
-- employees
-- ----------------------------
CREATE TABLE `employees` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Telephone` varchar(50) DEFAULT NULL,
  `Email` varchar(128) DEFAULT NULL,
  `Address` varchar(256) DEFAULT NULL,
  `Encode` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Encode` (`Encode`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8 ;

-- ----------------------------
-- machine_assigns
-- ----------------------------
CREATE TABLE `machine_assigns` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `MachineId` int(11) NOT NULL,
  `AssignType` int(11) NOT NULL DEFAULT '0',
  `AssignTime` datetime NOT NULL,
  `AssignComments` varchar(512) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=utf8 ;

-- ----------------------------
-- machine_users
-- ----------------------------
CREATE TABLE `machine_users` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `AssignId` int(11) NOT NULL,
  `UserEncode` varchar(16) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8;


-- ----------------------------
-- machines
-- ----------------------------
CREATE TABLE `machines` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Encode` varchar(16) NOT NULL,
  `Name` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8;

-- ----------------------------
-- product_actions
-- ----------------------------
CREATE TABLE `product_actions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ActionType` int(11) NOT NULL,
  `ActionTime` datetime NOT NULL,
  `ActionComments` varchar(512) DEFAULT NULL,
  `ProductId` int(11) DEFAULT NULL,
  `AdditionalId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=176 DEFAULT CHARSET=utf8;

-- ----------------------------
-- productadditions
-- ----------------------------
CREATE TABLE `productadditions` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `AdditionGuid` varchar(36) DEFAULT NULL,
  `TrackNumber` varchar(16) DEFAULT NULL,
  `Sender` varchar(36) DEFAULT NULL,
  `Receiver` varchar(16) DEFAULT NULL,
  `Departure` varchar(200) DEFAULT NULL,
  `Destination` varchar(200) DEFAULT NULL,
  `Comments` varchar(512) DEFAULT NULL,
  `SenderTelephone` varchar(64) DEFAULT NULL,
  `ReceiverTelephone` varchar(64) DEFAULT NULL,
  `ProductGuid` varchar(36) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- productbases
-- ----------------------------
CREATE TABLE `productbases` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Number` varchar(2) NOT NULL,
  `ParentNumber` varchar(2) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=107 DEFAULT CHARSET=utf8;

-- ----------------------------
-- products
-- ----------------------------
CREATE TABLE `products` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Name` varchar(100) NOT NULL,
  `Description` varchar(500) DEFAULT NULL,
  `ProductBaseNumber` varchar(2) NOT NULL,
  `BatchNumber` varchar(3) NOT NULL,
  `SerialNumber` varchar(3) NOT NULL,
  `YearNumber` int(2) DEFAULT NULL,
  `Status` int(1) DEFAULT NULL,
  `ParentId` int(11) DEFAULT NULL,
  `BoxId` int(11) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `NumberIndex` (`ProductBaseNumber`,`BatchNumber`,`SerialNumber`)
) ENGINE=InnoDB AUTO_INCREMENT=49 DEFAULT CHARSET=utf8 ;
