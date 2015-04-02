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
            $totalshots = intval($row->totalshots);
            return (object)array(
                    '0' => intval($row->rank),
                    '1' => $nickname,
                    '2' => format_2dp( 100 * $row->totalhits / max(1,$totalshots)) . '%',
                    '3' => $totalshots,
                    '4' => intval($row->totalhits),
                    '5' => $total_kills,
                    "DT_RowId" => "sn" . $i,                
                );
        });
    }
    public function GetMissions($draw, $start, $length, $search)
    {
        return $this->GetDataTable($draw, $start, $length, $search, "ReportMissionResults", function($row,$i) {
                return (object)array(
                        '0' => $row->ServerName,
                        '1' => $row->MissionStartTime,                    
                        '2' => $row->MissionEndTime,
                        '3' => $row->C2PlanesScore,
                        '4' => $row->C1PlanesScore,
                        '5' => $row->C2GroundScore,
                        '6' => $row->C1GroundScore,
                        "DT_RowId" => "mis" . $i,                
                    );
            });
    }
    public function GetWLPvP($draw, $start, $length, $search)
    {
        return $this->GetWL($draw, $start, $length, $search, "TopWLPvP" );
    }
    public function GetWLPvE($draw, $start, $length, $search)
    {
        return $this->GetWL($draw, $start, $length, $search, "TopWLPvE" );
    }
    public function GetTotalWL($draw, $start, $length, $search)
    {        
        return $this->GetWL($draw, $start, $length, $search, "TopTotalWL" );
    }
    
    public function GetWL($draw, $start, $length, $search, $procname )
    {
        return $this->GetDataTable($draw, $start, $length, $search, $procname, function($row,$i) {
                $nickname = $row->nickname;
                $wins = intval($row->wins);
                $losses = intval($row->losses);
                return (object)array(
                        '0' => intval($row->rank),
                        '1' => isset($row->division) ? $row->division : "E",                    
                        '2' => $nickname,
                        '3' => format_2dp($wins/max(1,$losses)),
                        '4' => $wins,
                        '5' => $losses,
                        "DT_RowId" => "wl" . $i,                
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
                $table->recordsTotal = $row->recordscount;
                $table->recordsFiltered = $row->recordscount;
                $obj = $func($row, $i);
                
                $i++;
                $table->data[] = $obj;
            }            
            $result->close();
            $this->db->nextresult();
            $json = $table->GetJSON();
            return $json;
        }
        return $default;        
    }
}
