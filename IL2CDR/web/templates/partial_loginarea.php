<?php if ($this->e($isloggedin)): ?>
<ul class="nav navbar-nav navbar-right">
    <li><a data-toggle="modal" data-target="#userModal" href="#"><?=$this->e($currentuser)?></a></li>
    <li><a href="/servers/">Servers</a></li>
    <li><a href="/logout/">Log Out</a></li>
</ul>
<?php else:?>
<ul class="nav navbar-nav navbar-right">
    <li><a data-toggle="modal" data-target="#loginModal" href="#">Log In</a></li>      
    <li><a data-toggle="modal" data-target="#signupModal" href="#">Sign Up</a></li>
</ul>
<?php endif; ?>
<!--
-->


