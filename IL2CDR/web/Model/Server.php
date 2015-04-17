<?php

/**
 * Server short summary.
 *
 * Server description.
 *
 * @version 1.0
 * @author user
 */
class Server
{
    public $Name, $Id;
    public $IsHidden;
    public $PlayerCount;
    public $IsInFilter;
    function __construct($name, $id)
    {
        $this->Name = $name;
        $this->Id = $id;
    }
}
