<?php
require_once 'Model/MissionEvent.php';
require_once 'Model/Auth.php';
require_once 'Model/TopScore.php';
require_once 'Model/TZ.php';
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
    private $tz;
    function __construct( League\Plates\Engine $templates)
    {
    	$this->templates = $templates;
        $auth = new Auth();
        $onpage = 10;
        $this->top = new TopScore();
        $missions = json_decode( $this->top->GetMissions(1,0,$onpage,null));
        $this->tz = new TZ();
        $totalWL =  json_decode($this->top->GetTotalWL(1,0,$onpage,null));
        $this->templates->addData([
            'isloggedin' => $auth->IsLoggedIn(),
            'currentuser' => $auth->CurrentUser,
            'stattoken' => $auth->StatToken,
            'table_wlpvp' => json_decode( $this->top->GetWLPvP(1,0,$onpage,null) ),
            'table_wlpve' => json_decode( $this->top->GetWLPvE(1,0,$onpage,null) ),
            'table_wltotal' => $totalWL,
            'table_missions' => $missions,
            'playersCount' => $totalWL->recordsTotal,
            'missionCount' => $missions->recordsTotal,
            'tz' => $this->tz->GetTimeZone(),
            ]);      
    }
    public function Get10minutesCache($name)
    {
        $token = null;

        if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
            $token = $_COOKIE['authtoken'];
        
        if( $token == null )
        {
            $content = __c()->get($name . '_' . $this->tz->GetTimeZone());
            if( $content == null )
            {
                $content = $this->templates->render($name);
                __c()->set($name . '_' . $this->tz->GetTimeZone(), $content,600);                   
            }                    
        }
        else
        {
            $content = __c()->get($token.$name . '_' . $this->tz->GetTimeZone());
            if( $content == null )
            {
                $content = $this->templates->render($name);
                __c()->set($token.$name . '_' . $this->tz->GetTimeZone() , $content ,600);
            }
            
        }
        if( $content == null )
            $content = $this->templates->render($name);
        
        return $content;
    }
    public function Get10minutesTopCache($draw,$start,$length, $search, $fallback)
    {
        $name = $draw . '_' . $start . '_' . $length . '_' . $search . '_' . $this->tz->GetTimeZone();
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
    public function GetJsonFromCache( $type, $draw, $start, $length, $search )
    {
        $playerCacheStatus = __c()->get('table_players');
        return __c()->get(sprintf("%s_%s_%s_%s_%s_%s_%s", $type, $draw, $start, $length, $search, $playerCacheStatus, $this->tz->GetTimeZone() ));            
    }
    public function AddJsonToCache( $type, $draw, $start, $length, $search, $content )
    {
        $playerCacheStatus = __c()->get('table_players');
        __c()->set( sprintf("%s_%s_%s_%s_%s_%s_%s", $type, $draw, $start, $length, $search, $playerCacheStatus, $this->tz->GetTimeZone() ), $content, 60000);
        return $content;
    }
    public function GetJsonWlPvP($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];
        $from_cache = $this->GetJsonFromCache( 'wlpvp', $draw, $start, $length, $search );
        if( $from_cache != null )
            return $from_cache;
        
        $result = $this->top->GetWLPvP($draw,$start,$length, $search);
        return $this->AddJsonToCache( 'wlpvp', $draw,$start,$length, $search, $result);
    }
    public function GetJsonWlPvE($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];

        $from_cache = $this->GetJsonFromCache( 'wlpve', $draw, $start, $length, $search );
        if( $from_cache != null )
            return $from_cache;
        $result = $this->top->GetWLPvE($draw,$start,$length, $search);
        return $this->AddJsonToCache( 'wlpve', $draw,$start,$length, $search, $result);
    }
    public function GetJsonWl($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];
        $from_cache = $this->GetJsonFromCache( 'wl', $draw, $start, $length, $search );
        if( $from_cache != null )
            return $from_cache;
        $result =  $this->top->GetTotalWL($draw,$start,$length, $search);   
        return $this->AddJsonToCache( 'wl', $draw,$start,$length, $search, $result);
    }    
    public function GetJsonSnipers($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];
        $from_cache = $this->GetJsonFromCache( 'snipers', $draw, $start, $length, $search );
        if( $from_cache != null )
            return $from_cache;
        $result =  $this->top->GetTotalSnipers($draw,$start,$length, $search);
        return $this->AddJsonToCache( 'snipers', $draw,$start,$length, $search, $result);
    }
    public function GetJsonMissions($request)
    {
        $draw = $request->get('draw');
        $start =  $request->get('start');
        $length = $request->get('length');
        $search = $request->get('search')['value'];
        $from_cache = $this->GetJsonFromCache( 'missions', $draw, $start, $length, $search );
        if( $from_cache != null )
            return $from_cache;
        $result =  $this->top->GetMissions($draw,$start,$length, $search);
        return $this->AddJsonToCache( 'missions', $draw,$start,$length, $search, $result);
    }
    public function GetMissions()
    {
        return  $this->Get10minutesCache('missions');
    }
    public function GetIndex( )
    {
        return  $this->Get10minutesCache('wlpvp');
    }
    public function GetWLPvP()
    {
        return $this->Get10minutesCache('wlpvp');
    }
    public function GetWLPvE()
    {
        return $this->Get10minutesCache('wlpve');
    }
    public function GetWL( )
    {
        return $this->Get10minutesCache('wl');
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
            unset($event);
            $event = null;
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