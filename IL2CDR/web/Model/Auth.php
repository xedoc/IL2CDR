<?php
require_once 'MySQL.php';
require_once 'Utils.php';
require_once 'Mailer.php';
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
    private $email, $password, $authToken;
    public $confirmtoken;
    private $templates;

    public $CurrentUser;
    public $StatToken;

    function __construct($email = null, $password = null, $authToken = null, $templates = null)
    {
        $this->db = new MySQL();
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
                    return 'User already exist!';
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
        if( isset( $_COOKIE['authtoken']) && !empty($_COOKIE['authtoken']) )
        {
            $token =  $_COOKIE['authtoken'];
            $result = $this->db->query( sprintf('CALL Login(%s,%s,%s)', $this->db->EaQ($this->email), $this->db->EaQ($this->password), $this->db->EaQ($token) ));            
            if( isset( $result ))
            {
                if( $obj = $result->fetch_object() )
                {
                    if( isset($obj->Email) && isset( $obj->AuthToken ) && $obj->AuthToken == $_COOKIE['authtoken'] )
                    {
                        $this->CurrentUser = $obj->Email;
                        $this->StatToken = $obj->Token;
                        return true;
                    }
                }
            }             
        }
        return false;
    }
    public function Login($remember)
    {
     	if( !$this->db->IsConnected )
             return false;
        
         $result = $this->db->query( sprintf('CALL Login(%s,%s,%s)', $this->db->EaQ($this->email), $this->db->EaQ($this->password), $this->db->EaQ($this->authToken) ));
         if( isset( $result ))
         {
             if( $obj = $result->fetch_object() )
             {
                 if( isset( $obj->AuthToken ))
                 {
                     if( !$remember && !$remember=='on')
                         setcookie( "authtoken", $obj->AuthToken, 0 , '/');
                     else
                         setcookie( "authtoken", $obj->AuthToken, time()+60*60*24*9000, '/');
                     
                     return true;
                 }
                 
             }
         }
        
        return false;        
    }
     
}
