$(document).ready(function () {

    //data sources
    var source =
    {
        dataType: "json",
        dataFields: [
            { name: 'Number', type: 'string' },
            { name: 'Name', type: 'string' }
        ],
        id: 'Number',
        url: 'Product/GetProductBases'
    };

    var source_employees =
            {
                dataType: "json",
                dataFields: [
                    { name: 'Encode', type: 'string' },
                    { name: 'Name', type: 'string' }
                ],
                id: 'Encode',
                url: 'Product/GetEmployeeList'
            };

    var sourcelist =
    {
        dataType: "json",
        dataFields: [
            { name: 'Number', type: 'string' },
            { name: 'Name', type: 'string' },
            { name: 'Description', type: 'string' },
            { name: 'StatusText', type: 'string' },
            { name: 'ProductGuid', type: 'string' },
            { name: 'ParentGuid', type: 'string' }
        ],
        hierarchy:
            {
                keyDataField: { name: 'ProductGuid' },
                parentDataField: { name: 'ParentGuid' }
            },
        id: 'ProductGuid',
        url: 'Product/GetProducts'
    };


    var toplist_setup =
    {
        dataType: "json",
        dataFields: [
            { name: 'Number', type: 'string' },
            { name: 'Name', type: 'string' },
            { name: 'Description', type: 'string' },
            { name: 'Status', type: 'string' },
            { name: 'ProductGuid', type: 'string' }
        ],
        id: 'ProductGuid',
        url: 'Product/GetTopProducts?status=0'
    };

    var toplist_test =
    {
        dataType: "json",
        dataFields: [
            { name: 'Number', type: 'string' },
            { name: 'Name', type: 'string' },
            { name: 'Description', type: 'string' },
            { name: 'Status', type: 'string' },
            { name: 'ProductGuid', type: 'string' }
        ],
        id: 'ProductGuid',
        url: 'Product/GetTopProducts?status=1,4'
    };

    var toplist_fix =
    {
        dataType: "json",
        dataFields: [
            { name: 'Number', type: 'string' },
            { name: 'Name', type: 'string' },
            { name: 'Description', type: 'string' },
            { name: 'Status', type: 'string' },
            { name: 'ProductGuid', type: 'string' }
        ],
        id: 'ProductGuid',
        url: 'Product/GetTopProducts?status=2'
    };

    var toplist_package =
    {
        dataType: "json",
        dataFields: [
            { name: 'Number', type: 'string' },
            { name: 'Name', type: 'string' },
            { name: 'Description', type: 'string' },
            { name: 'Status', type: 'string' },
            { name: 'ProductGuid', type: 'string' }
        ],
        id: 'ProductGuid',
        url: 'Product/GetTopProducts?status=3'
    };

    var toplist_deliever =
    {
        dataType: "json",
        dataFields: [
            { name: 'Number', type: 'string' },
            { name: 'Name', type: 'string' },
            { name: 'Description', type: 'string' },
            { name: 'Status', type: 'string' },
            { name: 'ProductGuid', type: 'string' }
        ],
        id: 'ProductGuid',
        url: 'Product/GetTopProducts?status=5'
    };

    var dataAdapterlist = new $.jqx.dataAdapter(sourcelist);
    var dataAdapterToplist_setup = new $.jqx.dataAdapter(toplist_setup);
    var dataAdapter = new $.jqx.dataAdapter(source);
    var dataAdapter_employee = new $.jqx.dataAdapter(source_employees);
    var dataAdapterToplist_test = new $.jqx.dataAdapter(toplist_test);
    var dataAdapterToplist_fix = new $.jqx.dataAdapter(toplist_fix);
    var dataAdapterToplist_package = new $.jqx.dataAdapter(toplist_package);
    var dataAdapterToplist_deliever = new $.jqx.dataAdapter(toplist_deliever);


    // Create window
    $('#customWindow_create').jqxWindow({
        width: 420,
        height: 320,
        resizable: true,
        autoOpen: false,
        okButton: $('#okButton'),
        cancelButton: $('#closeButton'),
        position: { x: 280, y: 190 },
        initContent: function () {
            $('#closeButton').jqxButton({ width: '80px' });
            $('#okButton').jqxButton({ width: '80px' });
            $("#dateInput_year").jqxDateTimeInput({ width: '200px', height: '25px' });

            $("#jqxComboBox_productbase").jqxComboBox({ displayMember: "Name", valueMember: "Number", source: dataAdapter, width: '200px', height: '25px' });

            // Create a jqxListBox
            $("#listBoxA").jqxListBox({
                source: dataAdapter_employee, width: 200, height: 90, displayMember: "Name", valueMember: "Encode", multiple: true, itemHeight: 18
            });

        }
    });

    //Setup window
    $('#customWindow_setup').jqxWindow({
        width: 420,
        height: 430,
        resizable: true,
        autoOpen: false,
        okButton: $('#okButton_setup'),
        cancelButton: $('#closeButton_setup'),
        position: { x: 280, y: 190 },
        initContent: function () {
            $('#closeButton_setup').jqxButton({ width: '80px' });
            $('#okButton_setup').jqxButton({ width: '80px' });
            $('#moveLeftButton').jqxButton({ width: '80px' });
            $('#moveRightButton').jqxButton({ width: '80px' });

            $("#jqxComboBox_products_setup").jqxComboBox(
                {
                    displayMember: "Name", valueMember: "ProductGuid",
                    source: dataAdapterToplist_setup, width: '150px', height: '25px'
                });

            $('#moveLeftButton').on('click', function (event) {
                var selectedIndex = $('#listBoxSetupLeft').jqxListBox('selectedIndex');
                var item = $("#listBoxSetupLeft").jqxListBox('getItem', selectedIndex);

                $("#listBoxSetupRight").jqxListBox('addItem', { label: item.label, value: item.value });
                $("#listBoxSetupLeft").jqxListBox('removeAt', selectedIndex);
            });

            $('#moveRightButton').on('click', function (event) {
                var selectedIndex = $('#listBoxSetupRight').jqxListBox('selectedIndex');
                var item = $("#listBoxSetupRight").jqxListBox('getItem', selectedIndex);

                $("#listBoxSetupLeft").jqxListBox('addItem', { label: item.label, value: item.value });
                $("#listBoxSetupRight").jqxListBox('removeAt', selectedIndex);
            });

            // Create a jqxListBox
            $("#listBoxA_setup").jqxListBox({
                source: dataAdapter_employee, width: 200, height: 90, displayMember: "Name", valueMember: "Encode", multiple: true, itemHeight: 18
            });

            $('#jqxComboBox_products_setup').on('select', function (event) {
                var args = event.args;
                if (args != undefined) {
                    var item = event.args.item;
                    if (item != null) {
                        
                        var selectIndex = $("#jqxComboBox_products_setup").jqxComboBox('getSelectedIndex');

                        if (selectIndex == null) {
                            return;
                        }
                        var sourceItem = $("#jqxComboBox_products_setup").jqxComboBox('source').records[selectIndex];
                        var baseNumber = sourceItem.Number;

                        var currentlist =
                        {
                            dataType: "json",
                            dataFields: [
                             { name: 'Number', type: 'string' },
                             { name: 'Name', type: 'string' },
                             { name: 'Description', type: 'string' },
                             { name: 'Status', type: 'string' },
                             { name: 'ProductGuid', type: 'string' }
                            ],
                            id: 'ProductGuid',
                            url: 'Product/GetProductsByBaseNumber?baseid=' + baseNumber + "&baseguid=" + item.value
                        };

                        var dataAdaptercurrentlist = new $.jqx.dataAdapter(currentlist);
                        dataAdaptercurrentlist.dataBind();

                        $("#listBoxSetupLeft").jqxListBox({
                            source: dataAdaptercurrentlist, width: 150, height: 140, displayMember: "Name", valueMember: "ProductGuid", multiple: true, itemHeight: 18
                        });

                        $("#listBoxSetupRight").jqxListBox({
                            source: [], width: 150, height: 140, displayMember: "Name", valueMember: "ProductGuid", multiple: true, itemHeight: 18
                        });
                    }
                }
            });

            // Create a jqxListBox
            $("#listBoxSetupLeft").jqxListBox({
                source: null, width: 150, height: 140, displayMember: "Name", valueMember: "ProductGuid", multiple: true, itemHeight: 18
            });


            $("#listBoxSetupRight").jqxTreeGrid(
            {
                width: 150,
                height: 140,
                source: dataAdapterlist,
                pageable: true,
                sortable: true,
                altRows: true,
                editable: false,
                selectionMode: 'singleRow',
                columns: [
                { text: '@ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_ENCODE")', datafield: 'Number', width: 250 },
                { text: '@ZJOASystem.Controllers.ResourceReader.GetString("PRODUCT_NAME")', datafield: 'Name', width: 200 }
                ]
            });



            $("#listBoxSetupRight").jqxListBox({
                source: [], width: 150, height: 200, displayMember: "Name", valueMember: "ProductGuid", multiple: true, itemHeight: 18
            });
        }
    });

    $('#customWindow_test').jqxWindow({
        width: 420,
        height: 320,
        resizable: true,
        autoOpen: false,
        okButton: $('#okButton_test'),
        cancelButton: $('#closeButton_test'),
        position: { x: 280, y: 190 },
        initContent: function () {
            $('#closeButton_test').jqxButton({ width: '80px' });
            $('#okButton_test').jqxButton({ width: '80px' });

            $("#jqxComboBox_products_test").jqxComboBox(
                {
                    displayMember: "Name", valueMember: "ProductGuid",
                    source: dataAdapterToplist_test, width: '150px', height: '25px'
                });
            // Create a jqxListBox
            $("#listBoxA_test").jqxListBox({
                source: dataAdapter_employee, width: 200, height: 90, displayMember: "Name", valueMember: "Encode", multiple: true, itemHeight: 18
            });

            $("#jqxRadioButton_qualified").jqxRadioButton({ width: 250, height: 25, checked: true });
            $("#jqxRadioButton_unqualified").jqxRadioButton({ width: 250, height: 25 });
        }
    });

    $('#customWindow_fix').jqxWindow({
        width: 420,
        height: 290,
        resizable: true,
        autoOpen: false,
        okButton: $('#okButton_fix'),
        cancelButton: $('#closeButton_fix'),
        position: { x: 280, y: 190 },
        initContent: function () {
            $('#closeButton_fix').jqxButton({ width: '80px' });
            $('#okButton_fix').jqxButton({ width: '80px' });

            $("#jqxComboBox_products_fix").jqxComboBox(
                {
                    displayMember: "Name", valueMember: "ProductGuid",
                    source: dataAdapterToplist_fix, width: '150px', height: '25px'
                });
            // Create a jqxListBox
            $("#listBoxA_fix").jqxListBox({
                source: dataAdapter_employee, width: 200, height: 90, displayMember: "Name", valueMember: "Encode", multiple: true, itemHeight: 18
            });

        }
    });

    $('#customWindow_package').jqxWindow({
        width: 420,
        height: 290,
        resizable: true,
        autoOpen: false,
        okButton: $('#okButton_package'),
        cancelButton: $('#closeButton_package'),
        position: { x: 280, y: 190 },
        initContent: function () {
            $('#closeButton_package').jqxButton({ width: '80px' });
            $('#okButton_package').jqxButton({ width: '80px' });

            $("#jqxComboBox_products_package").jqxComboBox(
                {
                    displayMember: "Name", valueMember: "ProductGuid",
                    source: dataAdapterToplist_package, width: '150px', height: '25px'
                });
            // Create a jqxListBox
            $("#listBoxA_package").jqxListBox({
                source: dataAdapter_employee, width: 200, height: 90, displayMember: "Name", valueMember: "Encode", multiple: true, itemHeight: 18
            });

        }
    });

    $('#customWindow_deliever').jqxWindow({
        width: 420,
        height: 450,
        resizable: true,
        autoOpen: false,
        okButton: $('#okButton_deliever'),
        cancelButton: $('#closeButton_deliever'),
        position: { x: 280, y: 190 },
        initContent: function () {
            $('#closeButton_deliever').jqxButton({ width: '80px' });
            $('#okButton_deliever').jqxButton({ width: '80px' });

            $("#jqxComboBox_products_deliever").jqxComboBox(
                {
                    displayMember: "Name", valueMember: "ProductGuid",
                    source: dataAdapterToplist_deliever, width: '150px', height: '25px'
                });
            // Create a jqxListBox
            $("#listBoxA_deliever").jqxListBox({
                source: dataAdapter_employee, width: 200, height: 90, displayMember: "Name", valueMember: "Encode", multiple: true, itemHeight: 18
            });

        }
    });

    // buttons
    $("#createButton").jqxLinkButton();
    $("#setupButton").jqxLinkButton();
    $("#testButton").jqxLinkButton();
    $("#fixButton").jqxLinkButton();
    $("#packageButton").jqxLinkButton();
    $("#delieverButton").jqxLinkButton();
    $("#printButton").jqxLinkButton();

    // create button click
    $("#createButton").on('click', function () {
        var isOpen = $('#customWindow_create').jqxWindow('isOpen');
        if (!isOpen) {
            $("#jqxComboBox_productbase").jqxComboBox('selectIndex', -1);
            $('#customWindow_create').jqxWindow('open');
        }
    });

    // setup button click
    $("#setupButton").on('click', function () {
        var isOpen = $('#customWindow_setup').jqxWindow('isOpen');
        if (!isOpen) {
            $("#jqxComboBox_products_setup").jqxComboBox('selectIndex', -1);
            $('#customWindow_setup').jqxWindow('open');
        }
    });

    // setup button click
    $("#testButton").on('click', function () {
        var isOpen = $('#customWindow_test').jqxWindow('isOpen');
        if (!isOpen) {
            $("#jqxComboBox_products_test").jqxComboBox('selectIndex', -1);
            $('#customWindow_test').jqxWindow('open');
        }
    });

    // fix button click
    $("#fixButton").on('click', function () {
        var isOpen = $('#customWindow_fix').jqxWindow('isOpen');
        if (!isOpen) {
            $("#jqxComboBox_products_fix").jqxComboBox('selectIndex', -1);
            $('#customWindow_fix').jqxWindow('open');
        }
    });

    // package button click
    $("#packageButton").on('click', function () {
        var isOpen = $('#customWindow_package').jqxWindow('isOpen');
        if (!isOpen) {
            $("#jqxComboBox_products_package").jqxComboBox('selectIndex', -1);
            $('#customWindow_package').jqxWindow('open');
        }
    });

    // package button click
    $("#delieverButton").on('click', function () {
        var isOpen = $('#customWindow_deliever').jqxWindow('isOpen');
        if (!isOpen) {
            $("#jqxComboBox_products_deliever").jqxComboBox('selectIndex', -1);
            $('#customWindow_deliever').jqxWindow('open');
        }
    });

    $("#okButton").on('click', function () {
        saveCreate();
    });

    $("#okButton_setup").on('click', function () {
        saveSetup();
    });

    $("#okButton_test").on('click', function () {
        saveTest();
    });

    $("#okButton_fix").on('click', function () {
        saveFix();
    });

    $("#okButton_package").on('click', function () {
        savePackage();
    });

    $("#okButton_deliever").on('click', function () {
        saveDeliever();
    });

    // grid
    $("#jqxgrid").jqxTreeGrid(
        {
            width: 800,
            source: dataAdapterlist,
            pageable: true,
            sortable: true,
            altRows: true,
            editable: false,
            selectionMode: 'singleRow',
            columns: [
                { text: '编码', datafield: 'Number', width: 250 },
                { text: '产品名称', datafield: 'Name', width: 200 },
                { text: '描述', datafield: 'Description' },
                { text: '状态', datafield: 'StatusText' }
            ]
        });



});

