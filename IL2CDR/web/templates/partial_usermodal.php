<div class="modal fade" id="userModal" role="dialog">
  <div class="modal-dialog modal-md">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <h4 class="modal-title">User panel</h4>
      </div>
      <div class="modal-body">
          <div class="form-group">
            <label for="token">Global statistics token</label>
            <input type="text" class="form-control" value="<?=$this->e($stattoken)?>" id="token" name="token" placeholder="Global statistics token">
          </div>
      </div>
    </div>
  </div>
</div>
