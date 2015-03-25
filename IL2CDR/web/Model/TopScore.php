<?php
require_once 'TopDataTable.php';
require_once 'MySQL.php';
require_once 'Utils.php';
/**
 * TopScore short summary.
 *
 * TopScore description.
 *
 * @version 1.0
 * @author meshkov
 */
class TopScore
{
    private $db;
    function __construct()
    {
    	$this->db = new MySQL();
    }
    private function GetCacheKey( $toptype,$draw,$start,$length, $search )
    {
        $name = $toptype . '_' . $draw . '_' . $start . '_' . $length . '_' . $search;
        
        $token = null;
        if( isset($_COOKIE['authtoken']) && !empty($_COOKIE['authtoken'])  )
            $token = $_COOKIE['authtoken'];
        
        if( $token == null )
        {
            return $name;            
        }
        else
        {
            return $token.$name;            
        }        
    }
    
    public function GetTotalSnipers($draw, $start, $length, $search)
    {
        return $this->GetDataTable($draw, $start, $length, $search, "TopTotalSnipers", function($row,$i) {
            $nickname = $row->nickname;
            $total_kills = intval($row->total_kills);
            $total_deaths = intval($row->total_deaths);
            return (object)array(
                    '0' => intval($row->rank),
                    '1' => $nickname,
                    '2' => format_2dp($total_kills/max(1,$total_deaths)),
                    '3' => $total_kills,
                    '4' => $total_deaths,
                    "DT_RowId" => "kd" . $i,                
                );
        });
    }
    
    public function GetTotalKD($draw, $start, $length, $search)
    {        
        return $this->GetDataTable($draw, $start, $length, $search, "TopTotalKD", function($row,$i) {
            $nickname = $row->nickname;
            $total_kills = intval($row->total_kills);
            $total_deaths = intval($row->total_deaths);
            return (object)array(
                    '0' => intval($row->rank),
                    '1' => $nickname,
                    '2' => format_2dp($total_kills/max(1,$total_deaths)),
                    '3' => $total_kills,
                    '4' => $total_deaths,
                    "DT_RowId" => "kd" . $i,                
                );
        });
    }
    
    private function GetDataTable($draw, $start, $length, $search, $procname, $func)
    {    
        $default = json_encode( new TopDataTable( $draw, 0,0,array()) );
        if( !$this->db->IsConnected )
            return $default;
        
        $result = $this->db->query( sprintf("CALL %s(%s,%s,%s)", $procname, $this->db->EaQ($search), $this->db->EaQ($start), $this->db->EaQ($length)) );
        if( $result )
        {
            $count = $result->num_rows;
            
            $table = new TopDataTable($draw, 0, $count, array());
            
            $i = intval($start);
            while( $row = $result->fetch_object())
            {
                $table->recordsTotal = $row->playercount;
                $table->recordsFiltered = $row->playercount;
                $obj = $func($row, $i);
                
                $i++;
                $table->data[] = $obj;
            }            
            $result->close();   
            $json = $table->GetJSON();
            return $json;
        }
        return $default;        
    }
}