function saveSetup() {
    var items = $("#listBoxSetupRight").jqxListBox('getItems');

    var allItemNumber = "";
    for (var i = 0; i < items.length; i++) {
        allItemNumber += items[i].value;
        if (i < items.length - 1) {
            allItemNumber += ', ';
        }
    }

    var mainitem = $("#jqxComboBox_products_setup").jqxComboBox('getSelectedItem');

    if (mainitem == null) {
        return;
    }

    var optItems = $("#listBoxA_setup").jqxListBox('getSelectedItems');
    var selection = "";
    for (var i = 0; i < optItems.length; i++) {
        selection += optItems[i].value + (i < optItems.length - 1 ? ", " : "");
    }

    var value = "{'ProductGuid':'" + mainitem.value + "',";
    value += "'ChildItems':'" + allItemNumber + "',";
    value += "'ActionEmployees':'" + selection + "'}";
    document.getElementById("product_setup").value = value;

    document.searchform.submit();

}
function saveCreate() {
    var items = $("#listBoxA").jqxListBox('getSelectedItems');
    var selection = "";
    for (var i = 0; i < items.length; i++) {
        selection += items[i].value + (i < items.length - 1 ? ", " : "");
    }

    var proItem = $("#jqxComboBox_productbase").jqxComboBox('getSelectedItem');
    if (proItem == null) {
        alert("请选择产品名类");
    }
    var yearItem = $("#dateInput_year").jqxDateTimeInput('getDate');
    var year = yearItem.getFullYear().toString();
    var month = yearItem.getMonth().toString();
    var day = yearItem.getDay().toString();
    if (month.length == 1) {
        month = "0" + month;
    }
    if (day.length == 1) {
        day = "0" + day;
    }
    var yearValue = year + month + day;
    var batchValue = document.getElementById("input_batch").value;
    var serValue = document.getElementById("input_serial").value;
    var numberValue = proItem.value + yearValue + batchValue + serValue;

    var value = "{'Number':'" + numberValue + "',";
    value += "'Name':'" + document.getElementById("input_name").value + "',";
    value += "'Description':'" + document.getElementById("input_description").value + "',";
    value += "'ActionType':1,";
    value += "'ActionEmployees':'" + selection + "'}";

    document.getElementById("product_create").value = value;

    document.searchform.submit();
}

