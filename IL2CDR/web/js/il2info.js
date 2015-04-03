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
        },
        deferLoading: playersCount,

    });
    $('#table_wlpvp').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/wlpvp/',
            type: 'GET'
        },
        deferLoading: playersCount,
    });
    $('#table_wlpve').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/wlpve/',
            type: 'GET'
        },
        deferLoading: playersCount,
    });

    $('#table_snipers').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
        serverSide: true,
        ajax: {
            url: '/json/snipers/',
            type: 'GET'
        },
        deferLoading: playersCount,
    });
    $('#table_survivors').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css('display', processing ? 'block' : 'none');
    }).DataTable({
    });

    var missionTable = $('#table_missions').DataTable({
        serverSide: true,
        ajax: {
            url: '/json/missions/',
            type: 'GET'
        },
        language: {
            processing: "Loading data...",
            searchPlaceholder: "Server name, start/end time"
        },
        dom: "rftS",
        paging: true,
        scrollY: 400,
        scroller: {
        loadingIndicator: true,
        deferLoading: missionCount,
    }
    });

    var tz = jstz.determine();        
        
    if (typeof (tz) === 'undefined') {
        timeZone = '';
    }
    else {
        timeZone = tz.name(); 
    }

    $.cookie("tz", timeZone);

    $("div.toolbar").html('');
});



