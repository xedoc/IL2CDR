<?php $this->layout('layout', ['title' => 'Top - Snipers'])?>

<div class="row">
    <div class="col-md-offset-3 col-md-6 text-center">            
        <h3>Total hits per shot</h3>
        <small class="text-muted"><i>Players vs bots, ground vehicles and static objects. Bombs included.</i></small>
    </div>
</div>
<div class="row">
<div class="top-buffer container-fluid">
    <div class="row">
        <div class="col-md-8 col-md-offset-2">
            <table id="table_snipers" class="nowrap table table-striped table-bordered">
            <thead>
            <tr>
                <th>#</th>
                <th>Nickname</th>
                <th>Hit rate</th>
                <th>Shots</th>
                <th>Hits</th>
                <th>Kills</th>
            </tr>
            </thead>
        </table>
        </div>
    </div>    
</div>
</div>

