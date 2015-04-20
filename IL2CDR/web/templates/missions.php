 <?php $this->layout('layout', ['title' => 'Missions'])?>

    <div class="row">
        <div class="col-md-12 text-center">
            <h3>Mission results</h3>
            <small class="text-muted"><i>Current, finished and interrupted</i></small>
        </div>
    </div>
    <div class="row">
        <div class="top-buffer">
          <div class="row">
            <div class="col-md-10 col-md-offset-1">
                <div class="form-inline dataTables_filter">
                    <label>Search:
                        <input type="search" class="form-control input-sm searchinput" data-searchtarget="table_missions" placeholder="Search mission">
                    </label>
                </div>
                <table id="table_missions" class="nowrap table table-striped table-bordered">
                    <thead>
                        <tr>
                            <th>SERVER <p><small>time</small></p></th>
                            <th>MISSION</th>
                            <th>SCORE</th>
                            <th>WINNER</th>
                        </tr>
                    </thead>
                     <?php $this->insert('partial_toptable', ['table' => $table_missions]) ?>
                </table>

                </div>
            </div>
        </div>    
    </div>

