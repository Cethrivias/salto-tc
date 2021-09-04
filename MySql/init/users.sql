SET FOREIGN_KEY_CHECKS=0;

CREATE TABLE users (
  id int(10) unsigned NOT NULL AUTO_INCREMENT,
  username varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  password varchar(255) COLLATE utf8mb4_unicode_ci NOT NULL,
  tag_id int(10) unsigned DEFAULT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY users_UN (username),
  KEY users_FK (tag_id),
  CONSTRAINT users_FK FOREIGN KEY (tag_id) REFERENCES tags (id) ON DELETE SET NULL ON UPDATE CASCADE
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

CREATE TABLE tags (
	id INT UNSIGNED auto_increment NOT NULL,
	name VARCHAR(255) NULL,
	CONSTRAINT tags_PK PRIMARY KEY (id)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

CREATE TABLE locks (
	id INT UNSIGNED auto_increment NOT NULL,
	name varchar(255) NOT NULL,
	CONSTRAINT locks_PK PRIMARY KEY (id)
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

CREATE TABLE access (
	id INT UNSIGNED auto_increment NOT NULL,
	tag_id INT UNSIGNED NOT NULL,
	lock_id INT UNSIGNED NOT NULL,
	CONSTRAINT access_PK PRIMARY KEY (id),
	CONSTRAINT access_UN UNIQUE KEY (tag_id,lock_id),
	CONSTRAINT tags_FK FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE ON UPDATE CASCADE,
	CONSTRAINT locks_FK FOREIGN KEY (lock_id) REFERENCES locks(id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_unicode_ci;

CREATE TABLE users_access_logs (
  id int(10) unsigned NOT NULL AUTO_INCREMENT,
  user_id int(10) unsigned NOT NULL,
  lock_id int(10) unsigned NOT NULL,
  created_at timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  KEY access_history_users_FK (user_id),
  KEY access_history_locks_FK (lock_id),
  CONSTRAINT access_history_locks_FK FOREIGN KEY (lock_id) REFERENCES locks (id),
  CONSTRAINT access_history_users_FK FOREIGN KEY (user_id) REFERENCES users (id)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

INSERT INTO tags (id, name) VALUES(1, 'Guest tag 1'), (2, 'Employee tag 1');

INSERT INTO users (id, username, password, tag_id) VALUES(1, 'Guest 1', '12345', 1), (2, 'Employee 1', '54321', 2);

INSERT INTO locks (id, name) VALUES(1, 'Tunnel'), (2, 'Office');

INSERT INTO access (id, tag_id, lock_id) VALUES(1, 1, 1), (2, 2, 1), (3, 2, 2);

SET FOREIGN_KEY_CHECKS=1;
