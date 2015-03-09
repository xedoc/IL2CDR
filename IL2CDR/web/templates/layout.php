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
        <li class="active"><a href="#">Kills/Deaths</a></li>
        <li><a href="#">Snipers</a></li>
        <li><a href="#">Survivors</a></li>
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

</body>

<script src="/js/jquery.min.js"></script>
<script src="/js/bootstrap.min.js"></script>
<script src="/js/il2info.js"></script>

</html>