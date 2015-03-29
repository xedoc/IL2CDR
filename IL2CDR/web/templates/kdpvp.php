<?php $this->layout('layout', ['title' => 'Leaderboard - PvP K/D'])?>
<div class="container">
    <div class="row">
        <div class="col-md-offset-3 col-md-6 text-center">
        <?php $this->insert('partial_kdtabs') ?>
        <h3>Player kills per death</h3>
        <small class="text-muted"><i>Player kills, kills by players and crashes</i></small>
        </div>
    </div>
    <div class="row">
        <div class="top-buffer container-fluid">
          <div class="row">
            <div class="col-md-8 col-md-offset-2">
                <table id="table_kdpvp" class="nowrap table table-striped table-bordered">
              <thead>
                <tr>
                  <th>#</th>
                    <th>Division</th>
                  <th>Nickname</th>
                  <th>K/D rate</th>
                  <th>Kills</th>
                  <th>Deaths</th>
                </tr>
              </thead>
            </table>
                                <?php $this->insert('partial_ajaxloader') ?>
            </div>
          </div>    
        </div>
    </div>
</div>
