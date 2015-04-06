 <?php $this->layout('layout', ['title' => 'Missions'])?>

    <div class="row">
        <div class="col-md-offset-3 col-md-6 text-center">
            <h3>Mission results</h3>
            <small class="text-muted"><i>Current, finished and interrupted</i></small>
        </div>
    </div>
    <div class="row">
        <div class="top-buffer">
          <div class="row">
            <div class="col-md-6 col-md-offset-3">
                <table id="table_missions" class="table table-striped table-bordered table-responsive">
                    <thead>
                        <tr>
                            <th class="tdcenter">Server<br /><small>start/end time</small></th>
                            <th class="tdcenter"><h4><?php $this->insert('svg/star', ['color' => '#AB0000', 'width' => '24px']) ?></h4>1st coalition</th>
                            <th class="tdcenter"><h4><?php $this->insert('svg/luft', ['color' => '#0F4981', 'width' => '24px']) ?></h4>2nd coalition</th>
                        </tr>
                    </thead>
                     <?php $this->insert('partial_toptable', ['table' => $table_missions]) ?>
                </table>

                </div>
            </div>
          </div>    
        </div>
    </div>
