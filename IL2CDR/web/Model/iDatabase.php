<?php

/**
 * Interface1 short summary.
 *
 * Interface1 description.
 *
 * @version 1.0
 * @author meshkov
 */
interface iDatabase
{
    function query($query);
    function connect();
    function disconnect();
}
