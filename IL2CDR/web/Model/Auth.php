<?php
require_once 'MySQL.php';
require_once 'Utils.php';
require_once 'Mailer.php';
require_once 'Cache.php';

/**
 * Auth short summary.
 *
 * Auth description.
 *
 * @version 1.0
 * @author meshkov
 */
class Auth
{
    private $db;
    public $email, $password, $authToken;
    public $confirmtoken;
    private $templates;

    public $CurrentUser;
    public $StatToken;

    function __construct($email = null, $password = null, $authToken = null, $templates = null, $db)
    {
        $this->db = $db;
        $this->email = $email;
        $this->password = $password;
        $this->authToken = $authToken;
        $this->templates = $templates;

    }
    public function Logout()
    {
        setcookie('authtoken', null, 0, '/');
        $this->templates->addData([
            'isloggedin' => false,
            'currentuser' => '',
            'stattoken' => '',
            ]);

    }
    public function ConfirmEmail($token)
    {
        if( !$this->db->IsConnected )
            return false;
        
        $result = $this->db->query( sprintf('CALL ConfirmEmail(%s)', $this->db->EaQ($token) ));
        if( isset($result ) && $result && $obj = $result->fetch_object())
        {
            if( isset($obj->result ))
            {
                
                return $obj->result;
            }
        }
        
        return false;
    }
    public function AddServerOwner()
    {
        if( !$this->db->IsConnected )
            return 'DB connection error';
        
                    
        $params = array(
            'Email' => $this->db->EaQ($this->email),
            'Password' => $this->db->EaQ($this->password)            
            );
        $this->db->setvars( $params );
        $result = $this->db->callproc( 'AddServerOwner' );

        if( isset($result ) && $result && $obj = $result->fetch_object() )
        {
            if( isset( $obj->result ))
            {
                if( $obj->result == 'DUP')
                {
                                    
                    return 'User already exist!';
                }
                else
                {
                    $this->confirmtoken = $obj->confirmtoken; 
                    $url = sprintf('http://il2.info/confirm/%s', $this->confirmtoken);
                    
                    $body =  $this->templates->render('mail_confirmation', ['url'=>$url]);
                    $m = new Mailer();                    
                    $m->Send($this->email, 'Email Confirmation', $body);
                    
                    
                    return 'OK';
                }
            }
            
        }

        return "Unknown error";
        
    }
    public function IsLoggedIn()
    {
        $cache = new Cache();
        if( isset( $_COOKIE['authtoken']) && !empty($_COOKIE['authtoken']) )
        {
            $token =  $_COOKIE['authtoken'];
        }
        else if( isset( $_COOKIE['srvtoken']) && !empty( $_COOKIE['srvtoken']))
        {
            $token =  $_COOKIE['srvtoken'];
        }
        else
        {
            return false;
        }
        
        if( $cache->GetCache( 'isauth' . $token) )
        {
            $this->CurrentUser = $cache->GetCache( 'email' . $token );
            $this->StatToken = $cache->GetCache( 'token' . $token );
            $this->SetTemplate();
            return true;
        }
                
        $result = $this->db->query( sprintf('CALL Login(%s,%s,%s)', $this->db->EaQ($this->email), $this->db->EaQ($this->password), $this->db->EaQ($token) ));            
        if( $result )
        {
            if( $obj = $result->fetch_object() )
            {
                if( isset($obj->Email) && isset( $obj->AuthToken ) && ($obj->AuthToken == $token || $obj->Token == $token) )
                {
                    $this->CurrentUser = $obj->Email;
                    $this->StatToken = $obj->Token;
                    $cache->AddCache( 'isauth' . $token, 86400, true );   
                    $cache->AddCache( 'email' . $token,  86400, $this->CurrentUser );
                    $cache->AddCache( 'token' . $token, 86400, $this->StatToken  );
                    $this->SetTemplate();
                    return true;
                }
            }
            
        }             
        return false;
    }
    private function SetTemplate()
    {
        $data['currentuser'] = $this->CurrentUser;
        $data['stattoken'] = $this->StatToken;
        $data['isloggedin'] = true;
        $this->templates->addData($data);
    }
    public function Login($remember)
    {
     	if( !$this->db->IsConnected )
             return false;
        
         $result = $this->db->query( sprintf('CALL Login(%s,%s,%s)', $this->db->EaQ($this->email), $this->db->EaQ($this->password), $this->db->EaQ($this->authToken) ));
         if( $result )
         {
             if( $obj = $result->fetch_object() )
             {
                 if( isset( $obj->AuthToken ))
                 {
                     if( !$remember && !$remember=='on')
                         setcookie( "authtoken", $obj->AuthToken, 0 , '/');
                     else
                         setcookie( "authtoken", $obj->AuthToken, time()+60*60*24*9000, '/');
                     
                     $this->CurrentUser = $obj->Email;
                     $this->StatToken = $obj->Token;
                     $this->SetTemplate();
                     return true;
                 }
                 
             }
             
         }
        
        return false;        
    }
     
}
