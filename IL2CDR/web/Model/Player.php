<?php

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
        return max(1,$kills) / max(1,$deaths);
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
    }
    public function setNickname()
    {
    }
    public function setDeaths()
    {
    }
    public function getDeaths()
    {
    }
    public function setHits()
    {
    }
    public function getHits()
    {
    }
    public function setSorties()
    {
    }
    public function getSorties()
    {
    }
}
