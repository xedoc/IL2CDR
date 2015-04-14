<?php

/**
 * Cache short summary.
 *
 * Cache description.
 *
 * @version 1.0
 * @author meshkov
 */
class Cache
{
    private $tz;
    function __construct()
    {
        $this->tz = new TZ();
    }
    public function AddCache($name, $seconds, $content)
    {
        $token = null;

        if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
            $token = $_COOKIE['authtoken'];
        
        if( $token == null )
        {
            __c()->set($name . '_' . $this->tz->GetTimeZone(), $content,$seconds);                   
        }
        else
        {
            __c()->set($token.$name . '_' . $this->tz->GetTimeZone() , $content ,$seconds);
        }
    }
    public function GetCache($name)
    {
        $token = null;

        if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
            $token = $_COOKIE['authtoken'];
        
        if( $token == null )
        {
            $content = __c()->get($name . '_' . $this->tz->GetTimeZone());
        }
        else
        {
            $content = __c()->get($token.$name . '_' . $this->tz->GetTimeZone());
        }
        return $content;    
    }
    
    public function UpdateTimestamp($name)
    {
        $this->AddCache( $name, 3600, time() );
    }
    
    public function Delete($name)
    {
        __c()->delete($name);
    }
}
