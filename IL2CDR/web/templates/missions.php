<?php $this->layout('layout', ['title' => 'Missions'])?>
    <div class="row">
        <div class="col-md-offset-3 col-md-6 text-center">
            <h3>Mission results</h3>
            <small class="text-muted"><i>Current, finished and interrupted</i></small>
        </div>
    </div>
    <div class="row">
        <div class="top-buffer container-fluid">
          <div class="row">
            <div class="col-md-8 col-md-offset-2">
            <table id="table_missions" class="nowrap table table-striped table-bordered">
                <thead>
                    <tr>
                        <th class="tdcenter" rowspan="2">#</th>
                        <th class="tdcenter" rowspan="2">Server</th>
                        <th class="tdcenter" rowspan="2">Start time</th>
                        <th class="tdcenter" rowspan="2">End time</th>
                        <th class="tdcenter" colspan="2">1st coalition</th>
                        <th class="tdcenter" colspan="2">2nd coalition</th>
                    </tr>
                    <tr>
                        <th class="tdcenter">Air</th>
                        <th class="tdcenter">Other</th>
                        <th class="tdcenter">Air</th>
                        <th class="tdcenter">Other</th>
                    </tr>
                </thead>
                 <?php $this->insert('partial_toptable', ['table' => $table_missions]) ?>
            </table>
            </div>
          </div>    
        </div>
    </div>
