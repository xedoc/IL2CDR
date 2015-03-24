--
-- Скрипт сгенерирован Devart dbForge Studio for MySQL, Версия 6.3.341.0
-- Домашняя страница продукта: http://www.devart.com/ru/dbforge/mysql/studio
-- Дата скрипта: 24.03.2015 9:01:23
-- Версия сервера: 5.5.42-37.1-log
-- Версия клиента: 4.1
--


CREATE TABLE objectsinfo (
  object_id binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  name varchar(50) DEFAULT NULL,
  class varchar(50) DEFAULT NULL,
  purpose varchar(50) DEFAULT NULL,
  PRIMARY KEY (object_id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 197
CHARACTER SET utf8
COLLATE utf8_general_ci;

CREATE TABLE players (
  nickname_id binary(16) NOT NULL,
  user_id binary(16) DEFAULT NULL,
  nickname varchar(255) DEFAULT NULL,
  botgroundkills int(11) DEFAULT 0,
  botairkills int(11) DEFAULT 0,
  playerairkills int(11) DEFAULT 0,
  killedbyplayers int(11) DEFAULT 0,
  killedbygroundbots int(11) DEFAULT 0,
  killedbyairbots int(11) DEFAULT 0,
  PRIMARY KEY (nickname_id),
  INDEX IDX_players_nickname (nickname)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 16384
CHARACTER SET utf8
COLLATE utf8_general_ci;

CREATE TABLE server_owners (
  serverowner_id binary(16) NOT NULL,
  Email varchar(50) DEFAULT NULL,
  Token varchar(32) DEFAULT NULL,
  PRIMARY KEY (serverowner_id),
  UNIQUE INDEX UK_server_owners_Email (Email)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 16384
CHARACTER SET utf8
COLLATE utf8_general_ci;

CREATE TABLE sorties (
  sortie_id binary(16) NOT NULL,
  plane_id binary(16) DEFAULT NULL,
  bullets int(11) DEFAULT NULL,
  shells int(11) DEFAULT NULL,
  fuel decimal(16, 2) DEFAULT NULL,
  payload varchar(16) DEFAULT NULL,
  skin varchar(255) DEFAULT NULL,
  weaponmods varchar(16) DEFAULT NULL,
  coalition tinyint(4) DEFAULT NULL,
  country varchar(50) DEFAULT NULL,
  dt datetime DEFAULT NULL,
  endbullets int(11) DEFAULT NULL,
  endshells int(11) DEFAULT NULL,
  enddt datetime DEFAULT NULL,
  PRIMARY KEY (sortie_id)
)
ENGINE = INNODB
AVG_ROW_LENGTH = 1489
CHARACTER SET utf8
COLLATE utf8_general_ci;

CREATE TABLE servers (
  server_id binary(16) NOT NULL,
  serverowner_id binary(16) DEFAULT NULL,
  ServerName varchar(70) DEFAULT NULL,
  PRIMARY KEY (server_id),
  CONSTRAINT FK_servers_server_owners_serverowner_id FOREIGN KEY (serverowner_id)
  REFERENCES server_owners (serverowner_id) ON DELETE CASCADE ON UPDATE CASCADE
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci;

CREATE TABLE missions (
  mission_id binary(16) NOT NULL,
  server_id binary(16) NOT NULL DEFAULT '\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0',
  GameDateTime datetime DEFAULT NULL,
  MissionID varchar(255) DEFAULT NULL,
  GameType int(11) DEFAULT NULL,
  MissionFile text DEFAULT NULL,
  SettingsFlags varchar(32) DEFAULT NULL,
  Mods int(11) DEFAULT NULL,
  Preset int(11) DEFAULT NULL,
  AQMID int(11) DEFAULT NULL,
  MissionStartTime datetime NOT NULL,
  MissionEndTime datetime DEFAULT NULL,
  PRIMARY KEY (mission_id),
  INDEX IDX_events_missionstart_server_id (server_id),
  CONSTRAINT FK_missions_servers_server_id FOREIGN KEY (server_id)
  REFERENCES servers (server_id) ON DELETE RESTRICT ON UPDATE RESTRICT
)
ENGINE = INNODB
AVG_ROW_LENGTH = 5461
CHARACTER SET utf8
COLLATE utf8_general_ci;

CREATE TABLE frags (
  frag_id binary(16) NOT NULL,
  attackersortie_id binary(16) DEFAULT NULL,
  mission_id binary(16) DEFAULT NULL,
  attackerplayer_id binary(16) DEFAULT NULL,
  attackerobject_id binary(16) DEFAULT NULL,
  targetplayer_id binary(16) DEFAULT NULL,
  targetobject_id binary(16) DEFAULT NULL,
  dt datetime DEFAULT NULL,
  hits int(11) DEFAULT NULL,
  damage decimal(16, 2) DEFAULT NULL,
  attacker_coalition tinyint(4) DEFAULT NULL,
  target_coalition tinyint(4) DEFAULT NULL,
  PRIMARY KEY (frag_id),
  INDEX IDX_frags_attackerobject_id (attackerobject_id),
  INDEX IDX_frags_attackerplayer_id (attackerplayer_id),
  INDEX IDX_frags_dt (dt),
  INDEX IDX_frags_mission_id (mission_id),
  INDEX IDX_frags_targetobject_id (targetobject_id),
  INDEX IDX_frags_targetplayer_id (targetplayer_id),
  CONSTRAINT FK_frags_missions_mission_id FOREIGN KEY (mission_id)
  REFERENCES missions (mission_id) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT FK_frags_objectsinfo_object_id FOREIGN KEY (attackerobject_id)
  REFERENCES objectsinfo (object_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_frags_objectsinfo_object_id_ FOREIGN KEY (targetobject_id)
  REFERENCES objectsinfo (object_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_frags_players_nickname_id FOREIGN KEY (attackerplayer_id)
  REFERENCES players (nickname_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_frags_players_nickname_id_ FOREIGN KEY (targetplayer_id)
  REFERENCES players (nickname_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_frags_sorties_sortie_id FOREIGN KEY (attackersortie_id)
  REFERENCES sorties (sortie_id) ON DELETE RESTRICT ON UPDATE RESTRICT
)
ENGINE = INNODB
AVG_ROW_LENGTH = 1820
CHARACTER SET utf8
COLLATE utf8_general_ci;

CREATE TABLE frag_supporters (
  frag_id binary(16) DEFAULT NULL,
  attackerplayer_id binary(16) DEFAULT NULL,
  attackerobject_id binary(16) DEFAULT NULL,
  damage decimal(16, 2) DEFAULT NULL,
  hits int(11) DEFAULT NULL,
  INDEX IDX_frag_supporters_attackerobject_id (attackerobject_id),
  INDEX IDX_frag_supporters_attackerplayer_id (attackerplayer_id),
  INDEX IDX_frag_supporters_frag_id (frag_id),
  CONSTRAINT FK_frag_supporters_frags_frag_id FOREIGN KEY (frag_id)
  REFERENCES frags (frag_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_frag_supporters_objectsinfo_object_id FOREIGN KEY (attackerobject_id)
  REFERENCES objectsinfo (object_id) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT FK_frag_supporters_players_nickname_id FOREIGN KEY (attackerplayer_id)
  REFERENCES players (nickname_id) ON DELETE RESTRICT ON UPDATE RESTRICT
)
ENGINE = INNODB
CHARACTER SET utf8
COLLATE utf8_general_ci;

DELIMITER $$

CREATE PROCEDURE AddKill ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
PROC:
  BEGIN
    IF (ISNULL(@AttackerPlayerID)
      OR @AttackerPlayerID = "")
      AND (ISNULL(@AttackerObjectID)
      OR @AttackerObjectID = "")
      THEN
      LEAVE PROC;
    END IF;
    IF (ISNULL(@TargetPlayerID)
      OR @TargetPlayerID = "")
      AND (ISNULL(@TargetObjectID)
      OR @TargetObjectID = "")
      THEN
      LEAVE PROC;
    END IF;

    INSERT INTO frags
    SET frag_id = uuid2bin(@EventID),
        attackersortie_id = uuid2bin(@SortieID),
        mission_id = uuid2bin(@MissionUUID),
        attackerplayer_id = uuid2bin(@AttackerPlayerID),
        attackerobject_id = uuid2bin(@AttackerObjectID),
        targetplayer_id = uuid2bin(@TargetPlayerID),
        targetobject_id = uuid2bin(@TargetObjectID),
        dt = @EventTime,
        hits = @Hits,
        damage = @Damage,
        attacker_coalition = @AttackerCoalition,
        target_coalition = @TargetCoalition
    ON DUPLICATE KEY UPDATE
    attackersortie_id = uuid2bin(@SortieID),
    mission_id = uuid2bin(@MissionUUID),
    attackerplayer_id = uuid2bin(@AttackerPlayerID),
    attackerobject_id = uuid2bin(@AttackerObjectID),
    targetplayer_id = uuid2bin(@TargetPlayerID),
    targetobject_id = uuid2bin(@TargetObjectID),
    hits = @Hits,
    damage = @Damage,
    attacker_coalition = @AttackerCoalition,
    target_coalition = @TargetCoalition;


  END
$$

CREATE PROCEDURE AddMissionEndEvent ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
PROC:
  BEGIN
    IF ISNULL(@ServerId)
      OR @ServerId = ""
      OR ISNULL(@MissionUUID)
      OR @MissionUUID = "" THEN
      LEAVE PROC;
    END IF;
    UPDATE missions
    SET MissionEndTime = @MissionEndTime
    WHERE server_id = uuid2bin(@ServerId)
    AND mission_id = uuid2bin(@MissionUUID);
  END
$$

CREATE PROCEDURE AddMissionStartEvent ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
PROC:
  BEGIN
    #Validate inbound data
    IF ISNULL(@ServerId)
      OR @ServerId = ""
      OR ISNULL(@MissionUUID)
      OR @MissionUUID = "" THEN
      LEAVE PROC;
    END IF;
    INSERT IGNORE INTO missions
    SET GameDateTime = @GameDateTime,
        MissionFile = @MissionFile,
        MissionID = @MissionID,
        GameType = @GameType,
        SettingsFlags = @SettingsFlags,
        Mods = @Mods,
        Preset = @Preset,
        AQMid = @AQMid,
        server_id = uuid2bin(@ServerId),
        mission_id = uuid2bin(@MissionUUID),
        MissionStartTime = @MissionStartTime;
  END
$$

CREATE PROCEDURE AddObjectInfo ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
BEGIN

  INSERT IGNORE INTO objectsinfo
  SET object_id = uuid2bin(@ObjectId),
      `name` = @Name,
      `class` = @Class,
      purpose = @Purpose;


END
$$

CREATE PROCEDURE AddPlayer ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
PROC:
  BEGIN
    IF ISNULL(@NickId)
      OR @NickId = "" THEN
      LEAVE PROC;
    END IF;
    IF ISNULL(@LoginId)
      OR @LoginId = "" THEN
      LEAVE PROC;
    END IF;

    INSERT INTO players
    SET nickname_id = uuid2bin(@NickId),
        user_id = uuid2bin(@LoginId),
        nickname = @NickName
    ON DUPLICATE KEY UPDATE
    nickname = @NickName;

  END
$$

CREATE PROCEDURE AddServer ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
PROC:
  BEGIN

    DECLARE server_id_ binary(16);
    DECLARE serverowner_id_ binary(16);

    IF NOT is_tokenvalid(@Token) THEN
      LEAVE PROC;
    END IF;

    SET server_id_ = uuid2bin(@ServerId);

    IF ISNULL(server_id_) THEN
      LEAVE PROC;
    END IF;
    IF is_serverexists(server_id_, @ServerName) THEN
      LEAVE PROC;
    END IF;

    SET serverowner_id_ = get_serverownerbytoken(@Token);

    IF ISNULL(serverowner_id_) THEN
      LEAVE PROC;
    END IF;

    INSERT INTO servers
    SET server_id = server_id_,
        ServerName = @ServerName,
        serverowner_id = serverowner_id_
    ON DUPLICATE KEY UPDATE
    ServerName = @ServerName;

  END
$$

CREATE PROCEDURE AddServerOwner ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
BEGIN
  INSERT INTO server_owners
  SET serverowner_id = uuid2bin(UUID()),
      FirstName = @FirstName,
      LastName = @LastName,
      Email = @Email,
      Token = @Token
  ON DUPLICATE KEY UPDATE
  FirstName = @FirstName,
  LastName = @LastName,
  Email = @Email,
  Token = @Token;


END
$$

CREATE PROCEDURE AddSortieEnd ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
PROC:
  BEGIN
    IF ISNULL(@SortieId)
      OR @SortieId = "" THEN
      LEAVE PROC;
    END IF;

    UPDATE sorties
    SET endbullets = @EndBullets,
        endshells = @EndShells,
        enddt = @EventTime
    WHERE sortie_id = uuid2bin(@SortieId);


  END
$$

CREATE PROCEDURE AddSortieStart ()
SQL SECURITY INVOKER
MODIFIES SQL DATA
PROC:
  BEGIN
    IF ISNULL(@SortieId)
      OR @SortieId = "" THEN
      LEAVE PROC;
    END IF;
    IF ISNULL(@NickId)
      OR @NickId = "" THEN
      LEAVE PROC;
    END IF;
    CALL AddPlayer();

    INSERT IGNORE INTO sorties
    SET sortie_id = uuid2bin(@SortieId),
        plane_id = uuid2bin(@PlaneId),
        bullets = @Bullets,
        shells = @Shells,
        fuel = @Fuel,
        payload = @Payload,
        skin = @Skin,
        weaponmods = @WeaponMods,
        country = @Country,
        coalition = @CoalitionIndex,
        dt = @EventTime;


  END
$$

CREATE FUNCTION get_serverownerbytoken (token_ varchar(32))
RETURNS binary(16)
SQL SECURITY INVOKER
READS SQL DATA
BEGIN

  DECLARE serverowner_id_ binary(16);

  IF ISNULL(token_) THEN
    RETURN NULL;
  END IF;
  IF LENGTH(token_) != 32 THEN
    RETURN NULL;
  END IF;

  SELECT
    serverowner_id INTO serverowner_id_
  FROM server_owners
  WHERE Token = token_ LIMIT 1;
  RETURN serverowner_id_;

END
$$

CREATE FUNCTION is_serverexists (ServerId_ binary(16), ServerName_ varchar(70))
RETURNS tinyint(1)
SQL SECURITY INVOKER
READS SQL DATA
BEGIN
  DECLARE result_ bool;

  IF ISNULL(ServerId_)
    OR ISNULL(ServerName_) THEN
    RETURN FALSE;
  END IF;

  SELECT
    EXISTS (SELECT
        server_id
      FROM servers
      WHERE server_id = ServerId_
      AND ServerName = ServerName_) INTO result_;

  RETURN result_;
END
$$

CREATE FUNCTION is_tokenvalid (token_ varchar(32))
RETURNS tinyint(1)
SQL SECURITY INVOKER
READS SQL DATA
BEGIN

  DECLARE result_ bool;

  IF ISNULL(token_) THEN
    RETURN FALSE;
  END IF;
  IF LENGTH(token_) != 32 THEN
    RETURN FALSE;
  END IF;

  SELECT
    EXISTS (SELECT
        serverowner_id
      FROM server_owners
      WHERE Token = token_) INTO result_;
  RETURN result_;

END
$$

CREATE FUNCTION uuid2bin (guid varchar(36))
RETURNS binary(16)
DETERMINISTIC
SQL SECURITY INVOKER
BEGIN
  IF ISNULL(guid) THEN
    RETURN NULL;
  END IF;
  RETURN UNHEX(REPLACE(guid, '-', ''));
END
$$

CREATE
DEFINER = CURRENT_USER
TRIGGER ins_frags
AFTER INSERT
ON frags
FOR EACH ROW
BEGIN
  DECLARE class_ varchar(50);
  IF NOT ISNULL(new.attackerplayer_id) THEN
    IF NOT ISNULL(new.targetplayer_id) THEN
      UPDATE players
      SET playerairkills = playerairkills + 1
      WHERE nickname_id = new.attackerplayer_id;
      UPDATE players
      SET killedbyplayers = killedbyplayers + 1
      WHERE nickname_id = new.targetplayer_id;
    ELSEIF NOT ISNULL(new.targetobject_id) THEN
      SELECT
        class INTO class_
      FROM objectsinfo
      WHERE object_id = new.targetobject_id LIMIT 1;
      IF NOT ISNULL(class_) THEN
        IF class_ = 'Plane' THEN
          UPDATE players
          SET botairkills = botairkills + 1
          WHERE nickname_id = new.attackerplayer_id;
        ELSE
          UPDATE players
          SET botgroundkills = botgroundkills + 1
          WHERE nickname_id = new.attackerplayer_id;
        END IF;
      END IF;
    END IF;
  ELSE
    IF NOT ISNULL(new.targetplayer_id)
      AND NOT ISNULL(new.attackerobject_id) THEN
      SELECT
        class INTO class_
      FROM objectsinfo
      WHERE object_id = new.attackerobject_id LIMIT 1;
      IF NOT ISNULL(class_) THEN
        IF class_ = 'Plane' THEN
          UPDATE players
          SET killedbyairbots = killedbyairbots + 1
          WHERE nickname_id = new.targetplayer_id;
        ELSE
          UPDATE players
          SET killedbygroundbots = killedbygroundbots + 1
          WHERE nickname_id = new.targetplayer_id;
        END IF;
      END IF;
    ELSEIF NOT ISNULL( new.targetplayer_id ) THEN
      UPDATE players
          SET crashes = crashes + 1
          WHERE nickname_id = new.targetplayer_id;
    END IF;
  END IF;
END
$$

DELIMITER ;