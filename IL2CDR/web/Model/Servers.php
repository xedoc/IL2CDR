<?php
require_once 'MySQL.php';
require_once 'Server.php';
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
    function __construct()
    {
        $this->db = new MySQL();
    }
    public function UpdateServers( $idlist, $ishiddenlist )
    {
        $hiddenservers = array();
        if( is_array( $ishiddenlist ))
        {
            foreach( $idlist as $key => $value )
            {
                if( $ishiddenlist[$key] )
                {
                    $hiddenservers[] = $idlist[$key];
                }
            }
        }
        $params = array(
                'AuthToken'  => $this->db->EaQ( $_COOKIE['authtoken'] ),
                'DisabledServers' => $this->db->EaQ( implode( ',', $hiddenservers ) )
            );
        
        $this->db->setvars($params);
        $this->db->callproc( 'UpdateServers');

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
            $this->db->nextresult();
        }
        return $servers;
        
    }

}
