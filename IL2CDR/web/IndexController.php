<?php
require_once 'Model/MissionEvent.php';
require_once 'Model/Auth.php';
require_once 'Model/TopScore.php';
require 'phpfastcache.php';

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
    private $cache;
    private $top;
    function __construct( League\Plates\Engine $templates)
    {
    	$this->templates = $templates;
        $auth = new Auth();
        $this->templates->addData([
            'isloggedin' => $auth->IsLoggedIn(),
            'currentuser' => $auth->CurrentUser,
            'stattoken' => $auth->StatToken,
            ]);      
        $this->top = new TopScore();
    }
    public function Get10minutesCache($name)
    {
        $token = null;
        if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
            $token = $_COOKIE['authtoken'];
        
        if( $token == null )
        {
            $content = __c()->get($name);
            if( $content == null )
            {
                $content = $this->templates->render($name);
                __c()->set($name, $content,600);                   
            }                    
        }
        else
        {
            $content = __c()->get($token.$name);
            if( $content == null )
            {
                $content = $this->templates->render($name);
                __c()->set($token.$name, $content ,600);
            }
            
        }
        if( $content == null )
            $content = $this->templates->render($name);
        
        return $content;
    }
    public function Get10minutesTopCache($draw,$start,$length, $search, $fallback)
    {
        $name = $draw . '_' . $start . '_' . $length . '_' . $search;
        $token = null;
        
        if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
            $token = $_COOKIE['authtoken'];
        
        if( $token == null )
        {
            $content = __c()->get($name);
            if( $content == null )
            {
                $content = $fallback($draw,$start,$length, $search);
                __c()->set($name, $content,600);                   
            }                    
        }
        else
        {
            $content = __c()->get($token.$name);
            if( $content == null )
            {
                $content = $fallback($draw,$start,$length, $search);
                __c()->set($token.$name, $content ,600);
            }
            
        }
        if( $content == null )
            $content = $fallback();
        
        return $content;
    }
    public function GetLogout()
    {
        $auth = new Auth(null,null,null,$this->templates);
        $auth->Logout();
        
    }
    public function GetJsonKdPvP($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];
        return $this->top->GetKDPvP($draw,$start,$length, $search);
    }
    public function GetJsonKdPvE($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];
        return $this->top->GetKDPvE($draw,$start,$length, $search);
    }
    public function GetJsonKd($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];
        return $this->top->GetTotalKD($draw,$start,$length, $search);
    }
    
    public function GetIndex( )
    {
        return  $this->Get10minutesCache('index');
    }
    public function GetKDPvP()
    {
        return $this->Get10minutesCache('kdpvp');
    }
    public function GetKDPvE()
    {
        return $this->Get10minutesCache('kdpve');
    }
    public function GetKD( )
    {
        return $this->Get10minutesCache('kd');
    }
    public function GetSnipers( )
    {
        return $this->Get10minutesCache('snipers');
    }
    public function GetSurvivors( )
    {
        return $this->Get10minutesCache('survivors');
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

function activeIfStartsWith($requestUri)
{
    $current_file_name = basename($_SERVER['REQUEST_URI'], ".php");
    if( substr( $current_file_name,0,strlen($requestUri))==$requestUri)
        echo 'class="active"';
}

?>