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
        $this->SetupRoutes();
     
    }
    
    function SetupRoutes( )
    {
        $this->indexController = new IndexController( new Engine('templates') );
    	$this->app->get('/', function() { echo $this->indexController->GetKD(); } );
    	$this->app->get('/kd/', function() { echo $this->indexController->GetKD(); } );
    	$this->app->get('/kdpvp/', function() { echo $this->indexController->GetKDPvP(); } );
    	$this->app->get('/kdpve/', function() { echo $this->indexController->GetKDPvE(); } );
    	$this->app->get('/snipers/', function() { echo $this->indexController->GetSnipers(); } );
    	$this->app->get('/survivors/', function() { echo $this->indexController->GetSurvivors(); } );
    	$this->app->get('/confirm/:token', function($token) { echo $this->indexController->GetConfirm($token); } );
    	$this->app->get('/logout/', function() { $this->indexController->GetLogout(); $this->app->redirect('/'); } );

        //top score json
        $this->app->get('/json/kd/', function() { echo $this->indexController->GetJsonKd($this->app->request); } );
        $this->app->get('/json/kdpvp/', function() { echo $this->indexController->GetJsonKdPvP($this->app->request); } );
        $this->app->get('/json/kdpve/', function() { echo $this->indexController->GetJsonKdPvE($this->app->request); } );
        
        $this->app->post('/e/', function() { 
            echo $this->indexController->PostEvent( gzdecode($this->app->request->getBody())); 
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