/* Mini DataTables Plugin.
 * Adds a search to the column headers.
 * Parts of this was taken from _fnFeatureHtmlFilter (jquery.dataTables.js) and can be subject to change.
 * As of initial creation, has not been tested with local data (not serverSide) since no pages operate that way as of yet.
 */
$(document).on('preInit.dt', function (e, settings) {
    if (!settings.oInit.searchHeaders) { //jump out if not enabled
        return;
    }

    var api = $.fn.dataTable.Api(settings);
    var thead = $(api.table().header());
    var tr = $('<tr/>');
    thead.append(tr);

    var serverSide = settings.oInit.serverSide;

    var searchDelay = settings.searchDelay !== null ?
        settings.searchDelay :
        serverSide ? 400 : 0;

    for (var i = 0; i < settings.aoColumns.length; i++) {
        var column = settings.aoColumns[i];
        var th = $('<th/>');
        tr.append(th);

        /* Continue loop if this column is specifically set to not have a search header (searchHeader) or
         *      if the column is not searchable (bSearchable)
         * NOTE: bSearchable is considered private and may change in future releases of DataTables
         */
        if (typeof column.searchHeader !== 'undefined' && !column.searchHeader
            || !column.bSearchable) {
            continue;
        }

        var input = $('<input/>', {
            'class': 'form-control form-control-sm',
            type: 'text'
        });
        th.append(input);

        (function (column, input) {
            input.on("input", $.fn.dataTable.util.throttle(function () {
                api.column(column.idx).search(input.val());
                api.draw();
            }, searchDelay));
        }(column, input));

    }
});