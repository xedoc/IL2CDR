<?php
require_once 'Auth.php';
/**
 * WebRcon short summary.
 *
 * WebRcon description.
 *
 * @version 1.0
 * @author Anton
 */
class WebRcon
{
    private $auth;
    function __construct($auth)
    {
        $this->auth = new Auth();
    }
    
    public function ExecCommand( $cmd, $serverid )
    {
        if( !$this->auth->IsLoggedIn() )
            return;
                
        
    }
}
