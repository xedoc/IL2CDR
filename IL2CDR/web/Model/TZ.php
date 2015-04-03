<?php

/**
 * TimeZone short summary.
 *
 * TimeZone description.
 *
 * @version 1.0
 * @author meshkov
 */
class TZ
{
    public function GetTimeZone()
    {
        if( isset($_COOKIE['tz']) && !empty($_COOKIE['tz'])  )
            return $_COOKIE['tz'];
        else
            return 'Europe/Kiev';
    }
}
