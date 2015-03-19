<?php
require_once 'MySQL.php';
require_once 'Utils.php';
/**
 * MissionEvent short summary.
 *
 * MissionEvent description.
 *
 * @version 1.0
 * @author meshkov
 */
class MissionEvent
{
    private $db;
    private $events;
    function __construct($text)
    {
        $this->db = new MySQL();
        $this->events = json_decode( $text );
    }
    private function GetTimeStamp($jsdate)
    {
        $matches = array();
        preg_match("/([-|\d]+)/", $jsdate, $matches);
        $seconds = 0;
        if( count($matches) > 0 )
        {
            $seconds = $matches[1];
            if( is_numeric($seconds) )
            {
                $len = strlen($seconds);
                if( $len > 3 )
                    $seconds = substr($seconds, 0, $len-3);
                    
                if( $seconds < 0 )
                    return sprintf( "DATE_ADD(FROM_UNIXTIME(0), interval %s second)",  $seconds);                
                else
                    return sprintf( "FROM_UNIXTIME(%s)", $seconds);
            }
        }
        return 0;       
    }
    
    public function SaveToDB( )
    {
        
        if( !isset( $this->events) || 
            empty($this->events) ||
            !is_array($this->events))
        {
            return false;
        }       
        foreach ($this->events as $event)
        {
            if( $event->Type < 0 )        
                continue;            
            
            $globalparams = array( 
                    'ServerId'=> $this->db->EaQ($event->Server->ServerId),
                    'MissionUUID'=> $this->db->EaQ($event->Server->CurrentMissionId),
                    'Token' => $this->db->EaQ($_COOKIE["srvtoken"]),
                );                    
            $this->db->setvars($globalparams);
            
            switch( $event->Type)
            {
                case 0:
                    $params = array( 
                            'EventID'=> $this->db->EaQ($event->EventID),
                            'GameDateTime' => $this->GetTimeStamp( $event->GameDateTime ),
                            'MissionFile' => $this->db->EaQ( $event->MissionFile ),
                            'MissionID' => $this->db->EaQ($event->MissionID),
                            'GameType' => $this->db->EaQ($event->GameType),
                            'SettingsFlags' => $this->db->EaQ($event->SettingsFlags),
                            'Mods' => $this->db->EaQ($event->Mods),
                            'Preset'=> $this->db->EaQ($event->Preset),
                            'AQMId'=> $this->db->EaQ($event->AQMId),
                            'MissionStartTime'=>  $this->GetTimeStamp( $event->MissionStartTime )
                        );                    
                    $this->db->setvars($params);
                    $this->db->callproc("AddMissionStartEvent");
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    $params = array( 
                           'EventID'=> $this->db->EaQ($event->EventID),
                           'MissionEndTime'=>  $this->GetTimeStamp( $event->MissionEndTime )
                       );                    
                    $this->db->setvars($params);
                    $this->db->callproc("AddMissionEndEvent");
                    break;
                case 8:
                    break;
                case 9:
                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    break;
                case 17:
                    break;
                case 20:
                    break;
                case 21:
                    break;
                case 9998:
                    foreach ($event->ObjectInfo as $info)
                    {
                        $params = array(
                            'ObjectId'=> $this->db->EaQ($info->ObjectId),
                            'Name'=> $this->db->EaQ($info->Name),
                            'Class'=> $this->db->EaQ($info->Class),
                            'Purpose'=> $this->db->EaQ($info->Purpose),
                        );                    
                        $this->db->setvars($params);
                        $this->db->callproc("AddObjectInfo");                    	
                    }
                    
                    break;
                case 9999:
                    $params = array(                         
                        'ServerName'=> $this->db->EaQ($event->Server->Name),
                    );                    
                    $this->db->setvars($params);
                    $this->db->callproc("AddServer");
                    break;
            }
       }
        

        
       return true;
    }
    
}
