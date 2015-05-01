<?php
function cacheHeaders($lastModifiedDate) {
    if ($lastModifiedDate) {
        if (isset($_SERVER['HTTP_IF_MODIFIED_SINCE']) && strtotime($_SERVER['HTTP_IF_MODIFIED_SINCE']) >= $lastModifiedDate) {
            if (php_sapi_name()=='CGI') {
                Header("Status: 304 Not Modified");
            } else {
                Header("HTTP/1.0 304 Not Modified");
            }
            exit;
        } else {
            $gmtDate = gmdate("D, d M Y H:i:s \G\M\T",$lastModifiedDate);
            header('Last-Modified: '.$gmtDate);
        }
    }
}
function lastModificationTime($time=0) {
    static $last_mod ;
    if (!isset($last_mod) || $time > $last_mod) {
        $last_mod = $time ;
    }
    return $last_mod ;
}

lastModificationTime(filemtime(__FILE__));
cacheHeaders(lastModificationTime());
// First of all send css header
header("Content-type: text/css");

ob_start ("ob_gzhandler");

// Array of css files
$css = array(
    'css/bootstrap.css',
    'css/stats.css',
    'css/bootstrap-select.min.css',
    'css/dataTables.bootstrap.css',
    'css/dataTables.scroller.css',
);

// Prevent a notice
$css_content = '';

// Loop the css Array
foreach ($css as $css_file) {
    // Load the content of the css file 
    $css_content .= file_get_contents($css_file);
}

// print the css content
echo $css_content;
?>