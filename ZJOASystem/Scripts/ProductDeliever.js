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
            $('#jqxFileUpload').jqxFileUpload({ width: 300, uploadUrl: 'ImportProductAction?actiontype=6', fileInputName: 'fileToUpload', multipleFilesUpload: false });
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
            { name: 'OperatorsText', type: 'string' },
            { name: 'AdditionalInfo', type: 'string' },
            { name: 'BoxNumber', type: 'string'}
        ],
        hierarchy:
        {
            keyDataField: { name: 'ProductNumber' },
            parentDataField: { name: 'ParentNumber' }
        },
        id: 'ProductNumber',
        url: 'GetActionRecords?actiontype=6'
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
        exportSettings: { fileName: null, collapsedRecords: false, recordsInView: true },
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
                text: '装箱编号', datafield: 'BoxNumber', width: 150
            },
            {
                text: '更多信息', datafield: 'AdditionalInfo'
            }
        ]
    });

    var printTemplateSource =
  {
      dataType: "json",
      dataFields: [
          { name: 'Name', type: 'string' },
          { name: 'Content', type: 'string' }
      ],
      id: 'Name',
      url: 'GetPrintTemplate'
  };

    var printTemplateDataAdapter = new $.jqx.dataAdapter(printTemplateSource);
    $("#jqxcombobox").jqxDropDownList({
        source: printTemplateDataAdapter,
        selectedIndex: 0,
        width: '200',
        height: '25',
        displayMember: "Name",
        valueMember: "Content"
    });

    $("#printButton").jqxButton({ width: '100', height: '25' });

    isIE = function () { //ie?
        if (!!window.ActiveXObject || "ActiveXObject" in window)
            return true;
        else
            return false;
    };
    String2XML = function (xmlString) {
        // for IE
        if (isIE()) {
            var xmlobject = new ActiveXObject("Microsoft.XMLDOM");
            xmlobject.async = "false";
            xmlobject.loadXML(xmlString);
            return xmlobject;
        }
            // for other browsers
        else {
            var parser = new DOMParser();
            var xmlobject = parser.parseFromString(xmlString, "text/xml");
            return xmlobject;
        }
    }

    function Map() {
        this.keys = new Array();
        this.data = new Array();

        this.set = function (key, value) {
            if (this.data[key] == null) {
                this.keys.push(value);
            }
            this.data[key] = value;
        };
        
        this.get = function (key) {
            return this.data[key];
        };
        
        this.remove = function (key) {
            this.keys.remove(key);
            this.data[key] = null;
        };
        
        this.isEmpty = function () {
            return this.keys.length == 0;
        };
        
        this.size = function () {
            return this.keys.length;
        };
    }

    createXmlElement = function (xmlDoc, map, key, parentElement) {
        var newEle = xmlDoc.createElement(key);
        parentElement.appendChild(newEle);

        var value = map.get(key.toLowerCase());

        newEle.text = value;

    }
    addExternalInfoXML = function (xmlDoc) {

        var nodeList = xmlDoc.getElementsByTagName("table");
        for (var i = 0; i < nodeList.length; i++) {
            var childrenRows = nodeList[i].childNodes;

            for (var n = 0; n < childrenRows.length; n++) {
                var rowEle = childrenRows[n];

                var addrContent = "";
                var addrInfoNodes = rowEle.getElementsByTagName("AdditionalInfo");
                if (addrInfoNodes.length > 0) {
                    addrContent = (isIE()) ? addrInfoNodes[0].text : addrInfoNodes[0].nodeValue;
                }

                var contentArray = addrContent.split("|");

                var map = new Map();
                if (contentArray.length > 0) {
                    for (var i = 0; i < contentArray.length; i++) {
                        var text = contentArray[i];
                        if (text.length > 0 && text.indexOf("=") >= 0) {
                            var keyvaluelist = text.split("=");
                            if (keyvaluelist.length > 1) {
                                map.set(keyvaluelist[0].toLowerCase(), keyvaluelist[1]);
                            }
                        }

                    }
                }

               
                var extraInfoEle = xmlDoc.createElement('ExtraInfo');
                rowEle.appendChild(extraInfoEle);

                createXmlElement(xmlDoc, map, "Sender", extraInfoEle);
                createXmlElement(xmlDoc, map, "SenderTelephone", extraInfoEle);
                createXmlElement(xmlDoc, map, "SenderCompany", extraInfoEle);
                createXmlElement(xmlDoc, map, "SenderAddress", extraInfoEle);
                createXmlElement(xmlDoc, map, "SenderPost", extraInfoEle);

                createXmlElement(xmlDoc, map, "Receiver", extraInfoEle);
                createXmlElement(xmlDoc, map, "ReceiverTelephone", extraInfoEle);
                createXmlElement(xmlDoc, map, "ReceiverCompany", extraInfoEle);
                createXmlElement(xmlDoc, map, "ReceiverAddress", extraInfoEle);
                createXmlElement(xmlDoc, map, "ReceiverPost", extraInfoEle);

                
            }
        }
    }
    $("#printButton").click(function () {
        var printItem = $("#jqxcombobox").jqxDropDownList('getSelectedItem');

        if (printItem && printItem != null) {
            var printContent = printItem.value;
            var gridContent = $("#treeGrid").jqxTreeGrid('exportData', 'xml');

            var xml = String2XML(gridContent);
            var xsl = String2XML(printContent);
            addExternalInfoXML(xml);

            var newWindow = window.open('', '', 'width=800, height=500, menubar=yes, resizable=yes, scrollbars=yes');

            if (isIE()) {
                var ex = xml.transformNode(xsl);
                newWindow.document.write(ex);
            }
            else {
                var xsltProcessor = new XSLTProcessor();
                xsltProcessor.importStylesheet(xsl);
                newWindow.document = xsltProcessor.transformToFragment(xml, newWindow.document);
            }

            newWindow.document.close();

        }

    });
});