CREATE DATABASE `prodigy`;

CREATE TABLE `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `username` varchar(45) NOT NULL,
  `wallet_address` varchar(60) NOT NULL,
  `last_login_on` datetime DEFAULT NULL,
  `created_on` varchar(45) NOT NULL DEFAULT 'NOW()',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `prodigy`.`user_seesions` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `session_id` VARCHAR(45) NOT NULL,
  `user_id` INT NOT NULL,
  `expires_on` DATETIME NOT NULL DEFAULT NOW(),
  PRIMARY KEY (`id`)
)  ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;