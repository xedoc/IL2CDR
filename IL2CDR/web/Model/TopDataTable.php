<?php
/**
 * TopDataTable short summary.
 *
 * TopDataTable description.
 *
 * @version 1.0
 * @author meshkov
 */
class TopDataTable
{
    public $draw = 1;
    public $recordsTotal = 1;
    public $recordsFiltered = 1;
    public $data = array();
    
    function __construct($draw, $recordsTotal, $recordsFiltered, $data)
    {
    	$this->draw = intval($draw);
        $this->recordsFiltered = intval($recordsFiltered);
        $this->recordsTotal = intval($recordsTotal);
        $this->data = $data;
    }
    
    public function GetJSON()
    {
        return json_encode($this);
    }
    
}
