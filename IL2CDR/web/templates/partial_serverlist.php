<?php foreach( $allservers as $server): ?>
 <option value="<?=$server->Id?>" <?=$server->IsInFilter?'selected':''?> ><?=$server->Name?></option>
<?php endforeach ?>
