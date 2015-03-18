<?php
require_once 'MySQL.php';
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
        preg_match("/(-|\d+)/", $jsdate, $matches);
        if( count($matches) > 0 )
            return sprintf( "DATE_ADD(FROM_UNIXTIME(0), interval %s second)",  $matches[1]);
        else
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
            if( !isset( $event->Type ) || empty( $event->Type ) )        
                continue;            
            
            switch( $event->Type)
            {
                case 0:
                    $params = array( 
                            'GameDateTime' => GetTimeStamp( $event->GameDateTime ),
                            'MissionFile' => $event->MissionFile,
                            'MissionID' => $event->MissionID,
                            'GameType' => $event->GameType,
                            'SettingsFlags' => $event->SettingsFlags,
                            'Mods' => $event->Mods,
                            'Preset'=> $event->Preset,
                            'AQMId'=> $event->AQMId,
                            'ServerId'=> $event->Server->ServerId,
                            'ServerName'=> $event->Server->Name,
                            'EventID'=> $event->EventID,
                            'MissionStartTime'=>  GetTimeStamp( $event->MissionStartTime )
                        );                    
                    $this->db->setvars($params);
                    $this->db->callproc("AddMissionEvent");
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
            }
       }
        

        
       return true;
    }
    
}
