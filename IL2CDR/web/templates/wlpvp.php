<?php $this->layout('layout', ['title' => 'Top - PvP W/L'])?>
    <div class="row">
        <div class="col-md-offset-3 col-md-6 text-center">
        <?php $this->insert('partial_wltabs'); ?>
        <h3>Player wins per loss</h3>
        <small class="text-muted"><i>Planes lost in combat with players and crashes</i></small>
        </div>
    </div>
    <div class="row">
        <div class="top-buffer container-fluid">
          <div class="row">
            <div class="col-md-8 col-md-offset-2">
            <div class="form-inline dataTables_filter">
                <label>Search:
                    <input type="search" class="form-control input-sm searchinput" data-searchtarget="table_wlpvp" placeholder="Search player">
                </label>
            </div>
            <table id="table_wlpvp" class="nowrap table table-striped table-bordered">
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Division</th>
                        <th>Nickname</th>
                        <th>W/L rate</th>
                        <th>Wins</th>
                        <th>Losses</th>
                    </tr>
                </thead>
                <?php $this->insert('partial_toptable', ['table' => $table_wlpvp]) ?>
            </table>
            </div>
          </div>    
        </div>
    </div>

