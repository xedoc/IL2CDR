<?php

function format_2dp( $value )
{
    return number_format((float)$value, 2, ',', '');
}

function generateToken()
{
    return bin2hex(openssl_random_pseudo_bytes(16));
}

?>