<?php
require 'Model/FifoPipe.php';

$pipe = new FifoPipe("test");

if( $_GET['w'] == 'w' )
    $pipe->Write('test');
else
    $pipe->Read();

?>