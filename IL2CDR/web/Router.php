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

require 'IndexController.php';
require 'Slim/Slim.php';

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
    	$this->app->get('/', function() { echo $this->indexController->GetIndex(); } );
    	$this->app->get('/kd/', function() { echo $this->indexController->GetKD(); } );
    	$this->app->get('/snipers/', function() { echo $this->indexController->GetSnipers(); } );
    	$this->app->get('/survivors/', function() { echo $this->indexController->GetSurvivors(); } );
        $this->app->post('/e/', function() { 
            echo $this->indexController->PostEvent($this->app->request->getBody()); 
        });
        $this->app->run();
    }
    
    
}
?>