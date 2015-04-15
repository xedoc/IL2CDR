<?php
require_once 'plates/Template/Directory.php';
require_once 'plates/Template/Data.php';
require_once 'plates/Template/FileExtension.php';
require_once 'plates/Template/Folder.php';
require_once 'plates/Template/Folders.php';
require_once 'plates/Template/Func.php';
require_once 'plates/Template/Functions.php';
require_once 'plates/Template/Name.php';
require_once 'plates/Template/Template.php';
require_once 'plates/Engine.php';

require_once 'IndexController.php';
require_once 'Slim/Slim.php';

use Slim\Slim;
use League\Plates\Engine;

/**
 * Router short summary.
 *
 * Router description.
 *
 * @version 1.0
 * @author Anton
 */
class Router
{
    private $app;
    private $indexController;
    function __construct()
    {
        Slim::registerAutoloader();
        $this->app = new Slim();
        //Content is gzipped anyway
        //$this->app->add( new \Slim\Middleware\Minify());
        $this->SetupRoutes();
     
    }
    
    function SetupRoutes( )
    {
        $this->indexController = new IndexController( new Engine('templates') );
    	$this->app->get('/', function() { echo $this->indexController->GetWL(); } );
    	$this->app->get('/wl/', function() { echo $this->indexController->GetWL(); } );
    	$this->app->get('/wlpvp/', function() { echo $this->indexController->GetWLPvP(); } );
    	$this->app->get('/wlpve/', function() { echo $this->indexController->GetWLPvE(); } );
    	$this->app->get('/snipers/', function() { echo $this->indexController->GetSnipers(); } );
    	$this->app->get('/survivors/', function() { echo $this->indexController->GetSurvivors(); } );
    	$this->app->get('/confirm/:token', function($token) { echo $this->indexController->GetConfirm($token); } );
    	$this->app->get('/logout/', function() { $this->indexController->GetLogout(); $this->app->redirect('/'); } );
    	$this->app->get('/missions/', function() { echo $this->indexController->GetMissions(); } );
    	$this->app->get('/servers/', function() { echo $this->indexController->GetServers(); } );
    	$this->app->get('/monitor/', function() { echo $this->indexController->GetMonitor(); } );

        $this->app->get('/json/wl/', function() { echo $this->indexController->GetJsonWl($this->app->request); } );
        $this->app->get('/json/wlpvp/', function() { echo $this->indexController->GetJsonWlPvP($this->app->request); } );
        $this->app->get('/json/wlpve/', function() { echo $this->indexController->GetJsonWlPvE($this->app->request); } );
        $this->app->get('/json/snipers/', function() { echo $this->indexController->GetJsonSnipers($this->app->request); } );
        $this->app->get('/json/missions/', function() { echo $this->indexController->GetJsonMissions($this->app->request); } );
        $this->app->get('/json/playerlist/:serverid', function($serverid) { echo $this->indexController->GetJsonPlayerList($serverid); } );
        
        
        $this->app->post('/e/', function() { 
            $unzipped = gzdecode($this->app->request->getBody());
            echo $this->indexController->PostEvent( $unzipped ); 
            unset($unzipped);
            $unzipped = null;
        });

        $this->app->post('/update/players/', function() { 
            $this->indexController->PostPlayers( $this->app->request ); 
        });
        $this->app->post('/update/servers/', function() { 
            $this->indexController->PostServers( $this->app->request );
            $this->app->redirect('/servers');
        });
        $this->app->post('/update/filter/', function() { 
            $this->indexController->PostFilter( $this->app->request );
        });
        $this->app->post('/signup/', function() { 
            echo $this->indexController->PostSignUp( $this->app->request ); 
        });
        $this->app->post('/login/', function() { 
            $this->indexController->PostLogIn( $this->app->request ); $this->app->redirect('/');
        });
        
        $this->app->run();
    }
    
    
}
?>