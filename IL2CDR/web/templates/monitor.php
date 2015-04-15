<?php $this->layout('layout', ['title' => 'Online monitor'])?>
<div class="row clearfix">
		<div class="col-md-6 column">
			<div class="panel panel-default">
				<div class="panel-heading">
					<h3 class="panel-title">
						Servers
					</h3>
				</div>
				<div class="panel-body  serverlist">
			        <ul class="nav nav-pills nav-stacked">
                        <?php foreach( $playersbyserver as $server): ?>
				        <?php if($firstserverid == $server->Id ): ?>
                        <li class="active">
                            <?php else: ?>
                        <li>
                            
                            <?php endif ?>
					             <a class="serveritem" data-id="<?=$server->Id?>" href="#"> 
                                     <span class="badge pull-right">
                                        <?=$server->PlayerCount?>
                                     </span>
                                     <?=$server->Name?>
					             </a>
				            </li>
                        <?php endforeach ?>
			        </ul>
				</div>
			</div>
		</div>
		<div class="col-md-6 column">
    		<div class="panel panel-default">
				<div class="panel-heading">
					<h3 class="panel-title">
						Players
					</h3>
				</div>
				<div class="panel-body">
                    <table class="table table-condensed">
				<thead>
					<tr>
						<th>
							Country
						</th>
						<th>
							Nickname
						</th>
						<th>
							Ping
						</th>
					</tr>
				</thead>
				<tbody id="playerlist">
                    <?php foreach( $serverplayers as $player): ?>
					    <tr>
                    		<td>
							    <?=$player->Country?>
						    </td>
						    <td>
							    <?=$player->Nickname?>
						    </td>
						    <td>
							    <?=$player->Ping?>
						    </td>
					    </tr>
                    <?php endforeach ?>

				</tbody>
			</table>
				</div>
			</div>
			            <div class="panel panel-default">
				<div class="panel-heading">
					<h3 class="panel-title">
						Description
					</h3>
				</div>
				<div class="panel-body">
			        <p>
				        Donec id elit non mi porta gravida at eget metus. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. Etiam porta sem malesuada magna mollis euismod. Donec sed odio dui.
			        </p>
			        <p>
				        <a class="btn" href="#">Details &gt;&gt;</a>
			        </p>
				</div>
			</div>			
		</div>

	</div>
