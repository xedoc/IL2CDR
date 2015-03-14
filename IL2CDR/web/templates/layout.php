<html>
<head>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><?=$this->e($title)?></title>
    <link rel="stylesheet" href="/css/bootstrap.min.css" type="text/css" />
</head>
<body>
<nav class="navbar navbar-default">
  <div class="container-fluid">
    <div class="navbar-header">
      <a class="navbar-brand" href="/">IL-2 Leaderboards</a>
    </div>

    <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
      <ul class="nav navbar-nav">
        <li <?=activeIfMatch("kd")?>><a href="/kd">Kills/Deaths</a></li>
        <li <?=activeIfMatch("snipers")?>><a href="/snipers">Snipers</a></li>
        <li <?=activeIfMatch("survivors")?>><a href="/survivors">Survivors</a></li>
      </ul>
      <form class="navbar-form navbar-right" role="search">
        <div class="form-group">
          <input type="text" class="form-control" placeholder="Search player">
        </div>
        <button type="submit" class="btn btn-default">Search</button>
      </form>
    </div>
  </div>
</nav>

<?=$this->section('content')?>



<div class="navbar-fixed-bottom">
    <footer class="navbar-fixed-bottom text-muted small">
        &copy; 2015.<br />
        This site is not affiliated with 1C Game Studios nor with 777 Studios.<br />
        Official site of the game is: <a href="http://www.il2sturmovik.com">www.il2sturmovik.com</a>
    </footer>

</div>

</body>

<script src="/js/jquery.min.js"></script>
<script src="/js/bootstrap.min.js"></script>
<script src="/js/il2info.js"></script>

</html>