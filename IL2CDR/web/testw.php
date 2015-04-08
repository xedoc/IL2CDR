<?php

require 'Model/FifoPipe.php';
$pipe = new FifoPipe("test");
$pipe->Write('test');
?>