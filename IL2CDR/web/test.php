<?php
require_once('phpfastcache.php');

//$cache = new phpFastCache("files");

echo 'before: ' .  __c()->get("prodcut"); 
__c()->set("prodcut", "test", 600); 
echo ' after:' .  __c()->get("prodcut"); 


?>