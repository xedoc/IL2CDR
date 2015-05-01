<!doctype html>
<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><?=$this->e($title)?></title>
    <style>
        @import "/css.php";
    </style>
    <!--<link rel="stylesheet" href="css.php" type="text/css" />-->
<!--    <link rel="stylesheet" href="/css/bootstrap.css" type="text/css" />
    <link rel="stylesheet" type="text/css" href="//cdn.datatables.net/plug-ins/f2c75b7247b/integration/bootstrap/3/dataTables.bootstrap.css"/>
    <link rel="stylesheet" href="/css/stats.css" type="text/css" />
    <link rel="stylesheet" href="//cdn.datatables.net/scroller/1.2.2/css/dataTables.scroller.css" type="text/css" />
    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/bootstrap-select/1.6.4/css/bootstrap-select.min.css" />-->

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

         <li <?=activeIfMatch("monitor")?>><a href="/monitor">Monitor</a></li>
         <li <?=activeIfStartsWith("wl")?> class="dropdown">           
            <a href="#" data-toggle="dropdown" class="dropdown-toggle">Wins/Losses<b class="caret"></b></a>
            <ul class="dropdown-menu">
                <li><a href="/wlpvp">Players vs Players</a></li>
                <li><a href="/wlpve">Players vs Environment</a></li>
                <li class="divider"></li>
                <li><a href="/wl">Total</a></li>
            </ul>
        </li>        
        <li <?=activeIfMatch("missions")?>><a href="/missions">Missions</a></li>
        <li><a data-toggle="modal" data-target="#filterModal" href="#">Filter</a></li>


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
    <div class="container text-center text-muted small">
        Time zone:&nbsp<?=$this->e($tz)?>
    </div>

</footer>

<?php $this->insert('partial_usermodal') ?>
<?php $this->insert('partial_loginmodal') ?>
<?php $this->insert('partial_signupmodal') ?>
<?php $this->insert('partial_filtermodal') ?>

</body>

<script type="text/javascript" src="/scripts.php?build=7&amp;load=js/modernizr.min,js/jquery.min,js/jquery.cookie.min,js/jquery.dataTables.min,js/dataTables.bootstrap,js/bootstrap.min,js/dataTables.scroller.min,js/jstz.min,js/bootstrap-select.min,js/jquery.json2html,js/json2html,js/il2info"> </script>
</html>