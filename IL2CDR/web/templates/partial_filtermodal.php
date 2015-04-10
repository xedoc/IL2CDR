<div class="modal fade" id="filterModal" role="dialog">
  <div class="modal-dialog modal-sm">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <h4 class="modal-title">Filter stats</h4>
      </div>
      <div class="modal-body">
          <form id="filter" action="/update/filter" method="post">
             <div class="form-group">
                <label for="servernamefilter">Server name:</label>
                  <select name="servers[]" class="selectpicker">
                      <?php $this->insert('partial_serverlist') ?>
                  </select>                 
             </div>
             <div class="form-group">
                <label for="servernamefilter">Difficulty:</label>
                  <select name="difficulties[]" class="selectpicker">
                      <?php $this->insert('partial_serverdifficulty') ?>
                  </select>                 
             </div>
             <button type="submit" class="btn btn-primary">Apply</button>
          </form>
      </div>
    </div>
  </div>
</div>
