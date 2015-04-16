$(".nav a").on("click", function () {
    $(".nav").find(".active").removeClass("active");
    $(this).parent().addClass("active");
});
$.extend($.fn.dataTable.defaults, {    
    ordering: false,
    pagingType: "full_numbers",
    language: {
        processing: "Loading data...",
        searchPlaceholder: "Search player"
    }
});
var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

$(document).ready(function () {
    $('.serveritem').on('click', function (e) {
        var serverId = $(this).data('id');

        $.ajax({
            url: "/json/playerlist/" + serverId,
            cache: false,
            success: function (json) {
                if (!(json instanceof Array))
                    json = JSON.parse(json);

                $('#playerlist').empty();

                if (json instanceof Array) {
                    if (json.length > 0) 
                    {
                        var transform = {
                            "tag": "tr", "children": [
                            { "tag": "td", "html": "${Country}", },
                            { "tag": "td", "html": "${Nickname}",},
                            { "tag": "td", "html": "${Ping}" },
                            ]
                        };

                        $('#playerlist').json2html(json, transform );                
                    }
                }
            },
            timeout: 15000,
            //error: function () {
            //    setTimeout(function () {
            //        refreshCommands()
            //    }, interval);

        });

    });
    $('#filter').on('submit', function (e) {
        e.preventDefault();  
        var data = $("#filter").find(":selected").val();
        console.log(data); 
    });


    $('#table_wl').on('processing.dt', function (e, settings, processing) {
        $('#processingIndicator').css( 'display', processing ? 'block' : 'none' );
    }).DataTable({
        dom: '<"toolbar">rtip',
        pageLength: 10,
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
        dom: '<"toolbar">rtip',
        pageLength: 10,
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
        dom: '<"toolbar">rtip',
        pageLength: 10,
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
        dom: '<"toolbar">rtip',
        pageLength: 10,
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
        "columnDefs": [
            { className: "tdcenter", "targets": [ 1, 2 ] }
        ],
        serverSide: true,
        ajax: {
            url: '/json/missions/',
            type: 'GET'
        },
        language: {
            processing: "Loading data...",
            searchPlaceholder: "Server name, start/end time"
        },
        deferLoading: missionCount,
        dom: "rftS",
        deferRender: true,
        paging: true,
        scrollY: 400,
        scroller: {
            rowHeight : 74,
            loadingIndicator: true,
        }
    });

    var searchInput = $(".searchinput")[0];
    var tables = $.fn.dataTable.tables();
    $(searchInput).keyup(function () {
        delay(function () {
            $(tables[0]).DataTable().search($(searchInput).val()).draw();
        }, 500);
    })

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





