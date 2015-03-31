<?php $this->layout('layout', ['title' => 'Top Wins/Losses'])?>
<div class="container-fluid">
  <div class="row">
    <div class="col-md-6 col-md-offset-3">
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
