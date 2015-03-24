$(".nav a").on("click", function () {
    $(".nav").find(".active").removeClass("active");
    $(this).parent().addClass("active");
});
$.extend($.fn.dataTable.defaults, {    
    ordering: false,
    pageLength: 100,
    pagingType: "full_numbers",
    dom: '<"toolbar">frtip',
    language: {
        searchPlaceholder: "Search player"
    }
});

$(document).ready(function () {
    $('#table_kd').DataTable({
    });

    $("div.toolbar").html('');
}); $(document).ready(function () {
    $('#table_snipers').DataTable({
    });

    $("div.toolbar").html('');
}); $(document).ready(function () {
    $('#table_survivors').DataTable({
    });

    $("div.toolbar").html('');
});