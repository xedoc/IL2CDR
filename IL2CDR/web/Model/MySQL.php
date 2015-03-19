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
        $this->connect();
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
        if( !$this->isConnected )
            return;
        
        $this->query( sprintf("CALL %s(%s)", $proc, $this->GetProcParams( $params )));
    }
    
    public function setvars($params)
    {
        if( !$this->isConnected )
            return;
        
        $this->query( sprintf("SET %s", $this->GetSetParams( $params ) ));
    }
    
    public function connect()
    {
        try
        {
            $this->my = new mysqli( $this->config["mysql_host"], 
                $this->config["mysql_user"], 
                $this->config["mysql_pass"],
                $this->config["mysql_db"]);        
        
            if (!mysqli_connect_errno()) { 
                $this->isConnected = true;
            }
        	    
        }
        catch (Exception $exception)
        {
                $this->isConnected = false;
        }
        
    }
    public function disconnect()
    {
        if( $this->isConnected )
            $this->my->close();
    }
    
    public function GetSetParams($p)
    {
        array_walk($p, create_function('&$i,$k','$i=" @$k=$i";'));
        return implode(",", $p);
    }
    public function GetProcParams($p)
    {
        array_walk($p, create_function('&$i,$k','$i=" $i";'));
        return implode(",", $p);
    }
    
    //Escape and quote given string
    public function EaQ($text)
    {
        if( !$this->isConnected )
            return null;

        return sprintf("'%s'", $this->my->real_escape_string( $text ));
    }
}