function saveTest() {
    var items = $("#listBoxA_test").jqxListBox('getSelectedItems');
    var selection = "";
    for (var i = 0; i < items.length; i++) {
        selection += items[i].value + (i < items.length - 1 ? ", " : "");
    }

    var selectIndex = $("#jqxComboBox_products_test").jqxComboBox('getSelectedIndex');

    if (selectIndex == null || selectIndex==-1) {
        return;
    }
    var sourceItem = $("#jqxComboBox_products_test").jqxComboBox('source').records[selectIndex];

    var qualifiedChecked = $("#jqxRadioButton_qualified").jqxRadioButton('checked');
    var status = 3;
    if (!qualifiedChecked) {
        status = 2;
    }

    var value = "{'Number':'" + sourceItem.Number + "',";
    value += "'ProductGuid':'" + sourceItem.ProductGuid + "',";
    value += "'Status':" + status + ",";
    value += "'ActionComments':'" + document.getElementById("test_comments").value + "',";
    value += "'ActionType':3,";
    value += "'ActionEmployees':'" + selection + "'}";

    document.getElementById("product_test").value = value;

    document.searchform.submit();
}

function saveFix() {
    var items = $("#listBoxA_fix").jqxListBox('getSelectedItems');
    var selection = "";
    for (var i = 0; i < items.length; i++) {
        selection += items[i].value + (i < items.length - 1 ? ", " : "");
    }

    var selectIndex = $("#jqxComboBox_products_fix").jqxComboBox('getSelectedIndex');

    if (selectIndex == null || selectIndex == -1) {
        return;
    }
    var sourceItem = $("#jqxComboBox_products_fix").jqxComboBox('source').records[selectIndex];

    var status = 4;
    
    var value = "{'Number':'" + sourceItem.Number + "',";
    value += "'ProductGuid':'" + sourceItem.ProductGuid + "',";
    value += "'Status':4,";
    value += "'ActionComments':'" + document.getElementById("fix_comments").value + "',";
    value += "'ActionType':4,";
    value += "'ActionEmployees':'" + selection + "'}";

    document.getElementById("product_fix").value = value;

    document.searchform.submit();
}

