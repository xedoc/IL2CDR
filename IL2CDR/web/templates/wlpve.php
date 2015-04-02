<?php $this->layout('layout', ['title' => 'Top - PvE W/L'])?>

<div class="row">
    <div class="col-md-offset-3 col-md-6 text-center">
    <?php $this->insert('partial_wltabs') ?>
    <h3>Bot/Ground units per loss</h3>
    <small class="text-muted"><i>Non player controlled planes, crashes, ground and static units</i></small>
    </div>
</div>
<div class="row">
    <div class="top-buffer container-fluid">
        <div class="row">
        <div class="col-md-8 col-md-offset-2">
            <table id="table_wlpve" class="nowrap table table-striped table-bordered">
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
                <?php $this->insert('partial_toptable', ['table' => $table_wlpve]) ?>
            </table>
        </div>
        </div>    
    </div>
</div>

