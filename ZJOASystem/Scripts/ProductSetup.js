$(document).ready(function () {
    $('#customWindow').jqxWindow({
        width: 420,
        height: 200,
        resizable: true,
        autoOpen: false,
        cancelButton: $('#closeButton'),
        position: { x: 280, y: 190 },
        initContent: function () {
            $('#closeButton').jqxButton({ width: '80px' });
            $('#jqxFileUpload').jqxFileUpload({ width: 300, uploadUrl: 'ImportProductAction?actiontype=2', fileInputName: 'fileToUpload', multipleFilesUpload: false });
        }
    });

    $("#importButton").jqxButton({ width: '200', height: '25' });
    $("#exportButton").jqxLinkButton({ width: '200', height: '25' });

    $("#importButton").on('click', function () {
        var isOpen = $('#customWindow').jqxWindow('isOpen');
        if (!isOpen) {
            $('#customWindow').jqxWindow('open');
        }
    });
    $('#jqxFileUpload').on('uploadEnd', function (event) {
        document.searchform.submit();
    });

    var newRowID = null;
    // prepare the data
    var source =
    {
        dataType: "json",
        dataFields: [
            { name: 'ProductNumber', type: 'string' },
            { name: 'ProductName', type: 'string' },
            { name: 'ParentNumber', type: 'string' },
            { name: 'ActionTime', type: 'date' },
            { name: 'ActionComments', type: 'string' },
            { name: 'OperatorsText', type: 'string' }
        ],
        hierarchy:
        {
            keyDataField: { name: 'ProductNumber' },
            parentDataField: { name: 'ParentNumber' }
        },
        id: 'ProductNumber',
        url: 'GetActionRecords?actiontype=2'
    };

    // prepare the data

    var dataAdapter = new $.jqx.dataAdapter(source);
    // create Tree Grid
    $("#treeGrid").jqxTreeGrid(
    {
        width: 800,
        source: dataAdapter,
        pageable: true,
        editable: false,
        showToolbar: false,
        pagerButtonsCount: 20,
        columnsResize: true,
        sortable: true,
        filterable: true,
        columns: [
            {
                text: '编号', datafield: 'ProductNumber', width: 100
            },
            {
                text: '名称', datafield: 'ProductName', width: 150
            },
            {
                text: '操作时间', datafield: 'ActionTime', width: 100, cellsFormat: 'd',
            },
            {
                text: '操作者', datafield: 'OperatorsText', width: 150
            },
            {
                text: '备注', datafield: 'ActionComments'
            }
        ]
    });
});