function savePackage() {
    var items = $("#listBoxA_package").jqxListBox('getSelectedItems');
    var selection = "";
    for (var i = 0; i < items.length; i++) {
        selection += items[i].value + (i < items.length - 1 ? ", " : "");
    }

    var selectIndex = $("#jqxComboBox_products_package").jqxComboBox('getSelectedIndex');

    if (selectIndex == null || selectIndex == -1) {
        return;
    }
    var sourceItem = $("#jqxComboBox_products_package").jqxComboBox('source').records[selectIndex];

    var status = 4;

    var value = "{'Number':'" + sourceItem.Number + "',";
    value += "'ProductGuid':'" + sourceItem.ProductGuid + "',";
    value += "'Status':5,";
    value += "'ActionComments':'" + document.getElementById("package_comments").value + "',";
    value += "'ActionType':5,";
    value += "'ActionEmployees':'" + selection + "'}";

    document.getElementById("product_package").value = value;

    document.searchform.submit();
}

// save deliever
function saveDeliever() {
    var items = $("#listBoxA_deliever").jqxListBox('getSelectedItems');
    var selection = "";
    for (var i = 0; i < items.length; i++) {
        selection += items[i].value + (i < items.length - 1 ? ", " : "");
    }

    var selectIndex = $("#jqxComboBox_products_deliever").jqxComboBox('getSelectedIndex');

    if (selectIndex == null || selectIndex == -1) {
        return;
    }
    var sourceItem = $("#jqxComboBox_products_deliever").jqxComboBox('source').records[selectIndex];

    var status = 4;

    var value = "{'Number':'" + sourceItem.Number + "',";
    value += "'ProductGuid':'" + sourceItem.ProductGuid + "',";
    value += "'Status':6,";
    value += "'ActionComments':'" + document.getElementById("package_comments").value + "',";
    value += "'ActionType':6,";
    value += "'ActionEmployees':'" + selection + "',";
    value += "'TrackNumber':'" + document.getElementById("input_tracknumber").value + "',";
    value += "'Sender':'" + document.getElementById("input_sender").value + "',";
    value += "'SenderTelephone':'" + document.getElementById("input_sendertelephone").value + "',";
    value += "'Departure':'" + document.getElementById("input_departure").value + "',";
    value += "'Receiver':'" + document.getElementById("input_receiver").value + "',";
    value += "'ReceiverTelephone':'" + document.getElementById("input_receivertelephone").value + "',";
    value += "'Destination':'" + document.getElementById("input_destination").value + "'}";

    document.getElementById("product_deliever").value = value;

    document.searchform.submit();
}