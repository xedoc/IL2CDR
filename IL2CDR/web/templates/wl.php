<?php $this->layout('layout', ['title' => 'Top - Total Wins/Losses'])?>
    <div class="row">
        <div class="col-md-offset-3 col-md-6 text-center">
             <?php $this->insert('partial_wltabs') ?>
            <h3>Total wins per loss</h3>
            <small class="text-muted"><i>Players,bots and ground units</i></small>
        </div>
    </div>
    <div class="row">
        <div class="top-buffer container-fluid">
          <div class="row">
            <div class="col-md-8 col-md-offset-2">
            <table id="table_wl" class="nowrap table table-striped table-bordered">
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
            </table>
                <?php $this->insert('partial_ajaxloader') ?>
            </div>
          </div>    
        </div>
    </div>
