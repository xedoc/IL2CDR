<?php
require_once 'Cache.php';
require_once 'MySQL.php';
/**
 * Players short summary.
 *
 * Players description.
 *
 * @version 1.0
 * @author meshkov
 */
class Players
{
    private $players;
    private $db;
    private $cache;
    function __construct($json)        
    {
        $this->players = json_decode( $json );        
        $this->db = new MySQL();
        $this->cache = new Cache();
    }
    
    public function UpdatePlayersOnline()
    {
        if( !$this->db->IsConnected )
            return;
            
        if( $this->players && $this->players->ServerId )
        {           
            $serverid = str_replace( "-", "", $this->players->ServerId);
            if( $this->players->Players )
            {
                $values = array();
                foreach( $this->players->Players as $player )
                {
                    $values[] = sprintf('(%s,%s,%s,%s,%s)', 
                        sprintf( 'uuid2bin(%s)', $this->db->EaQ( $this->players->ServerId )), 
                        sprintf( 'uuid2bin(%s)', $this->db->EaQ( $player->NickId )), 
                        $this->db->EaQ( $player->Ping ),
                        $this->db->EaQ( $player->CountryId ),
                        $this->db->EaQ( $player->Status )
                        );
                }
                $params = array(                        
                    'ServerId' => $this->db->EaQ( $this->players->ServerId ),
                    'Values' => $this->db->EaQ( implode( ',', $values ) )
                );
                
                $this->cache->Delete( 'playersonline_' . $serverid );
                
                $this->db->setvars($params);
                $this->db->callproc( 'UpdateOnlinePlayers');
            }
        }
        
    }
    
    public function GetPlayersOnline($serverid, $timestamp )
    {
        $default = array();
        $serverid = str_replace("-", "", $serverid );
        
        if( !$this->db->IsConnected || !$serverid || $serverid == '')
            return $default;
        
        $cacheid = 'playersonline_' . $serverid;
        $cached = $this->cache->GetCache( $cacheid );
        if( !$cached )
        {            
            $params = array(                        
                    'ServerId' => $this->db->EaQ( $serverid )
                );
            
            $this->db->setvars($params);
            $result = $this->db->query( "CALL GetOnlinePlayers()" );
            if( $result )
            {
                $count = $result->num_rows;
                if( $count <= 0 )
                {
                    return $default;                
                }   
                $table = array();
                while( $row = $result->fetch_object())
                {
                    $table[] = (object)array(
                        'ServerName' => $row->ServerName,
                        'NickName' => $row->Nickname,
                        'Ping' => $row->Ping,
                        'Country' => $row->Country
                        );
                }            
                $result->close();
                $this->db->nextresult();
                $this->cache->AddCache($cacheid, 60, $table );

                return $table;
            }
            
        }
        
        return $cached;       
    }
    
    
}
