<?php
require_once 'iDatabase.php';
/**
 * MySQL short summary.
 *
 * MySQL description.
 *
 * @version 1.0
 * @author meshkov
 */
class MySQL implements iDatabase
{
    private $config;    
    private $my;
    private $isConnected = false;
    function __construct()
    {
        $this->config = include('/config.php');
        
    }
    public function query($query)
    {
        if( $this->isConnected )
            return $this->my->query($query);
        else
            return null;
    }
    public function callproc($proc, $params = array())
    {
        $this->query( sprintf("CALL %s(%s)", $proc, $this->GetProcParams( $params )));
    }
    
    public function setvars($params)
    {
        $this->query( sprintf("SET %s", $this->GetSetParams( $params ) ));
    }
    
    public function connect($host, $user, $password, $database)
    {
        $this->my = new mysqli( $this->config["mysql_host"], 
            $this->config["mysql_user"], 
            $this->config["mysql_pass"],
            $this->config["mysql_db"]);        
        
        if (!mysqli_connect_errno()) { 
            $this->isConnected = true;
        } 
    }
    public function disconnect()
    {
        if( $this->isConnected )
            $this->my->close();
    }
    
    public function GetSetParams($pairs)
    {
        array_walk($p, create_function('&$i,$k','$i=" $k=\'$i\'";'));
        return implode(",", $p);
    }
    public function GetProcParams($pairs)
    {
        array_walk($p, create_function('&$i,$k','$i=" \'$i\'";'));
        return implode(",", $p);
    }
}
