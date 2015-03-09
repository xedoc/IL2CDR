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
        return $this->templates->render('index');
    }
    
    
    
}
