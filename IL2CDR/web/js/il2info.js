$(".nav a").on("click", function () {
    $(".nav").find(".active").removeClass("active");
    $(this).parent().addClass("active");
});
$.extend($.fn.dataTable.defaults, {    
    ordering: false,
    pageLength: 10,
    pagingType: "full_numbers",
    dom: '<"toolbar">frtip',
    language: {
        processing: "Loading data...",
        searchPlaceholder: "Search player"
    }
});

$(document).ready(function () {
    $('#table_kd').on( 'processing.dt', function ( e, settings, processing ) {
        $('#processingIndicator').css( 'display', processing ? 'block' : 'none' );
    } )
    .DataTable({
        serverSide: true,
        ajax: {
            url: '/json/kd/',
            type: 'GET'
        }
    });
    $('#table_kdpvp').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/kdpvp/',
            type: 'GET'
        }
    });
    $('#table_kdpve').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/kdpve/',
            type: 'GET'
        }
    });

    $('#table_snipers').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/snipers/',
            type: 'GET'
        }
    });
    $('#table_survivors').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
    });

    $("div.toolbar").html('');
});
