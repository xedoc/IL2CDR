<?php
require_once 'iDatabase.php';
require_once 'TZ.php';
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
    public $IsConnected = false;
    private $connection;
    
    function __construct()
    {
        $this->config = include(__DIR__ . '/../config.php');
        $this->timeZone = new TZ();
        $this->connect();
    }
    public function nextresult()
    {
        $this->my->next_result();
    }
    public function execute($query)
    {
        if( $this->IsConnected )
        {
            $this->my->real_query( $query );
            $this->my->store_result();
        }
        
    }
    public function query($query)
    {
        $result = false;
        if( $this->IsConnected )
        {
            $result = $this->my->query($query);
        }
        
        if( !$result )
        {
            echo $this->my->error;
        }
        
        return $result;

    }
    public function callproc($proc, $params = array())
    {
        if( !$this->IsConnected )
            return null;
        
        $this->TouchCache( $proc );
        $result = $this->query( sprintf("CALL %s(%s)", $proc, $this->GetProcParams( $params )));
        $res = $this->my->store_result();
        if( !$result )
        {
            echo $this->my->error;
        }
        
        return $result;
        
    }
    
    public function setvars($params)
    {
        if( !$this->IsConnected )
            return;
        
        $this->execute( sprintf("SET %s", $this->GetSetParams( $params ) ));
    }
    
    public function SetTimeZone()
    {
        $timezone = new TZ();
        $this->execute(sprintf('SET @UserTZ="%s"',$timezone->GetTimeZone()));
        
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
                $this->IsConnected = true;
                
            }
        	    
        }
        catch (Exception $exception)
        {
            $this->IsConnected = false;
        }
        
    }
    public function disconnect()
    {
        if( $this->IsConnected )
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
        if( !$this->IsConnected )
            return null;

        return sprintf("'%s'", $this->my->real_escape_string( $text ));
    }
    
    public function GetCacheStatus($name)
    {
        return __c()->get($name);
    }
    
    public function TouchCache($name)
    {        
        $players = array( "AddKill", "AddPlayer", "AddSortieEnd");
        
        if( in_array($name, $players ))
        {
            __c()->set('table_players', time(), 24 * 3600 * 30);                    
        }
    }
    
}
