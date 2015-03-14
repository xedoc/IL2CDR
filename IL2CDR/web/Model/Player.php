<?php
require_once 'Utils.php';
/**
 * Player short summary.
 *
 * Player description.
 *
 * @version 1.0
 * @author user
 */
class Player
{
    protected $nickname = 'lorem';
    protected $kills = 0;
    protected $deaths = 0;
    protected $hits = 0;
    protected $sorties = 0;
    
    
    public function getKD()
    {
        return format_2dp( max(1,$this->getKills()) / max(1,$this->getDeaths()) );
    }
    public function getHitsPerKill()
    {
        return format_2dp( max(1,$this->getKills()) / max(1, $this->getHits()) );
    }
    public function getSurviveRate()
    {
        return format_2dp( max(1, $this->getSorties()) / max(1, $this->getDeaths() ));
    }
    
    public function getKills()
    {
        return $this->kills;
    }
    public function setKills($kills)
    {
        $this->kills = $kills;
    }
    public function getNickname()
    {
        return $this->nickname;
    }
    public function setNickname($value)
    {
        $this->nickname = $value;
    }
    public function setDeaths($value)
    {
        $this->deaths = $value;
    }
    public function getDeaths()
    {
        return $this->deaths;
    }
    public function setHits($value)
    {
        $this->hits = $value;
    }
    public function getHits()
    {
        return $this->hits;
    }
    public function setSorties($value)
    {
        $this->sorties = $value;
    }
    public function getSorties()
    {
        return $this->sorties;
    }
}
