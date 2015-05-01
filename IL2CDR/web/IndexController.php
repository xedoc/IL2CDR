<?php
require_once 'Model/MissionEvent.php';
require_once 'Model/Auth.php';
require_once 'Model/TopScore.php';
require_once 'Model/Servers.php';
require_once 'Model/TZ.php';
require_once 'Model/Cache.php';
require_once 'Model/Filter.php';
require_once 'Model/Players.php';
require_once 'Model/Filter.php';
require_once 'Model/Server.php';
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
    private $auth;
    private $datacache;
    private $db;
    private $servers;
    function __construct( League\Plates\Engine $templates)
    {
        $this->db = new MySQL();
    	$this->templates = $templates;               
        $this->auth = new Auth(null, null, null, $templates, $this->db);
        $this->datacache = new Cache();
        $this->top = new TopScore($this->db);
        $this->tz = new TZ();        
        $this->servers = new Servers($this->db);
        $this->cache = new Cache();
        $servers = $this->servers;
           
            
        $playersbyserver = $servers->GetPlayerCountByServer();
            
        $plbyserv = array_values($playersbyserver);
        if( count($plbyserv) > 0 )
            $firstserver = $plbyserv[0];
        else
            $firstserver = new Server('','');
            
        if( count($playersbyserver) > 0 )
            $serverplayers = $servers->GetOnlinePlayers($firstserver->Id);
        else
            $serverplayers = array();
            
        $data = [ 
            'isloggedin' => $this->auth->IsLoggedIn(),     
            'currentuser' => $this->auth->CurrentUser,
            'stattoken' => $this->auth->StatToken,
            'playersCount' => 0,
            'missionCount' => 0,
            'firstserverid' => $firstserver->Id,
            'allservers' => $servers->GetVisibleServers(),
            'playersbyserver' => $playersbyserver,     
            'serverplayers' => $serverplayers,
            'difficulties' => $servers->GetDifficulties(),
            'tz' => $this->tz->GetTimeZone(),
            ];

        $this->templates->addData($data); 
            
        
    }

    public function Get10minutesCache($name)
    {
        $content = $this->cache->GetCache($name);        
        if( $content == null )
        {
            $content = $this->templates->render($name);
            $this->cache->AddCache($name, 600, $content );
        }                    
        
        return $content;
    }
    public function Get10minutesTopCache($draw,$start,$length, $search, $fallback)
    {
        $name = $draw . '_' . $start . '_' . $length . '_' . $search;

        $content = $this->cache->GetCache($name);        
        if( $content == null )
        {
            $content = $fallback($draw,$start,$length, $search);
            $this->cache->AddCache($name, 600, $content );
        }                            
        return $content;
    }
    public function GetLogout()
    {
        $auth = new Auth(null,null,null,$this->templates,$this->db);
        $auth->Logout();        
    }
    public function GetJsonFromCache( $type, $draw, $start, $length, $search )
    {
        $playerCacheStatus = __c()->get('table_players');
        $name = $draw . '_' . $start . '_' . $length . '_' . $search . '_' . $type . '_' . $playerCacheStatus;        
        return $this->cache->GetCache($name);        
    }
    public function AddJsonToCache( $type, $draw, $start, $length, $search, $content )
    {
        $playerCacheStatus = __c()->get('table_players');
        $name = $draw . '_' . $start . '_' . $length . '_' . $search . '_' . $type . '_' . $playerCacheStatus;        
        $this->cache->AddCache($name, 600, $content);
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
    public function GetJsonPlayerList($serverid)
    {
        $servers = $this->servers;
        return json_encode($servers->GetOnlinePlayers( $serverid ), JSON_HEX_QUOT | JSON_HEX_TAG | JSON_UNESCAPED_SLASHES);
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
    public function GetServers()
    {    
        $this->CheckAuth();
        
        $servers = $this->servers;
        return $this->templates->render('servers', ['servers' => $servers->GetServers()]);  
        
    }
    
    public function GetMonitor()
    {
               
        return $this->templates->render('monitor');  
        
    }
    
    public function PostFilter($request)
    {
        if( $request->isPost() )
        {
            $filter = new Filter($this->db);
            $id = $filter->GetFilterId( 
                $request->post('servers'), 
                $request->post('difficulties') 
            );
            

        }
        
    }
    
    public function PostPlayers($request)
    {
        $this->CheckAuth();

        if( $request->isPost() )
        {
            $players = new Players( $request->getBody(), $this->db );
            $players->UpdatePlayersOnline();
        }
        
    }
    
    public function PostServers($request)
    {
        $this->CheckAuth();
        
        if( $request->isPost() )
        {
            $servers = $this->servers;
            $servers->UpdateServers( 
                $request->post('servers'), 
                $request->post('ishidden') );
        }
        
    }
    
    public function CheckAuth()
    {
        $auth = new Auth(null,null,null,$this->templates,$this->db);
        
        if( !$auth->IsLoggedIn() )
        {
            echo $this->templates->render('message', ['message' => 'Authorization required!']);  
            die();
        }        
    }
    
    public function GetMissions()
    {
        $missions = $this->cache->GetCache( 'missions_cache');
        
        if( !$missions )
        {
            $missions = json_decode( $this->top->GetMissions(1,0,10,null));
            $this->cache->AddCache('missions_cache', 600, $missions);            
        }
        
        $data['missionCount'] = $missions->recordsTotal;
        $data['table_missions'] = $missions;
        $this->templates->addData($data);

        return  $this->Get10minutesCache('missions');
    }
    public function GetIndex( )
    {
        return  $this->Get10minutesCache('wlpvp');
    }
    public function GetWLPvP()
    {
        $wl = json_decode( $this->top->GetWLPvP(1,0,10,null) );
        $data['playersCount'] = $wl->recordsTotal;
        $data['table_wlpvp'] = $wl;
        $this->templates->addData($data);
        return $this->Get10minutesCache('wlpvp');
        
    }
    public function GetWLPvE()
    {
        $wl = json_decode( $this->top->GetWLPvE(1,0,10,null) );
        $data['playersCount'] = $wl->recordsTotal;
        $data['table_wlpve'] = $wl;
        $this->templates->addData($data);
        
        return $this->Get10minutesCache('wlpve');
    }
    public function GetWL( )
    {
        $wl = json_decode($this->top->GetTotalWL(1,0,10,null));
        $data['playersCount'] = $wl->recordsTotal;
        $data['table_wltotal'] = $wl;
        $this->templates->addData($data);
        
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
        $auth = new Auth(null,null,null,$this->templates,$this->db);
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
            $event = new MissionEvent( $json, $this->db );        
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
            $auth = new Auth($email, $password, null, $this->templates, $this->db);  
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
            $this->auth->email = $request->post('email');
            $this->auth->password = $request->post('password');
            $this->auth->authToken = null;
            
            if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
                 $this->auth->authToken = $_COOKIE['authtoken'];
                      
            
            
            if( $this->auth->Login( $request->post('remember')) )
            {     
                
                $data['currentuser'] = $this->auth->CurrentUser;
                $data['stattoken'] = $this->auth->StatToken;
                $data['isloggedin'] = $this->auth->IsLoggedIn();
                $this->templates->addData($data);
                return "";
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