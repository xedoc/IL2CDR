<?php
require_once 'TZ.php';
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
    private $token;
    private $filter;
    private $keysuffix;
    function __construct()
    {
        $this->tz = new TZ();
        $this->token = '';
        $this->filter = '';
        if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
            $this->token = $_COOKIE['authtoken'];

        if( isset($_COOKIE['filter']) && !empty($_COOKIE['filter'])  )
            $this->filter = $_COOKIE['filter'];
        
        $this->keysuffix = sprintf('_%s_%s_%s', $this->tz->GetTimeZone(), $this->filter, $this->token );
    }
    public function AddCache($name, $seconds, $content)
    {
        __c()->set($name . $this->keysuffix, $content,$seconds);                   
    }
    public function GetCache($name)
    {
        $content = __c()->get($name . $this->keysuffix);
        return $content;    
    }
    
    public function UpdateTimestamp($name)
    {
        $this->AddCache( $name . $this->keysuffix, 3600, time() );
    }
    
    public function Delete($name)
    {
        __c()->delete($name . $this->keysuffix );
    }
}
