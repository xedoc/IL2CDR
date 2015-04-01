<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="cache-control" content="max-age=0" />
    <meta http-equiv="cache-control" content="no-cache" />
    <meta http-equiv="expires" content="0" />
    <meta http-equiv="expires" content="Tue, 01 Jan 1980 1:00:00 GMT" />
    <meta http-equiv="pragma" content="no-cache" />

    <title><?=$this->e($title)?></title>
    <link rel="stylesheet" href="/css/bootstrap.css" type="text/css" />
    <link rel="stylesheet" type="text/css" href="//cdn.datatables.net/plug-ins/f2c75b7247b/integration/bootstrap/3/dataTables.bootstrap.css">
    <link rel="stylesheet" href="/css/stats.css" type="text/css" />
    <?php $this->insert('partial_jsconfig') ?>


</head>
<body>
<nav class="navbar navbar-default">
  <div class="container-fluid">
    <div class="navbar-header">
        <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#navcollapse">
            <span class="sr-only">Toggle navigation</span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
        </button>
      <a class="navbar-brand" href="/">IL-2 Leaderboards</a>
    </div>

    <div class="collapse navbar-collapse" id="navcollapse">
        <?php $this->insert('partial_loginarea') ?>

      <ul class="nav navbar-nav">
        <li <?=activeIfStartsWith("wl")?> class="dropdown">
                        <a href="#" data-toggle="dropdown" class="dropdown-toggle">Wins/Losses<b class="caret"></b></a>
                        <ul class="dropdown-menu">
                            <li><a href="/wlpvp">Players vs Players</a></li>
                            <li><a href="/wlpve">Players vs Environment</a></li>
                            <li class="divider"></li>
                            <li><a href="/wl">Total</a></li>
                        </ul>
                    </li>
        <li <?=activeIfMatch("snipers")?>><a href="/snipers">Snipers</a></li>
        <li <?=activeIfMatch("survivors")?>><a href="/survivors">Survivors</a></li>

      </ul>

    </div>
  </div>
</nav>


<div class="container">
    <?=$this->section('content')?>
</div>



<footer class="top-buffer text-muted small">
    <div class="container text-center">
        <p class="navbar-text col-md-12 col-sm-12 col-xs-12">    &copy; 2015. This site is not affiliated with 1C Game Studios nor with 777 Studios. Official site of the game is: <a href="http://www.il2sturmovik.com">www.il2sturmovik.com</a></p>
    </div>
</footer>

<?php $this->insert('partial_usermodal') ?>
<?php $this->insert('partial_loginmodal') ?>
<?php $this->insert('partial_signupmodal') ?>

</body>

<script src="/js/jquery.min.js"></script>    
<script type="text/javascript" charset="utf8" src="//cdn.datatables.net/1.10.5/js/jquery.dataTables.min.js"></script>
<script type="text/javascript" charset="utf8" src="//cdn.datatables.net/plug-ins/f2c75b7247b/integration/bootstrap/3/dataTables.bootstrap.js"></script>
<script src="/js/bootstrap.min.js"></script>
<script src="/js/il2info.js"></script>

</html>