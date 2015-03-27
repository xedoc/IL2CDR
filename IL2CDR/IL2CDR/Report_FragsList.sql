SELECT
  players.nickname AS attacker_nickname,
  players_1.nickname AS target_nickname,
  objectsinfo.name AS target_objectname,
  CONVERT_TZ(frags.dt, @@time_zone, '+01:00') AS frag_dt,
  frags.hits,
  frags.damage,
  frags.attacker_coalition,
  frags.target_coalition,
  CONVERT_TZ(missions.MissionStartTime, @@time_zone, '+01:00') AS mission_start,
  CONVERT_TZ(missions.MissionEndTime, @@time_zone, '+01:00') AS mission_end,
  servers.ServerName
FROM frags
  LEFT OUTER JOIN players
    ON frags.attackerplayer_id = players.nickname_id
  LEFT OUTER JOIN missions
    ON frags.mission_id = missions.mission_id
  LEFT OUTER JOIN servers
    ON missions.server_id = servers.server_id
  INNER JOIN players players_1
    ON frags.targetplayer_id = players_1.nickname_id
  INNER JOIN objectsinfo
    ON frags.targetobject_id = objectsinfo.object_id
