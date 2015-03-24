<?php
require_once 'Model/MissionEvent.php';
require_once 'Model/Auth.php';

/**
 * IndexController short summary.
 *
 * IndexController description.
 *
 * @version 1.0
 * @author Anton
 */
class IndexController
{
    private $templates;
    function __construct( League\Plates\Engine $templates)
    {
    	$this->templates = $templates;
        $auth = new Auth();
        $this->templates->addData([
            'isloggedin' => $auth->IsLoggedIn(),
            'currentuser' => $auth->CurrentUser,
            'stattoken' => $auth->StatToken,
            ]);
    }
    public function GetLogout()
    {
        $auth = new Auth(null,null,null,$this->templates);
        $auth->Logout();
        
    }
    public function GetIndex( )
    {
        return $this->templates->render('kd');
    }
    public function GetKD( )
    {
        return $this->templates->render('kd');
    }
    public function GetSnipers( )
    {
        return $this->templates->render('snipers');
    }
    public function GetSurvivors( )
    {
        return $this->templates->render('survivors');
    }
    
    public function GetConfirm( $token )
    {
        $auth = new Auth();
        if( $auth->ConfirmEmail( $token ) )
        {
            return $this->templates->render('message', ['message' => 'Email confirmed! You can login now.']);       
        }
        else
        {
            return $this->templates->render('message', ['message' => 'Unknown email']);       
        }
    }
    
    public function PostEvent($json)               
    {
        if( !isset($_COOKIE['srvtoken']) && empty($_COOKIE['srvtoken'])  )
            return "TOKEN FAIL";
        if( isset($json)  )
        {                    
            $event = new MissionEvent( $json );        
            if( $event )
            {
                if( $event->SaveToDB() )
                {
                    return "OK";
                }
                else 
                {
                    return "FAIL";
                }
            }        
        }
        return "UNKNOWN EVENT";          
        
    }
    
    public function PostSignUp($request)
    {        
        if( $request->isPost() )
        {
            $email = $request->post('email');
            $password = $request->post('password');
            $auth = new Auth($email, $password, null, $this->templates);  
            $addResult = $auth->AddServerOwner();
            if( $addResult == 'OK')
            {
                return $this->templates->render('message', ['message' => sprintf('Confirmation mail sent to %s',$email)]);                                
            }
            else
            {
                return $this->templates->render('message', ['message' => $addResult]);                
            }

        }
        
        return $this->templates->render('message', ['message' => 'Login failed!']);
    }
    public function PostLogIn($request)
    {
        if( $request->isPost() )
        {
            $email = $request->post('email');
            $password = $request->post('password');
            $authToken = null;
            
            if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
                $authToken = $_COOKIE['authtoken'];
            
            $auth = new Auth($email, $password, $authToken, $this->templates);  
            if( $auth->Login( $request->post('remember')) )
            {                                   
                return $this->GetIndex();
            }
        }
        
        return $this->templates->render('message', ['message' => 'Login failed!']);
    }
    
}

function activeIfMatch($requestUri)
{
    $current_file_name = basename($_SERVER['REQUEST_URI'], ".php");

    if ($current_file_name == $requestUri)
        echo 'class="active"';
}

?>