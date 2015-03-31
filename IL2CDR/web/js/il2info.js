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
    $('#table_wl').on( 'processing.dt', function ( e, settings, processing ) {
        $('#processingIndicator').css( 'display', processing ? 'block' : 'none' );
    } )
    .DataTable({
        serverSide: true,
        ajax: {
            url: '/json/wl/',
            type: 'GET'
        }
    });
    $('#table_wlpvp').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/wlpvp/',
            type: 'GET'
        }
    });
    $('#table_wlpve').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/wlpve/',
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
