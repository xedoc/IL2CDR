<?php
require_once 'TopDataTable.php';
require_once 'MySQL.php';
require_once 'Utils.php';
require_once 'Filter.php';
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
    function __construct($db)
    {
    	$this->db = $db;
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
        return $this->GetDataTable($draw, $start, $length, $search, "ReportMissionResults", function($row,$i) 
        {
            $c1total = $row->C1PlanesScore + $row->C1GroundScore;
            $c2total = $row->C2PlanesScore + $row->C2GroundScore;
       
            $startDate = date("M j,", strtotime($row->MissionStartTime));
            $startTime = date( " H:i", strtotime($row->MissionStartTime));
            $endDate = '';
            if( !empty($row->MissionEndTime) )
            {
                $endDate = date("M j,", strtotime( $row->MissionEndTime ));
                $endTime = date( " H:i", strtotime($row->MissionEndTime ));
            }                     
            else
            {
                $endTime = "--:--";
            }
            $startIcon = "<span class=\"glyphicon glyphicon-play\"></span>";
            $stopIcon = "<span class=\"glyphicon glyphicon-stop\"></span>";
            $axisIcon = "<span class=\"tdicon icon-axis\"></span>";
            $sovietIcon = "<span class=\"tdicon icon-soviet\"></span>";
            $mission = strtolower(basename($row->MissionFile, ".msnbin"));
            $planeIcon = "<span class=\"tdicon icon-plane\"></span>";
            $groundIcon = "<span class=\"tdicon icon-ground\"></span>";
            $sovietScore = sprintf("%s %s %s %s %s", $sovietIcon, $planeIcon, $row->C1PlanesScore, $groundIcon, $row->C1GroundScore);
            $axisScore = sprintf("%s %s %s %s %s", $axisIcon, $planeIcon, $row->C2PlanesScore, $groundIcon, $row->C2GroundScore);
            
            if( $row->ObjectiveCompleteBy )
            {
                $winnerIcon = $row->ObjectiveCompleteBy == "1" ? $sovietIcon : $axisIcon;
                $trigger = "objective completed";
            }
            else
            {
                if( $c1total > $c2total )
                    $winnerIcon = $sovietIcon;
                else if( $c1total < $c2total )
                    $winnerIcon = $axisIcon;
                else
                    $winnerIcon = '<b>DRAW</b>';                
                
                $trigger = "by score";
            }
            
            
            
            $winner = sprintf( "%s<br>%s", $winnerIcon, $trigger);
            $startend = sprintf("%s %s%s %s %s%s", $startIcon, $startDate, $startTime, $stopIcon, $endDate, $endTime);
            return (object)array(
                    '0' => sprintf("<strong>%s</strong><p><small>%s</small></p>", $row->ServerName, $startend),
                    '1' => $mission,
                    '2' => sprintf("%s<br/>%s", $sovietScore, $axisScore),
                    '3' => $winner,
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
        $default = json_encode( new TopDataTable( $draw, 0,0,array()), JSON_HEX_QUOT | JSON_HEX_TAG | JSON_UNESCAPED_SLASHES );
        if( !$this->db->IsConnected )
            return $default;
        
        $filter = new Filter($this->db);
        $params = array(
            'Filter' => $this->db->EaQ($filter->GetCurrentFilter()),       
            );
        $this->db->setvars( $params );
        $result = $this->db->query( sprintf("CALL %s(%s,%s,%s)", $procname, $this->db->EaQ($search), $this->db->EaQ($start), $this->db->EaQ($length)) );
        if( $result )
        {
            $count = $result->num_rows;
            if( $count <= 0 )
            {
                
                return $default;                
            }
             
            
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
            
            $json = $table->GetJSON();

            return $json;
        }
        return $default;        
    }
}
