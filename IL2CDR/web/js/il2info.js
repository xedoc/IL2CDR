var currentUrl = '';

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

    currentUrl = window.location.href;

    $('.serveritem').on('click', function (e) {
        var serverId = $(this).data('id');
        $('#plde > tbody ').empty();
        $('#plsu > tbody ').html('<tr><td colspan="3">Loading...</td></tr>');
        $.ajax({
            url: "/json/playerlist/" + serverId,
            cache: false,
            success: function (result) {
                json = JSON.parse(result);

                arde = [];
                arsu = [];
                for (i = 0; i < json.length ; i++)
                {
                    if (json[i].Country == 'Germany')
                        arde.push(json[i]);
                    else
                        arsu.push(json[i]);
                }

                var transform = {
                    tag: "tr",
                    "class": function () { return this.Country == 'Germany' ? 'axisbg' : this.Country == 'Russia' ? 'sovietbg' : ''; },
                    children: [
                    { "tag": "td", "html": '${Nickname}' },
                    { "tag": "td", "html": '${Ping}' },
                    ]
                };

                $('#plde > tbody ').empty();
                $('#plde > tbody ').json2html(arde, transform);
                $('#plsu > tbody ').empty();
                $('#plsu > tbody ').json2html(arsu, transform);
            },
            timeout: 30000
        });

    });
    //$('#filter').on('submit', function (e) {
    //    e.preventDefault();  
    //    var data = $("#filter").find(":selected").val();
    //    console.log(data); 
    //});


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
            { className: "tdcenter", "targets": [ 0,1,2,3 ] }
        ],
        serverSide: true,
        pageLength: 10,
        ajax: {
            url: '/json/missions/',
            type: 'GET'
        },
        language: {
            searchPlaceholder: "Server name, start/end time"
        },
        deferLoading: missionCount,
        dom: '<"toolbar">rtip',
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





