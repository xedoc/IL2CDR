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
    private $text;
    function __construct($text, $db)
    {
        $this->db = $db;
        try
        {
            $this->events = json_decode( $text );
        }
        catch( Exception $e)
        {
            file_put_contents( '../error_json.log', time() . ': ' . $e->getMessage() . ' Content: ' . $text . PHP_EOL, FILE_APPEND);
        }
        unset($text);
        $text = null;
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
        unset($matches);
        
        return 0;       
    }
    
    public function SaveToDB( )
    {        
        if( !$this->db->IsConnected  ||
            !isset( $this->events) || 
            empty($this->events) ||
            !is_array($this->events))
        {
            unset($this->events);
            return false;
        }       
        foreach ($this->events as $event)
        {
            if( $event->Type < 0 )        
                continue;            
            
            $globalparams = array( 
                    'Token' => $this->db->EaQ($_COOKIE["srvtoken"]),
                );
            
            if( isset( $event->Server ) )
            {
                $globalparams += array(
                    'MissionUUID'=> $this->db->EaQ($event->Server->CurrentMissionId),
                    'ServerId'=> $this->db->EaQ($event->Server->ServerId),                    
                );
            }

            if( isset( $event->EventID ) )
            {
                $globalparams += array(
                    'EventID'=> $this->db->EaQ($event->EventID),
                    'EventTime' => $this->GetTimeStamp($event->EventTime),
                );
            }
            
            $this->db->setvars($globalparams);
            
            switch( $event->Type)
            {
                case 0:
                    $params = array( 
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
                    $params = array(
                        'IsTeamKill' => $this->db->EaQ( $event->IsTeamKill ? '1' : '0' ),                                                
                        );
                    
                    if( isset( $event->AttackerPlayer ) )
                    {
                        $params += array(
                            'AttackerPlayerID' => $this->db->EaQ( $event->AttackerPlayer->NickId ),  
                            'SortieID' => $this->db->EaQ( $event->AttackerPlayer->SortieId )
                            );
                    }
                    else
                    {
                        $params += array(     
                           'AttackerPlayerID' => 'null',
                           'SortieID' => 'null'
                           );
                    }
                    
                    if( isset( $event->AttackerObject ) )
                    {
                        $params += array(
                            'AttackerObjectID' => $this->db->EaQ( $event->AttackerObject->ObjectId ),                            
                            );
                    }
                    else
                    {
                        $params += array(     
                           'AttackerObjectID' => 'null',
                           );
                    }
                    
                    if( isset( $event->TargetPlayer ) )
                    {
                        $params += array(
                            'TargetPlayerID' => $this->db->EaQ( $event->TargetPlayer->NickId ),                            
                            'TargetSortieID' => $this->db->EaQ( $event->TargetPlayer->SortieId ),
                            'IsInAir' => $this->db->EaQ( $event->TargetPlayer->IsInAir ? '1' : '0' ),
                            );
                    }
                    else
                    {
                        $params += array(     
                           'TargetPlayerID' => 'null',
                           'TargetSortieID' => 'null'
                           );
                    }
                    if( isset( $event->TargetObject ) )
                    {
                        $params += array(
                            'TargetObjectID' => $this->db->EaQ( $event->TargetObject->ObjectId ),                            
                            );
                    }
                    else
                    {
                        $params += array(     
                           'TargetObjectID' => 'null',
                           );
                    }
                    $params += array(
                            'Hits' =>  $this->db->EaQ( $event->Hits ),
                            'Damage' => $this->db->EaQ( $event->Damage),
                            'AttackerCoalition' => $this->db->EaQ( $event->AttackerCoalition ),
                            'TargetCoalition' => $this->db->EaQ( $event->TargetCoalition ),
                        );
                    
                    $this->db->setvars($params);
                    $this->db->callproc("AddKill");
                    
                    
                    
                    break;
                case 4:
                    if(  !isset( $event->Player ) || 
                         !isset( $event->Player->Plane ))
                        continue;
                    $params = array( 
                           'SortieID' => $this->db->EaQ( $event->Player->SortieId ),
                           'EndBullets' => $this->db->EaQ( $event->Bullets ),
                           'EndShells' => $this->db->EaQ( $event->Shells ),
                           'EndBombs' => $this->db->EaQ( $event->Bombs ),
                       );
                    $this->db->setvars($params);
                    $this->db->callproc("AddSortieEnd");
                    break;
                case 5:
                    break;
                case 6:
                    
                    break;
                case 7:
                    $params = array( 
                           'MissionEndTime'=>  $this->GetTimeStamp( $event->MissionEndTime )
                       );
                    $this->db->setvars($params);
                    $this->db->callproc("AddMissionEndEvent");
                    break;
                case 8:
                    $params = array( 
                           'CoalitionIndex'=>  $this->db->EaQ( $event->CoalitionIndex ),
                           'IsCompleted'=>  $this->db->EaQ( $event->IsCompleted ? '1' : '0' ),
                       );
                    //$params = array(
                    //        'Text' => $this->db->EaQ(sprintf('%s %s %s %s', 
                    //                $event->ObjectiveId, 
                    //                $event->IsPrimary,
                    //                $event->CoalitionIndex,
                    //                $event->IsCompleted
                    //                )));
                    $this->db->setvars($params);
                    $this->db->callproc("AddObjectiveCompleted");
                    break;
                case 9:
                    break;                    
                //Plane spawn
                case 10:
                    if( !isset( $event->Player ) || 
                        !isset( $event->Player->NickId ) || 
                        !isset( $event->Player->Plane ))
                        continue;
                    $params = array( 
                           'NickId' => $this->db->EaQ( $event->Player->NickId ),
                           'LoginId' => $this->db->EaQ( $event->Player->LoginId ),
                           'NickName' => $this->db->EaQ( $event->Player->NickName ),
                           'PlaneId' => $this->db->EaQ( $event->Player->Plane->ObjectId ),
                           'Plane' => $this->db->EaQ( $event->Player->Plane->Name ),
                           'Bullets' => $this->db->EaQ( $event->Player->Plane->Bullets ),
                           'Bombs' => $this->db->EaQ( $event->Player->Plane->Bombs ),
                           'Shells' => $this->db->EaQ( $event->Player->Plane->Shells ),
                           'Fuel' => $this->db->EaQ( $event->Player->Plane->Fuel ),
                           'Payload' => $this->db->EaQ( $event->Player->Plane->Payload ),
                           'Skin' => $this->db->EaQ( $event->Player->Plane->Skin ),
                           'WeaponMods' => $this->db->EaQ( $event->Player->Plane->WeaponMods ),
                           'Country' => $this->db->EaQ( $event->Player->Country->Name ),
                           'SortieId' => $this->db->EaQ( $event->Player->SortieId ),
                           'CoalitionIndex' => $this->db->EaQ( $event->Player->CoalitionIndex ),
                       );
                    $this->db->setvars($params);
                    $this->db->callproc("AddSortieStart");
                    break;
                case 11:
                    break;
                case 12:
                    if( !isset( $event->Object ) )
                        continue;
                    
                    $params = array(
                        'ObjectId'=> $this->db->EaQ($event->Object->ObjectId),
                        'Name'=> $this->db->EaQ($event->Object->Name),
                        'Class'=> $this->db->EaQ($event->Object->Classification),
                        'Purpose'=> $this->db->EaQ($event->Object->Purpose),
                    );                    
                    $this->db->setvars($params);
                    $this->db->callproc("AddObjectInfo");   
                   
                    break;
                case 13:
                    break;
                case 14:
                    break;
                case 15:
                    break;
                case 16:
                    if( !isset( $event->Player ) || 
                        !isset( $event->Player->Plane ))
                            continue;
                        $params = array( 
                               'SortieID' => $this->db->EaQ( $event->Player->SortieId ),
                               'IsFriendlyArea' => $this->db->EaQ( $event->IsFriendlyArea ? '1' : '0' ),
                               'IsInAir' => $this->db->EaQ( $event->Player->IsInAir ? '1' : '0' ),
                           );
                        $this->db->setvars($params);
                        $this->db->callproc("OnBotRemove");
                    break;
                case 17:
                    break;
                case 18:
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
                    if( substr($event->Server->Name,0,4) == 'PID:' )
                        continue;                    
                    
                    $params = array(      
                        'ServerName'=> $this->db->EaQ($event->Server->Name),
                    );                    
                    $this->db->setvars($params);
                    $this->db->callproc("AddServer");
                    break;
            }
       }
        
       unset($this->events);
       unset($this->db);
       
       gc_collect_cycles();

       return true;
    }
    
}
