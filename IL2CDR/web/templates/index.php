<?php $this->layout('layout', ['title' => 'IL2 Leaderboard'])?>
<div class="container-fluid">
  <div class="row">
    <div class="col-md-6 col-md-offset-3">
        <table id="table_kd" class="nowrap table table-striped table-bordered">
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
