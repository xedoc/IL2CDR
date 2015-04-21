<?php $this->layout('layout', ['title' => 'Online monitor'])?>
<div class="row clearfix">
		<div class="col-md-4 column">
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
		<div class="col-md-8 column">
    		<div class="panel panel-default">
				<div class="panel-heading">
					<h3 class="panel-title">
						Players
					</h3>
				</div>
				<div class="panel-body">
                    <div class="col-md-6 column">
                    <table class="table table-condensed"  id="plsu">
				        <thead>
					        <tr>
						        <th>
							        Nickname
						        </th>
						        <th>
							        Ping
						        </th>
					        </tr>
				        </thead>
				        <tbody>
                            <?php foreach( $serverplayers as $player): ?>
					            <?php if( $player->Country == 'Russia'):?>
                                    <tr class="sovietbg">
						                <td>
							                <?=$player->Nickname?>
						                </td>
						                <td>
							                <?=$player->Ping?>
						                </td>
    					            </tr>
					            <?php elseif( $player->Country == 'Neutral'): ?>
                                    <tr>
						                <td>
							                <?=$player->Nickname?>
						                </td>
						                <td>
							                <?=$player->Ping?>
						                </td>
	    				            </tr>
                                <?php endif?>
                            <?php endforeach ?>

				        </tbody>
			        </table>

                    </div>
                    <div class="col-md-6 column">
                        <table class="table table-condensed"  id="plde">
				        <thead>
					        <tr>
						        <th>
							        Nickname
						        </th>
						        <th>
							        Ping
						        </th>
					        </tr>
				        </thead>
				        <tbody>
                            <?php foreach( $serverplayers as $player): ?>
                                <?php if($player->Country == "Germany"): ?>
                                    <tr class="axisbg">
						                <td>
							                <?=$player->Nickname?>
						                </td>
						                <td>
							                <?=$player->Ping?>
						                </td>
					                </tr>
                                <?php endif?>
                            <?php endforeach ?>

				        </tbody>
			        </table>
                    </div>
                    
				</div>
			</div>
						
		</div>

	</div>
