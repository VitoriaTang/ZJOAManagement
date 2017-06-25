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
            $('#jqxFileUpload').jqxFileUpload({ width: 300, uploadUrl: 'ImportMachines', fileInputName: 'fileToUpload', multipleFilesUpload: false });
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
            { name: 'Encode', type: 'string' },
            { name: 'Name', type: 'string' },
            { name: 'AssignTime', type: 'date' },
            { name: 'AssignComments', type: 'string' },
            { name: 'UsersNameText', type: 'string' }
        ],
        id: 'Encode',
        url: 'GetMachineRecords'
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
                text: '设备编号', datafield: 'Encode', width: 100
            },
            {
                text: '名称', datafield: 'Name', width: 150
            },
            {
                text: '分配时间', datafield: 'AssignTime', width: 100, cellsFormat: 'd',
            },
            {
                text: '使用者', datafield: 'UsersNameText', width: 150
            },
            {
                text: '备注', datafield: 'AssignComments'
            }
        ]
    });
});