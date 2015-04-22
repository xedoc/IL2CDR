<?php

/**
 * Filter short summary.
 *
 * Filter description.
 *
 * @version 1.0
 * @author meshkov
 */
class Filter
{
    private $db;
    function __construct($db)
    {
        $this->db = $db;
    }
    public function GetCurrentFilter()
    {
        if( isset( $_COOKIE['filter']) && !empty($_COOKIE['filter']) )
        {
            return  $_COOKIE['filter'];
        }
        else
            return 'null';
    }
    public function GetFilteredServers()
    {
        if( !$this->db->IsConnected )
            return array();
            
        $params = array(
            'Filter' => $this->db->EaQ($this->GetCurrentFilter()),       
            );
        $this->db->setvars( $params );
        $result = $this->db->callproc('GetFilteredServers');
        $list = array();
        if( $result )
        {
            while( $obj = $result->fetch_object() )   
            {
                $list[] = $obj->ServerId;
            }
        }
        return $list;        
    }
     
    public function GetFilterId( $servers, $difficulties )
    {        
        if( !$this->db->IsConnected )
            setcookie( "filter", null, -1, '/' );
        
        if( !is_array($servers) )
        {
            $servers = array();
        }
        if( !is_array($difficulties))
        {
            $difficulties = array();
        }
        
        $server_values = array();
        foreach( $servers as $server )
            $server_values[] = sprintf('(%s)', sprintf( 'uuid2bin(%s)', $this->db->EaQ( $server )));

        $difficulty_values = array();
        foreach( $difficulties as $difficulty )
            $difficulty_values[] = sprintf('(%s)', $this->db->EaQ( $difficulty ));
        
        $params = array(
                'Servers'  => count($server_values) > 0 ? $this->db->EaQ( implode(',', $server_values ) ) : 'null',
                'Difficulties' => count($difficulty_values) > 0 ? $this->db->EaQ( implode( ',', $difficulty_values ) ) : 'null'
            );
        
        $this->db->setvars($params);
        $result = $this->db->callproc( 'UpdateFilter');
        
        if( $result )
        {
            $row = $result->fetch_object();
            setcookie( "filter", $row->FilterId, time()+60*60*24*9000, '/');
            
        }
        else
        {
            setcookie( "filter", null, -1, '/' );
        }
    }
}
