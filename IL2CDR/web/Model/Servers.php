<?php
require_once 'MySQL.php';
require_once 'Server.php';
require_once 'Filter.php';
/**
 * Servers short summary.
 *
 * Servers description.
 *
 * @version 1.0
 * @author meshkov
 */
class Servers
{
    private $db;
    function __construct($db)
    {
        $this->db = $db;
    }
    public function UpdateServers( $idlist, $ishiddenlist )
    {
        $hiddenservers = array();
        if( is_array( $ishiddenlist ) && is_array($idlist))
        {
            foreach( $ishiddenlist as $value )
            {
                $hiddenservers[] = $idlist[$value];
            }
        }
        $params = array(
                'AuthToken'  => $this->db->EaQ( $_COOKIE['authtoken'] ),
                'DisabledServers' => $this->db->EaQ( implode( ',', $hiddenservers ) )
            );
        
        $this->db->setvars($params);
        $this->db->callproc( 'UpdateServers');

    }
    public function GetDifficulties()
    {
        if( !$this->db->IsConnected )
            return array();        
        
        $result = $this->db->callproc( 'GetDifficulties');
        $difficulties = array();
        if( $result && $result->num_rows > 0)
        {
            while( $obj = $result->fetch_object())
            {
                if( $obj )
                {                    
                    $difficulties[] = $obj->Difficulty;
                }
            }
        }
        
        return $difficulties;    
    }
    
    public function GetPlayerCountByServer()
    {
        if( !$this->db->IsConnected )
            return array();        
        
        $result = $this->db->callproc( 'GetPlayerCountByServer');
        $servers = array();
        if( $result && $result->num_rows > 0)
        {
            while( $obj = $result->fetch_object())
            {
                if( $obj )
                {
                    $server = new Server($obj->ServerName,  $obj->ServerId );
                    $server->IsHidden = $obj->IsHidden;
                    $server->PlayerCount = $obj->PlayerCount;
                    $servers[] = $server;
                }
            }
        }
        
        return $servers;
    }
    
    public function GetOnlinePlayers($serverid)
    {
        if( !$serverid )
            return array();
        
        if( !$this->db->IsConnected )
            return array();        
        
        $params = array(
            'ServerId' => $this->db->EaQ( $serverid )
        );
        $this->db->setvars($params);
        $result = $this->db->callproc( 'GetOnlinePlayers');
        $players = array();
        if( $result && $result->num_rows > 0)
        {
            while( $obj = $result->fetch_object())
            {
                if( $obj )
                {
                    $players[] = (object)array(
                        'Nickname' => $obj->Nickname,
                        'Ping' => $obj->Ping,
                        'Country' => $obj->Country
                        );
                }
            }
            
        }
        return $players;
    }
    
    public function GetVisibleServers()
    {
        if( !$this->db->IsConnected )
            return array();        
        
        $filter = new Filter($this->db);
        $filteredservers = $filter->GetFilteredServers();
        
        $result = $this->db->callproc( 'GetVisibleServers');
        $servers = array();
        if( $result && $result->num_rows > 0)
        {
            while( $obj = $result->fetch_object())
            {
                if( $obj )
                {
                    $server = new Server($obj->ServerName,  $obj->ServerId );
                    $server->IsHidden = $obj->IsHidden;
                    $server->IsInFilter = in_array( $server->Id, $filteredservers );
                    $servers[] = $server;
                }
            }
            
        }
        return $servers;
    }
    public function GetServers()
    {
        if( !$this->db->IsConnected )
            return array();
        
        $params = array(
                'AuthToken' => $this->db->EaQ( $_COOKIE['authtoken'] )
            );
        $this->db->setvars($params);
        $result = $this->db->callproc( 'GetServers');
        $servers = array();
        if( $result && $result->num_rows > 0)
        {
            while( $obj = $result->fetch_object())
            {
                if( $obj )
                {
                    $server = new Server($obj->ServerName,  $obj->ServerId );
                    $server->IsHidden = $obj->IsHidden;
                    $servers[] = $server;
                }
            }
            
        }
        return $servers;
        
    }

}
