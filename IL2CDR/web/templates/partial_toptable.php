    <?php foreach( $table->data as $row): ?>
    <tr>
        <?php $ar = (array)$row; ?>
        <?php for ($i = 0; $i < count( $ar ) - 1; $i++): ?>
        <td><?=$row->{strval($i)}?></td>
        <?php endfor ?>
    </tr>
    <?php endforeach ?>
