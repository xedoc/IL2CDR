<?php

/**
 * FifoPipe short summary.
 *
 * FifoPipe description.
 *
 * @version 1.0
 * @author meshkov
 */
class FifoPipe
{
    private $filename;
    function __construct($id)
    {
        $this->filename = "../fifo_" . $id . ".pipe";
        
        
        if(!file_exists($this->filename))
        {
            if (function_exists('posix_mkfifo'))
            {
                posix_mkfifo($this->filename, 0700);
            }
            else
            {
                $this->filename = sprintf("testpipe", php_uname('n'));
                touch($this->filename);
            }
        }
        
    }
    
    public function Write($text)
    {        
        $pipe_write = fopen($this->filename, 'w');
        fwrite($pipe_write, "Hello world\n");        
        fclose($pipe_write);
    }
    
    public function Read()
    {       
        touch($this->filename);
    	$pipe_read = fopen($this->filename, 'r');
        $output = fgets($pipe_read);                       
        if (!function_exists('posix_mkfifo'))
        {
            $i = 10;
           while(!$output)
           {
               usleep(500000);
               $output = fgets($pipe_read);
               $i--;
               if( $i <= 0 )
                   break;
           }
           fclose($pipe_read);
           unlink($this->filename);
        }
        else
        {
            fclose($pipe_read);
        }
        echo 'Received from the pipe: '. $output . "\n";        

    }
    
}
