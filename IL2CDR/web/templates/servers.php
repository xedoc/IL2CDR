<?php $this->layout('layout', ['title' => 'Game servers management'])?>
    <h3 class="text-center">Server management</h3>
    <form role="form" method="post" action="/update/servers">
    <div class=" col-md-8 col-md-offset-2">

        <?php if( count($servers) > 0): ?>
        <div class="row">
            <div class="form-group text-center">
                <div class="col-md-2">                   
                    Hide stats
                </div>
                <div class="col-md-10 text-left">
                    Server Name
                </div>
            </div>        
        </div>
        <?php for( $i=0; $i<count($servers); $i++): ?>
        <div class="row">
            <div class="form-group">
                <input type="hidden" name="servers[]" value="<?=$servers[$i]->Id?>" />
                <div class="col-md-2 text-center">                   
                    <div class="form-control"><input name="ishidden[]" value="<?=$i?>" type="checkbox" <?=$servers[$i]->IsHidden?'checked':''?>/></div>
                </div>
                <div class="col-md-8">
                    <label class="form-control"><?=$servers[$i]->Name?></label>
                </div>
                <div class="col-md-2">
                    <input data-toggle="modal" data-target="#rconModal" type="button" value="RCON" class="btn btn-warning"/>
                </div>
            </div>
        </div>
        <?php endfor ?>
       

        <div class="row">
            <div class="form-group">
                <div class="top-buffer col-md-2 col-md-offset-5"><input type="submit" class="btn btn-success" value="Apply"/></div>
            </div>
        </div>
        <?php else:?>
        <div class="col-md-12 alert alert-danger" role="alert">
            <div class="center-block text-center">
            <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span><span>&nbsp;You don't have any servers to manage</span>
            </div>
        </div>
        <?php endif?>
    </div>
    </form>

<div class="modal fade" id="rconModal" role="dialog">
  <div class="modal-dialog modal-md">
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <h4 class="modal-title">RCON for </h4>
      </div>
      <div class="modal-body">

      </div>
    </div>
  </div>
</div>
