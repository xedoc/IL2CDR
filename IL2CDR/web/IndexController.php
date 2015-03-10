<?php
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
    
    
    
}

function activeIfMatch($requestUri)
{
    $current_file_name = basename($_SERVER['REQUEST_URI'], ".php");

    if ($current_file_name == $requestUri)
        echo 'class="active"';
}